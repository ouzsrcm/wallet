-- Categories tablosu
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Icon NVARCHAR(50),
    Color NVARCHAR(7) NOT NULL,
    Type INT NOT NULL,
    ParentCategoryId UNIQUEIDENTIFIER,
    IsSystem BIT NOT NULL DEFAULT 0,
    -- BaseEntity properties
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    CreatedByIp NVARCHAR(50),
    UpdatedDate DATETIME2,
    UpdatedBy NVARCHAR(100),
    UpdatedByIp NVARCHAR(50),
    -- SoftDeleteEntity properties
    DeletedAt DATETIME2,
    DeletedByUserId NVARCHAR(100),
    DeletedByIp NVARCHAR(50),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion BIGINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Categories_ParentCategory FOREIGN KEY (ParentCategoryId) 
        REFERENCES Categories(Id) ON DELETE NO ACTION
);

-- Transactions tablosu
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL,
    TransactionDate DATETIME2 NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Type INT NOT NULL,
    PaymentMethod INT NOT NULL,
    Reference NVARCHAR(100),
    IsRecurring BIT NOT NULL DEFAULT 0,
    RecurringPeriod INT,
    -- BaseEntity properties
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    CreatedByIp NVARCHAR(50),
    UpdatedDate DATETIME2,
    UpdatedBy NVARCHAR(100),
    UpdatedByIp NVARCHAR(50),
    -- SoftDeleteEntity properties
    DeletedAt DATETIME2,
    DeletedByUserId NVARCHAR(100),
    DeletedByIp NVARCHAR(50),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion BIGINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Transactions_Person FOREIGN KEY (PersonId) 
        REFERENCES Persons(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_Category FOREIGN KEY (CategoryId) 
        REFERENCES Categories(Id) ON DELETE NO ACTION
);

-- Receipts tablosu
CREATE TABLE Receipts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TransactionId UNIQUEIDENTIFIER NOT NULL,
    StoreName NVARCHAR(100) NOT NULL,
    StoreAddress NVARCHAR(500),
    TaxNumber NVARCHAR(20),
    ReceiptNo NVARCHAR(50),
    ReceiptDate DATETIME2 NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    TaxAmount DECIMAL(18,2),
    DiscountAmount DECIMAL(18,2),
    PaymentDetails NVARCHAR(500),
    Notes NVARCHAR(1000),
    -- BaseEntity properties
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    CreatedByIp NVARCHAR(50),
    UpdatedDate DATETIME2,
    UpdatedBy NVARCHAR(100),
    UpdatedByIp NVARCHAR(50),
    -- SoftDeleteEntity properties
    DeletedAt DATETIME2,
    DeletedByUserId NVARCHAR(100),
    DeletedByIp NVARCHAR(50),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion BIGINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Receipts_Transaction FOREIGN KEY (TransactionId) 
        REFERENCES Transactions(Id) ON DELETE NO ACTION
);

-- ReceiptItems tablosu
CREATE TABLE ReceiptItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ReceiptId UNIQUEIDENTIFIER NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Barcode NVARCHAR(50),
    Quantity DECIMAL(18,3) NOT NULL,
    Unit INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    TaxRate DECIMAL(5,2),
    TaxAmount DECIMAL(18,2),
    DiscountAmount DECIMAL(18,2),
    -- BaseEntity properties
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    CreatedByIp NVARCHAR(50),
    UpdatedDate DATETIME2,
    UpdatedBy NVARCHAR(100),
    UpdatedByIp NVARCHAR(50),
    -- SoftDeleteEntity properties
    DeletedAt DATETIME2,
    DeletedByUserId NVARCHAR(100),
    DeletedByIp NVARCHAR(50),
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion BIGINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ReceiptItems_Receipt FOREIGN KEY (ReceiptId) 
        REFERENCES Receipts(Id) ON DELETE CASCADE
);

-- İndeksler aynı kalabilir