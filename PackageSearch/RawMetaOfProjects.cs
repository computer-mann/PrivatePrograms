using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageSearch
{
    public class RawMetaOfProjects
    {
        public int Count { get; set; }
        public List<ProjectItem> Value { get; set; }
    }

    public class ProjectItem
    {
        public string ObjectId { get; set; }
        public string GitObjectType { get; set; }
        public string CommitId { get; set; }
        public string Path { get; set; }
        public bool IsFolder { get; set; }
        public string Url { get; set; }
    }
}
