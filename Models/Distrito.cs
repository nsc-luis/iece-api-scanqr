using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace IECE_WebApiScanQR.Models
{
    public class Distrito
    {
        [Key]
        public int idDistrito { get; set; }
        public string? nombreDistrito { get; set; }
        public string? tipoDistrito { get; set; }
        public int numeroDistrito { get; set; }
        public string? areaDistrito { get; set; }
    }
}
