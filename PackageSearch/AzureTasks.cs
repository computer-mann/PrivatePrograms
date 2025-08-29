using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PackageSearch
{
    internal class AzureTasks
    {
        static readonly string csprojPath = "D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\";
        static readonly string organization = "hubtel";
        static readonly string[] projects =
        {
            "Back-End", "Back-office", "Consumer", "AI Lab", "Gov", "Inventory",
            "Innovations", "Notifications", "Orders", "Payments", "Producer", "Web-Apps"
         };
        static readonly string pat = "";  // Replace with your PAT
        // List Repositories in Each Project Name
        public static async Task ListRepositoriesInEachAzureProjectName()
        {


            string outputPath = "repos_by_project_name.txt";

            using var client = new HttpClient();
            using var writer = new StreamWriter(Path.Combine(csprojPath, outputPath), append: false); // Overwrite each run

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            foreach (var project in projects)
            {
                string safeProject = Uri.EscapeDataString(project);
                var url = $"https://dev.azure.com/{organization}/{safeProject}/_apis/git/repositories?api-version=7.0";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    await writer.WriteLineAsync($"❌ Failed to retrieve repositories for project: {project}");
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var repos = doc.RootElement.GetProperty("value");

                await writer.WriteLineAsync($"📂 Project: {project}");

                foreach (var repo in repos.EnumerateArray())
                {
                    string repoName = repo.GetProperty("name").GetString();
                    string repoUrl = repo.GetProperty("webUrl").GetString();
                    await writer.WriteLineAsync($"    🔹 {repoName} — {repoUrl}");
                }

                await writer.WriteLineAsync(); // Blank line between projects
            }

            Console.WriteLine($"✅ Repository listing complete. Results saved to: {Path.GetFullPath(outputPath)}");
        }

        public static async Task CountProjectsUsingHubtelInternalSdks()
        {
            string[] fileExtensions = { ".csproj", "Directory.Packages.props", "packages.config" };
            var sdkList = File.ReadAllLines(Path.Combine(csprojPath, "PackageSdksList.txt"))
                              .Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            var sdkUsageMap = sdkList.ToDictionary(sdk => sdk, sdk => new HashSet<string>());
            var sdkUsageLock = new object();

            int csprojCount = 0;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            var repoTasks = new List<Task>();

            foreach (var project in projects)
            {
                Console.WriteLine($"\n🔍 Scanning project: {project}");

                var repoUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories?api-version=7.0";
                var repoResp = await client.GetAsync(repoUrl);
                if (!repoResp.IsSuccessStatusCode) continue;

                var repoJson = JsonDocument.Parse(await repoResp.Content.ReadAsStringAsync());

                foreach (var repo in repoJson.RootElement.GetProperty("value").EnumerateArray())
                {
                    repoTasks.Add(Task.Run(async () =>
                    {
                        var repoName = repo.GetProperty("name").GetString();
                        var repoId = repo.GetProperty("id").GetString();
                        Console.WriteLine($"  📁 {project}/{repoName}");

                        var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                        var itemsResp = await client.GetAsync(itemsUrl);
                        if (!itemsResp.IsSuccessStatusCode) return;

                        var paths = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync())
                                    .RootElement.GetProperty("value")
                                    .EnumerateArray()
                                    .Where(item => item.TryGetProperty("path", out _))
                                    .Select(item => item.GetProperty("path").GetString())
                                    .Where(path => fileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                    .ToList();

                        // Count .csproj files
                        var csprojFiles = paths.Where(path => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)).ToList();
                        Interlocked.Add(ref csprojCount, csprojFiles.Count);

                        var foundSdks = new HashSet<string>();

                        foreach (var path in paths)
                        {
                            var fileUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?path={Uri.EscapeDataString(path)}&api-version=7.0&includeContent=true";
                            var fileResp = await client.GetAsync(fileUrl);
                            if (!fileResp.IsSuccessStatusCode) continue;

                            var content = await fileResp.Content.ReadAsStringAsync();

                            foreach (var sdk in sdkList)
                            {
                                if (content.Contains(sdk, StringComparison.OrdinalIgnoreCase))
                                {
                                    foundSdks.Add(sdk);
                                }
                            }
                        }

                        lock (sdkUsageLock)
                        {
                            foreach (var sdk in foundSdks)
                            {
                                sdkUsageMap[sdk].Add($"{project}/{repoName}");
                            }
                        }
                    }));
                }
            }

            await Task.WhenAll(repoTasks);

            // Final output
            Console.WriteLine($"\n📦 Total C# .csproj projects: {csprojCount}");

            Console.WriteLine("\n📊 SDK Usage Summary:");
            foreach (var kvp in sdkUsageMap.OrderByDescending(kvp => kvp.Value.Count))
            {
                if (kvp.Value.Count > 0)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value.Count} repo(s)");
                }
            }
        }

        public static async Task TotalNumberOfDotnetProjectsAtHubtel()
        {
            int csprojCount = 0;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            var repoTasks = new List<Task>();

            foreach (var project in projects)
            {
                Console.WriteLine($"\n🔍 Scanning project: {project}");

                var repoUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories?api-version=7.0";
                var repoResp = await client.GetAsync(repoUrl);
                if (!repoResp.IsSuccessStatusCode) continue;

                var repoJson = JsonDocument.Parse(await repoResp.Content.ReadAsStringAsync());

                foreach (var repo in repoJson.RootElement.GetProperty("value").EnumerateArray())
                {
                    repoTasks.Add(Task.Run(async () =>
                    {
                        var repoName = repo.GetProperty("name").GetString();
                        var repoId = repo.GetProperty("id").GetString();
                        Console.WriteLine($"  📁 {project}/{repoName}");

                        var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                        var itemsResp = await client.GetAsync(itemsUrl);
                        if (!itemsResp.IsSuccessStatusCode) return;

                        var paths = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync())
                            .RootElement.GetProperty("value")
                            .EnumerateArray()
                            .Where(item => item.TryGetProperty("path", out _))
                            .Select(item => item.GetProperty("path").GetString())
                            .Where(path => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        Interlocked.Add(ref csprojCount, paths.Count);
                    }));
                }
            }

            await Task.WhenAll(repoTasks);

            Console.WriteLine($"\n📦 Total C# .csproj projects: {csprojCount}");
        }

        public static async Task FindAllTheOutdatedSdksInTheSpecifiedRepos()
        {

        }

        public static async Task CountAppSettingsFilesInRepos(List<string> repoUrls)
        {
            var appSettingsFilenames = new[] { "appsettings.json" };
            var repoAppSettingsCount = new ConcurrentDictionary<string, int>();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

            var tasks = repoUrls.Select(async repoUrl =>
            {
                try
                {
                    // Parse URL: https://dev.azure.com/{organization}/{project}/_git/{repoName}
                    var uri = new Uri(repoUrl);
                    var segments = uri.AbsolutePath.Trim('/').Split('/');
                    if (segments.Length < 4 || segments[2] != "_git") return;
    
                    string project = segments[1];                       // "Back-End"
                    string repoName = segments[3];                      // "Hubtel.Authentication"

                    Console.WriteLine($"\n🔍 Scanning: {project}/{repoName}");

                    // Get repository ID
                    var repoInfoUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{Uri.EscapeDataString(repoName)}?api-version=7.0";
                    var repoResp = await client.GetAsync(repoInfoUrl);
                    if (!repoResp.IsSuccessStatusCode) return;

                    var repoJson = JsonDocument.Parse(await repoResp.Content.ReadAsStringAsync());
                    var repoId = repoJson.RootElement.GetProperty("id").GetString();

                    // Get all items in the repo
                    var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                    var itemsResp = await client.GetAsync(itemsUrl);
                    if (!itemsResp.IsSuccessStatusCode) return;

                    var itemsJson = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync());
                    var paths = itemsJson.RootElement.GetProperty("value")
                                    .EnumerateArray()
                                    .Where(item => item.TryGetProperty("path", out _))
                                    .Select(item => item.GetProperty("path").GetString())
                                    .Where(path => appSettingsFilenames.Any(name =>
                                        Path.GetFileName(path).Equals(name, StringComparison.OrdinalIgnoreCase)))
                                    .ToList();

                    repoAppSettingsCount[$"{project}/{repoName}"] = paths.Count;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing {repoUrl}: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);

            // Output summary
            Console.WriteLine("\n📊 AppSettings.json File Count Per Repository:");
            if (repoAppSettingsCount.Count == 0)
            {
                Console.WriteLine("No appsettings.json files found in the specified repositories.");
                return;
            }
            foreach (var kvp in repoAppSettingsCount.OrderByDescending(kvp => kvp.Value))
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value} file(s)");
            }

            var totalCount = repoAppSettingsCount.Values.Sum();
            Console.WriteLine($"\n📦 Total appsettings.json files across all repositories: {totalCount}");

            Console.ReadKey();
        }

        public static async Task CountHubtelSdksBelowVersionAGivenVersionNumber(List<string> repoUrls,int version)
        {
            var fileExtensions = new[] { ".csproj", "packages.config", "Directory.Packages.props" };
            var versionLimit = new Version(version, 0, 0);
            var sdkUsageMap = new ConcurrentDictionary<string, HashSet<string>>(); // sdkName -> set of repos

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

            var tasks = repoUrls.Select(async repoUrl =>
            {
                try
                {
                    var uri = new Uri(repoUrl);
                    var segments = uri.AbsolutePath.Trim('/').Split('/');
                    if (segments.Length < 4 || segments[2] != "_git")
                    {
                        Console.WriteLine($"❌ Invalid repo URL format: {repoUrl}");
                        return;
                    }

                    string project = segments[1];
                    string repoName = segments[3];
                    string repoKey = $"{project}/{repoName}";

                    Console.WriteLine($"\n🔍 Scanning: {repoKey}");

                    // Get repository ID
                    var repoInfoUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{Uri.EscapeDataString(repoName)}?api-version=7.0";
                    var repoResp = await client.GetAsync(repoInfoUrl);
                    if (!repoResp.IsSuccessStatusCode) return;

                    var repoJson = JsonDocument.Parse(await repoResp.Content.ReadAsStringAsync());
                    var repoId = repoJson.RootElement.GetProperty("id").GetString();

                    // Get files
                    var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                    var itemsResp = await client.GetAsync(itemsUrl);
                    if (!itemsResp.IsSuccessStatusCode) return;

                    var itemsJson = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync());
                    var paths = itemsJson.RootElement.GetProperty("value")
                        .EnumerateArray()
                        .Where(item => item.TryGetProperty("path", out _))
                        .Select(item => item.GetProperty("path").GetString())
                        .Where(path => fileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    var foundSdks = new HashSet<string>();

                    foreach (var path in paths)
                    {
                        var fileUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?path={Uri.EscapeDataString(path)}&api-version=7.0&includeContent=true";
                        var fileResp = await client.GetAsync(fileUrl);
                        if (!fileResp.IsSuccessStatusCode) continue;

                        var content = await fileResp.Content.ReadAsStringAsync();

                        // .csproj and props
                        var matches = Regex.Matches(content, @"<PackageReference\s+Include\s*=\s*""(Hubtel[^""]+)""\s+Version\s*=\s*""([^""]+)""", RegexOptions.IgnoreCase);
                        foreach (Match match in matches)
                        {
                            var sdkName = match.Groups[1].Value;
                            var versionStr = match.Groups[2].Value;

                            if (Version.TryParse(versionStr, out var version) && version < versionLimit)
                            {
                                foundSdks.Add(sdkName);
                            }
                        }

                        // packages.config
                        var pkgMatches = Regex.Matches(content, @"<package\s+id\s*=\s*""(Hubtel[^""]+)""\s+version\s*=\s*""([^""]+)""", RegexOptions.IgnoreCase);
                        foreach (Match match in pkgMatches)
                        {
                            var sdkName = match.Groups[1].Value;
                            var versionStr = match.Groups[2].Value;

                            if (Version.TryParse(versionStr, out var version) && version < versionLimit)
                            {
                                foundSdks.Add(sdkName);
                            }
                        }
                    }

                    foreach (var sdk in foundSdks)
                    {
                        sdkUsageMap.AddOrUpdate(
                            sdk,
                            _ => new HashSet<string> { repoKey },
                            (_, set) => { lock (set) { set.Add(repoKey); } return set; }
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error in {repoUrl}: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);

            Console.WriteLine("\n📊 Hubtel SDK Usage (version < 8.0.0):");
            foreach (var kvp in sdkUsageMap.OrderByDescending(kvp => kvp.Value.Count))
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Count} repo(s)");
            }

            Console.WriteLine($"\n📦 Total SDKs found below version 8.0.0: {sdkUsageMap.Count}");
        }


        public static async Task CreatingTheDependencyGraph(List<string> azureRepoUrls)
        {
            string[] fileExtensions = { ".csproj", "Directory.Packages.props", "packages.config" };
            var sdkList = File.ReadAllLines(Path.Combine(csprojPath, "PackageSdksList.txt"))
                              .Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            var sdkUsageMap = sdkList.ToDictionary(sdk => sdk, sdk => new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            var sdkUsageLock = new object();

            int csprojCount = 0;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            var repoTasks = new List<Task>();

            foreach (var repoUrl in azureRepoUrls)
            {
                repoTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        // Extract parts from URL
                        var uri = new Uri(repoUrl);
                        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                        if (segments.Length < 4 || segments[2] != "_git")
                        {
                            Console.WriteLine($"❌ Skipping invalid repo URL: {repoUrl}");
                            return;
                        }

                        var project = segments[1];
                        var repoName = segments[3];

                        Console.WriteLine($"\n🔍 Scanning: {project}/{repoName}");

                        var repoMetaUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{Uri.EscapeDataString(repoName)}?api-version=7.0";
                        var repoMetaResp = await client.GetAsync(repoMetaUrl);
                        if (!repoMetaResp.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"❌ Failed to get repo ID for {project}/{repoName}");
                            return;
                        }

                        var repoJson = JsonDocument.Parse(await repoMetaResp.Content.ReadAsStringAsync());
                        var repoId = repoJson.RootElement.GetProperty("id").GetString();

                        var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                        var itemsResp = await client.GetAsync(itemsUrl);
                        if (!itemsResp.IsSuccessStatusCode) return;

                        var paths = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync())
                                .RootElement.GetProperty("value")
                                .EnumerateArray()
                                .Where(item => item.TryGetProperty("path", out _))
                                .Select(item => item.GetProperty("path").GetString())
                                .Where(path => fileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                        var csprojFiles = paths.Where(path => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)).ToList();
                        Interlocked.Add(ref csprojCount, csprojFiles.Count);

                        foreach (var path in paths)
                        {
                            var fileUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?path={Uri.EscapeDataString(path)}&api-version=7.0&includeContent=true";
                            var fileResp = await client.GetAsync(fileUrl);
                            if (!fileResp.IsSuccessStatusCode) continue;

                            var content = await fileResp.Content.ReadAsStringAsync();

                            foreach (var sdk in sdkList)
                            {
                                if (content.Contains(sdk, StringComparison.OrdinalIgnoreCase))
                                {
                                    lock (sdkUsageLock)
                                    {
                                        sdkUsageMap[sdk].Add($"{project}/{repoName}{path}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error processing repo: {repoUrl} - {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(repoTasks);

            Console.WriteLine($"\n📦 Total C# .csproj projects: {csprojCount}");
            Console.WriteLine("\n📊 SDK Usage Summary (SDK => Project Files):");

            foreach (var kvp in sdkUsageMap.OrderByDescending(kvp => kvp.Value.Count))
            {
                if (kvp.Value.Count > 0)
                {
                    Console.WriteLine($"\n🔧 {kvp.Key} used in {kvp.Value.Count} file(s):");
                    foreach (var file in kvp.Value.OrderBy(f => f))
                    {
                        Console.WriteLine($"    - {file}");
                    }
                }
            }
            string csvPath = "D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\SdkUsageReportForECG.csv";

            using (var writer = new StreamWriter(csvPath))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField("SDK Name");
                csv.WriteField("File Path");
                csv.NextRecord();

                foreach (var kvp in sdkUsageMap.OrderByDescending(kvp => kvp.Value.Count))
                {
                    foreach (var file in kvp.Value.OrderBy(f => f))
                    {
                        csv.WriteField(kvp.Key);
                        csv.WriteField(file);
                        csv.NextRecord();
                    }
                }
            }

            Console.WriteLine($"\n📄 CSV written to: {Path.GetFullPath(csvPath)}");
        }

        public static async Task FindNumberOfProjectsBelowVersion8(List<string> azureRepoUrls)
        {
            string[] fileExtensions = { ".csproj" };
            var projectsBelow8 = new List<(string RepoPath, string Version)>();
            var projectsOnOrAbove8 = new List<(string RepoPath, string Version)>();
            int csprojCount = 0;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            var repoTasks = new List<Task>();

            foreach (var repoUrl in azureRepoUrls)
            {
                repoTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var uri = new Uri(repoUrl);
                        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                        if (segments.Length < 4 || segments[2] != "_git")
                        {
                            Console.WriteLine($"❌ Skipping invalid repo URL: {repoUrl}");
                            return;
                        }

                        var project = segments[1];
                        var repoName = segments[3];

                        Console.WriteLine($"\n🔍 Scanning: {project}/{repoName}");

                        var repoMetaUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{Uri.EscapeDataString(repoName)}?api-version=7.0";
                        var repoMetaResp = await client.GetAsync(repoMetaUrl);
                        if (!repoMetaResp.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"❌ Failed to get repo ID for {project}/{repoName}");
                            return;
                        }

                        var repoJson = JsonDocument.Parse(await repoMetaResp.Content.ReadAsStringAsync());
                        var repoId = repoJson.RootElement.GetProperty("id").GetString();

                        var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                        var itemsResp = await client.GetAsync(itemsUrl);
                        if (!itemsResp.IsSuccessStatusCode) return;

                        var paths = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync())
                                .RootElement.GetProperty("value")
                                .EnumerateArray()
                                .Where(item => item.TryGetProperty("path", out _))
                                .Select(item => item.GetProperty("path").GetString())
                                .Where(path => fileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                .Where(path => !Path.GetFileNameWithoutExtension(path).Contains(".Tests", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                        Interlocked.Add(ref csprojCount, paths.Count);

                        foreach (var path in paths)
                        {
                            var fileUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?path={Uri.EscapeDataString(path)}&api-version=7.0&includeContent=true";
                            var fileResp = await client.GetAsync(fileUrl);
                            if (!fileResp.IsSuccessStatusCode) continue;

                            var content = await fileResp.Content.ReadAsStringAsync();

                            string version = ExtractFrameworkVersion(content);

                            if (!string.IsNullOrEmpty(version))
                            {
                                if (IsVersionBelow8(version))
                                {
                                    lock (projectsBelow8)
                                        projectsBelow8.Add(($"{project}/{repoName}{path}", version));
                                }
                                else
                                {
                                    lock (projectsOnOrAbove8)
                                        projectsOnOrAbove8.Add(($"{project}/{repoName}{path}", version));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error processing repo: {repoUrl} - {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(repoTasks);

            Console.WriteLine($"\n📦 Total C# .csproj projects: {csprojCount}");
            Console.WriteLine($"\n🔽 Projects BELOW .NET 8.0: {projectsBelow8.Count}");
            Console.WriteLine($"\n🔼 Projects ON or ABOVE .NET 8.0: {projectsOnOrAbove8.Count}");

            void PrintProjects(List<(string RepoPath, string Version)> list, string label)
            {
                Console.WriteLine($"\n{label}");
                foreach (var (path, version) in list.OrderBy(p => p.RepoPath))
                {
                    Console.WriteLine($"- {version} → {path}");
                }
            }

            PrintProjects(projectsBelow8, "📉 Projects < 8.0:");
            PrintProjects(projectsOnOrAbove8, "📈 Projects >= 8.0:");

            string csvPath = "D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\FrameworkVersionReport.csv";
            using (var writer = new StreamWriter(csvPath))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField("Framework Version");
                csv.WriteField("File Path");
                csv.NextRecord();

                foreach (var entry in projectsBelow8.Concat(projectsOnOrAbove8))
                {
                    csv.WriteField(entry.Version);
                    csv.WriteField(entry.RepoPath);
                    csv.NextRecord();
                }
            }

            Console.WriteLine($"\n📄 CSV written to: {Path.GetFullPath(csvPath)}");
        }

        public static string ExtractFrameworkVersion(string csprojContent)
        {
            try
            {
                var xdoc = XDocument.Parse(csprojContent);

                var tf = xdoc.Descendants("TargetFramework").FirstOrDefault()?.Value ??
                         xdoc.Descendants("TargetFrameworks").FirstOrDefault()?.Value.Split(';').FirstOrDefault();

                if (tf != null && tf.StartsWith("net"))
                {
                    var numeric = tf.Substring(3);
                    if (numeric.Length == 2) numeric = $"{numeric[0]}.{numeric[1]}";
                    else if (numeric.Length == 3) numeric = $"{numeric[0]}.{numeric[1]}.{numeric[2]}";
                    return numeric;
                }

                var sdkAttr = xdoc.Root?.Attribute("Sdk")?.Value;
                return sdkAttr ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static bool IsVersionBelow8(string version)
        {
            if (Version.TryParse(version, out var parsed))
            {
                return parsed.Major < 8;
            }
            return false;
        }

        public static async Task FindSdkUsageGroupedByVersion(List<string> azureRepoUrls)
        {
            var sdkList = File.ReadAllLines(Path.Combine(csprojPath, "PackageSdksList.txt"))
                              .Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            var sdkUsageMap = new Dictionary<string, Dictionary<string, HashSet<string>>>(StringComparer.OrdinalIgnoreCase);
            int csprojCount = 0;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            var repoTasks = new List<Task>();

            foreach (var repoUrl in azureRepoUrls)
            {
                repoTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var uri = new Uri(repoUrl);
                        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                        if (segments.Length < 4 || segments[2] != "_git")
                        {
                            Console.WriteLine($"❌ Skipping invalid repo URL: {repoUrl}");
                            return;
                        }

                        var project = segments[1];
                        var repoName = segments[3];
                        Console.WriteLine($"\n🔍 Scanning: {project}/{repoName}");

                        var repoMetaUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{Uri.EscapeDataString(repoName)}?api-version=7.0";
                        var repoMetaResp = await client.GetAsync(repoMetaUrl);
                        if (!repoMetaResp.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"❌ Failed to get repo ID for {project}/{repoName}");
                            return;
                        }

                        var repoJson = JsonDocument.Parse(await repoMetaResp.Content.ReadAsStringAsync());
                        var repoId = repoJson.RootElement.GetProperty("id").GetString();

                        var itemsUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?recursionLevel=Full&api-version=7.0";
                        var itemsResp = await client.GetAsync(itemsUrl);
                        if (!itemsResp.IsSuccessStatusCode) return;

                        var paths = JsonDocument.Parse(await itemsResp.Content.ReadAsStringAsync())
                                .RootElement.GetProperty("value")
                                .EnumerateArray()
                                .Where(item => item.TryGetProperty("path", out _))
                                .Select(item => item.GetProperty("path").GetString())
                                .Where(path => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                                .Where(path => !Path.GetFileNameWithoutExtension(path).Contains(".Tests", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                        Interlocked.Add(ref csprojCount, paths.Count);

                        foreach (var path in paths)
                        {
                            var fileUrl = $"https://dev.azure.com/{organization}/{Uri.EscapeDataString(project)}/_apis/git/repositories/{repoId}/items?path={Uri.EscapeDataString(path)}&api-version=7.0&includeContent=true";
                            var fileResp = await client.GetAsync(fileUrl);
                            if (!fileResp.IsSuccessStatusCode) continue;

                            var content = await fileResp.Content.ReadAsStringAsync();

                            try
                            {
                                var xdoc = XDocument.Parse(content);

                                var packageRefs = xdoc.Descendants("PackageReference")
                                    .Where(pr => pr.Attribute("Include") != null)
                                    .Select(pr => new
                                    {
                                        Name = pr.Attribute("Include")?.Value,
                                        Version = pr.Attribute("Version")?.Value ?? pr.Element("Version")?.Value ?? "Unknown"
                                    });

                                foreach (var pr in packageRefs)
                                {
                                    if (sdkList.Contains(pr.Name, StringComparer.OrdinalIgnoreCase))
                                    {
                                        lock (sdkUsageMap)
                                        {
                                            if (!sdkUsageMap.ContainsKey(pr.Name))
                                                sdkUsageMap[pr.Name] = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

                                            if (!sdkUsageMap[pr.Name].ContainsKey(pr.Version))
                                                sdkUsageMap[pr.Name][pr.Version] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                                            sdkUsageMap[pr.Name][pr.Version].Add($"{project}/{repoName}{path}");
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Console.WriteLine($"⚠️ Skipping invalid XML in: {project}/{repoName}{path}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error processing repo: {repoUrl} - {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(repoTasks);

            Console.WriteLine($"\n📦 Total C# .csproj projects (excluding .Tests): {csprojCount}");
            Console.WriteLine("\n📊 SDK Usage Summary (From PackageSdksList.txt, grouped by version):");

            foreach (var sdk in sdkUsageMap.OrderBy(k => k.Key))
            {
                Console.WriteLine($"\n🔧 {sdk.Key}:");
                foreach (var version in sdk.Value.OrderBy(v => v.Key))
                {
                    Console.WriteLine($"   - Version {version.Key}: {version.Value.Count} project(s)");
                }
            }

            string csvPath = "D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\SdkVersionUsageReportECG.csv";
            using (var writer = new StreamWriter(csvPath))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField("SDK Name");
                csv.WriteField("Version");
                csv.WriteField("Usage Count");
                csv.NextRecord();

                foreach (var sdk in sdkUsageMap.OrderBy(k => k.Key))
                {
                    foreach (var version in sdk.Value.OrderBy(v => v.Key))
                    {
                        csv.WriteField(sdk.Key);
                        csv.WriteField(version.Key);
                        csv.WriteField(version.Value.Count);
                        csv.NextRecord();
                    }
                }
            }

            Console.WriteLine($"\n📄 SDK usage CSV written to: {Path.GetFullPath(csvPath)}");
        }


    }

}


