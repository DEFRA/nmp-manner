CREATE TABLE [dbo].[SubSoils]
(
	[ID] INT IDENTITY(1,1) NOT NULL, 
    [Name] NVARCHAR(100) NOT NULL,
    [VolumetricMeasure] INT NOT NULL, 
    [AvailableWaterCapacity] INT NOT NULL, 
	CONSTRAINT [PK_SubSoils] PRIMARY KEY CLUSTERED ([ID] ASC)	 
)