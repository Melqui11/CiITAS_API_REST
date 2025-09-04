using System.ComponentModel.DataAnnotations;

namespace CiITAS_API_REST.Models
{
    public class createUser
    {
        public string NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string SECOND_LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE_NUMBER { get; set; }
    }
}
