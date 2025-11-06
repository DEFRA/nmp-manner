/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
--TRUNCATE TABLE [dbo].[ManureTypes]

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IncorpMethodsIncorpDelays' AND TABLE_SCHEMA = 'DBO')
BEGIN

ALTER TABLE IncorpMethodsIncorpDelays
DROP CONSTRAINT FK_IncorpMethodsIncorpDelays_IncorporationDelay;

-- Temporarily update IDs to a safe temporary value to avoid conflicts
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 999 WHERE IncorporationDelayID = 16;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 998 WHERE IncorporationDelayID = 9;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 997 WHERE IncorporationDelayID = 10;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 996 WHERE IncorporationDelayID = 17;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 995 WHERE IncorporationDelayID = 11;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 994 WHERE IncorporationDelayID = 12;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 993 WHERE IncorporationDelayID = 13;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 992 WHERE IncorporationDelayID = 14;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 991 WHERE IncorporationDelayID = 15;

-- Now perform the actual update with the correct IDs
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 9 WHERE IncorporationDelayID = 999;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 10 WHERE IncorporationDelayID = 998;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 11 WHERE IncorporationDelayID = 997;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 12 WHERE IncorporationDelayID = 996;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 13 WHERE IncorporationDelayID = 995;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 14 WHERE IncorporationDelayID = 994;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 15 WHERE IncorporationDelayID = 993;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 16 WHERE IncorporationDelayID = 992;
UPDATE IncorpMethodsIncorpDelays SET IncorporationDelayID = 17 WHERE IncorporationDelayID = 991;

-- Add the foreign key constraint back
ALTER TABLE IncorpMethodsIncorpDelays
ADD CONSTRAINT FK_IncorpMethodsIncorpDelays_IncorporationDelay FOREIGN KEY (IncorporationDelayID) REFERENCES IncorporationDelays(ID)

END