-- Sample data for Nationalities
INSERT INTO [dbo].[Nationalities] (
    [Id], [Code], [Name], [NativeName], [Alpha3Code], [NumericCode], 
    [PhoneCode], [Capital], [Region], [SubRegion], [FlagUrl], [IsActive],
    [CreatedDate], [CreatedBy]
) VALUES 
(NEWID(), 'TR', 'Turkey', N'Türkiye', 'TUR', '792', '+90', 'Ankara', 'Asia', 'Western Asia', 'https://flagcdn.com/tr.svg', 1, GETDATE(), 'System'),
(NEWID(), 'US', 'United States', 'United States', 'USA', '840', '+1', 'Washington, D.C.', 'Americas', 'Northern America', 'https://flagcdn.com/us.svg', 1, GETDATE(), 'System'),
(NEWID(), 'GB', 'United Kingdom', 'United Kingdom', 'GBR', '826', '+44', 'London', 'Europe', 'Northern Europe', 'https://flagcdn.com/gb.svg', 1, GETDATE(), 'System');

-- Sample data for Persons
DECLARE @Person1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Person2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Person3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Persons] (
    [Id], [FirstName], [LastName], [MiddleName], [DateOfBirth], [Gender],
    [NationalityId], [TaxNumber], [IdNumber], [Language], [TimeZone], [Currency],
    [CreatedDate], [CreatedBy]
) VALUES 
(@Person1Id, 'John', 'Doe', NULL, '1990-01-15', 'Male', 'US', '123456789', 'A123456', 'en-US', 'America/New_York', 'USD', GETDATE(), 'System'),
(@Person2Id, 'Jane', 'Smith', 'Marie', '1985-06-22', 'Female', 'GB', '987654321', 'B987654', 'en-GB', 'Europe/London', 'GBP', GETDATE(), 'System'),
(@Person3Id, 'Ahmet', 'Yılmaz', NULL, '1988-03-10', 'Male', 'TR', '12345678901', 'C123456', 'tr-TR', 'Europe/Istanbul', 'TRY', GETDATE(), 'System');

-- Sample data for Users
DECLARE @User1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Users] (
    [Id], [PersonId], [EmailNotificationsEnabled], [PushNotificationsEnabled], 
    [SMSNotificationsEnabled], [CreatedDate], [CreatedBy]
) VALUES 
(@User1Id, @Person1Id, 1, 1, 1, GETDATE(), 'System'),
(@User2Id, @Person2Id, 1, 0, 1, GETDATE(), 'System'),
(@User3Id, @Person3Id, 1, 1, 0, GETDATE(), 'System');

-- Sample data for UserCredentials
INSERT INTO [dbo].[UserCredentials] (
    [Id], [UserId], [Username], [Email], [PhoneNumber], 
    [PasswordHash], [PasswordSalt], [IsActive], [IsEmailVerified],
    [Roles], [CreatedDate], [CreatedBy]
) VALUES 
(NEWID(), @User1Id, 'johndoe', 'john.doe@example.com', '+12125551234',
'hash123', 'salt123', 1, 1, '["User"]', GETDATE(), 'System'),
(NEWID(), @User2Id, 'janesmith', 'jane.smith@example.com', '+447911123456',
'hash456', 'salt456', 1, 1, '["User"]', GETDATE(), 'System'),
(NEWID(), @User3Id, 'ahmetyilmaz', 'ahmet.yilmaz@example.com', '+905551234567',
'hash789', 'salt789', 1, 1, '["Admin","User"]', GETDATE(), 'System');

-- Sample data for PersonAddresses
INSERT INTO [dbo].[PersonAddresses] (
    [Id], [PersonId], [AddressType], [AddressName], [AddressLine1],
    [AddressLine2], [District], [City], [State], [Country], [PostalCode],
    [IsDefault], [Latitude], [Longitude], [CreatedDate], [CreatedBy]
) VALUES 
(NEWID(), @Person1Id, 'Home', 'Home Address', '123 Main St', 'Apt 4B',
'Manhattan', 'New York', 'NY', 'USA', '10001', 1, 40.7128, -74.0060,
GETDATE(), 'System'),
(NEWID(), @Person2Id, 'Work', 'Office', '45 Oxford Street', NULL,
'Westminster', 'London', NULL, 'UK', 'W1D 2DT', 1, 51.5074, -0.1278,
GETDATE(), 'System'),
(NEWID(), @Person3Id, 'Home', 'Ev Adresi', 'Bağdat Caddesi No:123', 'Daire 5',
'Kadıköy', 'Istanbul', NULL, 'Turkey', '34744', 1, 40.9632, 29.0550,
GETDATE(), 'System');

-- Sample data for PersonContacts
INSERT INTO [dbo].[PersonContacts] (
    [Id], [PersonId], [ContactType], [ContactName], [ContactValue],
    [CountryCode], [AreaCode], [IsPhoneVerified], [IsEmailVerified],
    [IsDefault], [IsPrimary], [CreatedDate], [CreatedBy]
) VALUES 
(NEWID(), @Person1Id, 'Email', 'Work Email', 'john.doe@work.com',
NULL, NULL, 0, 1, 1, 1, GETDATE(), 'System'),
(NEWID(), @Person1Id, 'Phone', 'Mobile', '5551234567',
'+1', '212', 1, 0, 1, 1, GETDATE(), 'System'),
(NEWID(), @Person2Id, 'Email', 'Personal Email', 'jane.smith@personal.com',
NULL, NULL, 0, 1, 1, 1, GETDATE(), 'System'),
(NEWID(), @Person2Id, 'Phone', 'Work Phone', '7911123456',
'+44', '20', 1, 0, 1, 0, GETDATE(), 'System'),
(NEWID(), @Person3Id, 'Email', 'Kişisel Email', 'ahmet.yilmaz@gmail.com',
NULL, NULL, 0, 1, 1, 1, GETDATE(), 'System'),
(NEWID(), @Person3Id, 'Phone', 'Cep Telefonu', '5551234567',
'+90', '532', 1, 0, 1, 1, GETDATE(), 'System');