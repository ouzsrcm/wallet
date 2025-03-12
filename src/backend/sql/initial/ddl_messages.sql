-- Messages tablosunu oluştur
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Messages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Messages] (
        -- Primary Key
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
    
    -- Açıklama ekle
    EXEC sp_addextendedproperty 
        @name = N'MS_Description',
        @value = N'Kullanıcılar arası mesajlaşma tablosu',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'Messages';
        
    -- Kolon açıklamaları
    EXEC sp_addextendedproperty 
        @name = N'MS_Description',
        @value = N'Mesajın benzersiz kimliği',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'Messages',
        @level2type = N'COLUMN',
        @level2name = N'Id';
        
    EXEC sp_addextendedproperty 
        @name = N'MS_Description',
        @value = N'Mesajı gönderen kullanıcının kimliği',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'Messages',
        @level2type = N'COLUMN',
        @level2name = N'SenderId';
        
    -- ... diğer kolonlar için benzer açıklamalar
END;

-- Örnek test verileri
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Messages])
BEGIN
    DECLARE @JohnId UNIQUEIDENTIFIER;
    DECLARE @JaneId UNIQUEIDENTIFIER;
    DECLARE @AhmetId UNIQUEIDENTIFIER;

    SELECT @JohnId = u.Id
    FROM Users u
    INNER JOIN UserCredentials uc ON u.Id = uc.UserId
    WHERE uc.Username = 'johndoe';

    SELECT @JaneId = u.Id
    FROM Users u
    INNER JOIN UserCredentials uc ON u.Id = uc.UserId
    WHERE uc.Username = 'janesmith';

    SELECT @AhmetId = u.Id
    FROM Users u
    INNER JOIN UserCredentials uc ON u.Id = uc.UserId
    WHERE uc.Username = 'ahmetyilmaz';

    -- İlk mesaj zinciri
    DECLARE @WelcomeMessageId UNIQUEIDENTIFIER = NEWID();
    INSERT INTO [dbo].[Messages] (
        [Id],
        [SenderId], 
        [ReceiverId], 
        [Subject], 
        [Content], 
        [IsRead],
        [ReadAt],
        [CreatedDate],
        [CreatedBy],
        [CreatedByIp]
    )
    VALUES (
        @WelcomeMessageId,
        @JohnId,
        @JaneId,
        'Welcome to Wallet App',
        'Hi Jane, welcome to our new financial management platform. Let me know if you need any help!',
        1,
        DATEADD(MINUTE, 5, GETDATE()),
        DATEADD(DAY, -3, GETDATE()),
        'johndoe',
        '127.0.0.1'
    );

    -- Jane'in yanıtı
    INSERT INTO [dbo].[Messages] (
        [SenderId], 
        [ReceiverId], 
        [ParentMessageId],
        [Subject], 
        [Content], 
        [IsRead],
        [ReadAt],
        [CreatedDate],
        [CreatedBy],
        [CreatedByIp]
    )
    VALUES (
        @JaneId,
        @JohnId,
        @WelcomeMessageId,
        'Re: Welcome to Wallet App',
        'Thanks John! The platform looks great. I have a few questions about the budget tracking features.',
        1,
        DATEADD(MINUTE, 15, GETDATE()),
        DATEADD(DAY, -3, GETDATE()),
        'janesmith',
        '127.0.0.1'
    );

    -- Okunmamış yeni mesaj
    INSERT INTO [dbo].[Messages] (
        [SenderId], 
        [ReceiverId], 
        [Subject], 
        [Content], 
        [IsRead],
        [CreatedDate],
        [CreatedBy],
        [CreatedByIp]
    )
    VALUES (
        @JaneId,
        @AhmetId,
        'Question about Budget Templates',
        'Hi Ahmet, I saw that you created some great budget templates. Would you mind sharing how you structured your categories?',
        0,
        DATEADD(HOUR, -1, GETDATE()),
        'janesmith',
        '127.0.0.1'
    );
END; 