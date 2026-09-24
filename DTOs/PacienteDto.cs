namespace ClinicaAPI.DTOs
{
    public class PacienteDto
    {
        public int Id { get; set; }
        public int IdPaciente { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime Fecha { get; set; } 
        public string Telefono { get; set; }
        public string Genero { get; set; }
        public string EstadoCivil { get; set; }
        public string Direccion { get; set; }
        public string Clinica { get; set; }
        // ==========================================
        // ANTECEDENTES PERSONALES
        // ==========================================
        public DateTime? FechaAntecedentesPersonales { get; set; }
        public string AntecedentesPersona { get; set; }

        // ==========================================
        // ANTECEDENTES FAMILIARES
        // ==========================================
        public DateTime? FechaAntecedentesFamiliares { get; set; }
        public string AntecedentesFamilia { get; set; }
        // ==========================================
        // ANTECEDENTES QUIRÚRGICOS
        // ==========================================
        public DateTime? FechaAntecedentesQuirurgicos { get; set; }
        public string AntecedentesQuirurgico { get; set; }

        // ==========================================
        // HÁBITOS
        // ==========================================
        public DateTime? FechaHabitos { get; set; }
        public string DescripcionHabitos { get; set; }


        // ==========================================
        // INMUNIZACIÓN
        // ==========================================
        public DateTime? FechaEstadoInmunizacion { get; set; }
        public string EstadoInmunizacion { get; set; } 


        // ==========================================
        // GINECO-OBSTÉTRICOS
        // ==========================================
        public DateTime? FechaGinecoObstetricos { get; set; }
        public int? Gestaciones { get; set; }
        public int? Partos { get; set; }
        public int? Cesareas { get; set; }
        public int? Abortos { get; set; }
        public int? HijosVivos { get; set; }
        public int? HijosMuertos { get; set; }

        // ==========================================
        // ACTIVIDAD FÍSICA
        // ==========================================
        public DateTime? FechaActividadFisica { get; set; }
        public string NivelActividadFisica { get; set; }

        // ==========================================
        // ALERGIAS
        // ==========================================
        public DateTime? FechaAlergias { get; set; }
        public string EstadoAlergia { get; set; }
        public string Alergia { get; set; }

        // ==========================================
        // MEDICACIÓN ACTUAL
        // ==========================================
        public DateTime? FechaMedicacionActual { get; set; }
        public string Medicacion { get; set; }

        // ==========================================
        // HISTORIA DE ENFERMEDAD ACTUAL
        // ==========================================
        public DateTime? FechaHistoriaEnfermedad { get; set; }
        public string HistoriaEnfermedad { get; set; }

        // ==========================================
        // EXAMEN FÍSICO
        // ==========================================
        public DateTime? FechaExamenFisico { get; set; }
        public string PresionArterial { get; set; }
        public int? FrecuenciaCardiaca { get; set; }
        public int? FrecuenciaRespiratoria { get; set; }
        public decimal? SaturacionOxigeno { get; set; }
        public decimal? PesoExamenFisico { get; set; }
        public decimal? Temperatura { get; set; }

        // ==========================================
        // ÍNDICE DE MASA CORPORAL
        // ==========================================
        public DateTime? FechaMC { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Estatura { get; set; }
        public decimal? IndiceMasaCorporal { get; set; }

        // ==========================================
        // REVISIÓN DE APARATOS Y SISTEMAS
        // ==========================================
        public DateTime? FechaROAS { get; set; }
        public string RevisionAparatosSistemas { get; set; }

        // ==========================================
        // LABORATORIOS
        // ==========================================
        public DateTime? FechaLaboratorios { get; set; }
        public string ResultadosLaboratorio { get; set; }

        // ==========================================
        // ELECTROCARDIOGRAMA
        // ==========================================
        public DateTime? FechaECG { get; set; }
        public string InterpretacionElectrocardiograma { get; set; }

        // ==========================================
        // IMÁGENES
        // ==========================================
        public DateTime? FechaImagenes { get; set; }
        public string EstudiosImagen { get; set; }

        // ==========================================
        // RIESGO CARDIOVASCULAR
        // ==========================================
        public DateTime? FechaRiesgoCardiovascular { get; set; }
        public string ResultadoEvaluacion { get; set; }

        // ==========================================
        // IMPRESIÓN DIAGNÓSTICA
        // ==========================================
        public DateTime? FechaImpresionDiagnostica { get; set; }
        public string Diagnostica { get; set; }

        // ==========================================
        // PLAN TERAPÉUTICO
        // ==========================================
        public DateTime? FechaPlanTerapeutico { get; set; }
        public string TratamientoIndicado { get; set; }

    }
}
