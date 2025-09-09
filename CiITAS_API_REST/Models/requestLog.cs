using System;
using System.ComponentModel.DataAnnotations;

namespace CiITAS_API_REST.Models
{
    public class requestLog
    {
        [Key] 
        public int LogId { get; set; }

        public int StatusCode { get; set; }

        public string Response { get; set; }

        public string Payload { get; set; }

        public DateTime ExecutionDate { get; set; }
    }
}
