CREATE TABLE [dbo].[TopSoils]
(
	[ID] INT IDENTITY(1,1) NOT NULL, 
    [Name] NVARCHAR(100) NOT NULL,
	CONSTRAINT [PK_TopSoils] PRIMARY KEY CLUSTERED ([ID] ASC)	 
)