using Newtonsoft.Json;

namespace HRMS_WEB.Entities
{
    public class UploadData
    {
        public int id { get; set; }
        public int uploadID { get; set; }
        public int? detectedTemplateID { get; set; }
        public string docType { get; set; }
        public string fieldJson { get; set; }
        public string tableJson { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}