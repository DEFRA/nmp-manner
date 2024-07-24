CREATE TABLE [dbo].[ApplicationMethodsIncorpMethods]
(	
   ApplicationMethodID INT NOT NULL,
   IncorporationMethodID INT NOT NULL,
   CONSTRAINT [PK_ApplicationMethodsIncorpMethods]  PRIMARY KEY (ApplicationMethodID,IncorporationMethodID ASC),
   CONSTRAINT [FK_ApplicationMethodsIncorpMethods_ApplicationMethods] FOREIGN KEY (ApplicationMethodID) REFERENCES ApplicationMethods(ID),
   CONSTRAINT [FK_ApplicationMethodsIncorpMethods_IncorporationMethods] FOREIGN KEY (IncorporationMethodID) REFERENCES IncorporationMethods(ID)
)
