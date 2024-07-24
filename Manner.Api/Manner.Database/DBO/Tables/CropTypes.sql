CREATE TABLE [dbo].[CropTypes] (
    [ID]               INT            NOT NULL,
    [Name]             NVARCHAR (250) NOT NULL,
    [Use]              NVARCHAR (50)  NOT NULL,
    [CropUptakeFactor] INT            NOT NULL, 
    CONSTRAINT [PK_MannerCropTypes] PRIMARY KEY ([ID])
);

