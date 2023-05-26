namespace HRMS_WEB.Entities
{
    public class ComparisonLog
    {
        public int ID { get; set; }
        public int FirstExternalDataID { get; set; }
        public int SecondExternalDataID { get; set; }
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Couplings { get; set; }

        public ExternalData FirstExternalData { get; set; }
        public ExternalData SecondExternalData { get; set; }
    }
}