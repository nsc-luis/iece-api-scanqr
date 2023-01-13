using System.ComponentModel.DataAnnotations;

namespace IECE_WebApiScanQR.Models
{
    public class Sector
    {
        [Key]
        public int idSector { get; set; }
        public string? tipoSector { get; set; }
        public int numeroSector { get; set; }
        public string? nombreSector { get; set; }
        public int idDistrito { get; set; }
    }
}
