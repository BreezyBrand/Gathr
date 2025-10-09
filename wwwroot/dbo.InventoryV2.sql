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

