using HRMS_WEB.Models;

namespace HRMS_WEB.Entities
{
    public class TableComponent
    {
        public int ID { get; set; }
        public string PageNo { get; set; }
        public string Flavor { get; set; }
        public bool SplitText { get; set; }
        public int EdgeTol { get; set; }
        public int RowTol { get; set; }
        public string Area { get; set; }
        public bool FlagSize { get; set; }
        public int TemplateID { get; set; }
        public string Columns { get; set; }
        public string Headings { get; set; }
        

        public Template Template { get; set; }
    }
}