SELECT
	(CASE WHEN COALESCE(c.flavor_name, '') = '' THEN c.name
		  ELSE c.flavor_name
	END) AS 'Name',
	c.[set],
	c.collector_number,
	(CASE 
		WHEN Mark = 'f-etch'	THEN p.usd_etched
		WHEN Mark = 'f-pre'		THEN p.usd_foil
		WHEN Mark = 'f-pp'		THEN p.usd
		WHEN Mark = 'pp'		THEN p.usd
		WHEN Mark = 'f'			THEN p.usd_foil
		WHEN Mark = 'f list'	THEN p.usd_foil
		WHEN Mark = '*pp*'		THEN p.usd
		WHEN Mark = 'list'		THEN p.usd
		WHEN Mark = 'f *pp*'	THEN p.usd_foil
		ELSE p.usd
	END) AS 'Value',
	p.usd,
	p.usd_foil,
	p.usd_etched
FROM
	InventoryV2 i
	JOIN PriceHistory p ON i.Card_Id = p.CardId
	JOIN Cards c ON c.Id = i.Card_Id
ORDER BY c.[set]