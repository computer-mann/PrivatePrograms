using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationForLogging
{
    public class Analytics
    {
        public Page Page { get; set; }
        public Customer Customer { get; set; }
        public Action Action { get; set; }
    }

    public class Page
    {
        public string Channel { get; set; }
        public string AppVersion { get; set; }
        public string ViewName { get; set; }
        public string DeviceType { get; set; }
        public string CreatedAt { get; set; }
        public string SectionName { get; set; }
        public string AppName { get; set; }
        public string AppBuildNumber { get; set; }
        public string TapName { get; set; }
        public string TapId { get; set; }
        public string UiType { get; set; }
        public string PageName { get; set; }
        public string TapShortName { get; set; }
        public string ViewId { get; set; }
        public string ViewShortName { get; set; }
        public string SearchId { get; set; }
        public bool SearchResultFound { get; set; }
        public string SearchQuery { get; set; }
        public string SearchShortName { get; set; }
        public string SearchName { get; set; }
        public string SearchSelectedResult { get; set; }
        public string PurchasePaymentType { get; set; }
        public string PurchaseErrorMessage { get; set; }
        public string PurchasePaymentChannel { get; set; }
        public decimal PurchaseAmount { get; set; }
    }

    public class Action
    {
        public string ActionName { get; set; }
        public bool IsSuccessful { get; set; }
        public string ApiBaseUrl { get; set; }
        public string Payload { get; set; }
        public double ApiResponseTimeInSeconds { get; set; }
        public double ApiResponseSizeInMegabytes { get; set; }
        public string ApiStatus { get; set; }
        public string ApiUrl { get; set; }
        public int ApiStatusCode { get; set; }
        public string ReasonForFailure { get; set; }
        public string ValidationErrors { get; set; }
        public double ApiResponseSizeInKilobytes { get; set; }
    }

    public class Customer
    {
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
