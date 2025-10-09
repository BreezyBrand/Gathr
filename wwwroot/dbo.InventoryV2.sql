CREATE TABLE [dbo].[InventoryV2] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
	[Card_Id]        VARCHAR (250)  NULL,
    [Mark]           VARCHAR (10)  NULL,
    [Location]       VARCHAR (100) NULL,
    [Confirmed]      INT            NULL,
    [Confirmed_date] DATETIME2 (7)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_InventoryV2_ToCards] FOREIGN KEY ([Card_Id]) REFERENCES [Cards]([Id])
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


