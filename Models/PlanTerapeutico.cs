namespace ClinicaAPI.Models
{
    public class PlanTerapeutico
    {
        public int IdPlanTerapeutico { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? TratamientoIndicado { get; set; } 
    }
}
