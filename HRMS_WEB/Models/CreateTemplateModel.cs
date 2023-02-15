using System.Collections.Generic;

namespace HRMS_WEB.Models
{
    public class CreateTemplateModel
    {
        public string template_name { get; set; }
        public List<RegexItem> templateRegexList { get; set; }
    }

    public class RegexItem
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
}