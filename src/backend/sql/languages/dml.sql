-- Örnek dil kayıtları
INSERT INTO [dbo].[Languages] (
    [Id], [Code], [Name], [NativeName], [FlagUrl], [IsActive], [IsDefault],
    [LocalizationCode], [DateFormat], [TimeFormat], [CurrencyFormat], [NumberFormat],
    [CreatedBy], [CreatedDate], [RowVersion]
)
VALUES 
    -- Türkçe (Varsayılan)
    (NEWID(), 'tr', 'Turkish', N'Türkçe', 'flags/tr.png', 1, 1,
    'tr-TR', 'dd.MM.yyyy', 'HH:mm', N'₺#,##0.00', '#.##0,00',
    'System', GETUTCDATE(), 0),

    -- İngilizce (US)
    (NEWID(), 'en', 'English', 'English', 'flags/en.png', 1, 0,
    'en-US', 'MM/dd/yyyy', 'hh:mm tt', '$#,##0.00', '#,##0.00',
    'System', GETUTCDATE(), 0),

    -- İngilizce (UK)
    (NEWID(), 'en-GB', 'English (UK)', 'English', 'flags/gb.png', 1, 0,
    'en-GB', 'dd/MM/yyyy', 'HH:mm', N'£#,##0.00', '#,##0.00',
    'System', GETUTCDATE(), 0),

    -- Almanca
    (NEWID(), 'de', 'German', 'Deutsch', 'flags/de.png', 1, 0,
    'de-DE', 'dd.MM.yyyy', 'HH:mm', N'#.##0,00 €', '#.##0,00',
    'System', GETUTCDATE(), 0),

    -- Fransızca
    (NEWID(), 'fr', 'French', N'Français', 'flags/fr.png', 1, 0,
    'fr-FR', 'dd/MM/yyyy', 'HH:mm', N'# ##0,00 €', '# ##0,00',
    'System', GETUTCDATE(), 0),

    -- İspanyolca
    (NEWID(), 'es', 'Spanish', N'Español', 'flags/es.png', 1, 0,
    'es-ES', 'dd/MM/yyyy', 'HH:mm', N'#.##0,00 €', '#.##0,00',
    'System', GETUTCDATE(), 0),

    -- İtalyanca
    (NEWID(), 'it', 'Italian', 'Italiano', 'flags/it.png', 1, 0,
    'it-IT', 'dd/MM/yyyy', 'HH:mm', N'€ #.##0,00', '#.##0,00',
    'System', GETUTCDATE(), 0),

    -- Rusça
    (NEWID(), 'ru', 'Russian', N'Русский', 'flags/ru.png', 1, 0,
    'ru-RU', 'dd.MM.yyyy', 'HH:mm', N'# ##0,00 ₽', '# ##0,00',
    'System', GETUTCDATE(), 0),

    -- Japonca
    (NEWID(), 'ja', 'Japanese', N'日本語', 'flags/jp.png', 1, 0,
    'ja-JP', 'yyyy/MM/dd', 'HH:mm', N'¥#,##0', '#,##0',
    'System', GETUTCDATE(), 0),

    -- Çince (Basitleştirilmiş)
    (NEWID(), 'zh', 'Chinese', N'中文', 'flags/cn.png', 1, 0,
    'zh-CN', 'yyyy/MM/dd', 'HH:mm', N'¥#,##0.00', '#,##0.00',
    'System', GETUTCDATE(), 0);

-- Aktif dilleri kontrol etme sorgusu
SELECT [Code], [Name], [NativeName], [IsDefault], [LocalizationCode]
FROM [dbo].[Languages]
WHERE [IsActive] = 1 AND [IsDeleted] = 0
ORDER BY [IsDefault] DESC, [Name];