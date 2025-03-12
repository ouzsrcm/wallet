-- Languages tablosu
CREATE TABLE [dbo].[Languages] (
    -- Primary Key
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    
    -- Language entity özellikleri
    [Code] NVARCHAR(10) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [NativeName] NVARCHAR(100) NULL,
    [FlagUrl] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [LocalizationCode] NVARCHAR(20) NULL,
    [DateFormat] NVARCHAR(50) NULL,
    [TimeFormat] NVARCHAR(50) NULL,
    [CurrencyFormat] NVARCHAR(50) NULL,
    [NumberFormat] NVARCHAR(50) NULL,
    
    -- AuditableEntity'den gelen alanlar
    [CreatedBy] NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedBy] NVARCHAR(50) NULL,
    [ModifiedDate] DATETIME2 NULL,
    [RowVersion] INT NOT NULL DEFAULT 0,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Browser] NVARCHAR(50) NULL,
    [BrowserVersion] NVARCHAR(50) NULL,
    [Platform] NVARCHAR(50) NULL,
    [DeviceType] NVARCHAR(50) NULL,
    [OperatingSystem] NVARCHAR(50) NULL,
    
    -- SoftDeleteEntity'den gelen alanlar
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedBy] NVARCHAR(50) NULL,
    [DeletedDate] DATETIME2 NULL,
    [DeletedIpAddress] NVARCHAR(50) NULL,
    [DeletedUserAgent] NVARCHAR(500) NULL,
    [DeletionReason] NVARCHAR(500) NULL,
    
    CONSTRAINT [PK_Languages] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Languages_Code] UNIQUE ([Code]),
    CONSTRAINT [UQ_Languages_IsDefault] UNIQUE ([IsDefault])
);

-- İndeksler
CREATE NONCLUSTERED INDEX [IX_Languages_Code] 
ON [dbo].[Languages] ([Code]) 
INCLUDE ([Name], [IsActive], [IsDeleted]);

CREATE NONCLUSTERED INDEX [IX_Languages_IsActive] 
ON [dbo].[Languages] ([IsActive]) 
WHERE [IsDeleted] = 0;

CREATE NONCLUSTERED INDEX [IX_Languages_IsDeleted] 
ON [dbo].[Languages] ([IsDeleted]) 
INCLUDE ([Code], [Name], [IsActive]);

CREATE NONCLUSTERED INDEX [IX_Languages_CreatedDate] 
ON [dbo].[Languages] ([CreatedDate]) 
INCLUDE ([CreatedBy]);


-- PersonLanguage tablosu
CREATE TABLE [dbo].[PersonLanguages] (
    -- Primary Key ve Foreign Keys
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PersonId] UNIQUEIDENTIFIER NOT NULL,
    [LanguageId] UNIQUEIDENTIFIER NOT NULL,
    
    -- PersonLanguage özellikleri
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [ProficiencyLevel] INT NOT NULL DEFAULT 1,
    
    -- AuditableEntity özellikleri
    [CreatedBy] NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedBy] NVARCHAR(50) NULL,
    [ModifiedDate] DATETIME2 NULL,
    [RowVersion] INT NOT NULL DEFAULT 0,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Browser] NVARCHAR(50) NULL,
    [BrowserVersion] NVARCHAR(50) NULL,
    [Platform] NVARCHAR(50) NULL,
    [DeviceType] NVARCHAR(50) NULL,
    [OperatingSystem] NVARCHAR(50) NULL,
    
    -- SoftDeleteEntity özellikleri
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedBy] NVARCHAR(50) NULL,
    [DeletedDate] DATETIME2 NULL,
    [DeletedIpAddress] NVARCHAR(50) NULL,
    [DeletedUserAgent] NVARCHAR(500) NULL,
    [DeletionReason] NVARCHAR(500) NULL,
    
    CONSTRAINT [PK_PersonLanguages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PersonLanguages_Persons] FOREIGN KEY ([PersonId]) 
        REFERENCES [dbo].[Persons] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PersonLanguages_Languages] FOREIGN KEY ([LanguageId]) 
        REFERENCES [dbo].[Languages] ([Id]) ON DELETE NO ACTION
);

-- İndeksler
CREATE NONCLUSTERED INDEX [IX_PersonLanguages_PersonId] 
ON [dbo].[PersonLanguages] ([PersonId]) 
INCLUDE ([LanguageId], [IsPrimary], [ProficiencyLevel], [IsDeleted]);

CREATE NONCLUSTERED INDEX [IX_PersonLanguages_LanguageId] 
ON [dbo].[PersonLanguages] ([LanguageId]) 
INCLUDE ([PersonId], [IsPrimary], [ProficiencyLevel], [IsDeleted]);

CREATE UNIQUE INDEX [IX_PersonLanguages_PersonId_IsPrimary] 
ON [dbo].[PersonLanguages] ([PersonId], [IsPrimary]) 
WHERE [IsPrimary] = 1 AND [IsDeleted] = 0;
