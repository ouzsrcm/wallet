-- Messages tablosunu oluştur
CREATE TABLE [dbo].[Messages] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    
    -- İlişkiler
    [SenderId] UNIQUEIDENTIFIER NOT NULL,
    [ReceiverId] UNIQUEIDENTIFIER NOT NULL,
    [ParentMessageId] UNIQUEIDENTIFIER NULL,
    
    -- Mesaj içeriği
    [Subject] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(4000) NOT NULL,
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadAt] DATETIME2 NULL,
    
    -- Audit kolonları
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    
    -- Soft delete
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    
    -- İlişki kısıtlamaları
    CONSTRAINT [FK_Messages_Users_SenderId] 
        FOREIGN KEY ([SenderId]) REFERENCES [dbo].[Users] ([Id])
        ON DELETE NO ACTION,
        
    CONSTRAINT [FK_Messages_Users_ReceiverId] 
        FOREIGN KEY ([ReceiverId]) REFERENCES [dbo].[Users] ([Id])
        ON DELETE NO ACTION,
        
    CONSTRAINT [FK_Messages_Messages_ParentMessageId] 
        FOREIGN KEY ([ParentMessageId]) REFERENCES [dbo].[Messages] ([Id])
        ON DELETE NO ACTION
);

-- İndeksler
CREATE INDEX [IX_Messages_SenderId] ON [dbo].[Messages] ([SenderId]);
CREATE INDEX [IX_Messages_ReceiverId] ON [dbo].[Messages] ([ReceiverId]);
CREATE INDEX [IX_Messages_ParentMessageId] ON [dbo].[Messages] ([ParentMessageId]);
CREATE INDEX [IX_Messages_IsRead] ON [dbo].[Messages] ([IsRead]);
CREATE INDEX [IX_Messages_CreatedDate] ON [dbo].[Messages] ([CreatedDate]);
CREATE INDEX [IX_Messages_IsDeleted] ON [dbo].[Messages] ([IsDeleted]);



-- MessageAttachments tablosunu oluştur
CREATE TABLE [dbo].[MessageAttachments] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [MessageId]     UNIQUEIDENTIFIER NOT NULL,
    [FileName]      NVARCHAR(255)    NOT NULL,
    [ContentType]   NVARCHAR(100)    NOT NULL,
    [FileSize]      BIGINT           NOT NULL,
    [FilePath]      NVARCHAR(1000)   NOT NULL,
    [CreatedDate]   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]     NVARCHAR(50)     NOT NULL,
    [ModifiedDate]  DATETIME2        NULL,
    [ModifiedBy]    NVARCHAR(50)     NULL,
    [IsDeleted]     BIT              NOT NULL DEFAULT 0,
    [DeletedAt]     DATETIME2        NULL,
    [DeletedByUserId] NVARCHAR(50)   NULL,

    CONSTRAINT [PK_MessageAttachments] 
        PRIMARY KEY CLUSTERED ([Id] ASC),
    
    CONSTRAINT [FK_MessageAttachments_Messages] 
        FOREIGN KEY ([MessageId]) 
        REFERENCES [dbo].[Messages] ([Id])
        ON DELETE CASCADE
);

-- İndeksler
CREATE NONCLUSTERED INDEX [IX_MessageAttachments_MessageId]
    ON [dbo].[MessageAttachments] ([MessageId] ASC);

CREATE NONCLUSTERED INDEX [IX_MessageAttachments_IsDeleted]
    ON [dbo].[MessageAttachments] ([IsDeleted] ASC);
