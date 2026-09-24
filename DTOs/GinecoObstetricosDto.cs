namespace ClinicaAPI.DTOs
{
    public class GinecoObstetricosDto 
    {
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }

        public int? Gestaciones { get; set; }
        public int? Partos { get; set; }
        public int? Cesareas { get; set; }
        public int? Abortos { get; set; }
        public int? HijosVivos { get; set; }
        public int? HijosMuertos { get; set; }

        // Campo recibido desde Vue para validar el género.
        // NO corresponde a una columna de GinecoObstetricos.
        public string? Genero { get; set; }

    }
}