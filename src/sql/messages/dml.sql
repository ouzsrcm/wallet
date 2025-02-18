-- Önce kullanıcı ID'lerini değişkenlere alalım
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

-- İlk mesaj zinciri: Hoş geldin mesajları
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
    [CreatedBy]
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
    'johndoe'
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
    [CreatedBy]
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
    'janesmith'
);

-- John'un tekrar yanıtı
INSERT INTO [dbo].[Messages] (
    [SenderId], 
    [ReceiverId], 
    [ParentMessageId],
    [Subject], 
    [Content], 
    [IsRead],
    [ReadAt],
    [CreatedDate],
    [CreatedBy]
)
VALUES (
    @JohnId,
    @JaneId,
    @WelcomeMessageId,
    'Re: Re: Welcome to Wallet App',
    'Of course! I''d be happy to help. You can set up budget categories and track your spending in real-time.',
    1,
    DATEADD(MINUTE, 30, GETDATE()),
    DATEADD(DAY, -3, GETDATE()),
    'johndoe'
);

-- İkinci mesaj zinciri: Teknik destek
DECLARE @TechnicalQuestionId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [dbo].[Messages] (
    [Id],
    [SenderId], 
    [ReceiverId], 
    [Subject], 
    [Content], 
    [IsRead],
    [ReadAt],
    [CreatedDate],
    [CreatedBy]
)
VALUES (
    @TechnicalQuestionId,
    @AhmetId,
    @JohnId,
    'Technical Question about Transactions',
    'Hi John, I noticed something interesting in the transaction system. When I try to add a recurring payment, it shows different amounts in different currencies. Is this expected behavior?',
    1,
    DATEADD(HOUR, 1, GETDATE()),
    DATEADD(DAY, -1, GETDATE()),
    'ahmetyilmaz'
);

-- John'un teknik soruya yanıtı
INSERT INTO [dbo].[Messages] (
    [SenderId], 
    [ReceiverId], 
    [ParentMessageId],
    [Subject], 
    [Content], 
    [IsRead],
    [ReadAt],
    [CreatedDate],
    [CreatedBy]
)
VALUES (
    @JohnId,
    @AhmetId,
    @TechnicalQuestionId,
    'Re: Technical Question about Transactions',
    'Hi Ahmet, thanks for bringing this up. This is actually by design - the system automatically converts amounts using current exchange rates. You can disable this in your profile settings if you prefer.',
    1,
    DATEADD(HOUR, 2, GETDATE()),
    DATEADD(DAY, -1, GETDATE()),
    'johndoe'
);

-- Okunmamış yeni mesaj
INSERT INTO [dbo].[Messages] (
    [SenderId], 
    [ReceiverId], 
    [Subject], 
    [Content], 
    [IsRead],
    [CreatedDate],
    [CreatedBy]
)
VALUES (
    @JaneId,
    @AhmetId,
    'Question about Budget Templates',
    'Hi Ahmet, I saw that you created some great budget templates. Would you mind sharing how you structured your categories? I''m trying to improve my budget organization.',
    0,
    DATEADD(HOUR, -1, GETDATE()),
    'janesmith'
);

-- Sistem bildirimi
INSERT INTO [dbo].[Messages] (
    [SenderId], 
    [ReceiverId], 
    [Subject], 
    [Content], 
    [IsRead],
    [CreatedDate],
    [CreatedBy]
)
VALUES (
    @JohnId,
    @JaneId,
    'New Feature Announcement',
    'We''ve just launched a new feature: Bill Splitting! You can now easily split expenses with friends and family. Check it out in the Transactions section.',
    0,
    GETDATE(),
    'system'
);