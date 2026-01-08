DELETE FROM SpreadsheetRow;
DBCC CHECKIDENT ('SpreadsheetRow', RESEED, 0);
GO

SELECT * FROM SpreadsheetRow

SELECT
	s.Id,
	s.Qty,
	s._Set,
	s._SetNumber,
	s.Mark,
	s.Language,
	c.name,
	c.type_line,
	c.rarity,
	s.Confirmed,
	s.Location,
	'' as 'Type 1',
	'' as 'Type 2',
	'' as 'Type 3',
	s.Note
FROM
	SpreadsheetRow s
	JOIN Cards c ON CONCAT(s._Set,s._SetNumber) = CONCAT(c.[set],c.[collector_number])
ORDER BY s.Id

SELECT COUNT(ID) FROM InventoryV2

SELECT 
	* 
FROM 
	SpreadsheetView
ORDER BY
	[Set Code], CAST(collector_number as int)




DELETE FROM InventoryV2
DBCC CHECKIDENT ('[InventoryV2]', RESEED, 0);
GO



DECLARE @QTY INT, @CardCounter INT = 1, @Offset INT = 0, @Debug INT = 0, @SSID INT;


WHILE (SELECT COUNT(ID) FROM SpreadsheetRow)>0
BEGIN
	
	SET @SSID = (SELECT TOP 1 Id FROM SpreadsheetRow)
	SET @QTY = (SELECT Qty FROM SpreadsheetRow WHERE Id = @SSID);
	SELECT @QTY


	WHILE @CardCounter <= @QTY
	BEGIN

		--CHANGE TO INSERT INTO/UPDATE...
		SELECT 
			c.Id as 'Card_Id',
			s.Mark as 'Mark',
			s.[Location] as 'Location',
			s.Confirmed as 'Confirmed',
			GETDATE() as 'Confirmed Date',
			s.[Language] as 'Language',
			'Google' as 'UpdateUser'
		FROM 
			SpreadsheetRow s
			JOIN Cards c ON CONCAT(s._Set,s._SetNumber) = CONCAT(c.[set],c.[collector_number])	
		WHERE s.Id=@SSID;

		SET @CardCounter = @CardCounter + 1
	END

	SELECT 'Delete from DB'
	DELETE FROM SpreadsheetRow WHERE Id=@SSID	
	SET @CardCounter = 1

END