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

        public DbSet<AntecedentesPersonales> AntecedentesPersona { get; set; }
        public DbSet<AntecedentesFamiliares> AntecedentesFamiliar { get; set; }
        public DbSet<AntecedentesQuirurgicos> AntecedentesQuirurgico { get; set; }
        public DbSet<GinecoObstetricos> GinecoObstetrico { get; set; }
        public DbSet<Habitos> Habito { get; set; }
        public DbSet<Inmunizacion> Inmunizaciones { get; set; }
        public DbSet<ActividadFisica> ActividadFisicas{ get; set; }
        public DbSet<Alergias> Alergia { get; set; }
        public DbSet<MedicacionActual> MedicacionActuales { get; set; }
        public DbSet<HistoriaEnfermedadActual> Hea { get; set; }
        public DbSet<ExamenFisico> ExamenFisicos { get; set; }
        public DbSet<MC> Imc { get; set; }
        public DbSet<ROAS> Roa { get; set; }
        public DbSet<Laboratorios> Laboratorio { get; set; }
        public DbSet<ECG> Ecg { get; set; }
        public DbSet<Imagenes> Imagen { get; set; }
        public DbSet<RiesgoCardiovascular> RiesgoCardiovasculares { get; set; }
        public DbSet<ImpresionDiagnostica> ImpresionDiagnosticas { get; set; }
        public DbSet<PlanTerapeutico> PlanTerapeuticos { get; set; } 
    }
}
