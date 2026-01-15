USE [CardCatalog]
GO

PRINT 'Adding dbo.UpdateCardListing'
	GO

CREATE OR ALTER PROCEDURE [dbo].[UpdateCardListing]
AS
BEGIN

PRINT 'Adding new cards...'

INSERT INTO Cards (id)
SELECT
    r.[id] as 'id'
FROM
	RawCards r
	LEFT JOIN Cards c ON r.id = c.id
WHERE
	c.id IS NULL;

PRINT 'Updating card details...'

UPDATE
	Cards
SET
	[object] = r.[object],
	[oracle_id] = r.[oracle_id],
	[multiverse_ids] = r.[multiverse_ids],
	[mtgo_id] = r.[mtgo_id],
	[arena_id] = r.[arena_id],
	[tcgplayer_id] = r.[tcgplayer_id],
	[cardmarket_id] = r.[cardmarket_id],
	[name] = r.[name],
	[lang] = r.[lang],
	[released_at] = r.[released_at],
	[uri] = r.[uri],
	[scryfall_uri] = r.[scryfall_uri],
	[layout] = r.[layout],
	[highres_image] = r.[highres_image],
	[image_status] = r.[image_status],
	[image_uris] = r.[image_uris],
	[mana_cost] = r.[mana_cost],
	[cmc] = r.[cmc],
	[type_line] = r.[type_line],
	[oracle_text] = r.[oracle_text],
	[colors] = r.[colors],
	[color_identity] = r.[color_identity],
	[keywords] = r.[keywords],
	[produced_mana] = r.[produced_mana],
	[legalities] = r.[legalities],
	[games] = r.[games],
	[reserved] = r.[reserved],
	[game_changer] = r.[game_changer],
	[foil] = r.[foil],
	[nonfoil] = r.[nonfoil],
	[finishes] = r.[finishes],
	[oversized] = r.[oversized],
	[promo] = r.[promo],
	[reprint] = r.[reprint],
	[variation] = r.[variation],
	[set_id] = r.[set_id],
	[set] = r.[set],
	[set_name] = r.[set_name],
	[set_type] = r.[set_type],
	[set_uri] = r.[set_uri],
	[set_search_uri] = r.[set_search_uri],
	[scryfall_set_uri] = r.[scryfall_set_uri],
	[rulings_uri] = r.[rulings_uri],
	[prints_search_uri] = r.[prints_search_uri],
	[collector_number] = r.[collector_number],
	[digital] = r.[digital],
	[rarity] = r.[rarity],
	[card_back_id] = r.[card_back_id],
	[artist] = r.[artist],
	[artist_ids] = r.[artist_ids],
	[illustration_id] = r.[illustration_id],
	[border_color] = r.[border_color],
	[frame] = r.[frame],
	[full_art] = r.[full_art],
	[textless] = r.[textless],
	[booster] = r.[booster],
	[story_spotlight] = r.[story_spotlight],
	[prices] = r.[prices],
	[related_uris] = r.[related_uris],
	[purchase_uris] = r.[purchase_uris],
	[mtgo_foil_id] = r.[mtgo_foil_id],
	[power] = r.[power],
	[toughness] = r.[toughness],
	[flavor_text] = r.[flavor_text],
	[edhrec_rank] = r.[edhrec_rank],
	[penny_rank] = r.[penny_rank],
	[all_parts] = r.[all_parts],
	[promo_types] = r.[promo_types],
	[security_stamp] = r.[security_stamp],
	[card_faces] = r.[card_faces],
	[preview] = r.[preview],
	[watermark] = r.[watermark],
	[frame_effects] = r.[frame_effects],
	[loyalty] = r.[loyalty],
	[printed_name] = r.[printed_name],	
	[resource_id] = r.[resource_id],
	[tcgplayer_etched_id] = r.[tcgplayer_etched_id],
	[flavor_name] = r.[flavor_name],
	[attraction_lights] = r.[attraction_lights],
	[color_indicator] = r.[color_indicator],
	[printed_type_line] = r.[printed_type_line],
	[printed_text] = r.[printed_text],
	[variation_of] = r.[variation_of],
	[life_modifier] = r.[life_modifier],
	[hand_modifier] = r.[hand_modifier],
	[content_warning] = r.[content_warning],
	[defense] = r.[defense]
FROM
	Cards c
	JOIN RawCards r ON c.Id=r.Id;

PRINT 'Removing Raw Data...'
DELETE FROM RawCards;


PRINT 'Complete!'
END;
	GO

PRINT 'Adding dbo.UpdateSecondaryTables'
	GO

