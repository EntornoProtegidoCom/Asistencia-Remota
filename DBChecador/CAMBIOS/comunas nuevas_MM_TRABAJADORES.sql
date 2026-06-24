USE [DB_CHECADOR];
GO

-- 1. Agregar columna 'status' (bit, con valor por defecto 0)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[MM_TRABAJADORES]') AND name = 'status')
BEGIN
    ALTER TABLE [dbo].[MM_TRABAJADORES] 
    ADD [status] BIT NOT NULL DEFAULT 0;
END
GO

-- 2. Agregar columna 'create_time' (datetime, con valor por defecto la fecha actual)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[MM_TRABAJADORES]') AND name = 'create_time')
BEGIN
    ALTER TABLE [dbo].[MM_TRABAJADORES] 
    ADD [create_time] DATETIME NOT NULL DEFAULT GETDATE();
END
GO

-- 3. Agregar columna 'Create_ID_USUARIO' (varchar(25))
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[MM_TRABAJADORES]') AND name = 'Create_ID_USUARIO')
BEGIN
    ALTER TABLE [dbo].[MM_TRABAJADORES] 
    ADD [Create_ID_USUARIO] VARCHAR(25) NULL;
END
GO

-- 4. Agregar columna 'update_time' (datetime, con valor por defecto la fecha actual)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[MM_TRABAJADORES]') AND name = 'update_time')
BEGIN
    ALTER TABLE [dbo].[MM_TRABAJADORES] 
    ADD [update_time] DATETIME NOT NULL DEFAULT GETDATE();
END
GO

-- 5. Agregar columna 'Update_ID_USUARIO' (varchar(25))
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[MM_TRABAJADORES]') AND name = 'Update_ID_USUARIO')
BEGIN
    ALTER TABLE [dbo].[MM_TRABAJADORES] 
    ADD [Update_ID_USUARIO] VARCHAR(25) NULL;
END
GO

-- 6. Agregar columna 'Foto2' (image o varbinary(max))
-- Nota: Aunque usaste 'image' en el alter inicial, recuerda que en versiones actuales de SQL Server 
-- 'image' está deprecado. Se recomienda 'varbinary(max)', pero mantengo 'image' por compatibilidad con tu requerimiento.
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[MM_TRABAJADORES]') AND name = 'Foto2')
BEGIN
    ALTER TABLE [dbo].[MM_TRABAJADORES] 
    ADD [Foto2] IMAGE NULL;
END
GO