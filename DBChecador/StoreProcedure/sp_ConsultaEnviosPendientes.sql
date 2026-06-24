USE [DB_CHECADOR]
GO

/****** Object:  StoredProcedure [dbo].[sp_ConsultaEnviosPendientes]    Script Date: 26/11/2025 01:37:47 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Roberto Rosas
-- Create date: 24/11/2025
-- Description:	Consulta controla el flujo de envios de coerreos de checadas para el sistema CHECADOR WEB
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ConsultaEnviosPendientes]
	@param ntext
AS
BEGIN
/*
sp_ConsultaEnviosPendientes'<root><accion>C</accion></root>'
sp_ConsultaEnviosPendientes'<accion>C</accion>'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
*/
	DECLARE
    @accion         varchar(1),
	@NuIntentos		smallint,
	@ID				int,
	@xmlDoc         xml;

	BEGIN TRY	
		SET @xmlDoc = TRY_CAST(@param AS xml);

		IF @xmlDoc IS NULL
		BEGIN
			-- parámetro no es XML válido: lanzar error controlado
			RAISERROR('Parámetro XML inválido o nulo en @param', 16, 1);
			RETURN;
		END

		SELECT
			@accion			= r.value('(accion/text())[1]', 'varchar(1)'),
			@ID			= r.value('(ID/text())[1]', 'int')
		FROM @xmlDoc.nodes('/root') AS T(r);
		
		IF @accion = 'C' BEGIN
			

			SELECT  T.Correo,
					H.HashRegistro,
					CONVERT(varchar(10), H.Horaregistro, 103) + ' ' + CONVERT(varchar(8), H.Horaregistro, 14) AS Horaregistro,
					CASE WHEN H.TipoRegistro = 1 THEN 'ENTRADA'
						 WHEN H.TipoRegistro = 2 THEN 'SALIDA'
						 WHEN H.TipoRegistro = 3 THEN 'SALIDA' END AS TipoRegistro,
					T.Nombre + ' ' + ISNULL(T.ApellidoP, '') + ' ' + ISNULL(T.ApellifoM, '') AS NombreCompleto,
					H.ID,
					T.IDTrabajador
			FROM [dbo].[MM_TRABAJADORES] T
			INNER JOIN [dbo].[MM_HORARIO] H ON T.IDTrabajador = H.IDTrabajador
			WHERE H.[stEnviado] = 0
			AND ISNULL(H.NuIntentos, 0) < 2;

		END
		
		IF @accion = 'U' BEGIN
			SELECT 1 AS [Exito]
			
			SET @NuIntentos = (Select ISNULL(NuIntentos,0) FROM dbo.MM_HORARIO WHERE ID = @ID 
			AND stEnviado = 0) + 1;
			
			UPDATE dbo.MM_HORARIO
            SET stEnviado = 1,
			NuIntentos = @NuIntentos
            WHERE ID = @ID 
			AND stEnviado = 0;
		END 
		IF @accion = 'E' BEGIN
			SELECT 1 AS [Exito]
			
			SET @NuIntentos = (Select ISNULL(NuIntentos,0) FROM dbo.MM_HORARIO WHERE ID = @ID 
			AND stEnviado = 0) + 1;
			
			UPDATE dbo.MM_HORARIO
            SET NuIntentos = @NuIntentos
            WHERE ID = @ID 
			AND stEnviado = 0;
		END 

	END TRY
	BEGIN CATCH
		SELECT 0 AS EXITO, ERROR_MESSAGE() AS MENSAJE
	END CATCH
	

END
