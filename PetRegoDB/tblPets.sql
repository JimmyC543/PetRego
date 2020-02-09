CREATE TABLE [dbo].[tblPets]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FKOwnerId] UNIQUEIDENTIFIER NULL, 
    [FKAnimalTypeId] INT NOT NULL, 
    [Name] NCHAR(50) NOT NULL, 
    CONSTRAINT [FK_tblPet_tblOwners] FOREIGN KEY ([FKOwnerId]) REFERENCES tblOwners(Id), 
    CONSTRAINT [FKAnimalTypeId] FOREIGN KEY ([FKAnimalTypeId]) REFERENCES [LkpAnimal]([Id])
)