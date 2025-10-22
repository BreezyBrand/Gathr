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

SELECT * FROM PriceHistory