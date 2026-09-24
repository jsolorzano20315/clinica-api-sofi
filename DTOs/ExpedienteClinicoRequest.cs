using ClinicaAPI.Models;

namespace ClinicaAPI.DTOs
{
    public class ExpedienteClinicoRequest
    {
        public Paciente Paciente { get; set; }

        public AntecedentesPersonales? AntecedentesPersonales { get; set; }
        public AntecedentesFamiliares? AntecedentesFamiliares { get; set; }
        public AntecedentesQuirurgicos? AntecedentesQuirurgicos { get; set; }

        public GinecoObstetricos? GinecoObstetricos { get; set; }

        public Habitos? Habitos { get; set; }
        public Inmunizacion? Inmunizacion { get; set; }
        public ActividadFisica? ActividadFisica { get; set; }
        public Alergias? Alergias { get; set; }
        public MedicacionActual? MedicacionActual { get; set; }

        public HistoriaEnfermedadActual? HEA { get; set; }
        public ExamenFisico? ExamenFisico { get; set; }
        public MC? MC { get; set; }
        public ROAS? ROAS { get; set; }

        public Laboratorios? Laboratorios { get; set; }
        public ECG? ECG { get; set; }
        public Imagenes? Imagenes { get; set; }

        public RiesgoCardiovascular? RiesgoCardiovascular { get; set; }
        public ImpresionDiagnostica? ImpresionDiagnostica { get; set; }
        public PlanTerapeutico? PlanTerapeutico { get; set; }
    }
}
