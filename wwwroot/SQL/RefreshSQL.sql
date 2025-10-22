
DROP TABLE [dbo].[PriceHistory];
DROP TABLE [dbo].[Legalities];
DROP TABLE [dbo].[InventoryV2];
DROP TABLE [dbo].[TransactionLog]
DROP TABLE [dbo].[Images]

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
    [InventoryId]     INT NOT NULL,
    [Card_Id]         VARCHAR (250) NULL,
    [UpdateType]      VARCHAR(100) NULL,
    [TransactionDate] DATETIME2(7) NULL,
    [Description]     VARCHAR(MAX) NULL    
    PRIMARY KEY CLUSTERED ([Id] ASC),    
);

CREATE TABLE [dbo].[Images] (
    [Id]          VARCHAR (250) NOT NULL,
    [small]       VARCHAR (250) NULL,
    [normal]      VARCHAR (250) NULL,
    [large]       VARCHAR (250) NULL,
    [png]         VARCHAR (250) NULL,
    [art_crop]    VARCHAR (250) NULL,
    [border_crop] VARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
);


INSERT INTO 
	Legalities (CardId) 
Select 
	id 
FROM Cards c
	LEFT JOIN Legalities l ON l.CardId = c.id
WHERE 
	l.CardId IS NULL

UPDATE
	Legalities
SET
	[standard] =		(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.standard') WHERE c.Id = CardId),
	future =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.future') WHERE c.Id = CardId),
	historic =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.historic') WHERE c.Id = CardId),
	timeless =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.timeless') WHERE c.Id = CardId),
	gladiator =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.gladiator') WHERE c.Id = CardId),
	pioneer =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.pioneer') WHERE c.Id = CardId),
	explorer =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.explorer') WHERE c.Id = CardId),
	modern =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.modern') WHERE c.Id = CardId),
	legacy =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.legacy') WHERE c.Id = CardId),
	pauper =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.pauper') WHERE c.Id = CardId),
	vintage =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.vintage') WHERE c.Id = CardId),
	penny =				(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.penny') WHERE c.Id = CardId),
	commander =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.commander') WHERE c.Id = CardId),
	oathbreaker =		(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.oathbreaker') WHERE c.Id = CardId),
	standardbrawl =		(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.standardbrawl') WHERE c.Id = CardId),
	brawl =				(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.brawl') WHERE c.Id = CardId),
	alchemy =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.alchemy') WHERE c.Id = CardId),
	paupercommander =	(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.paupercommander') WHERE c.Id = CardId),
	duel =				(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.duel') WHERE c.Id = CardId),
	oldschool =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.oldschool') WHERE c.Id = CardId),
	premodern =			(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.premodern') WHERE c.Id = CardId),
	predh =				(SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.legalities,CHAR(39),'"'),'None','"N/A"'),'$.predh') WHERE c.Id = CardId),
	Update_Date =		GETDATE()
FROM
	Cards c
	JOIN Legalities l ON c.id = l.CardId

INSERT INTO 
	PriceHistory(CardId) 
Select 
	id 
FROM Cards c
	LEFT JOIN PriceHistory ph ON ph.CardId = c.id
WHERE 
	ph.CardId IS NULL

UPDATE
	PriceHistory
SET	
	usd = (SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.usd') WHERE c.Id = CardId),
	usd_foil = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.usd_foil') WHERE c.Id = CardId),
	usd_etched = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.usd_etched') WHERE c.Id = CardId),
	eur = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.eur') WHERE c.Id = CardId),
	eur_foil = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.eur_foil') WHERE c.Id = CardId),
	eur_etched = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.eur_etched') WHERE c.Id = CardId),
	tix = (SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.prices,CHAR(39),'"'),'None','"N/A"'),'$.tix') WHERE c.Id = CardId),
	Update_Date = GETDATE()
FROM
	Cards c
	JOIN PriceHistory ph ON c.id=ph.CardId

INSERT INTO 
	Images(Id) 
Select 
	c.Id
FROM Cards c
	LEFT JOIN Images i ON i.Id = c.id
WHERE 
	i.Id IS NULL
	AND c.image_uris IS NOT NULL
	AND c.image_uris <> ''

UPDATE
	Images
SET	
	small = (SELECT TOP 1 JSON_VALUE(REPLACE(REPLACE(c.image_uris,CHAR(39),'"'),'None','"N/A"'),'$.small') WHERE c.Id = i.Id),
	normal = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.image_uris,CHAR(39),'"'),'None','"N/A"'),'$.normal') WHERE c.Id = i.Id),
	large = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.image_uris,CHAR(39),'"'),'None','"N/A"'),'$.large') WHERE c.Id = i.Id),
	png = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.image_uris,CHAR(39),'"'),'None','"N/A"'),'$.png') WHERE c.Id = i.Id),
	art_crop = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.image_uris,CHAR(39),'"'),'None','"N/A"'),'$.art_crop') WHERE c.Id = i.Id),
	border_crop = (SELECT TOP 1  JSON_VALUE(REPLACE(REPLACE(c.image_uris,CHAR(39),'"'),'None','"N/A"'),'$.border_crop') WHERE c.Id = i.Id)	
FROM
	Cards c
	JOIN Images i ON c.id=i.Id



EXEC [dbo].[SeedDatabase]