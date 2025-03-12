-- Create Currency table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Currencies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Currencies] (
        -- Primary Key
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        
        -- Currency properties
        [Code] NVARCHAR(3) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Symbol] NVARCHAR(10) NOT NULL,
        [Flag] NVARCHAR(50) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDefault] BIT NOT NULL DEFAULT 0,
        [DecimalPlaces] INT NOT NULL DEFAULT 2,
        [Format] NVARCHAR(20) NULL,
        [ExchangeRate] DECIMAL(18,6) NULL,
        [LastUpdated] DATETIME2 NULL,
        
        -- BaseEntity properties
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT 'System',
        [CreatedByIp] NVARCHAR(50) NULL,
        [UpdatedDate] DATETIME2 NULL,
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedByIp] NVARCHAR(50) NULL,
        
        -- SoftDeleteEntity properties
        [DeletedAt] DATETIME2 NULL,
        [DeletedByUserId] NVARCHAR(100) NULL,
        [DeletedByIp] NVARCHAR(50) NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [RowVersion] BIGINT NOT NULL DEFAULT 0,
        
        -- Constraints
        CONSTRAINT [UQ_Currencies_Code] UNIQUE ([Code]),
        CONSTRAINT [UQ_Currencies_IsDefault] UNIQUE ([IsDefault]) WHERE [IsDefault] = 1
    );
    
    PRINT 'Currencies table created successfully';
END
ELSE
BEGIN
    PRINT 'Currencies table already exists';
END

-- Add indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Currencies_Code' AND object_id = OBJECT_ID(N'[dbo].[Currencies]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Currencies_Code] ON [dbo].[Currencies]([Code]) WHERE [IsDeleted] = 0;
    PRINT 'Index IX_Currencies_Code created successfully';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Currencies_IsDefault' AND object_id = OBJECT_ID(N'[dbo].[Currencies]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Currencies_IsDefault] ON [dbo].[Currencies]([IsDefault]) WHERE [IsDefault] = 1 AND [IsDeleted] = 0;
    PRINT 'Index IX_Currencies_IsDefault created successfully';
END

-- Insert default currencies if they don't exist
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'TRY')
BEGIN
    INSERT INTO [dbo].[Currencies] ([Id], [Code], [Name], [Symbol], [Flag], [IsActive], [IsDefault], [DecimalPlaces], [Format], [ExchangeRate], [LastUpdated], [CreatedDate], [CreatedBy])
    VALUES (NEWID(), 'TRY', 'Turkish Lira', '₺', '🇹🇷', 1, 1, 2, '{0} {1}', 1.000000, GETUTCDATE(), GETUTCDATE(), 'System');
    PRINT 'Added TRY currency';
END

IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'USD')
BEGIN
    INSERT INTO [dbo].[Currencies] ([Id], [Code], [Name], [Symbol], [Flag], [IsActive], [IsDefault], [DecimalPlaces], [Format], [ExchangeRate], [LastUpdated], [CreatedDate], [CreatedBy])
    VALUES (NEWID(), 'USD', 'US Dollar', '$', '🇺🇸', 1, 0, 2, '{1}{0}', 0.031000, GETUTCDATE(), GETUTCDATE(), 'System');
    PRINT 'Added USD currency';
END

IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'EUR')
BEGIN
    INSERT INTO [dbo].[Currencies] ([Id], [Code], [Name], [Symbol], [Flag], [IsActive], [IsDefault], [DecimalPlaces], [Format], [ExchangeRate], [LastUpdated], [CreatedDate], [CreatedBy])
    VALUES (NEWID(), 'EUR', 'Euro', '€', '🇪🇺', 1, 0, 2, '{1}{0}', 0.028000, GETUTCDATE(), GETUTCDATE(), 'System');
    PRINT 'Added EUR currency';
END

IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'GBP')
BEGIN
    INSERT INTO [dbo].[Currencies] ([Id], [Code], [Name], [Symbol], [Flag], [IsActive], [IsDefault], [DecimalPlaces], [Format], [ExchangeRate], [LastUpdated], [CreatedDate], [CreatedBy])
    VALUES (NEWID(), 'GBP', 'British Pound', '£', '🇬🇧', 1, 0, 2, '{1}{0}', 0.024000, GETUTCDATE(), GETUTCDATE(), 'System');
    PRINT 'Added GBP currency';
END 