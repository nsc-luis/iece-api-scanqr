using System.ComponentModel.DataAnnotations;

namespace IECE_WebApiScanQR.Models
{
    public class Registro
    {
        [Key]
        public int idRegistro { get; set; }
        public int idDistrito { get; set; }
        public int idSector { get; set; }
        public string? nombre { get; set; }
        public string? apellidoPaterno { get; set; }
        public string? apellidoMaterno { get; set; }
        public int idCategoria { get; set; }
        public bool bautizado { get; set; }
        public string? codigo { get; set; }
        public string verificador { get; set; }
    }
}
