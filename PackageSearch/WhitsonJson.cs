


namespace PackageSearch
{
    public class WhitsonJson
    {
        public string message { get; set; }
        public int code { get; set; }
        public Data data { get; set; }
        public string subCode { get; set; }
        public object errors { get; set; }
    }

    public class Data
    {
        public Result[] results { get; set; }
        public int lowerBound { get; set; }
        public int upperBound { get; set; }
        public int pageIndex { get; set; }
        public int totalPages { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
    }

    public class Result
    {
        public string groupName { get; set; }
        public Supervisor[] supervisors { get; set; }
        public Productteam[] productTeams { get; set; }
        public string id { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class Supervisor
    {
        public string name { get; set; }
        public string jobLevel { get; set; }
        public string[] domain { get; set; }
        public string email { get; set; }
        public string id { get; set; }
    }

    public class Productteam
    {
        public string name { get; set; }
        public Member[] members { get; set; }
        public Repository[] repositories { get; set; }
        public string id { get; set; }
    }

    public class Member
    {
        public string name { get; set; }
        public string jobLevel { get; set; }
        public string[] domain { get; set; }
        public string email { get; set; }
        public string id { get; set; }
    }

    public class Repository
    {
        public string name { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string sonarQubeKey { get; set; }
    }
}
