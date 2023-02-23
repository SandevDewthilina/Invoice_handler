namespace HRMS_WEB.Models
{
    public class RegexComponent
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Area { get; set; }
        public bool IsArea { get; set; }
        public int TemplateID { get; set; }

        public Template Template { get; set; }
    }
}