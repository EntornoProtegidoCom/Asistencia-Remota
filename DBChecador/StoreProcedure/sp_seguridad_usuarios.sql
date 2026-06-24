USE [DB_CHECADOR]
GO

/****** Object:  StoredProcedure [dbo].[sp_seguridad_usuarios]    Script Date: 26/11/2025 01:38:50 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[sp_seguridad_usuarios]
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

