USE [DB_CHECADOR]
GO
/****** Object:  StoredProcedure [dbo].[sp_registro_checador]    Script Date: 17/06/2026 01:06:54 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[sp_registro_checador]
	@param ntext	
AS
BEGIN
/*
sp_registro_checador '<root><accion>I</accion><IDTrabajador>1</IDTrabajador><PIN>2422</PIN></root>'
sp_registro_checador '<root><accion>C</accion><IDTrabajador>25</IDTrabajador><PIN>165</PIN></root>'
sp_registro_checador '<root><accion>M</accion><HashBuscar>B617BAD992E8346489E94464099FE95766D2C81650E148671444774D49971F60</HashBuscar></root>'
sp_registro_checador '<root><accion>M</accion><IDBuscar>25</IDBuscar></root>'
sp_registro_checador '<root><accion>T</accion><fhinicial>2024-05-01</fhinicial><fhFinal>2025-12-03</fhFinal></root>'
sp_registro_checador '<root><accion>W</accion><IDTrabajador>14</IDTrabajador></root>'
*******************************************************************************
**   File: sp_registro_checador.sql
**   Name: sp_registro_checador
**   Desc: Procedimiento para obtener los registros del checador
**
**   Auth: Roberto Rosas
**   Date: 07-05-2024
*******************************************************************************
**   Change History
*******************************************************************************
**   Date:      Author:       Description:
**   --------   --------      -------------------------------------------
**    
*******************************************************************************/
set nocount on
DECLARE
    @accion         varchar(1),
    @IDTrabajador   int,
    @PIN            int,
    @Horaregistro   datetime = GETDATE(),
    @TipoRegistro   smallint = 1,
    @IPRegistro     nvarchar(50),
    @NBEquipo       nvarchar(50),
    @fhinicial      datetime,
    @fhFinal        datetime,
	@Base			NVARCHAR(100),
	@Ahora			DATETIME,
	@Hash			VARBINARY(32),
	@HashBuscar    VARCHAR(512),
	@IDBuscar		int,
    @hDoc1          int,            -- ya no se usa con XML nativo, pero lo dejo por compatibilidad si hace falta
    @xmlDoc         xml;