CREATE OR ALTER PROCEDURE [dbo].[UpdateSecondaryTables] AS
BEGIN 
	PRINT 'Updating Legalities'
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
	
	PRINT 'Updating Prices'
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

	PRINT 'Updating Images'
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

END;
	GO

PRINT 'Adding dbo.InsertCardOnSetAndNum'
	GO

CREATE OR ALTER PROCEDURE [dbo].[InsertCardOnSetAndNum]
	@Set NVARCHAR(MAX), 
	@Num NVARCHAR(MAX), 
	@Mark NVARCHAR(MAX),
	@Lang NVARCHAR(MAX),
	@Location NVARCHAR(MAX), 
	@Confirmed INT, 
	@QTY INT
AS
BEGIN
	DECLARE @CardID NVARCHAR(MAX) = (SELECT Id FROM Cards WHERE [set] = @Set AND [collector_number] = @Num);
	DECLARE @Count INT = 1;
	SELECT @CardID;

	CREATE TABLE #TempInventory(
		[Id]             INT           IDENTITY (1, 1) NOT NULL,
		[Card_Id]        VARCHAR (250) NULL,
		[Mark]           VARCHAR (10)  NULL,
		[Location]       VARCHAR (100) NULL,
		[Confirmed]      BIT           NULL,
		[Confirmed_date] DATETIME2 (7) NULL,
		[Language]       VARCHAR (100) NULL,
		[UpdateUser]     VARCHAR (100) NULL
	)

	CREATE TABLE #TempLocation(
		[ID] INT IDENTITY(1,1) PRIMARY KEY,
		[Location] NVARCHAR(MAX) NULL
	)

	INSERT INTO #TempLocation SELECT value FROM string_split(@location,char(10))
	
	IF (SELECT COUNT(Id) FROM #TempLocation) < @QTY
		BEGIN
			WHILE (SELECT COUNT(Id) FROM #TempLocation) < @QTY
				BEGIN
					INSERT INTO #TempLocation ([Location]) VALUES ('N/A')
				END
		END

	--If there are too many options for location than there are for quantity, use whole block
	IF (SELECT COUNT(Id) FROM #TempLocation) > @QTY
		BEGIN
			INSERT INTO #TempInventory (Card_Id,[Mark],[Location],[Confirmed],[Confirmed_date],[Language],[UpdateUser])
			SELECT
				@CardID,
				@Mark,
				@Location,
				@Confirmed,
				GETDATE(),
				@Lang,
				'Python'
		END
	ELSE
		BEGIN
			INSERT INTO #TempInventory (Card_Id,[Mark],[Location],[Confirmed],[Confirmed_date],[Language],[UpdateUser])
			SELECT
				@CardID,
				@Mark,
				[Location],
				@Confirmed,
				GETDATE(),
				@Lang,
				'Python'
			FROM
				#TempLocation			
		END

	SELECT * FROM #TempInventory

	INSERT INTO InventoryV2
	SELECT Card_Id,[Mark],[Location],[Confirmed],[Confirmed_date],[Language],[UpdateUser]
	FROM #TempInventory
	
	SELECT * FROM InventoryV2

	DROP TABLE #TempLocation
	DROP TABLE #TempInventory

END

PRINT 'Adding dbo.UpdateSecondaryTables'
	GO
	
--DROP TABLE [dbo].[PriceHistory];
--	GO

--DROP TABLE [dbo].[Legalities];
--	GO

--DROP TABLE [dbo].[InventoryV2];
--	GO

--PRINT 'Dropping Transaction Log'
--	GO

--DROP TABLE [dbo].[TransactionLog]
--	GO

PRINT 'Dropping Locations'
	GO

DROP TABLE [dbo].[Locations]
	GO

--DROP TABLE [dbo].[Images]
--	GO

PRINT 'Dropping Inventory Tags'
	GO

DROP TABLE [dbo].[InvTags]
	GO

--DROP VIEW [dbo].[SpreadsheetView]
--	GO

--CREATE TABLE [dbo].[InventoryV2] (
--    [Id]             INT           IDENTITY (1, 1) NOT NULL,
--    [Card_Id]        VARCHAR (250) NULL,
--    [Mark]           VARCHAR (10)  NULL,
--    [Location]       VARCHAR (100) NULL,
--    [Confirmed]      BIT           NULL,
--    [Confirmed_date] DATETIME2 (7) NULL,
--	[Language]		 VARCHAR (100) NULL,
--	[UpdateUser]     VARCHAR (100) NULL,
--    PRIMARY KEY CLUSTERED ([Id] ASC),
--    CONSTRAINT [FK_InventoryV2_ToCards] FOREIGN KEY ([Card_Id]) REFERENCES [dbo].[Cards] ([id])
--);
--	GO

PRINT 'Creating the Inventory Tags table'
	GO

CREATE TABLE [dbo].[InvTags]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	InvId INT NOT NULL,
	tagNAme VARCHAR(100) NOT NULL
)
	GO

PRINT 'Creating the Locations table'
	GO

CREATE TABLE [dbo].[Locations]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] VARCHAR(255) NOT NULL,
	[Type] VARCHAR(100) NOT NULL
)
	GO

