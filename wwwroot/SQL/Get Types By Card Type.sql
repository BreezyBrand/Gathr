SELECT DISTINCT
	id,
	type_line,
	'Whole Card' as [Card Type]
FROM 
	Cards c
WHERE CHARINDEX('Token',type_line) = 0
AND	  CHARINDEX('//', type_line) < 1
UNION ALL SELECT 
				id,
				CASE WHEN CHARINDEX('//', type_line) > 2 THEN SUBSTRING(type_line, 1, CHARINDEX('//', type_line) - 1) 
				ELSE type_line
				END as type_line,
				'Split Card A'
				FROM Cards			
WHERE CHARINDEX('Token',type_line) = 0
AND	  CHARINDEX('//', type_line) > 1
UNION ALL SELECT 
				id,
				CASE WHEN CHARINDEX('//', type_line) > 2 THEN SUBSTRING(type_line, LEN(SUBSTRING(type_line, 1, CHARINDEX('//', type_line) - 1) )+5, 1000) 
				ELSE type_line
				END as Back,
				'Split Card B'				
				FROM Cards			
WHERE CHARINDEX('Token',type_line) = 0
AND	  CHARINDEX('//', type_line) > 1
ORDER BY id
	