using System.Collections.Generic;
using HRMS_WEB.Entities;

namespace HRMS_WEB.Models
{
    public class CreateTemplateModel
    {
        public CreateTemplateModel()
        {
            tablesList = new List<TableComponentBody>();
        }
        public string template_name { get; set; }
        public string selectedSupplier { get; set; }
        public List<RegexItem> templateRegexList { get; set; }
        public List<TableComponentBody> tablesList { get; set; }
    }

    public class TableComponentBody
    {
        public int id { get; set; }
        public string page_no { get; set; }
        public string flavor { get; set; }
        public bool split_text { get; set; }
        public int edge_tol { get; set; }
        public int row_tol { get; set; }
        public string area { get; set; }
        public bool font_sensitive { get; set; }
        public string columns { get; set; }
        public string headings { get; set; }
    }
    public class RegexItem
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string area { get; set; }
        public bool isArea { get; set; }
        public bool isGoogleVision { get; set; }
    }
}