--CREATE TABLE [dbo].[PriceHistory] (
--    [CardId]      VARCHAR (250) NOT NULL,
--    [usd]         VARCHAR (10)  NULL,
--    [usd_foil]    VARCHAR (10)  NULL,
--    [usd_etched]  VARCHAR (10)  NULL,
--    [eur]         VARCHAR (10)  NULL,
--    [eur_foil]    VARCHAR (10)  NULL,
--    [eur_etched]  VARCHAR (10)  NULL,
--    [tix]         VARCHAR (10)  NULL,
--    [Update_Date] DATETIME2 (7) NULL
--);
--	GO

--CREATE TABLE [dbo].[Legalities] (
--    [CardId]          VARCHAR (250) NOT NULL,
--    [standard]        VARCHAR (50)  NULL,
--    [future]          VARCHAR (50)  NULL,
--    [historic]        VARCHAR (50)  NULL,
--    [timeless]        VARCHAR (50)  NULL,
--    [gladiator]       VARCHAR (50)  NULL,
--    [pioneer]         VARCHAR (50)  NULL,
--    [explorer]        VARCHAR (50)  NULL,
--    [modern]          VARCHAR (50)  NULL,
--    [legacy]          VARCHAR (50)  NULL,
--    [pauper]          VARCHAR (50)  NULL,
--    [vintage]         VARCHAR (50)  NULL,
--    [penny]           VARCHAR (50)  NULL,
--    [commander]       VARCHAR (50)  NULL,
--    [oathbreaker]     VARCHAR (50)  NULL,
--    [standardbrawl]   VARCHAR (50)  NULL,
--    [brawl]           VARCHAR (50)  NULL,
--    [alchemy]         VARCHAR (50)  NULL,
--    [paupercommander] VARCHAR (50)  NULL,
--    [duel]            VARCHAR (50)  NULL,
--    [oldschool]       VARCHAR (50)  NULL,
--    [premodern]       VARCHAR (50)  NULL,
--    [predh]           VARCHAR (50)  NULL,
--    [Update_Date]     DATETIME2 (7) NULL
--);
--	GO

--PRINT 'Creating the Transaction Log table'
--	GO

--CREATE TABLE [dbo].[TransactionLog] (
--    [Id]              INT           IDENTITY (1, 1) NOT NULL,
--    [InventoryId]     INT           NOT NULL,
--    [Card_Id]         VARCHAR (250) NULL,
--    [UpdateType]      VARCHAR (100) NULL,
--    [TransactionDate] DATETIME2 (7) NULL,
--	[TransactionUser] VARCHAR (100) NULL,
--    [Description]     VARCHAR (MAX) NULL,
--    PRIMARY KEY CLUSTERED ([Id] ASC)
--);
--	GO

--CREATE TABLE [dbo].[Images] (
--    [Id]          VARCHAR (250) NOT NULL,
--    [small]       VARCHAR (250) NULL,
--    [normal]      VARCHAR (250) NULL,
--    [large]       VARCHAR (250) NULL,
--    [png]         VARCHAR (250) NULL,
--    [art_crop]    VARCHAR (250) NULL,
--    [border_crop] VARCHAR (250) NULL,
--    PRIMARY KEY CLUSTERED ([Id] ASC)
--);
--	GO

PRINT 'Creating or altering spreadsheet view'
	
	GO

CREATE OR ALTER VIEW [dbo].[SpreadsheetView]
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
	GROUP BY c.id, c.[name], c.[set], c.collector_number, i.Mark, i.[Location],i.[Language], c.rarity, c.type_line, usd, usd_etched, usd_foil;
	GO


PRINT 'Populating the Inventory Tags with expensive cards'
	GO

INSERT INTO InvTags (InvId,tagName)
SELECT
	i.Id,
	'Expensive'
FROM
	InventoryV2 i 
	LEFT JOIN Cards c ON i.Card_Id = c.id
	LEFT JOIN SpreadsheetView s ON s.id = c.id
WHERE 
	s.Price > 5
	GO

EXEC UpdateCardListing
	GO

EXEC UpdateSecondaryTables
	GO