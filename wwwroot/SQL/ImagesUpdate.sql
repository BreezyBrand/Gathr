DELETE FROM Images

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

SELECT * FROM Images