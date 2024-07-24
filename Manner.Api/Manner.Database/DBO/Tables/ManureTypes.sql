CREATE TABLE [dbo].[ManureTypes]
(
	[ID] [int] NOT NULL IDENTITY(0,1),
	[Name] [nvarchar](100) NOT NULL,
	[ManureGroupID] [int] NOT NULL,
	[ManureTypeCategoryID] [int] NOT NULL,
	[CountryID] [int] NOT NULL,
	[HighReadilyAvailableNitrogen] [bit] NOT NULL,
	[IsLiquid] [bit] NOT NULL,
	[DryMatter] [decimal](18, 2) NOT NULL,
	[TotalN] [decimal](18, 2) NOT NULL,
	[NH4N] [decimal](18, 2) NOT NULL,
	[Uric] [decimal](18, 2) NOT NULL,
	[NO3N] [decimal](18, 2) NOT NULL,
	[P2O5] [decimal](18, 2) NOT NULL,
	[K2O] [decimal](18, 2) NOT NULL,
	[SO3] [decimal](18, 2) NOT NULL,
	[MgO] [decimal](18, 2) NOT NULL,
	[P2O5Available] [int] NOT NULL,
	[K2OAvailable] [int] NOT NULL,
	[NMaxConstant] [decimal](18, 2) NOT NULL,
	[ApplicationRateArable] [int] NOT NULL,
	[ApplicationRateGrass] [int] NOT NULL,	
	CONSTRAINT [PK_ManureTypes] PRIMARY KEY CLUSTERED ([ID] ASC),	
	CONSTRAINT [FK_ManureTypes_ManureGroups] FOREIGN KEY (ManureGroupID) REFERENCES ManureGroups(ID),
    CONSTRAINT [FK_ManureTypes_Countries] FOREIGN KEY (CountryID) REFERENCES Countries(ID),
	CONSTRAINT [FK_ManureTypes_ManureTypeCategories] FOREIGN KEY ([ManureTypeCategoryID]) REFERENCES [dbo].[ManureTypeCategories] ([ID])

)
