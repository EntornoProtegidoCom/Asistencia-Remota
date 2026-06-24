USE [DB_CHECADOR]
GO

/****** Object:  StoredProcedure [dbo].[sp_catalogo_trabajadores ]    Script Date: 26/11/2025 01:36:20 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER   PROCEDURE [dbo].[sp_catalogo_trabajadores]
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