﻿CREATE TABLE [dbo].[ManureTypes]
(
	[ID] INT NOT NULL IDENTITY(0,1),
	[Name] NVARCHAR(100) NOT NULL,
	[ManureGroupID] INT NOT NULL,
	[ManureTypeCategoryID] [int] NOT NULL,
	[CountryID] INT NOT NULL,
	[HighReadilyAvailableNitrogen] BIT NOT NULL,
	[IsLiquid] BIT NOT NULL,
	[DryMatter] DECIMAL(18, 2) NOT NULL,
	[TotalN] DECIMAL(18, 2) NOT NULL,
	[NH4N] DECIMAL(18, 2) NOT NULL,
	[Uric] DECIMAL(18, 2) NOT NULL,
	[NO3N] DECIMAL(18, 2) NOT NULL,
	[P2O5] DECIMAL(18, 2) NOT NULL,
	[K2O] DECIMAL(18, 2) NOT NULL,
	[SO3] DECIMAL(18, 2) NOT NULL,
	[MgO] DECIMAL(18, 2) NOT NULL,
	[P2O5Available] INT NOT NULL,
	[K2OAvailable] INT NOT NULL,
	[NMaxConstant] DECIMAL(18, 2) NOT NULL,
	[ApplicationRateArable] INT NOT NULL,
	[ApplicationRateGrass] INT NOT NULL,	
	[SO3AvaiableAutumnOther] DECIMAL(18, 2) NOT NULL,
	[SO3AvaiableAutumnOsrGrass] DECIMAL(18, 2) NOT NULL,
	[SO3AvailableSpring] DECIMAL(18, 2) NOT NULL,
	[PercentOfTotalNForUseInNmaxCalculation] INT NULL,
	[SortOrder] INT NOT NULL Default 0,
    CONSTRAINT [PK_ManureTypes] PRIMARY KEY CLUSTERED ([ID] ASC),	
	CONSTRAINT [FK_ManureTypes_ManureGroups] FOREIGN KEY (ManureGroupID) REFERENCES ManureGroups(ID),
    CONSTRAINT [FK_ManureTypes_Countries] FOREIGN KEY (CountryID) REFERENCES Countries(ID),
	CONSTRAINT [FK_ManureTypes_ManureTypeCategories] FOREIGN KEY ([ManureTypeCategoryID]) REFERENCES [dbo].[ManureTypeCategories] ([ID])

)
