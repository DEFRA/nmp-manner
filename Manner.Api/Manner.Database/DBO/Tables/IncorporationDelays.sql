CREATE TABLE [dbo].[IncorporationDelays] (
    [ID]        INT IDENTITY(1,1) NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [Hours] INT            NULL,
    [CumulativeHours]   INT            NULL,
    [ApplicableFor] [nvarchar](1) NULL,
    CONSTRAINT [PK_IncorporationDelays] PRIMARY KEY CLUSTERED ([ID] ASC)
);


