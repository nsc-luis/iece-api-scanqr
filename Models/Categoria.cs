using System.ComponentModel.DataAnnotations;

namespace IECE_WebApiScanQR.Models
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }
        public string? nombreCategoria { get; set; }
    }
}
