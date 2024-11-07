CREATE TABLE [dbo].[ApplicationMethods]
(
	[ID] INT  NOT NULL IDENTITY(1,1), 
    [Name] NVARCHAR(100) NOT NULL ,
	[ApplicableForGrass] [nvarchar](1) NULL,
	[ApplicableForArableAndHorticulture] [nvarchar](1) NULL,
	CONSTRAINT [PK_ApplicationMethods] PRIMARY KEY CLUSTERED ([ID] ASC)
)
