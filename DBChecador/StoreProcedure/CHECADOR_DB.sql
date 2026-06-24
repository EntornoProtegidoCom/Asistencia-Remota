USE [DB_CHECADOR]
GO
/****** Object:  StoredProcedure [dbo].[sp_seguridad_usuarios]    Script Date: 12/06/2026 06:56:31 p. m. ******/
DROP PROCEDURE [dbo].[sp_seguridad_usuarios]
GO
/****** Object:  StoredProcedure [dbo].[sp_registro_checador]    Script Date: 12/06/2026 06:56:31 p. m. ******/
DROP PROCEDURE [dbo].[sp_registro_checador]
GO
/****** Object:  StoredProcedure [dbo].[sp_ConsultaEnviosPendientes]    Script Date: 12/06/2026 06:56:31 p. m. ******/
DROP PROCEDURE [dbo].[sp_ConsultaEnviosPendientes]
GO
/****** Object:  StoredProcedure [dbo].[sp_catalogo_trabajadores ]    Script Date: 12/06/2026 06:56:31 p. m. ******/
DROP PROCEDURE [dbo].[sp_catalogo_trabajadores ]
GO
ALTER TABLE [dbo].[MM_HORARIO] DROP CONSTRAINT [FK_MM_HORAR_REFERENCE_MM_TRABAJA]
GO
ALTER TABLE [dbo].[DD_HUELLAS] DROP CONSTRAINT [FK_DD_HUELA_REFERENCE_MM_EMPLE]
GO
ALTER TABLE [dbo].[MM_TRABAJADORES] DROP CONSTRAINT [DF__MM_TRABAJ__updat__160F4887]
GO
ALTER TABLE [dbo].[MM_TRABAJADORES] DROP CONSTRAINT [DF__MM_TRABAJ__creat__151B244E]
GO
ALTER TABLE [dbo].[MM_TRABAJADORES] DROP CONSTRAINT [DF__MM_TRABAJ__statu__14270015]
GO
ALTER TABLE [dbo].[MM_HORARIO] DROP CONSTRAINT [DF__MM_HORARI__NuInt__01142BA1]
GO
ALTER TABLE [dbo].[MM_HORARIO] DROP CONSTRAINT [DF__MM_HORARI__stEnv__6E01572D]
GO
/****** Object:  Table [dbo].[SS_USUARIOS]    Script Date: 12/06/2026 06:56:31 p. m. ******/
IF  EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[SS_USUARIOS]') AND type in (N'U'))
DROP TABLE [dbo].[SS_USUARIOS]
GO
/****** Object:  Table [dbo].[MM_TRABAJADORES]    Script Date: 12/06/2026 06:56:31 p. m. ******/
IF  EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[MM_TRABAJADORES]') AND type in (N'U'))
DROP TABLE [dbo].[MM_TRABAJADORES]
GO
/****** Object:  Table [dbo].[MM_HORARIO]    Script Date: 12/06/2026 06:56:31 p. m. ******/
IF  EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[MM_HORARIO]') AND type in (N'U'))
DROP TABLE [dbo].[MM_HORARIO]
GO
/****** Object:  Table [dbo].[DD_HUELLAS]    Script Date: 12/06/2026 06:56:31 p. m. ******/
IF  EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[DD_HUELLAS]') AND type in (N'U'))
DROP TABLE [dbo].[DD_HUELLAS]
GO
/****** Object:  Table [dbo].[DD_HUELLAS]    Script Date: 12/06/2026 06:56:31 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DD_HUELLAS]
(
	[IDTrabajador] [int] NULL,
	[Huella] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MM_HORARIO]    Script Date: 12/06/2026 06:56:31 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MM_HORARIO]
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDTrabajador] [int] NULL,
	[Horaregistro] [datetime] NOT NULL,
	[TipoRegistro] [smallint] NOT NULL,
	[IPRegistro] [varchar](20) NULL,
	[NBEquipo] [varchar](50) NULL,
	[Foto] [image] NULL,
	[HashRegistro] [varbinary](32) NULL,
	[stEnviado] [bit] NULL,
	[NuIntentos] [smallint] NULL,
	CONSTRAINT [PK_MM_HORARIO] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MM_TRABAJADORES]    Script Date: 12/06/2026 06:56:32 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MM_TRABAJADORES]
(
	[IDTrabajador] [int] NOT NULL,
	[Nombre] [varchar](20) NOT NULL,
	[ApellidoP] [varchar](20) NOT NULL,
	[ApellifoM] [varchar](20) NOT NULL,
	[Foto] [varbinary](max) NULL,
	[PIN] [int] NULL,
	[Contrasena] [varchar](10) NOT NULL,
	[IDCARD] [varchar](40) NULL,
	[Correo] [nvarchar](100) NULL,
	[status] [bit] NULL,
	[create_time] [datetime] NULL,
	[Create_ID_USUARIO] [varchar](25) NULL,
	[update_time] [datetime] NULL,
	[Update_ID_USUARIO] [varchar](25) NULL,
	[Foto2] [image] NULL,
	CONSTRAINT [PK_MM_EMPLEADOS] PRIMARY KEY CLUSTERED 
(
	[IDTrabajador] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SS_USUARIOS]    Script Date: 12/06/2026 06:56:32 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SS_USUARIOS]
(
	[ID_USUARIO] [varchar](25) NOT NULL,
	[ID_PERFIL] [int] NOT NULL,
	[NB_NOMBRE] [varchar](100) NULL,
	[NB_PATERNO] [varchar](100) NULL,
	[NB_MATERNO] [varchar](100) NULL,
	[TX_PASSWORD] [varchar](64) NULL,
	[NB_EMAIL] [varchar](100) NULL,
	[ST_VIGENTE] [varchar](1) NULL,
	CONSTRAINT [PK_SS_USUARIOS] PRIMARY KEY NONCLUSTERED 
(
	[ID_USUARIO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MM_HORARIO] ADD  DEFAULT ((0)) FOR [stEnviado]
GO
ALTER TABLE [dbo].[MM_HORARIO] ADD  DEFAULT ((0)) FOR [NuIntentos]
GO
ALTER TABLE [dbo].[MM_TRABAJADORES] ADD  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[MM_TRABAJADORES] ADD  DEFAULT (getdate()) FOR [create_time]
GO
ALTER TABLE [dbo].[MM_TRABAJADORES] ADD  DEFAULT (getdate()) FOR [update_time]
GO
ALTER TABLE [dbo].[DD_HUELLAS]  WITH CHECK ADD  CONSTRAINT [FK_DD_HUELA_REFERENCE_MM_EMPLE] FOREIGN KEY([IDTrabajador])
REFERENCES [dbo].[MM_TRABAJADORES] ([IDTrabajador])
GO
ALTER TABLE [dbo].[DD_HUELLAS] CHECK CONSTRAINT [FK_DD_HUELA_REFERENCE_MM_EMPLE]
GO
ALTER TABLE [dbo].[MM_HORARIO]  WITH CHECK ADD  CONSTRAINT [FK_MM_HORAR_REFERENCE_MM_TRABAJA] FOREIGN KEY([IDTrabajador])
REFERENCES [dbo].[MM_TRABAJADORES] ([IDTrabajador])
GO
ALTER TABLE [dbo].[MM_HORARIO] CHECK CONSTRAINT [FK_MM_HORAR_REFERENCE_MM_TRABAJA]
GO
/****** Object:  StoredProcedure [dbo].[sp_catalogo_trabajadores ]    Script Date: 12/06/2026 06:56:32 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [dbo].[sp_catalogo_trabajadores ]
	@param ntext,
	@Foto2 varbinary(max) = NULL
-- SEGUNDO PARAMETRO: Opcional, por defecto NULL
AS
BEGIN
	/*
Ejemplos de uso desde SQL Server:
-- Sin enviar fotografía:
EXEC [dbo].[sp_catalogo_trabajadores] @param = '<root><accion>C</accion></root>'

-- Enviando fotografía (suponiendo una cadena binaria):
EXEC [dbo].[sp_catalogo_trabajadores] @param = '<root><accion>I</accion>...</root>', @Foto2 = 0x01020304...
/*
sp_catalogo_trabajadores  '<root><accion>C</accion><txt>rene</txt></root>'
sp_catalogo_trabajadores  '<root><accion>C</accion><IDTrabajador>1</IDTrabajador><PIN>1234</PIN></root>'
sp_catalogo_trabajadores  '<root><accion>U</accion><IDTrabajador>30</IDTrabajador><Nombre>DISPONIBLE</Nombre><ApellidoP>temporal</ApellidoP><ApellidoM>temporal</ApellidoM><PIN>3475</PIN><Correo>TEMPORAL</Correo><status>1</status><Update_ID_USUARIO>usrAdmin</Update_ID_USUARIO><Foto2>/9j/4AAQSkZJRg...</Foto2></root>'
sp_catalogo_trabajadores  '<root><accion>I</accion><Nombre>ISABEL</Nombre><ApellidoP>TALZINTAN</ApellidoP><ApellidoM>MORALES</ApellidoM><PIN>9576</PIN><Correo>administracion18@urglobal.com</Correo><status>1</status><Create_ID_USUARIO>usrAdmin</Create_ID_USUARIO></root>'
*/
*/
	SET NOCOUNT ON

	DECLARE
    @accion             varchar(1),
    @IDTrabajador       int,
    @PIN                int,
    @Nombre			    varchar(20),
	@ApellidoP		    varchar(20),
	@ApellidoM		    varchar(20),
	@Correo			    nvarchar(100),
    @txt                varchar(60),
    @Filtro             NVARCHAR(100) = NULL,
    @FiltroLike         NVARCHAR(110) = NULL,
    @hDoc1              int,            
    @xmlDoc             xml,
    
    -- NUEVAS VARIABLES PARA LOS CAMPOS AGREGADOS (Excepto @Foto2 que ya viene arriba)
    @status             bit,
    @Create_ID_USUARIO  varchar(25),
    @Update_ID_USUARIO  varchar(25);

	BEGIN TRY
	SET @xmlDoc = TRY_CAST(@param AS xml);

    IF @xmlDoc IS NULL
    BEGIN
		RAISERROR('Parámetro XML inválido o nulo en @param', 16, 1);
		RETURN;
	END

    -- Extraer valores desde el nodo raíz usando métodos XML nativos
    SELECT
		@accion			    = r.value('(accion/text())[1]', 'varchar(1)'),
		@IDTrabajador	    = r.value('(IDTrabajador/text())[1]', 'int'),
		@PIN			    = r.value('(PIN/text())[1]', 'int'),
		@Nombre		        = r.value('(Nombre/text())[1]', 'varchar(20)'),
		@ApellidoP		    = r.value('(ApellidoP/text())[1]', 'varchar(20)'),
		@ApellidoM		    = r.value('(ApellidoM/text())[1]', 'varchar(20)'),
		@Correo		        = r.value('(Correo/text())[1]', 'nvarchar(100)'),
		@txt                = r.value('(txt/text())[1]', 'nvarchar(100)'),

		-- Mapeo de los nuevos nodos del XML
		@status             = r.value('(status/text())[1]', 'bit'),
		@Create_ID_USUARIO  = r.value('(Create_ID_USUARIO/text())[1]', 'varchar(25)'),
		@Update_ID_USUARIO  = r.value('(Update_ID_USUARIO/text())[1]', 'varchar(25)')
	FROM @xmlDoc.nodes('/root') AS T(r);	


	IF @accion = 'C' BEGIN
		SELECT 1

		IF (@txt IS NOT NULL AND LTRIM(RTRIM(@txt)) <> '')
            SET @Filtro = LTRIM(RTRIM(@txt));

		IF (@Filtro IS NOT NULL)
            SET @FiltroLike = N'%' + @Filtro + N'%';

		SELECT [IDTrabajador]
			  , [Nombre]
			  , [ApellidoP]
			  , [ApellifoM]
			  , ISNULL([ApellidoP],'') + ' ' + ISNULL([ApellifoM],'') + ' ' + ISNULL([Nombre],'') as [NombreCompleto]			  
			  , [PIN]
			  , [Correo]
			  , [status]
			  , [create_time]
			  , [Create_ID_USUARIO]
			  , [update_time]
			  , [Update_ID_USUARIO]
		FROM [dbo].[MM_TRABAJADORES] T
		Where (@IDTrabajador IS NULL OR T.IDTrabajador = @IDTrabajador)
			AND (
                @FiltroLike IS NULL
			OR (
                    T.[Nombre]     COLLATE Modern_Spanish_CI_AI LIKE @FiltroLike COLLATE Modern_Spanish_CI_AI
			OR T.[ApellidoP]  COLLATE Modern_Spanish_CI_AI LIKE @FiltroLike COLLATE Modern_Spanish_CI_AI
			OR T.[ApellifoM]  COLLATE Modern_Spanish_CI_AI LIKE @FiltroLike COLLATE Modern_Spanish_CI_AI
			OR CAST(T.IDTrabajador AS NVARCHAR(20)) LIKE @FiltroLike  
                )
            )
		order by [NombreCompleto]

	END


	IF @accion = 'I' BEGIN

		SET @IDTrabajador = (SELECT MAX(ISNULL(IDTrabajador,0))
		FROM [dbo].[MM_TRABAJADORES]) + 1

		INSERT INTO [dbo].[MM_TRABAJADORES]
			([IDTrabajador]
			,[Nombre]
			,[ApellidoP]
			,[ApellifoM]
			,[PIN]
			,[Correo]
			,[Contrasena]
			,[status]
			,[create_time]
			,[Create_ID_USUARIO]
			,[update_time]
			,[Update_ID_USUARIO]
			,[Foto2])
		VALUES
			(@IDTrabajador
                       , @Nombre 
                       , @ApellidoP
                       , @ApellidoM
                       , @PIN
                       , @Correo
                       , 'URGL' + convert(varchar(4), @PIN)
                       , ISNULL(@status, 0)
                       , GETDATE()
                       , @Create_ID_USUARIO
                       , GETDATE()
                       , @Create_ID_USUARIO
                       , @Foto2)
		-- Utiliza el parámetro opcional recibido

		SELECT 1 AS [EXITO]

	END
	
	IF @accion = 'U' BEGIN

		UPDATE [dbo].[MM_TRABAJADORES]
        SET [Nombre] = @Nombre
            ,[ApellidoP] = @ApellidoP
            ,[ApellifoM] = @ApellidoM
            ,[PIN] = @PIN
            ,[Correo] = @Correo
            ,[status] = ISNULL(@status, [status])
            ,[update_time] = GETDATE()
            ,[Update_ID_USUARIO] = ISNULL(@Update_ID_USUARIO, [Update_ID_USUARIO])
            ,[Foto2] = ISNULL(@Foto2, [Foto2]) -- Mantiene la foto anterior si @Foto2 es NULL
        WHERE [IDTrabajador] = @IDTrabajador

		SELECT 1 AS [Exito]

	END

	END TRY
	BEGIN CATCH
		SELECT 0 AS EXITO, ERROR_MESSAGE() AS MENSAJE
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ConsultaEnviosPendientes]    Script Date: 12/06/2026 06:56:32 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Roberto Rosas
-- Create date: 24/11/2025
-- Description:	Consulta controla el flujo de envios de coerreos de checadas para el sistema CHECADOR WEB
-- =============================================
CREATE PROCEDURE [dbo].[sp_ConsultaEnviosPendientes]
	@param ntext
