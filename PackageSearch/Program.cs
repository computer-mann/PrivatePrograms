using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PackageSearch
{
    internal class Program
    {
        static string pat = "";//removed
        static async Task Main(string[] args)
        {
            //  await AzureTasks.ListRepositoriesInEachProjectName();
            // await AzureTasks.CountProjectsUsingHubtelInternalSdks();
            //await AzureTasks.TotalNumberOfDotnetProjectsAtHubtel();

            //find runnable projects in each repo and write to csv
            // in significant sectors
            List<string> repoList = [
                "https://dev.azure.com/hubtel/Gov/_git/ECG-Backend",
                "https://dev.azure.com/hubtel/Back-End/_git/Hubtel.InstantServices.Insurance",
                "https://dev.azure.com/hubtel/Back-End/_git/Hubtel.InsuranceMerchantPlatform",
                "https://dev.azure.com/hubtel/Back-End/_git/Hubtel.MtnEcgProxy"
                ];

            // List<string> repoList = ["https://dev.azure.com/hubtel/Back-End/_git/Hubtel.Authentication","https://dev.azure.com/hubtel/Back-End/_git/HubtelSchedulingEngine"];
            //await AzureTasks.CountAppSettingsFilesInRepos(repoList);
            // await AzureTasks.CountHubtelSdksBelowVersionAGivenVersionNumber(repoList, 8);
            // await AzureTasks.CreatingTheDependencyGraph(repoList);
            //await AzureTasks.FindNumberOfProjectsBelowVersion8(repoList);
            await AzureTasks.FindSdkUsageGroupedByVersion(repoList);
        }

        private static async Task<string> GetRepos(string organization, string project)
        {
            try
            {
                var url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories?api-version=6.0";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));
                return await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        private static async Task<List<string>> SearchForCsProjFiles(string organization, string project, string repo_id)
        {
            try
            {
                
                var url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repo_id}/items?scopePath=/&recursionLevel=full&includeContent=true&api-version=6.0";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));
                var stringResult = await client.GetAsync(url);
                if (!stringResult.IsSuccessStatusCode)
                {
                    Console.WriteLine(stringResult.Content.ReadAsStringAsync().Result);
                    Console.WriteLine($"Error in getting the list of projects for project {project} and repo Id {repo_id}");
                    return new List<string>();
                }
                var stringContent = await stringResult.Content.ReadAsStringAsync();
                var rawMetaOfProjects = JsonConvert.DeserializeObject<RawMetaOfProjects>(stringContent);
                return rawMetaOfProjects.Value.Where(x => !x.Path.StartsWith("/tests/")
                                                          && !x.Path.StartsWith("/examples/")
                                                          && !x.Path.Contains("sdk", StringComparison.OrdinalIgnoreCase)
                                                          && x.Path.EndsWith(".csproj")).Select(path => path.Path).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }
        private static List<string> SdkList()
        {
            var list = new List<string>();
            var packageList = File.ReadAllLines("D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\PackageSdksList.txt");

            foreach (var package in packageList)
            {
                list.Add(package);
            }
            return list;
        }

        private static async Task<string> GetCsProjFileContents(string organization, string project, string repo_id, string csprojFilePath)
        {
            
            var url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repo_id}/items?path={csprojFilePath}&includeContent=true&api-version=6.0";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));
            Console.WriteLine("Searching contents for " + csprojFilePath);
            return await client.GetStringAsync(url);
        }

        private static async Task GenerateCsvForSdksAndTheirCountInAllProjects()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var whitsonJsonPath = "D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\products.json";
            var jsonWhitson = JsonConvert.DeserializeObject<WhitsonJson>(File.ReadAllText(whitsonJsonPath));
            var team_and_group = BuildRepoNameToProductGroupAndTeam(jsonWhitson);
            int totalProjectCount = 0;
            var organization = "hubtel"; // Azure DevOps organization 
            string[] projects =
                [
                    "Back-End",
                    "Back-office",
                    "Consumer",
                    "AI Lab",
                    "Gov",
                    "Inventory",
                    "Innovations",
                    "Notifications",
                    "Orders",
                    "Payments",
                    "Producer",
                    "Web-Apps"
                ];
            var sdkList = SdkList();
            var sdk_count = new Dictionary<string, int>();
            foreach (var sdk in sdkList)
            {
                sdk_count[sdk] = 0;
            }
            foreach (var project in projects)
            {
                var allRepos = await GetRepos(organization, project);
                if (string.IsNullOrEmpty(allRepos))
                {
                    Console.WriteLine($"Cannot get the list of repos for project {project}");
                    continue;
                }

                var stringResult = await GetRepos(organization, project);
                var repoList = JsonConvert.DeserializeObject<BackendRepoList>(stringResult);
                foreach (var repo in repoList.Value)
                {
                    var csProjFiles = await SearchForCsProjFiles(organization, project, repo.Id);
                    foreach (var csProjFile in csProjFiles)
                    {
                        totalProjectCount += 1;
                        var csProjContents = await GetCsProjFileContents(organization, project, repo.Id, csProjFile);
                        foreach (var sdk in sdkList)
                        {
                            if (csProjContents.Contains(sdk))
                            {
                                sdk_count[sdk] += 1;
                            }
                        }
                    }
                }
            }
            foreach (var kv in sdk_count)
            {
                Console.WriteLine($"{kv.Key} : {kv.Value}");

            }
            //write to csv
            using (var writer = new StreamWriter($"D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\PackageSearchResult_{DateTime.Today.ToLongDateString()}.csv"))
                {
                    writer.WriteLine("Package,Count");
                    foreach (var kv in sdk_count)
                    {
                        writer.WriteLine($"{kv.Key},{kv.Value}");
                    }
                    writer.Flush();
                }
               
                stopwatch.Stop();
            
            Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
            Console.WriteLine("Total project count " + totalProjectCount);
        }

        private static Dictionary<string, Groupings> BuildRepoNameToProductGroupAndTeam(WhitsonJson whitsonJson)
        {
            var dict = new Dictionary<string, Groupings>();
            foreach (var result in whitsonJson.data.results)
            {
                foreach (var productTeam in result.productTeams)
                {
                    foreach (var repository in productTeam.repositories)
                    {
                        dict[repository.name] = new Groupings(result.groupName,productTeam.name);
                    }
                }
            }
            return dict;
        }
    }

    internal record Groupings(string ProductGroup="None", string ProductTeam="None");
}

