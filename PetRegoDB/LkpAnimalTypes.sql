CREATE TABLE [dbo].[LkpAnimal]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [AnimalType] NCHAR(20) NOT NULL, 
    [FoodSource] NCHAR(20) NOT NULL
)