AS
BEGIN
	--sp_ConsultaEnviosPendientes'<root><accion>C</accion></root>'
	--sp_ConsultaEnviosPendientes'<accion>C</accion>'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
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


		SELECT T.Correo,
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
		UPDATE dbo.MM_HORARIO
            SET stEnviado = 1
            WHERE ID = @ID
			AND stEnviado = 0;
	END 
		IF @accion = 'E' BEGIN
		SELECT 1 AS [Exito]

		SET @NuIntentos = (Select ISNULL(NuIntentos,0)
		FROM dbo.MM_HORARIO
		WHERE ID = @ID
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
GO
/****** Object:  StoredProcedure [dbo].[sp_registro_checador]    Script Date: 12/06/2026 06:56:32 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_registro_checador]
	@param ntext,
	@img_data image = null
AS
BEGIN
	/*
sp_registro_checador '<root><accion>I</accion><IDTrabajador>1</IDTrabajador><PIN>2422</PIN></root>'
sp_registro_checador '<root><accion>C</accion><IDTrabajador>25</IDTrabajador><PIN>165</PIN></root>'
sp_registro_checador '<root><accion>M</accion><HashBuscar>3D953D0CC26550DC7F092EE292A990E434322C9A4771EFB73B6D69AABD8AABC4</HashBuscar></root>'
sp_registro_checador '<root><accion>M</accion><IDBuscar>62583</IDBuscar></root>'
sp_registro_checador '<root><accion>T</accion><IDTrabajador>13</IDTrabajador><fhinicial>2024-07-23</fhinicial><fhFinal>2025-09-04</fhFinal></root>'
sp_registro_checador '<root><accion>W</accion><IDTrabajador>14</IDTrabajador></root>'
sp_registro_checador '<root><accion>F</accion><IDBuscar>63586</IDBuscar></root>'
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
		print @HashBuscar
		SELECT
			T.IDTrabajador,
			T.Nombre + ' ' + ISNULL(T.ApellidoP,'') + ' ' + ISNULL(T.ApellifoM,'') AS NombreCompleto,
			T.Correo,
			LOWER(CONVERT(VARCHAR(512), H.HashRegistro, 2)) AS HashHex,
			CONVERT(varchar(10), H.HoraRegistro, 103) + ' ' + CONVERT(varchar(8), H.HoraRegistro, 14) AS HoraRegistro,
			CASE WHEN H.TipoRegistro = 1 THEN 'ENTRADA'
				 WHEN H.TipoRegistro = 2 THEN 'SALIDA'
				 WHEN H.TipoRegistro = 3 THEN 'SALIDA' END AS TipoRegistro,
			H.ID,
			H.Foto
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
	IF @accion = 'F' BEGIN
		SELECT 1 AS EXITO

		Select H.Foto
		FROM [dbo].[MM_HORARIO] H
		WHERE H.ID = @IDBuscar

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
			SET @Base = CAST(@IDTrabajador AS VARCHAR(10)) + '|' + CONVERT(VARCHAR(30), @Ahora, 126);
			-- ISO 8601 (yyyy-mm-ddThh:mm:ss.mmm)

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
				IF (select TOP 1
					datediff(mi, [Horaregistro], GETDATE())
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
					SET @TipoRegistro = (SELECT TOP 1
						H.TipoRegistro
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
						,[Foto]
						)
					VALUES
						(@IDTrabajador,
							@Ahora,
							@TipoRegistro,
							@IPRegistro,
							@NBEquipo,
							@Hash,
							0,
							@img_data
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
					,[Foto]
					)
				VALUES
					(@IDTrabajador,
						@Ahora,
						@TipoRegistro,
						@IPRegistro,
						@NBEquipo,
						@Hash,
						0,
						@img_data
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
			ISNULL(h.NBEquipo, '') AS NBEquipo,
			h.ID
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
	
	
GO
/****** Object:  StoredProcedure [dbo].[sp_seguridad_usuarios]    Script Date: 12/06/2026 06:56:32 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_seguridad_usuarios]
	@param ntext
AS
BEGIN
	/*
sp_seguridad_usuarios '<root><accion>C</accion><ID_USUARIO>1</ID_USUARIO><Password>Arena2auto</Password></root>'
*******************************************************************************
**   File: sp_seguridad_usuarios.sql
**   Name: sp_seguridad_usuarios
**   Desc: Procedimiento para validar usuario
**
**   Auth: Roberto Rosas
**   Date: 20-10-2025
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
    @ID_USUARIO     varchar(25),
    @Password       NVARCHAR(4000),   
    @HashedPassword VARCHAR(64),
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
		@ID_USUARIO	= r.value('(ID_USUARIO/text())[1]', 'nvarchar(25)'),
		@Password		= r.value('(Password/text())[1]', 'nvarchar(50)')

	FROM @xmlDoc.nodes('/root') AS T(r);	

    set @HashedPassword = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', @Password), 2);

	IF @accion = 'C' BEGIN

		SELECT
			CASE WHEN EXISTS(
            SELECT 1
			FROM dbo.SS_USUARIOS
			WHERE ID_USUARIO = @ID_USUARIO
				AND TX_PASSWORD = @HashedPassword
				AND ISNULL(ST_VIGENTE, 0) = 'A'
        ) THEN 1 ELSE 0 END AS IsValid;


	END	
	
	END TRY
	BEGIN CATCH
		SELECT 0 AS EXITO, ERROR_MESSAGE() AS MENSAJE
	END CATCH
END
	
	
GO