/* get the list of repos for a project
 * 
 *  foreach (var project in projects)
            {
              
                var allRepos = await GetRepos(organization, project);
                
                if(string.IsNullOrEmpty(allRepos))
                {
                    Console.WriteLine($"Cannot get the list of repos for project {project}");
                    continue;
                }
                var repoList = JsonConvert.DeserializeObject<BackendRepoList>(allRepos);
                ; //project to type
                foreach (var repo in repoList.Value)
                {
                    Console.WriteLine($"Searching for csproj files in {repo.Name}");
                    var csProjFiles = await SearchForCsProjFiles(organization, project, repo.Id);
                    foreach (var csProjFile in csProjFiles)
                    {
                        var projName = Path.GetFileNameWithoutExtension(csProjFile);
                        rawDictionary[projName] = "all";
                        if (projName.EndsWith(".Api", StringComparison.OrdinalIgnoreCase))
                        {
                            mappedDictionary[projName] = "Api";
                        }
                        if (projName.EndsWith(".Consumer", StringComparison.OrdinalIgnoreCase))
                        {
                            mappedDictionary[projName] = "Consumer";
                        }
                        if (projName.EndsWith(".Job", StringComparison.OrdinalIgnoreCase))
                        {
                            mappedDictionary[projName] = "Job";
                        }

                    }
                }
            }
            //write to csv
            using (var writer = new StreamWriter($"D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\mappedProjectList_{DateTime.Now.Millisecond}.csv"))
            {
                writer.WriteLine("ProjectName,Type");
                foreach (var kv in mappedDictionary)
                {
                    writer.WriteLine($"{kv.Key},{kv.Value}");
                }
                writer.Flush();
            }
            using (var writer = new StreamWriter($"D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\rawProjectList_{DateTime.Now.Millisecond}.csv"))
            {
                writer.WriteLine("ProjectName,Type");
                foreach (var kv in rawDictionary)
                {
                    writer.WriteLine($"{kv.Key},{kv.Value}");
                }
                writer.Flush();
            }

 * 
 */

