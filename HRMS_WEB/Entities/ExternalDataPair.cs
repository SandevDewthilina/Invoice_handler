namespace HRMS_WEB.Entities
{
    public class ExternalDataPair
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int ExternalDataID { get; set; }
        public ExternalData ExternalData { get; set; }
    }
}