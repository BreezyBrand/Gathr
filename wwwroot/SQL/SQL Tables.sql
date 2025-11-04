DROP TABLE [dbo].[PriceHistory];
DROP TABLE [dbo].[Legalities];
DROP TABLE [dbo].[InventoryV2];
DROP TABLE [dbo].[TransactionLog]
DROP TABLE [dbo].[Images]
DROP TABLE [dbo].[InvTags]

CREATE TABLE [dbo].[InventoryV2] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [Card_Id]        VARCHAR (250) NULL,
    [Mark]           VARCHAR (10)  NULL,
    [Location]       VARCHAR (100) NULL,
    [Confirmed]      BIT           NULL,
    [Confirmed_date] DATETIME2 (7) NULL,
	[Language]		 VARCHAR (100) NULL,
    [UpdateUser]     VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InventoryV2_ToCards] FOREIGN KEY ([Card_Id]) REFERENCES [dbo].[Cards] ([id])
);

CREATE TABLE [dbo].[InvTags]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	InvId INT NOT NULL,
	tagNAme VARCHAR(100) NOT NULL
)

CREATE TABLE [dbo].[PriceHistory] (
    [CardId]      VARCHAR (250) NOT NULL,
    [usd]         VARCHAR (10)  NULL,
    [usd_foil]    VARCHAR (10)  NULL,
    [usd_etched]  VARCHAR (10)  NULL,
    [eur]         VARCHAR (10)  NULL,
    [eur_foil]    VARCHAR (10)  NULL,
    [eur_etched]  VARCHAR (10)  NULL,
    [tix]         VARCHAR (10)  NULL,
    [Update_Date] DATETIME2 (7) NULL
);

CREATE TABLE [dbo].[Legalities] (
    [CardId]          VARCHAR (250) NOT NULL,
    [standard]        VARCHAR (50)  NULL,
    [future]          VARCHAR (50)  NULL,
    [historic]        VARCHAR (50)  NULL,
    [timeless]        VARCHAR (50)  NULL,
    [gladiator]       VARCHAR (50)  NULL,
    [pioneer]         VARCHAR (50)  NULL,
    [explorer]        VARCHAR (50)  NULL,
    [modern]          VARCHAR (50)  NULL,
    [legacy]          VARCHAR (50)  NULL,
    [pauper]          VARCHAR (50)  NULL,
    [vintage]         VARCHAR (50)  NULL,
    [penny]           VARCHAR (50)  NULL,
    [commander]       VARCHAR (50)  NULL,
    [oathbreaker]     VARCHAR (50)  NULL,
    [standardbrawl]   VARCHAR (50)  NULL,
    [brawl]           VARCHAR (50)  NULL,
    [alchemy]         VARCHAR (50)  NULL,
    [paupercommander] VARCHAR (50)  NULL,
    [duel]            VARCHAR (50)  NULL,
    [oldschool]       VARCHAR (50)  NULL,
    [premodern]       VARCHAR (50)  NULL,
    [predh]           VARCHAR (50)  NULL,
    [Update_Date]     DATETIME2 (7) NULL
);

CREATE TABLE [dbo].[TransactionLog] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [InventoryId]     INT           NOT NULL,
    [Card_Id]         VARCHAR (250) NULL,
    [UpdateType]      VARCHAR (100) NULL,
    [TransactionDate] DATETIME2 (7) NULL,
	[TransactionUser] VARCHAR (100) NULL,
    [Description]     VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



CREATE TABLE [dbo].[Images] (
    [Id]          VARCHAR (250) NOT NULL,
    [small]       VARCHAR (250) NULL,
    [normal]      VARCHAR (250) NULL,
    [large]       VARCHAR (250) NULL,
    [png]         VARCHAR (250) NULL,
    [art_crop]    VARCHAR (250) NULL,
    [border_crop] VARCHAR (250) NULL,
	[side]		  varchar(1) null
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO;

CREATE VIEW [dbo].[SpreadsheetView]
	AS 
	SELECT 
		c.id,
		COUNT(c.id) as 'QTY',
		c.[name],
		UPPER(c.[set]) as 'Set Code',
		c.[collector_number],
		i.Mark,
		i.[Location],
		i.[Language],
		c.rarity,
		c.type_line,
		TRY_CAST((CASE WHEN i.Mark LIKE '%f%' THEN usd_foil
		      WHEN i.Mark LIKE '%etch%'THEN usd_etched			  
			  ELSE usd
		END) as decimal(18,2)) as 'Price'
	FROM 
		[InventoryV2] i 
		LEFT JOIN Cards c ON c.id=i.Card_Id
		LEFT JOIN PriceHistory p ON i.Card_Id = p.CardId
	GROUP BY c.id, c.[name], c.[set], c.collector_number, i.Mark, i.[Location],i.[Language], c.rarity, c.type_line, usd, usd_etched, usd_foil
