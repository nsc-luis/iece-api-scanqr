using Microsoft.EntityFrameworkCore;
using IECE_WebApiScanQR.Models;
using System.Security.Principal;

namespace IECE_WebApiScanQR.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Distrito> Distrito { get; set; }
        public DbSet<Sector> Sector { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Registro> Registro { get; set; }
        public DbSet<Asistencia> Asistencia { get; set; }
    }
}