using System.ComponentModel.DataAnnotations;

namespace IECE_WebApiScanQR.Models
{
    public class Asistencia
    {
        [Key]
        public int idAsistencia { get; set; }
        public int idDistrito { get; set; }
        public int idSector { get; set; }
        public int idRegistro { get; set; }
        public int idCategoria { get; set; }
        public bool bautizado { get; set; }
        public DateTime fecha { get; set; }
    }
}
