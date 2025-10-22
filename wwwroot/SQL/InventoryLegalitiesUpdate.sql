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

SELECT * FROM Legalities
