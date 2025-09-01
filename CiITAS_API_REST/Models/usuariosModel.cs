using System.ComponentModel.DataAnnotations;

namespace CiITAS_API_REST.Models
{
    public class usuariosModel
    {
        [Key]
        public int USER_ID { get; set; }

        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }

        public DateTime CREATION_DATE { get; set; }

        public string STATUS {  get; set; } 

      

    }
}
