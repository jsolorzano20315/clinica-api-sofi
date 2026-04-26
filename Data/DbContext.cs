namespace ClinicaAPI.Data
{
    using ClinicaAPI.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Numerics;

    public class ClinicaContext : DbContext
    {
        public ClinicaContext(DbContextOptions<ClinicaContext> options) : base(options) { }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Doctor> Doctores { get; set; } 
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Historial> Historiales { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<Roles> Rol { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }

        public DbSet<Facturas> Factura { get; set; }  
    }
}
