-- AuditLogs tablosunu oluştur
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        -- Primary Key (IEntity'den)
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        
        -- Audit kolonları (AuditableEntity'den)
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedByIp] NVARCHAR(50) NULL,
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedByIp] NVARCHAR(50) NULL,
        
        -- Soft delete kolonları (SoftDeleteEntity'den)
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [DeletedAt] DATETIME2 NULL,
        [DeletedByUserId] NVARCHAR(100) NULL,
        [DeletedByIp] NVARCHAR(50) NULL,
        
        -- AuditLog spesifik kolonlar
        [EntityName] NVARCHAR(100) NOT NULL,
        [ActionType] NVARCHAR(50) NOT NULL,
        [UserId] NVARCHAR(100) NOT NULL,
        [UserName] NVARCHAR(100) NOT NULL,
        [ActionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [OldValues] NVARCHAR(4000) NULL,
        [NewValues] NVARCHAR(4000) NULL,
        [AffectedColumns] NVARCHAR(1000) NULL,
        [PrimaryKey] NVARCHAR(100) NULL,
        [TableName] NVARCHAR(100) NULL,
        [IPAddress] NVARCHAR(50) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [RequestUrl] NVARCHAR(500) NULL,
        [RequestMethod] NVARCHAR(20) NULL,
        [RequestBody] NVARCHAR(4000) NULL,
        [ResponseStatusCode] INT NULL,
        [ErrorMessage] NVARCHAR(4000) NULL,
        [StackTrace] NVARCHAR(4000) NULL,
        [AdditionalInfo] NVARCHAR(4000) NULL
    );

    -- İndeksler
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_ActionDate] ON [dbo].[AuditLogs] ([ActionDate]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityName] ON [dbo].[AuditLogs] ([EntityName]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_ActionType] ON [dbo].[AuditLogs] ([ActionType]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_IsDeleted] ON [dbo].[AuditLogs] ([IsDeleted]);

    -- Örnek veri ekle
    INSERT INTO [dbo].[AuditLogs] (
        [EntityName],
        [ActionType],
        [UserId],
        [UserName],
        [ActionDate],
        [OldValues],
        [NewValues],
        [TableName],
        [IPAddress],
        [RequestMethod],
        [CreatedBy]
    )
    VALUES 
    (
        'User',
        'Create',
        'system',
        'System',
        GETDATE(),
        NULL,
        '{"Username": "johndoe", "Email": "john@example.com"}',
        'Users',
        '127.0.0.1',
        'POST',
        'System'
    ),
    (
        'User',
        'Update',
        'system',
        'System',
        GETDATE(),
        '{"Email": "john@example.com"}',
        '{"Email": "john.doe@example.com"}',
        'Users',
        '127.0.0.1',
        'PUT',
        'System'
    ),
    (
        'User',
        'Delete',
        'system',
        'System',
        GETDATE(),
        '{"Id": "...", "Username": "johndoe"}',
        NULL,
        'Users',
        '127.0.0.1',
        'DELETE',
        'System'
    );
END;