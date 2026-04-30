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
               ,[Motivo]
               ,[Tipo]
               ,[Telefono]
               ,[Estado]
               ,[Clinica])
           VALUES
                (@PacienteId
                ,@DoctorId
                ,@Fecha
                ,@Motivo
                ,@Tipo
                ,@Telefono
                ,@Estado
                ,@Clinica)
		                  ";

        //Listar Citas
        public static string QueryListaCitas = @"                   					
			SELECT a.[Id]
                  ,a.[PacienteId]
                  ,a.[Fecha]
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
			INSERT INTO [dbo].[Paciente]
                   ([Nombre]
                   ,[Apellido]
                   ,[FechaNacimiento]
                   ,[Fecha]
                   ,[Telefono]
                   ,[Direccion]
                   ,[Clinica])
             VALUES
                (@Nombre
                ,@Apellido
                ,@FechaNacimiento
                ,@Fecha
                ,@Telefono
                ,@Direccion
                ,@Clinica)
		                  ";

        //Modificar Pacientes
        public static string QueryModificarPacientes = @"                      					
			UPDATE [dbo].[Paciente]
                   SET [Nombre] = @Nombre
                      ,[Apellido] = @Apellido
                      ,[FechaNacimiento] = @FechaNacimiento
                      ,[Telefono] = @Telefono
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
                [Id],
                CONCAT([Nombre], ' ', [Apellido]) AS NombreCompleto,
                [FechaNacimiento],
                [Fecha],
                [Telefono],
                [Direccion],
                [clinica]
            FROM [dbo].[Paciente]
            WHERE Clinica = @Clinica
               ORDER BY Fecha desc
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
    }
}
