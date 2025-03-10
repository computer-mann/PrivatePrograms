using System.Text.Json;

namespace SerializationForLogging
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var analytics = new Analytics
            {
                Page = new Page
                {
                    Channel = "Web",
                    AppVersion = "1.5.0",
                    ViewName = "Dashboard",
                    DeviceType = "Desktop",
                    CreatedAt = "2025-01-30T15:45:00Z",
                    SectionName = "Home",
                    AppName = "SpendClarity",
                    AppBuildNumber = "1500",
                    TapName = "Settings",
                    TapId = "btn-settings",
                    UiType = "Button",
                    PageName = "HomePage",
                    TapShortName = "Settings",
                    ViewId = "view-456",
                    ViewShortName = "Dash",
                    SearchId = "search-654",
                    SearchResultFound = true,
                    SearchQuery = "Recent Transactions",
                    SearchShortName = "Txns",
                    SearchName = "Transaction Search",
                    SearchSelectedResult = "Transaction #789",
                    PurchasePaymentType = "Credit Card",
                    PurchaseErrorMessage = "",
                    PurchasePaymentChannel = "Visa",
                    PurchaseAmount = 250.75m
                },
                Customer = new Customer
                {
                    CustomerPhoneNumber = "+1234567890",
                    CustomerEmail = "customer@example.com",
                    CustomerId = "CUST-002",
                    CustomerName = "Jane Doe"
                },
                Action = new Action
                {
                    ActionName = "MakePayment",
                    IsSuccessful = true,
                    ApiBaseUrl = "https://api.spendclarity.com",
                    Payload = "{\"transactionId\":\"TXN-101\",\"amount\":250.75,\"currency\":\"USD\"}",
                    ApiResponseTimeInSeconds = 0.275,
                    ApiResponseSizeInMegabytes = 0.003,
                    ApiStatus = "Success",
                    ApiUrl = "https://api.spendclarity.com/payments",
                    ApiStatusCode = 200,
                    ReasonForFailure = "",
                    ValidationErrors = "",
                    ApiResponseSizeInKilobytes = 3.072
                }
            };

            
            //var properties = GetJsonPropertyPathsAndValues(analytics, "Analytics");

            
            //foreach (var kvp in properties)
            //{
            //    Console.WriteLine($"{kvp.Key} <===> {kvp.Value}");
            //}
            Console.WriteLine(JsonSerializer.Serialize(analytics));
            Console.WriteLine("\n \n \n");
            var newObject = new
            {
                Message="analytics",
                AnalyticObject = analytics
            };
            Console.WriteLine(JsonSerializer.Serialize(newObject));
        }
        public static Dictionary<string, object> GetJsonPropertyPathsAndValues(object obj, string parentPath)
        {
            var pathsWithValues = new Dictionary<string, object>();
            var json = JsonSerializer.Serialize(obj);
            var document = JsonDocument.Parse(json);
            ExtractPathsAndValues(document.RootElement, parentPath, pathsWithValues);
            return pathsWithValues;
        }

        private static void ExtractPathsAndValues(JsonElement element, string currentPath, Dictionary<string, object> pathsWithValues)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    ExtractPathsAndValues(property.Value, $"{currentPath}.{property.Name}", pathsWithValues);
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    ExtractPathsAndValues(item, $"{currentPath}[{index}]", pathsWithValues);
                    index++;
                }
            }
            else
            {
                pathsWithValues[currentPath] = JsonElementToValue(element);
            }
        }

        private static object JsonElementToValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out long l) ? l :
                                        element.TryGetDouble(out double d) ? d : (object)element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.GetRawText()
            };
        }
    }

}

/*
 * 
 *  var jsonPath = "D:\\Projects\\dotnet\\PrivatePrograms\\SerializationForLogging\\example.json";
            //will come as an object like this
            var analytics= JsonSerializer.Deserialize<Analytics>(File.ReadAllText(jsonPath));
            var className = analytics.GetType().Name;

            var dictSerialized = JsonSerializer.Deserialize<Dictionary<string,object>>(JsonSerializer.Serialize(analytics));
            Console.WriteLine("Hello, World!");
 */