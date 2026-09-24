namespace ClinicaAPI
{
    public class StaticResources
    {
        #region LOGIN 

        public static string QueryUserLogin = @"                  					
			SELECT [Id]
                  ,[Nombre]
                  ,[Email]
                  ,[Password]
                  ,[Rol]
                  ,[Clinica]
              FROM [dbo].[Usuario]
                WHERE Email = @Email AND Password = @Password							";
        #endregion

        #region CITAS 

        //Crear Citas
        public static string QueryCrearCitas = @"                    					
			INSERT INTO [dbo].[Citas]
               ([PacienteId]
               ,[DoctorId]
               ,[Fecha]
               ,[Hora]
               ,[Motivo]
               ,[Tipo]
               ,[Telefono]
               ,[Estado]
               ,[Clinica])
           VALUES
                (@PacienteId
                ,@DoctorId
                ,@Fecha
                ,@Hora
                ,@Motivo
                ,@Tipo
                ,@Telefono
                ,@Estado
                ,@Clinica)

                SELECT CAST(SCOPE_IDENTITY() as int) as Id;
		                  ";

        //Listar Citas
        public static string QueryListaCitas = @"                   					
			SELECT a.[Id]
                  ,a.[PacienteId]
                  ,a.[Fecha]
                  ,a.[Hora]
                  ,a.[Motivo]
                  ,a.[Tipo]
                  ,a.[Estado]
                  ,b.[Telefono]
                  ,c.Nombre as NombreDoctor
                  ,a.DoctorId 
                  ,CONCAT(b.[Nombre], ' ', b.[Apellido]) AS NombreCompleto
                  ,a.Clinica
              FROM [dbo].[Citas] a, [dbo].[Paciente] b, [dbo].[Doctor] c
                 WHERE a.PacienteId = b.Id
                 and a.DoctorId  = c.id
                 and  a.Clinica = @Clinica
              ORDER BY a.Fecha desc
							";

        //Modificar Citas
        public static string QueryModificarCitas = @"                     					
			UPDATE [dbo].[Citas]
                   SET [Estado] = @Estado
                      ,[Motivo] = @Motivo
                      ,[Tipo] = @Tipo
                      ,[Fecha] = @Fecha
                      ,[Hora] = @Hora
                 WHERE Id = @Id
		                  ";

        //Eliminar Citas
        public static string QueryEliminarCitas = @"                      					
		        DELETE FROM [dbo].[Citas]
                 WHERE Id = @Id
		                  ";

        #endregion

        #region PACIENTES 

        //Crear Pacientes
        public static string QueryCrearPacientes = @"
                INSERT INTO Paciente
                (
                    Nombre,
                    Apellido,
                    FechaNacimiento,
                    Fecha,
                    Telefono,
                    Genero,
                    EstadoCivil,
                    Direccion,
                    Clinica
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @Nombre,
                    @Apellido,
                    @FechaNacimiento,
                    @Fecha,
                    @Telefono,
                    @Genero,
                    @EstadoCivil,
                    @Direccion,
                    @Clinica
                );
            ";

        //Modificar Pacientes
        public static string QueryModificarPacientes = @"                      					
			UPDATE [dbo].[Paciente]
                   SET [Nombre] = @Nombre
                      ,[Apellido] = @Apellido
                      ,[FechaNacimiento] = @FechaNacimiento
                      ,[Telefono] = @Telefono
                      ,[Genero] = @Genero
                      ,[EstadoCivil] = @EstadoCivil
                      ,[Direccion] = @Direccion
                 WHERE Id = @Id
		                  ";

        //Eliminar Pacientes
        public static string QueryEliminarPacientes = @"                       					
		        DELETE FROM [dbo].[Paciente]
                 WHERE Id = @Id
		                  ";

        //Listar Pacientes
        public static string QueryListaPacientes = @"                   						
           SELECT
                    -- ==========================================
                    -- PACIENTE
                    -- ==========================================
                    P.Id,
                    P.Id AS IdPaciente,
                    P.Nombre,
                    P.Apellido,
                    CONCAT(P.Nombre, ' ', P.Apellido) AS NombreCompleto,
                    P.FechaNacimiento,
                    P.Fecha AS FechaPaciente,
                    P.Telefono,
                    P.Genero,
                    P.EstadoCivil,
                    P.Direccion,
                    P.Clinica,

                    -- ==========================================
                    -- ANTECEDENTES PERSONALES
                    -- ==========================================
                    AP.Fecha AS FechaAntecedentesPersonales,
                    AP.AntecedentesPersona,

                    -- ==========================================
                    -- ANTECEDENTES FAMILIARES
                    -- ==========================================
                    AF.Fecha AS FechaAntecedentesFamiliares,
                    AF.AntecedentesFamilia,

                    -- ==========================================
                    -- ANTECEDENTES QUIRÚRGICOS
                    -- ==========================================
                    AQ.Fecha AS FechaAntecedentesQuirurgicos,
                    AQ.antecedentesQuirurgico,

                    -- ==========================================
                    -- HÁBITOS
                    -- ==========================================
                    H.Fecha AS FechaHabitos,
                    H.DescripcionHabitos,

                    -- ==========================================
                    -- INMUNIZACIÓN
                    -- ==========================================
                    INM.Fecha AS FechaEstadoInmunizacion,
                    INM.EstadoInmunizacion,

                    -- ==========================================
                    -- GINECO-OBSTÉTRICOS
                    -- ==========================================
                    GO.Fecha AS FechaGinecoObstetricos,
                    GO.Gestaciones,
                    GO.Partos,
                    GO.Cesareas,
                    GO.Abortos,
                    GO.HijosVivos,
                    GO.HijosMuertos,

                    -- ==========================================
                    -- ACTIVIDAD FÍSICA
                    -- ==========================================
                    AFI.Fecha AS FechaActividadFisica,
                    AFI.NivelActividadFisica,

                    -- ==========================================
                    -- ALERGIAS
                    -- ==========================================
                    AL.Fecha AS FechaAlergias,
                    AL.EstadoAlergia,
                    AL.Alergia,

                    -- ==========================================
                    -- MEDICACIÓN ACTUAL
                    -- ==========================================
                    MA.Fecha AS FechaMedicacionActual,
                    MA.Medicacion,

                    -- ==========================================
                    -- HISTORIA DE ENFERMEDAD ACTUAL
                    -- ==========================================
                    HEA.Fecha AS FechaHistoriaEnfermedad,
                    HEA.HistoriaEnfermedad,

                    -- ==========================================
                    -- EXAMEN FÍSICO
                    -- ==========================================
                    EF.Fecha AS FechaExamenFisico,
                    EF.PresionArterial,
                    EF.FrecuenciaCardiaca,
                    EF.FrecuenciaRespiratoria,
                    EF.SaturacionOxigeno,
                    EF.Peso AS PesoExamenFisico,
                    EF.Temperatura,

                    -- ==========================================
                    -- ÍNDICE DE MASA CORPORAL
                    -- ==========================================
                    MC.Fecha AS FechaMC,
                    MC.Peso,
                    MC.Estatura,
                    MC.IndiceMasaCorporal,

                    -- ==========================================
                    -- REVISIÓN DE APARATOS Y SISTEMAS
                    -- ==========================================
                    ROAS.Fecha AS FechaROAS,
                    ROAS.RevisionAparatosSistemas,

                    -- ==========================================
                    -- LABORATORIOS
                    -- ==========================================
                    LAB.Fecha AS FechaLaboratorios,
                    LAB.ResultadosLaboratorio,

                    -- ==========================================
                    -- ELECTROCARDIOGRAMA
                    -- ==========================================
                    ECG.Fecha AS FechaECG,
                    ECG.InterpretacionElectrocardiograma,

                    -- ==========================================
                    -- IMÁGENES
                    -- ==========================================
                    IMG.Fecha AS FechaImagenes,
                    IMG.EstudiosImagen,

                    -- ==========================================
                    -- RIESGO CARDIOVASCULAR
                    -- ==========================================
                    RC.Fecha AS FechaRiesgoCardiovascular,
                    RC.ResultadoEvaluacion,

                    -- ==========================================
                    -- IMPRESIÓN DIAGNÓSTICA
                    -- ==========================================
                    ID.Fecha AS FechaImpresionDiagnostica,
                    ID.Diagnostica,

                    -- ==========================================
                    -- PLAN TERAPÉUTICO
                    -- ==========================================
                    PT.Fecha AS FechaPlanTerapeutico,
                    PT.TratamientoIndicado

                FROM Paciente P

                LEFT JOIN AntecedentesPersonales AP
                    ON AP.IdPaciente = P.Id
                    AND AP.Clinica = P.Clinica

                LEFT JOIN AntecedentesFamiliares AF
                    ON AF.IdPaciente = P.Id
                    AND AF.Clinica = P.Clinica

                LEFT JOIN AntecedentesQuirurgicos AQ
                    ON AQ.IdPaciente = P.Id
                    AND AQ.Clinica = P.Clinica

                LEFT JOIN Habitos H
                    ON H.IdPaciente = P.Id
                    AND H.Clinica = P.Clinica

                LEFT JOIN Inmunizacion INM
                    ON INM.IdPaciente = P.Id
                    AND INM.Clinica = P.Clinica

                LEFT JOIN GinecoObstetricos GO
                    ON GO.IdPaciente = P.Id
                    AND GO.Clinica = P.Clinica

                LEFT JOIN ActividadFisica AFI
                    ON AFI.IdPaciente = P.Id
                    AND AFI.Clinica = P.Clinica

                LEFT JOIN Alergias AL
                    ON AL.IdPaciente = P.Id
                    AND AL.Clinica = P.Clinica

                LEFT JOIN MedicacionActual MA
                    ON MA.IdPaciente = P.Id
                    AND MA.Clinica = P.Clinica

                LEFT JOIN HistoriaEnfermedadActual HEA
                    ON HEA.IdPaciente = P.Id
                    AND HEA.Clinica = P.Clinica

                LEFT JOIN ExamenFisico EF
                    ON EF.IdPaciente = P.Id
                    AND EF.Clinica = P.Clinica

                LEFT JOIN MC
                    ON MC.IdPaciente = P.Id
                    AND MC.Clinica = P.Clinica

                LEFT JOIN ROAS
                    ON ROAS.IdPaciente = P.Id
                    AND ROAS.Clinica = P.Clinica

                LEFT JOIN Laboratorios LAB
                    ON LAB.IdPaciente = P.Id
                    AND LAB.Clinica = P.Clinica

                LEFT JOIN ECG
                    ON ECG.IdPaciente = P.Id
                    AND ECG.Clinica = P.Clinica

                LEFT JOIN Imagenes IMG
                    ON IMG.IdPaciente = P.Id
                    AND IMG.Clinica = P.Clinica

                LEFT JOIN RiesgoCardiovascular RC
                    ON RC.IdPaciente = P.Id
                    AND RC.Clinica = P.Clinica

                LEFT JOIN ImpresionDiagnostica ID
                    ON ID.IdPaciente = P.Id
                    AND ID.Clinica = P.Clinica

                LEFT JOIN PlanTerapeutico PT
                    ON PT.IdPaciente = P.Id
                    AND PT.Clinica = P.Clinica

                WHERE P.Clinica = @Clinica

                ORDER BY FechaPaciente DESC;


							";

        #endregion

        #region DATOS DE IDENTIFICACION DEL PACIENTE 

        //Crear AntecedentesPersonales
        public static string QueryCrearAntecedentesPersonales = @"                      					
			INSERT INTO AntecedentesPersonales
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        AntecedentesPersona
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @AntecedentesPersona
                    )
		                  ";

        //Modificar AntecedentesPersonales
        public static string QueryModificarAntecedentesPersonales = @"                       					
			 UPDATE AntecedentesPersonales
                    SET
                        Fecha = @Fecha,
                        AntecedentesPersona = @AntecedentesPersona
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar AntecedentesPersonales
        public static string QueryListaAntecedentesPersonales = @"                    					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                AntecedentesPersona
            FROM AntecedentesPersonales
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear AntecedentesFamiliares
        public static string QueryCrearAntecedentesFamiliares = @"                       					
			INSERT INTO AntecedentesFamiliares
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        AntecedentesFamilia
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @AntecedentesFamilia
                    )
		                  ";

        //Modificar AntecedentesFamiliares
        public static string QueryModificarAntecedentesFamiliares = @"                        					
			   UPDATE AntecedentesFamiliares
                SET
                    Fecha = @Fecha,
                    AntecedentesFamilia = @AntecedentesFamilia
                WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar AntecedentesFamiliares
        public static string QueryListaAntecedentesFamiliares = @"                     					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                AntecedentesFamilia
            FROM AntecedentesFamiliares
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear AntecedentesQuirurgicos
        public static string QueryCrearAntecedentesQuirurgicos = @"                        					
			INSERT INTO AntecedentesQuirurgicos
                        (
                            IdPaciente,
                            Clinica,
                            Fecha,
                            AntecedentesQuirurgico
                        )
                        VALUES
                        (
                            @IdPaciente,
                            @Clinica,
                            @Fecha,
                            @AntecedentesQuirurgico
                        )
		                                          ";

        //Modificar AntecedentesQuirurgicos
        public static string QueryModificarAntecedentesQuirurgicos = @"                         					
			   UPDATE AntecedentesQuirurgicos
                SET
                    Fecha = @Fecha,
                    AntecedentesQuirurgico = @AntecedentesQuirurgico
                WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar AntecedentesQuirurgicos
        public static string QueryListaAntecedentesQuirurgicos = @"                      					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                AntecedentesQuirurgico
            FROM AntecedentesQuirurgicos
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear habitos
        public static string QueryCrearHabitos = @"                         					
			INSERT INTO Habitos
                (
                    IdPaciente,
                    Clinica,
                    Fecha,
                    DescripcionHabitos
                )
                VALUES
                (
                    @IdPaciente,
                    @Clinica,
                    @Fecha,
                    @DescripcionHabitos    
                )
		                                          ";

        //Modificar habitos
        public static string QueryModificarhabitos = @"                          					
			   UPDATE Habitos
                    SET
                        Fecha = @Fecha,
                        DescripcionHabitos = @DescripcionHabitos
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar habitos
        public static string QueryListahabitos = @"                       					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                DescripcionHabitos
            FROM Habitos
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear GinecoObstetricos
        public static string QueryCrearGinecoObstetricos = @"                          					
			INSERT INTO GinecoObstetricos
                (
                    IdPaciente,
                    Clinica,
                    Fecha,
                    Gestaciones,
                    Partos,
                    Cesareas,
                    Abortos,
                    HijosVivos,
                    HijosMuertos
                )
                VALUES
                (
                    @IdPaciente,
                    @Clinica,
                    @Fecha,
                    @Gestaciones,
                    @Partos,
                    @Cesareas,
                    @Abortos,
                    @HijosVivos,
                    @HijosMuertos
                )
		                                                          ";

        //Modificar GinecoObstetricos
        public static string QueryModificarGinecoObstetricos = @"                           					
			    UPDATE GinecoObstetricos
                    SET
                        Fecha = @Fecha,
                        Gestaciones = @Gestaciones,
                        Partos = @Partos,
                        Cesareas = @Cesareas,
                        Abortos = @Abortos,
                        HijosVivos = @HijosVivos,
                        HijosMuertos = @HijosMuertos
                    WHERE IdPaciente = @IdPaciente
		                  ";


        //Listar GinecoObstetricos
        public static string QueryListaGinecoObstetricos = @"                        					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                Gestaciones,
                Partos,
                Cesareas,
                Abortos,
                HijosVivos,
                HijosMuertos
            FROM GinecoObstetricos
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear Inmunizacion
        public static string QueryCrearInmunizacion = @"                          					
			INSERT INTO Inmunizacion
                        (
                            IdPaciente,
                            Clinica,
                            Fecha,
                            EstadoInmunizacion
                        )
                        VALUES
                        (
                            @IdPaciente,
                            @Clinica,
                            @Fecha,
                            @EstadoInmunizacion
                        )
		                                                          ";

        //Modificar Inmunizacion
        public static string QueryModificarInmunizacion = @"                            					
			    UPDATE Inmunizacion
                        SET
                            Fecha = @Fecha,
                            EstadoInmunizacion = @EstadoInmunizacion
                        WHERE IdPaciente = @IdPaciente
		                  ";


        //Listar Inmunizacion
        public static string QueryListaInmunizacion = @"                         					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                Gestaciones,
                Partos,
                Cesareas,
                Abortos,
                HijosVivos,
                HijosMuertos
            FROM GinecoObstetricos
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear ActividadFisica
        public static string QueryCrearActividadFisica = @"                           					
			INSERT INTO ActividadFisica
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        NivelActividadFisica
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @NivelActividadFisica
                    )
		                                                          ";


        //Modificar ActividadFisica
        public static string QueryModificarActividadFisica = @"                             					
			    UPDATE ActividadFisica
                    SET
                        Fecha = @Fecha,
                        NivelActividadFisica = @NivelActividadFisica
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar ActividadFisica
        public static string QueryListaActividadFisica = @"                          					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                NivelActividadFisica
            FROM ActividadFisica
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear Alergias
        public static string QueryCrearAlergias = @"                           					
			INSERT INTO Alergias
                (
                    IdPaciente,
                    Clinica,
                    Fecha,
                    EstadoAlergia,
                    Alergia
                )
                VALUES
                (
                    @IdPaciente,
                    @Clinica,
                    @Fecha,
                    @EstadoAlergia,
                    @Alergia
                )
		                                                          ";

        //Modificar Alergias
        public static string QueryModificarAlergias = @"                              					
			   UPDATE Alergias
                    SET
                        Fecha = @Fecha,
                        EstadoAlergia = @EstadoAlergia,
                        Alergia = @Alergia
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar Alergias
        public static string QueryListaAlergias = @"                           					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                EstadoAlergia,
                Alergia
            FROM Alergias
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear MedicacionActual
        public static string QueryCrearMedicacionActual = @"                            					
			INSERT INTO MedicacionActual
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        Medicacion
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @Medicacion
                    )
		                                                                              ";

        //Modificar MedicacionActual
        public static string QueryModificarMedicacionActual = @"                               					
			    UPDATE MedicacionActual
                    SET
                        Fecha = @Fecha,
                        Medicacion = @Medicacion
                    WHERE IdPaciente = @IdPaciente
		                  ";


        //Listar MedicacionActual
        public static string QueryListaMedicacionActual = @"                            					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                Medicacion
            FROM MedicacionActual
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear HEA
        public static string QueryCrearHEA = @"                             					
			INSERT INTO HistoriaEnfermedadActual
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        HistoriaEnfermedad
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @HistoriaEnfermedad
)
		                                                                              ";

        //Modificar HEA
        public static string QueryModificarHEA = @"                                					
			    UPDATE HistoriaEnfermedadActual
                    SET
                         Fecha = @Fecha,
                        HistoriaEnfermedad = @HistoriaEnfermedad
                    WHERE IdPaciente = @IdPaciente
		                  ";


        //Listar HEA
        public static string QueryListaHEA = @"                             					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                HistoriaEnfermedad
            FROM HistoriaEnfermedadActual
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear ExamenFisico
        public static string QueryCrearExamenFisico = @"                              					
			INSERT INTO ExamenFisico
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        PresionArterial,
                        FrecuenciaCardiaca,
                        FrecuenciaRespiratoria,
                        SaturacionOxigeno,
                        Peso,
                        Temperatura
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @PresionArterial,
                        @FrecuenciaCardiaca,
                        @FrecuenciaRespiratoria,
                        @SaturacionOxigeno,
                        @Peso,
                        @Temperatura
                    )
		                                                                              ";

        //Modificar ExamenFisico
        public static string QueryModificarExamenFisico = @"                                					
			    UPDATE ExamenFisico
                    SET
                        Fecha = @Fecha,
                        PresionArterial = @PresionArterial,
                        FrecuenciaCardiaca = @FrecuenciaCardiaca,
                        FrecuenciaRespiratoria = @FrecuenciaRespiratoria,
                        SaturacionOxigeno = @SaturacionOxigeno,
                        Peso = @Peso,
                        Temperatura = @Temperatura
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar ExamenFisico
        public static string QueryListaExamenFisico = @"                              					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                PresionArterial,
                FrecuenciaCardiaca,
                FrecuenciaRespiratoria,
                SaturacionOxigeno,
                Peso,
                Temperatura
            FROM ExamenFisico
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear IMC
        public static string QueryCrearMC = @"                               					
			INSERT INTO MC
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        Peso,
                        Estatura,
                        IndiceMasaCorporal
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @Peso,
                        @Estatura,
                        @IndiceMasaCorporal
                    )
		                                                                              ";

        //Modificar IMC
        public static string QueryModificarIMC = @"                                 					
			     UPDATE MC
                SET
                    Fecha = @Fecha,
                    Peso = @Peso,
                    Estatura = @Estatura,
                    IndiceMasaCorporal = @IndiceMasaCorporal
                WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar IMC
        public static string QueryListaIMC = @"                               					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                Peso,
                Estatura,
                IndiceMasaCorporal
            FROM MC
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear ROAS
        public static string QueryCrearROAS = @"                                					
			INSERT INTO ROAS
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        RevisionAparatosSistemas
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @RevisionAparatosSistemas 
                    )
		                                                                                                  ";

        //Modificar ROAS
        public static string QueryModificarROAS = @"                                  					
			      UPDATE ROAS
                    SET
                        Fecha = @Fecha,
                        RevisionAparatosSistemas = @RevisionAparatosSistemas
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar ROAS
        public static string QueryListaROAS = @"                                					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                RevisionAparatosSistemas
            FROM ROAS
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear Laboratorios
        public static string QueryCrearLaboratorios = @"                                 					
			INSERT INTO Laboratorios
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        ResultadosLaboratorio
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @ResultadosLaboratorio
                    )
		                                                                                                  ";

        //Modificar Laboratorios
        public static string QueryModificarLaboratorios = @"                                   					
			        UPDATE Laboratorios
                    SET
                        Fecha = @Fecha,
                        ResultadosLaboratorio = @ResultadosLaboratorio
                    WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar Laboratorios
        public static string QueryListaLaboratorios = @"                                 					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                ResultadosLaboratorio
            FROM Laboratorios
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear ECG
        public static string QueryCrearECG = @"                                 					
			INSERT INTO ECG
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        InterpretacionElectrocardiograma
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @InterpretacionElectrocardiograma
                    )
		                                                                                                  ";


        //Modificar ECG
        public static string QueryModificarECG = @"                                    					
			         UPDATE ECG
                        SET
                            Fecha = @Fecha,
                            InterpretacionElectrocardiograma = @InterpretacionElectrocardiograma
                        WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar ECG
        public static string QueryListaECG = @"                                  					
			SELECT
                IdPaciente,
                Clinica,
                Fecha,
                InterpretacionElectrocardiograma
            FROM ECG
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear Imagenes
        public static string QueryCrearImagenes = @"                                  					
			INSERT INTO Imagenes
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        EstudiosImagen
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @EstudiosImagen
                    )
		                                                                                                  ";

        //Modificar Imagenes
        public static string QueryModificarImagenes = @"                                     					
			         UPDATE Imagenes
                        SET
                            Fecha = @Fecha,
                            EstudiosImagen = @EstudiosImagen
                        WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar Imagenes
        public static string QueryListaImagenes = @"                                   					
		SELECT
                IdPaciente,
                Clinica,
                Fecha,
                EstudiosImagen
            FROM Imagenes
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear RiesgoCardiovascular
        public static string QueryCrearRiesgoCardiovascular = @"                                   					
			INSERT INTO RiesgoCardiovascular
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        ResultadoEvaluacion
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @ResultadoEvaluacion
                    )
		                                                                                                  ";


        //Modificar RiesgoCardiovascular
        public static string QueryModificarRiesgoCardiovascular = @"                                      					
			         UPDATE RiesgoCardiovascular
                        SET
                            Fecha = @Fecha,
                            ResultadoEvaluacion = @ResultadoEvaluacion
                        WHERE IdPaciente = @IdPaciente
		                  ";


        //Listar RiesgoCardiovascular
        public static string QueryListaRiesgoCardiovascular = @"                                    					
		SELECT
                IdPaciente,
                Clinica,
                Fecha,
                ResultadoEvaluacion
            FROM RiesgoCardiovascular
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";


        //Crear ImpresionDiagnostica
        public static string QueryCrearImpresionDiagnostica = @"                                    					
			INSERT INTO ImpresionDiagnostica
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        Diagnostica
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @Diagnostica
                    )
		                                                                                                  ";

        //Modificar ImpresionDiagnostica
        public static string QueryModificarImpresionDiagnostica = @"                                       					
			         UPDATE ImpresionDiagnostica
                        SET
                            Fecha = @Fecha,
                            Diagnostica = @Diagnostica
                        WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar ImpresionDiagnostica
        public static string QueryListaImpresionDiagnostica = @"                                     					
		SELECT
                IdPaciente,
                Clinica,
                Fecha,
                Diagnostica
            FROM ImpresionDiagnostica
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
							";

        //Crear PlanTerapeutico
        public static string QueryCrearPlanTerapeutico = @"                                     					
			INSERT INTO PlanTerapeutico
                    (
                        IdPaciente,
                        Clinica,
                        Fecha,
                        TratamientoIndicado
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @Clinica,
                        @Fecha,
                        @TratamientoIndicado
                    )
		                                                                                                  ";

        //Modificar PlanTerapeutico
        public static string QueryModificarPlanTerapeutico = @"                                        					
			         UPDATE PlanTerapeutico
                        SET
                            Fecha = @Fecha,
                            TratamientoIndicado = @TratamientoIndicado
                        WHERE IdPaciente = @IdPaciente
		                  ";

        //Listar PlanTerapeutico
        public static string QueryListaPlanTerapeutico = @"                                     					
		SELECT
                IdPaciente,
                Clinica,
                Fecha,
                TratamientoIndicado
            FROM PlanTerapeutico
            WHERE Clinica = @Clinica;
							";


        #endregion

        #region DOCTORES  

        //Crear Doctores
        public static string QueryCrearDoctores = @"                     					
			INSERT INTO [dbo].[Doctor]
                   ([Nombre]
                   ,[EspecialidadId]
                   ,[Telefono]
                   ,[Email])
             VALUES
                (@Nombre
                ,@EspecialidadId
                ,@Telefono
                ,@Email)
		                  ";

        //Listar Doctores
        public static string QueryListaDoctores = @"                     								
            SELECT a.[Id]
                  ,a.[Nombre] as NombreDoctor
                  ,a.[EspecialidadId]
                  ,a.[Telefono]
                  ,a.[Email]
                  ,b.[Nombre] as NombreEspecialida
              FROM [dbo].[Doctor] a, [dbo].[Especialidad] b
              WHERE a.EspecialidadId = b.Id
               and  a.Email = @Email
							";

        //Modificar Doctores   
        public static string QueryModificarDoctores = @"                       					
			UPDATE [dbo].[Doctor]
                   SET [Nombre] = @Nombre
                      ,[EspecialidadId] = @EspecialidadId
                      ,[Telefono] = @Telefono
                      ,[Email] = @Email
                 WHERE Id = @Id
		                  ";

        //Eliminar Doctores
        public static string QueryEliminarDoctores = @"                        					
		        DELETE FROM [dbo].[Doctor]
                 WHERE Id = @Id
		                  ";

        #endregion

        #region USUARIOS 

        //Crear Usuarios
        public static string QueryCrearUsuarios = @"                      					
			INSERT INTO Usuario
                (
                    Nombre,
                    Email,
                    Password,
                    Rol,
                    Clinica,
                    CodigoVerificacion,
                    CorreoVerificado
                )
                VALUES
                (
                    @Nombre,
                    @Email,
                    @Password,
                    @Rol,
                    @Clinica,
                    @CodigoVerificacion,
                    @CorreoVerificado
                )
		                  ";

        #endregion

        #region CALENDARIO  
        //Crear Calendario 
        public static string QueryCrearCalndario = @"                    					
			INSERT INTO [dbo].[PacientesT]
               ([Nombre]
               ,[Telefono])
         VALUES
                (@Nombre
                ,@Telefono)
		                  ";

        //Listar Calendario
        public static string QueryListaCalendario = @"                    					
			SELECT a.[Id]
              ,a.[Nombre]
              ,a.[Telefono]
              ,b.[PacienteId]
              ,B.[Fecha]
              ,b.[Motivo]
          FROM [dbo].[PacientesT] a, [dbo].[Citas] b
          WHERE a.[Id] = b.[PacienteId]
							";
        #endregion

        #region ESPECIALIDADES  

        //Crear Especialidades
        public static string QueryCrearEspecialidades = @"                     					
			INSERT INTO [dbo].[Especialidad]
                   ([Nombre])
             VALUES
                (@Nombre)
		                  ";

        //Listar Especialidades
        public static string QueryListaEspecialidades = @"                     								
            SELECT a.[Id]
                  ,a.[Nombre] as NombreEspecialidad
              FROM [dbo].[Especialidad] a, [dbo].[Doctor] b
              WHERE a.id = b.EspecialidadId
               and  a.Email = @Email
							";

        //Modificar Especialidades   
        public static string QueryModificarEspecialidades = @"                       					
			UPDATE [dbo].[Especialidad]
                   SET [Nombre] = @Nombre
                 WHERE Id = @Id
		                  ";

        //Eliminar Especialidades
        public static string QueryEliminarEspecialidades = @"                        					
		        DELETE FROM [dbo].[Especialidad]
                 WHERE Id = @Id
		                  ";

        #endregion

        #region FACTURACION  

        //Crear Facturacion
        public static string QueryCrearFacturacion = @"                      					
			INSERT INTO [dbo].[Factura]
                   ([PacienteId]
                   ,[Fecha]
                   ,[Total]
                   ,[Clinica])
             VALUES
                (@PacienteId
                ,@Fecha
                ,@Total
                ,@Clinica)
		                  ";

        //Listar Facturas
        public static string QueryListarFactura = @"                       					
			SELECT  a.Id,
                    a.Fecha,
		            b.Nombre + ' ' + b.Apellido AS Paciente,
		            c.Motivo,
	                a.PrecioUnitario,
		            a.Total
            FROM [dbo].[Factura] a, [dbo].[Paciente] b, [dbo].[Citas] c
            WHERE 1 = 1
	            and a.Clinica = b.Clinica
	            and a.Clinica = C.Clinica
                and b.Clinica = C.Clinica
	            and a.PacienteId = b.Id
	            and a.PacienteId = c.PacienteId
                and a.Clinica = @Clinica)
		                  ";

        #endregion

        #region REPORTES CITAS  

        //Lista de Citas por fecha inicio y fecha fin
        public static string QueryListaCitasFecha = @"                          							
               SELECT 
                    a.[Id],
                    a.[Fecha],
                    a.[Hora],
                    d.Nombre AS Especialidad,
                    c.Nombre AS NombreDoctor,
                    a.[Tipo],
                    a.[Motivo],
                    a.[Estado],
                    a.[PacienteId],
                    CONCAT(b.[Nombre], ' ', b.[Apellido]) AS NombrePaciente,
                    b.[Telefono],
                    a.Clinica
                FROM [dbo].[Citas] a
                    JOIN [dbo].[Paciente] b ON a.PacienteId = b.Id
                    JOIN [dbo].[Doctor] c ON a.DoctorId = c.Id
                    JOIN [dbo].[Especialidad] d ON c.EspecialidadId = d.Id
                WHERE a.Clinica = @Clinica
                      AND a.Fecha >= @FechaInicio
                      AND a.Fecha < DATEADD(DAY, 1, @FechaFin)
                ORDER BY a.Fecha DESC
		                  ";

        //Total Citas por año actual
        public static string QueryTotalCitasPorAñoActual = @"                          							
                SELECT COUNT(*) AS Total
                FROM [dbo].[Citas] a
                INNER JOIN [dbo].[Paciente] b 
                    ON a.PacienteId = b.Id 
                    AND a.Clinica = b.Clinica
                INNER JOIN [dbo].[Usuario] c 
                    ON a.Clinica = c.Clinica
                WHERE a.Clinica = @Clinica
                    AND YEAR(a.Fecha) = YEAR(GETDATE())
		                  ";


        //Total Citas Confirmado
        public static string QueryTotalCitasConfirmado = @"                        					
			SELECT COUNT(*) AS Total 
                            FROM [dbo].[Citas]
                            WHERE Clinica = @Clinica
                                  AND Estado = 'Confirmada' 
                                  AND YEAR(Fecha) = YEAR(GETDATE())
		                  ";

        //Total Citas pendiente
        public static string QueryTotalCitasPendiente = @"                         					
			  SELECT COUNT(*) AS Total 
                            FROM [dbo].[Citas]
                            WHERE Clinica = @Clinica
                                  AND Estado = 'Pendiente' 
                                  AND YEAR(Fecha) = YEAR(GETDATE())
		                  ";

        //Total Citas canceladas
        public static string QueryTotalCitasCanceladas = @"                         						
              SELECT COUNT(*) AS Total 
                            FROM [dbo].[Citas]
                            WHERE Clinica = @Clinica
                                  AND Estado = 'Cancelada' 
                                  AND YEAR(Fecha) = YEAR(GETDATE())
		                  ";

        #endregion

        #region REPORTES PACIENTES  

        //Total Pacientes 
        public static string QueryTotalPacientes = @"                          					
			SELECT 
                COUNT( Id) AS TotalPacientes
            FROM [dbo].[Paciente] 
            WHERE Clinica = @Clinica
		                  ";

        //Listar Pacientes por fechaInicio y fechaFin
        public static string QueryListaPacientesFecha = @"                    					
			SELECT 
                [Id],
                CONCAT([Nombre], ' ', [Apellido]) AS NombreCompleto,
                [FechaNacimiento],
                [Fecha],
                [Telefono],
                [Direccion],
                [clinica]
            FROM [dbo].[Paciente]
            WHERE Clinica = @Clinica
                AND Fecha >= @FechaInicio
                AND Fecha < DATEADD(DAY, 1, @FechaFin)
              ORDER BY Fecha desc
							";

        #endregion

        #region REPORTES CITAS   

        //Historial Clinico por Pacientes, fecha inicio y fecha fin
        public static string QueryVerHistorialClinico = @"                          							
              SELECT 
                CONCAT(b.[Nombre], ' ', b.[Apellido]) AS NombrePaciente,
                b.[Telefono],
                STRING_AGG(
                    CONCAT(
                        a.Motivo, 
                        ' (', 
                        FORMAT(a.Fecha, 'dd/MM/yyyy'), 
                        ')'
                    ), 
                    ', '
                ) WITHIN GROUP (ORDER BY a.Fecha DESC) AS Motivos,
                MAX(a.Fecha) AS UltimaFechaCita ,
                COUNT(a.Id) AS TotalCitas,
                a.Clinica
            FROM [dbo].[Citas] a
            JOIN [dbo].[Paciente] b ON a.PacienteId = b.Id
            WHERE a.Clinica = @Clinica  
            GROUP BY 
                b.Nombre, 
                b.Apellido,
                b.Telefono,
                a.Clinica     
            ORDER BY UltimaFechaCita DESC
		                  ";

        #endregion
    }
}
