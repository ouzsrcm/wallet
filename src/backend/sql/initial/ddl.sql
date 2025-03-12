-- Create database
CREATE DATABASE WalletDb
GO

USE WalletDb
GO

-- Create Persons table
CREATE TABLE [dbo].[Persons] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [MiddleName] NVARCHAR(50) NULL,
    [DateOfBirth] DATETIME2 NOT NULL,
    [Gender] NVARCHAR(20) NOT NULL,
    [NationalityId] NVARCHAR(20) NULL,
    [TaxNumber] NVARCHAR(20) NULL,
    [IdNumber] NVARCHAR(20) NULL,
    [ProfilePictureUrl] NVARCHAR(500) NULL,
    [Language] NVARCHAR(10) NULL,
    [TimeZone] NVARCHAR(50) NULL,
    [Currency] NVARCHAR(3) NULL,
    -- Audit columns
    [RowVersion] INT NOT NULL DEFAULT 1,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- Create Users table
CREATE TABLE [dbo].[Users] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [PersonId] UNIQUEIDENTIFIER NOT NULL,
    [EmailNotificationsEnabled] BIT NOT NULL DEFAULT 1,
    [PushNotificationsEnabled] BIT NOT NULL DEFAULT 1,
    [SMSNotificationsEnabled] BIT NOT NULL DEFAULT 1,
    -- Audit columns
    [RowVersion] INT NOT NULL DEFAULT 1,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_Users_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([Id])
);

-- Create UserCredentials table
CREATE TABLE [dbo].[UserCredentials] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Username] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(100) NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    [PasswordHash] NVARCHAR(100) NOT NULL,
    [PasswordSalt] NVARCHAR(100) NOT NULL,
    [PasswordChangedAt] DATETIME2 NULL,
    [RequirePasswordChange] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsEmailVerified] BIT NOT NULL DEFAULT 0,
    [IsPhoneVerified] BIT NOT NULL DEFAULT 0,
    [IsTwoFactorEnabled] BIT NOT NULL DEFAULT 0,
    [TwoFactorType] NVARCHAR(20) NULL,
    [TwoFactorKey] NVARCHAR(100) NULL,
    [SecurityStamp] NVARCHAR(100) NULL,
    [FailedLoginAttempts] INT NOT NULL DEFAULT 0,
    [IsLocked] BIT NOT NULL DEFAULT 0,
    [LockoutEndDate] DATETIME2 NULL,
    [LastLoginIP] NVARCHAR(50) NULL,
    [LastLoginDate] DATETIME2 NULL,
    [LastFailedLoginIP] NVARCHAR(50) NULL,
    [LastFailedLoginDate] DATETIME2 NULL,
    [RefreshToken] NVARCHAR(100) NULL,
    [RefreshTokenExpireDate] DATETIME2 NULL,
    [PasswordResetToken] NVARCHAR(100) NULL,
    [PasswordResetTokenExpireDate] DATETIME2 NULL,
    [EmailVerificationToken] NVARCHAR(100) NULL,
    [EmailVerificationTokenExpireDate] DATETIME2 NULL,
    [PhoneVerificationToken] NVARCHAR(100) NULL,
    [PhoneVerificationTokenExpireDate] DATETIME2 NULL,
    [RememberMe] BIT NOT NULL DEFAULT 0,
    [DeviceId] NVARCHAR(100) NULL,
    [DeviceName] NVARCHAR(100) NULL,
    [IsCurrentDevice] BIT NOT NULL DEFAULT 0,
    [Roles] NVARCHAR(MAX) NOT NULL,
    -- Audit columns
    [RowVersion] INT NOT NULL DEFAULT 1,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_UserCredentials_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

-- Create PersonAddresses table
CREATE TABLE [dbo].[PersonAddresses] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [PersonId] UNIQUEIDENTIFIER NOT NULL,
    [AddressType] NVARCHAR(20) NULL,
    [AddressName] NVARCHAR(100) NULL,
    [AddressLine1] NVARCHAR(200) NULL,
    [AddressLine2] NVARCHAR(200) NULL,
    [District] NVARCHAR(100) NULL,
    [City] NVARCHAR(100) NULL,
    [State] NVARCHAR(100) NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [PostalCode] NVARCHAR(20) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [VerifiedAt] DATETIME2 NULL,
    [Latitude] DECIMAL(18,15) NULL,
    [Longitude] DECIMAL(18,15) NULL,
    -- Audit columns
    [RowVersion] INT NOT NULL DEFAULT 1,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_PersonAddresses_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([Id])
);

-- Create PersonContacts table
CREATE TABLE [dbo].[PersonContacts] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [PersonId] UNIQUEIDENTIFIER NOT NULL,
    [ContactType] NVARCHAR(20) NULL,
    [ContactName] NVARCHAR(100) NULL,
    [ContactValue] NVARCHAR(200) NULL,
    [CountryCode] NVARCHAR(5) NULL,
    [AreaCode] NVARCHAR(5) NULL,
    [IsPhoneVerified] BIT NOT NULL DEFAULT 0,
    [PhoneVerifiedAt] DATETIME2 NULL,
    [IsEmailVerified] BIT NOT NULL DEFAULT 0,
    [EmailVerifiedAt] DATETIME2 NULL,
    [Description] NVARCHAR(500) NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsPublic] BIT NOT NULL DEFAULT 0,
    [EmailNotificationsEnabled] BIT NOT NULL DEFAULT 0,
    [SMSNotificationsEnabled] BIT NOT NULL DEFAULT 0,
    [VerificationToken] NVARCHAR(100) NULL,
    [VerificationTokenExpireDate] DATETIME2 NULL,
    -- Audit columns
    [RowVersion] INT NOT NULL DEFAULT 1,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_PersonContacts_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([Id])
);

-- Create Nationalities table
CREATE TABLE [dbo].[Nationalities] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Code] NVARCHAR(2) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [NativeName] NVARCHAR(100) NULL,
    [Alpha3Code] NVARCHAR(3) NULL,
    [NumericCode] NVARCHAR(3) NULL,
    [PhoneCode] NVARCHAR(5) NULL,
    [Capital] NVARCHAR(100) NULL,
    [Region] NVARCHAR(50) NULL,
    [SubRegion] NVARCHAR(50) NULL,
    [FlagUrl] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    -- Audit columns
    [RowVersion] INT NOT NULL DEFAULT 1,
    [IPAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME2 NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedByIp] NVARCHAR(50) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedByUserId] NVARCHAR(100) NULL,
    [DeletedByUserName] NVARCHAR(100) NULL,
    [DeletedByIp] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- Create Indexes
CREATE UNIQUE INDEX [IX_UserCredentials_Username] ON [dbo].[UserCredentials] ([Username]) WHERE [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_UserCredentials_Email] ON [dbo].[UserCredentials] ([Email]) WHERE [Email] IS NOT NULL AND [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_UserCredentials_PhoneNumber] ON [dbo].[UserCredentials] ([PhoneNumber]) WHERE [PhoneNumber] IS NOT NULL AND [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_Nationalities_Code] ON [dbo].[Nationalities] ([Code]) WHERE [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_Nationalities_Alpha3Code] ON [dbo].[Nationalities] ([Alpha3Code]) WHERE [Alpha3Code] IS NOT NULL AND [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_Nationalities_NumericCode] ON [dbo].[Nationalities] ([NumericCode]) WHERE [NumericCode] IS NOT NULL AND [IsDeleted] = 0;