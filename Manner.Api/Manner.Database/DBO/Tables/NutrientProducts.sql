CREATE TABLE [dbo].[NutrientProducts]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[Name] NVARCHAR(255) NOT NULL,
	[NutrientID] INT NOT NULL,
	[NutrientPercentage] Decimal(18, 2) NOT NULL,
	[IsNutrientDefaultProduct] BIT NOT NULL,
	[MeasurementUnit] NVARCHAR(20) NOT NULL,
	CONSTRAINT [PK_NutrientProducts] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_NutrientProducts_Nutrients] FOREIGN KEY ([NutrientID]) REFERENCES [dbo].[Nutrients]([ID])
)
