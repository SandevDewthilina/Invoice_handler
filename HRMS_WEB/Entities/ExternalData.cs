using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.util;

namespace HRMS_WEB.Entities
{
    public class ExternalData
    {
        public int ID { get; set; }
        public string ChassisNumber { get; set; }
        public string Type { get; set; }

        public List<ExternalDataPair> ExternalDataPairs { get; set; }
    }
}