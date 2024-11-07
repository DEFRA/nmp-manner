CREATE TABLE [dbo].[IncorpMethodsIncorpDelays]
(
    IncorporationMethodID INT NOT NULL,
    IncorporationDelayID INT NOT NULL,
    CONSTRAINT [PK_IncorpMethodsIncorpDelays] PRIMARY KEY (IncorporationMethodID, IncorporationDelayID ASC),
    CONSTRAINT [FK_IncorpMethodsIncorpDelays_IncorporationMethods]  FOREIGN KEY (IncorporationMethodID) REFERENCES IncorporationMethods(ID),
    CONSTRAINT [FK_IncorpMethodsIncorpDelays_IncorporationDelay]  FOREIGN KEY (IncorporationDelayID) REFERENCES IncorporationDelays(ID)
)