BEGIN TRY
	SET @xmlDoc = TRY_CAST(@param AS xml);

    IF @xmlDoc IS NULL
    BEGIN
        -- parámetro no es XML válido: lanzar error controlado
        RAISERROR('Parámetro XML inválido o nulo en @param', 16, 1);
        RETURN;
    END

    -- Extraer valores desde el nodo raíz usando métodos XML nativos
    SELECT
        @accion			= r.value('(accion/text())[1]', 'varchar(1)'),
        @IDTrabajador	= r.value('(IDTrabajador/text())[1]', 'int'),
        @PIN			= r.value('(PIN/text())[1]', 'int'),
        -- Si hay Horaregistro en XML lo parseamos; si no, mantenemos GETDATE() por defecto
        --@Horaregistro	= COALESCE(TRY_CAST(r.value('(Horaregistro/text())[1]', 'nvarchar(50)') AS datetime), @Horaregistro),
        @IPRegistro		= r.value('(IPRegistro/text())[1]', 'nvarchar(50)'),
        @NBEquipo		= r.value('(NBEquipo/text())[1]', 'nvarchar(50)'),
        @fhinicial		= TRY_CAST(r.value('(fhinicial/text())[1]', 'nvarchar(50)') AS datetime),
        @fhFinal		= TRY_CAST(r.value('(fhFinal/text())[1]', 'nvarchar(50)') AS datetime),
		@HashBuscar		= r.value('(HashBuscar/text())[1]', 'varchar(512)'),
		@IDBuscar		= r.value('(IDBuscar/text())[1]', 'int')
    FROM @xmlDoc.nodes('/root') AS T(r);	


	IF @accion = 'C' BEGIN
		IF EXISTS ( SELECT 1 
						FROM [dbo].[MM_TRABAJADORES]
						WHERE 	IDTrabajador = @IDTrabajador
						AND PIN = @PIN)
			BEGIN
			
			SELECT 1 AS EXITO

			SELECT H.IDTrabajador,
					T.Nombre + ' ' + ISNULL(T.ApellidoP, '') + ' ' + ISNULL(T.ApellifoM, '') AS NombreCompleto,
					CONVERT(varchar(10), H.Horaregistro,103) + ' ' + CONVERT(varchar(8), H.Horaregistro,14) AS Horaregistro,
					CASE  WHEN H.TipoRegistro = 1 THEN 'ENTRADA'
						  WHEN H.TipoRegistro = 2 THEN 'SALIDA'
						  WHEN H.TipoRegistro = 3 THEN 'SALIDA' END AS TipoRegistro,
					H.NBEquipo,
					T.Correo
			FROM [dbo].[MM_HORARIO] H
			INNER JOIN [dbo].[MM_TRABAJADORES] T ON T.IDTrabajador = H.IDTrabajador
			WHERE H.IDTrabajador = @IDTrabajador
			AND H.Horaregistro >= DATEADD(DAY, -7, GETDATE())
			ORDER BY H.Horaregistro
		END
		ELSE BEGIN
			SELECT 2 AS EXITO, 'EL USUARIO Y/0 CONTRASEÑA NO SON CORRECTOS, INTENTELO DE NUEVO.'
		END
	END

	IF @accion = 'M' BEGIN
		
		Select 1 AS 'Exito'

		SELECT 
			T.IDTrabajador,
			T.Nombre + ' ' + ISNULL(T.ApellidoP,'') + ' ' + ISNULL(T.ApellifoM,'') AS NombreCompleto,
			T.Correo,
			LOWER(CONVERT(VARCHAR(512), H.HashRegistro, 2)) AS HashHex,
			CONVERT(varchar(10), H.HoraRegistro, 103) + ' ' + CONVERT(varchar(8), H.HoraRegistro, 14) AS HoraRegistro,
			CASE WHEN H.TipoRegistro = 1 THEN 'ENTRADA'
				 WHEN H.TipoRegistro = 2 THEN 'SALIDA'
				 WHEN H.TipoRegistro = 3 THEN 'SALIDA' END AS TipoRegistro,
			H.ID
		FROM dbo.MM_TRABAJADORES T
		INNER JOIN dbo.MM_HORARIO H ON T.IDTrabajador = H.IDTrabajador
		WHERE 
			(@IDBuscar IS NULL OR H.ID = @IDBuscar OR T.IDTrabajador = @IDBuscar)
			AND (@HashBuscar IS NULL OR LOWER(CONVERT(VARCHAR(512), H.HashRegistro, 2)) = @HashBuscar)
		ORDER BY H.HoraRegistro DESC;
	END

	IF @accion = 'W' BEGIN
		
		Select 1 AS 'Exito'
		
		SELECT W.IDTrabajador,
		ISNULL(W.ApellidoP,'') + ' ' + ISNULL(W.ApellifoM,'') + ' ' + ISNULL(W.Nombre,'') AS NombreCompleto
		FROM [dbo].[MM_TRABAJADORES] W
		order by W.ApellidoP

	END

	
	IF @accion = 'I' 
	BEGIN
		--VALIDA SI EL USUARIO Y PASSWORD SON CORRECTOS
		IF EXISTS ( SELECT 1 
					FROM [dbo].[MM_TRABAJADORES]
					WHERE 	IDTrabajador = @IDTrabajador
					AND PIN = @PIN)
		BEGIN

			SET @Ahora	= GETDATE()

			/* Cadena base (normalizada) */
			SET @Base = CAST(@IDTrabajador AS VARCHAR(10)) + '|' + CONVERT(VARCHAR(30), @Ahora, 126); -- ISO 8601 (yyyy-mm-ddThh:mm:ss.mmm)
							
			-- SHA-256 disponible desde SQL Server 2012; si usas 2008 R2 cambia a 'SHA1' o 'MD5'
			SET @Hash =HASHBYTES('SHA2_256', @Base);

			--VALIDA SI ES EL PRIMER REGISTRO DEL DÍA
			if EXISTS (	
						SELECT 1 
						FROM [dbo].[MM_HORARIO] 
						WHERE [IDTrabajador] = @IDTrabajador 
						AND CONVERT(varchar(10), Horaregistro,103) = CONVERT(varchar(10), getdate(),103)
						--AND DAY([Horaregistro]) = DAY(GETDATE())
						) 
			BEGIN
				--REVISA SI HAN PASADO AL MENOS 5 DESDE EL UTLIMO REGISTRO REALIZADO.
				IF (select TOP 1 datediff(mi, [Horaregistro], GETDATE())
					FROM [dbo].[MM_HORARIO] 
						WHERE [IDTrabajador] = @IDTrabajador 
						AND CONVERT(varchar(10), Horaregistro,103) = CONVERT(varchar(10), getdate(),103)
						--AND DAY([Horaregistro]) = DAY(GETDATE())
						ORDER BY [Horaregistro] DESC) <= 5
				BEGIN						
					select 2 as EXITO, 'DEBE ESPERAR EL MENOS 5 MINUTOS PARA REALIZAR UN SEGUNDO REGISTRO'
				END
				--SI HAN PASADO 5 MINUTOS VALIDA EL TIPO DEL REGISTRO MAS REVIENTE
				ELSE BEGIN
					SET @TipoRegistro = (SELECT TOP 1  H.TipoRegistro
											FROM [dbo].[MM_HORARIO] H
											WHERE [IDTrabajador] = @IDTrabajador 
											AND CONVERT(varchar(10), Horaregistro,103) = CONVERT(varchar(10), getdate(),103)
											--AND DAY([Horaregistro]) = DAY(GETDATE())
											ORDER BY [Horaregistro] DESC)
					IF @TipoRegistro = 1 begin
						SET @TipoRegistro = 2
					END
					ELSE BEGIN
						SET @TipoRegistro = 1
					END
						BEGIN TRY	
							

							INSERT INTO [dbo].[MM_HORARIO]
								([IDTrabajador]
								,[Horaregistro]
								,[TipoRegistro]
								,[IPRegistro]
								,[NBEquipo]
								,[HashRegistro]
								,[stEnviado]
								)
							VALUES
								(@IDTrabajador,
								@Ahora,
								@TipoRegistro,
								@IPRegistro,
								@NBEquipo,
								@Hash,
								0
								)

							SELECT 1 AS EXITO, 'SE GRABO SU REGISTRO CON EXITO' AS MENSAJE

							SELECT Correo,
								@Hash as rHash,
								CONVERT(varchar(10), @Ahora,103) + ' ' + CONVERT(varchar(8), @Ahora,14) AS Horaregistro,
								CASE  WHEN @TipoRegistro = 1 THEN 'ENTRADA'
									  WHEN @TipoRegistro = 2 THEN 'SALIDA'
									  WHEN @TipoRegistro = 3 THEN 'SALIDA' END AS TipoRegistro,
								Nombre + ' ' + ISNULL(ApellidoP, '') + ' ' + ISNULL(ApellifoM, '') AS NombreCompleto
							FROM [dbo].[MM_TRABAJADORES] 
							Where [IDTrabajador] = @IDTrabajador
							

						END TRY
						BEGIN CATCH
							SELECT 0 AS EXITO, ERROR_MESSAGE() AS MENSAJE
						END CATCH

				END
			END
			--SI ES EL PRIMER REGISTRO LO INSERTA
			ELSE BEGIN
				BEGIN TRY	
					INSERT INTO [dbo].[MM_HORARIO]
						([IDTrabajador]
						,[Horaregistro]
						,[TipoRegistro]
						,[IPRegistro]
						,[NBEquipo]
						,[HashRegistro]
						,[stEnviado]
						)
					VALUES
						(@IDTrabajador,
						@Ahora,
						@TipoRegistro,
						@IPRegistro,
						@NBEquipo,
						@Hash,
						0
						)

					SELECT 1 AS EXITO, 
					'SE GRABO SU REGISTRO CON EXITO' AS MENSAJE 
					

					SELECT Correo,
						@Hash as rHash,
						CONVERT(varchar(10), @Ahora,103) + ' ' + CONVERT(varchar(8), @Ahora,14) AS Horaregistro,
						CASE  WHEN @TipoRegistro = 1 THEN 'ENTRADA'
						  WHEN @TipoRegistro = 2 THEN 'SALIDA'
						  WHEN @TipoRegistro = 3 THEN 'SALIDA' END AS TipoRegistro,
						Nombre + ' ' + ISNULL(ApellidoP, '') + ' ' + ISNULL(ApellifoM, '') AS NombreCompleto
					FROM [dbo].[MM_TRABAJADORES] 
					Where [IDTrabajador] = @IDTrabajador

				END TRY
				BEGIN CATCH
					SELECT 0 AS EXITO, ERROR_MESSAGE() AS MENSAJE
				END CATCH
			END
		END
		-- SI NO ES CORRECTO EL USUARIO Y/0 PASSSWORD EVNIAMOS MENSAJE
		ELSE BEGIN
			SELECT 2 AS EXITO, 'EL USUARIO Y/0 CONTRASEÑA NO SON CORRECTOS, INTENTELO DE NUEVO.'
		END
	END
		
	IF @accion = 'T' BEGIN

		Select 1 AS 'Exito'

		SELECT top 1000
			h.IDTrabajador    AS IDTrabajador,
			ISNULL(t.Nombre, '') + ' ' + ISNULL(t.ApellidoP, '') + ' ' + ISNULL(t.ApellifoM, '')  AS NombreCompleto,
			-- Formato dd/MM/yyyy HH:mm
			CONVERT(VARCHAR(10), h.Horaregistro, 103) + ' ' + LEFT(CONVERT(VARCHAR(8), h.Horaregistro, 108),5) AS Horaregistro,
			CASE 
				WHEN h.TipoRegistro = 1 THEN 'Entrada'
				WHEN h.TipoRegistro = 2 THEN 'Salida'
				ELSE 'Desconocido'
			END AS TipoRegistro,
			ISNULL(h.NBEquipo, '') AS NBEquipo
		FROM dbo.MM_HORARIO AS h
		LEFT JOIN dbo.MM_Trabajadores AS t
			ON h.IDTrabajador = t.IDTrabajador
		WHERE h.IDTrabajador = ISNULL(@IDTrabajador, h.IDTrabajador)
		AND h.Horaregistro >= ISNULL(@fhinicial, '1900-01-01')
		AND h.Horaregistro < DATEADD(day, 1, ISNULL(@fhFinal, GETDATE()))
		ORDER BY h.Horaregistro DESC;

	END
	END TRY
	BEGIN CATCH
		SELECT 0 AS EXITO, ERROR_MESSAGE() AS MENSAJE
	END CATCH
END
	
	
