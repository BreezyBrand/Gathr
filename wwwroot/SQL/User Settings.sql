DROP TABLE [dbo].[UserSettings];

CREATE TABLE [dbo].[UserSettings]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Option] VARCHAR(255) not null,
	[Value] VARCHAR(255) not null
)

INSERT INTO UserSettings ([Option],[Value]) VALUES 
('DefaultGoogleSpreadsheet','1JzLf5LCDhzBetx_8T26RrKUDAGJDa4-zPzf8YqPmEaA'),
('DefaultGoogleRange','Class Data!A2:N26');