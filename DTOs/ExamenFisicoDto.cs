namespace ClinicaAPI.DTOs
{
    public class ExamenFisicoDto
    {
        public int IdExamenFisico { get; set; }
        public int IdPaciente { get; set; }
        public string Clinica { get; set; }
        public DateTime Fecha { get; set; }
        public string PresionArterial { get; set; }
        public int FrecuenciaCardiaca { get; set; }
        public int FrecuenciaRespiratoria { get; set; }
        public decimal SaturacionOxigeno { get; set; }
        public decimal PesoEF { get; set; }
        public decimal Temperatura { get; set; }
    }
}
