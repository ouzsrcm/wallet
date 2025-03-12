-- =============================================
-- Currency Table DDL Script
-- =============================================

-- Create Currency table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Currencies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Currencies] (
        -- Primary Key (from SoftDeleteEntity/BaseEntity)
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        
        -- Currency specific properties
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
        
        -- Audit properties (from BaseEntity)
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT 'System',
        [CreatedByIp] NVARCHAR(50) NULL,
        [UpdatedDate] DATETIME2 NULL,
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedByIp] NVARCHAR(50) NULL,
        
        -- Soft delete properties (from SoftDeleteEntity)
        [DeletedAt] DATETIME2 NULL,
        [DeletedByUserId] NVARCHAR(100) NULL,
        [DeletedByIp] NVARCHAR(50) NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [RowVersion] BIGINT NOT NULL DEFAULT 0,
        
        -- Constraints based on CurrencyConfiguration
        CONSTRAINT [UQ_Currencies_Code] UNIQUE ([Code]),
        CONSTRAINT [UQ_Currencies_IsDefault] UNIQUE ([IsDefault]) WHERE [IsDefault] = 1
    );
    
    PRINT 'Currencies table created successfully';
END
ELSE
BEGIN
    PRINT 'Currencies table already exists';
END

-- Add indexes based on CurrencyConfiguration
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

-- =============================================
-- Currency Table DML Script (Sample Data)
-- =============================================

-- Insert common currencies if they don't exist
-- Turkish Lira (TRY) as default currency
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'TRY')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'TRY', 'Turkish Lira', '₺', '🇹🇷', 
        1, 1, 2, '{0} {1}', 
        1.000000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added TRY currency (Turkish Lira) as default';
END

-- US Dollar (USD)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'USD')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'USD', 'US Dollar', '$', '🇺🇸', 
        1, 0, 2, '{1}{0}', 
        0.031000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added USD currency (US Dollar)';
END

-- Euro (EUR)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'EUR')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'EUR', 'Euro', '€', '🇪🇺', 
        1, 0, 2, '{1}{0}', 
        0.028000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added EUR currency (Euro)';
END

-- British Pound (GBP)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'GBP')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'GBP', 'British Pound', '£', '🇬🇧', 
        1, 0, 2, '{1}{0}', 
        0.024000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added GBP currency (British Pound)';
END

-- Japanese Yen (JPY)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'JPY')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'JPY', 'Japanese Yen', '¥', '🇯🇵', 
        1, 0, 0, '{1}{0}', 
        4.500000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added JPY currency (Japanese Yen)';
END

-- Swiss Franc (CHF)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'CHF')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'CHF', 'Swiss Franc', 'Fr', '🇨🇭', 
        1, 0, 2, '{1} {0}', 
        0.027000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added CHF currency (Swiss Franc)';
END

-- Canadian Dollar (CAD)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'CAD')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'CAD', 'Canadian Dollar', 'C$', '🇨🇦', 
        1, 0, 2, '{1}{0}', 
        0.042000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added CAD currency (Canadian Dollar)';
END

-- Australian Dollar (AUD)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'AUD')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'AUD', 'Australian Dollar', 'A$', '🇦🇺', 
        1, 0, 2, '{1}{0}', 
        0.046000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added AUD currency (Australian Dollar)';
END

-- Chinese Yuan (CNY)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'CNY')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'CNY', 'Chinese Yuan', '¥', '🇨🇳', 
        1, 0, 2, '{1}{0}', 
        0.220000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added CNY currency (Chinese Yuan)';
END

-- Indian Rupee (INR)
IF NOT EXISTS (SELECT * FROM [dbo].[Currencies] WHERE [Code] = 'INR')
BEGIN
    INSERT INTO [dbo].[Currencies] (
        [Id], [Code], [Name], [Symbol], [Flag], 
        [IsActive], [IsDefault], [DecimalPlaces], [Format], 
        [ExchangeRate], [LastUpdated], 
        [CreatedDate], [CreatedBy]
    )
    VALUES (
        NEWID(), 'INR', 'Indian Rupee', '₹', '🇮🇳', 
        1, 0, 2, '{1}{0}', 
        2.580000, GETUTCDATE(), 
        GETUTCDATE(), 'System'
    );
    PRINT 'Added INR currency (Indian Rupee)';
END

-- =============================================
-- Verify Data
-- =============================================
SELECT 
    [Id], [Code], [Name], [Symbol], [Flag], 
    [IsActive], [IsDefault], [DecimalPlaces], [Format], 
    [ExchangeRate], [LastUpdated]
FROM [dbo].[Currencies]
WHERE [IsDeleted] = 0
ORDER BY [IsDefault] DESC, [Name] ASC; 