/*
 * 
 * foreach (var project in projects)
            {
                var allRepos = await GetRepos(organization, project);
                if (string.IsNullOrEmpty(allRepos))
                {
                    Console.WriteLine($"Cannot get the list of repos for project {project}");
                    continue;
                }
                var repoList = JsonConvert.DeserializeObject<BackendRepoList>(allRepos);
                foreach (var repo in repoList.Value)
                {
                    Console.WriteLine($"Searching for csproj files in {repo.Name}");

                    var csProjFiles = await SearchForCsProjFiles(organization, project, repo.Id);
                    foreach (var csProjFile in csProjFiles)
                    {
                        var projName = Path.GetFileNameWithoutExtension(csProjFile);
                        var excelData = new ExcelData();
                        excelData.Project = projName;
                        excelData.Repository = repo.WebUrl;
                        excelData.Status = "Not Started";
                        team_and_group.TryGetValue(repo.Name, out Groupings value);
                        var teamgroupPair = value ?? new Groupings();
                        //productTeam.Name is product team //results.groupname is product group
                        excelData.ProductGroup = teamgroupPair.ProductGroup.Replace(",","-");
                        excelData.ProductTeam = teamgroupPair.ProductTeam.Replace(",", "-");
                        if (projName.EndsWith(".Api", StringComparison.OrdinalIgnoreCase))
                        {
                            excelData.ProjectType = "Api";
                        }
                        if (projName.EndsWith(".Consumer", StringComparison.OrdinalIgnoreCase))
                        {
                            excelData.ProjectType = "Consumer";
                        }
                        if (projName.EndsWith(".Job", StringComparison.OrdinalIgnoreCase))
                        {
                            excelData.ProjectType = "Job";
                        }
                        finalResult.Add(excelData);
                    }
                }
            }
            //write to csv
            using (var writer = new StreamWriter($"D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\newDataDogTracking_{DateTime.Now.Millisecond}.csv"))
            {
                var sb = new StringBuilder();
                foreach (var datum in finalResult)
                {
                    sb.AppendLine($"{datum.Project},{datum.Repository},{datum.ProjectType},{datum.ProductGroup},{datum.ProductTeam},{datum.Status}");
                }
                writer.Write(sb.ToString());
                writer.Flush();
                writer.Close();
            }

            Console.WriteLine($"whole thing took {stopwatch.Elapsed.Seconds} seconds");
 */

/**
 *   var stopwatch = new Stopwatch();
            stopwatch.Start();
            var whitsonJsonPath = "D:\\Projects\\dotnet\\PrivatePrograms\\PackageSearch\\products.json";
            var jsonWhitson = JsonConvert.DeserializeObject<WhitsonJson>(File.ReadAllText(whitsonJsonPath));
            var team_and_group = BuildRepoNameToProductGroupAndTeam(jsonWhitson);
            var organization = "hubtel"; // Azure DevOps organization 
            string[] projects =
                [
                    "Back-End",
                    "Back-office",
                    "Consumer",
                    "AI Lab",
                    "Gov",
                    "Inventory",
                    "Innovations",
                    "Notifications",
                    "Orders",
                    "Payments",
                    "Producer",
                    "Web-Apps"
                ];
            await GenerateCsvForSdksAndTheirCountInAllProjects();
 **/