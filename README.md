# Pet Rego code project from Servian

This project contains my attempt at the PetRego challenge.

---

## Structure

I've tried to implement a service/repository pattern in this solution, to allow for separation of the business logic in the services, and the data access in the repositories.

In the presentation layer, there are two controllers: Owners and Pets.
The Service and Repository layers are also split into these two sections, as they perform very similar tasks with only marginally differing logic (for now, however I've tried to keep open the possibility for more complex logic in the future).


## Api versions
Because in v2.0 there are enhancements on the original 1.0 APIs, I've taken advantage of .NET Core's Api Versioning in the Startup class. By default, the APIs default is v1.0, and in order to call any other version, one needs to add `"X-Version": "major.minor"` in the headers of the api call, where `major` and `minor` are the major and minor versions of the targeted API respectively.

The main difference between v1.0 and v2.0 endpoints is that the data returned from v2.0 endpoints may include a `FoodSource` property, as this is relevent to v2.0 but not v1.0.

## Endpoints

All API calls are in the format `{domain}/api/{endpoint}/{parameter, when applicable}`

### Owner endpoints

* GetOwners: `{domain}/api/owners/`
	* Gets all owners, as well as the pets for each owner.
	* Works with both v1.0 and v2.0, however v2.0 also returns the food source for each pet.

* GetOwner: `{domain}/api/owners/{guid}`
	* Gets a specific owner by id, as well as any pets they may have.
	* Works with both v1.0 and v2.0, however v2.0 also returns the food source for each pet.

* GetPetsByOwnerId: `{domain}/api/owners/{guid}/pets`
	* Gets all pets for a specific owner by the provided owner id.
	* Works with both v1.0 and v2.0, however v2.0 also returns the food source for each pet.

* PutOwner: `{domain}/api/owners/{guid}`
	* Updates the details of the owner with id provided.
	* The body of the request must contain guid `OwnerId` (which needs to match the guid in the url), string `FirstName`, and string `LastName`. It is not possible to update the pets of an owner via this endpoint.
	* Works with both v1.0 and v2.0, with no differences.

* PostOwner: `{domain}/api/owners/{guid}`
	* Updates the details of the owner with id provided.
	* The body of the request must contain guid `OwnerId` (which needs to match the guid in the url), string `FirstName`, and string `LastName`. It is not possible to add the pets of an owner via this endpoint.
	* Works with both v1.0 and v2.0, with no differences.

* DeleteOwner: `{domain}/api/owners/{guid}`
	* Deletes a specific owner by id, as well as any pets they may have.
	* Works the same with both v1.0 and v2.0.

### Pet endpoints

* GetPets: `{domain}/api/pets/`
	* Gets all pets.
	* Works with both v1.0 and v2.0, however v2.0 also returns the food source for each pet.

* GetPet: `{domain}/api/pets/{guid}`
	* Gets a specific pet by id.
	* Works with both v1.0 and v2.0, however v2.0 also returns the food source the pet.

* GetSuitablePetFood: `{domain}/api/pets/{guid}/food`
	* Gets all pets for a specific pet by the provided owner id.
	* Works with both v1.0 and v2.0, however v2.0 also returns the food source for each pet.

* PutPet: `{domain}/api/pets/{guid}`
	* Updates the details of the pet with id provided.
	* The body of the request must contain guid `PetId` (which needs to match the guid in the url), string `Name`, integer `AnimalTypeId` (see below), and may contain the nullable integer `OwnerId`.
	* Works with both v1.0 and v2.0, with no differences.

* PostPet: `{domain}/api/pets/{guid}`
	* Updates the details of the pet with id provided.
	* The body of the request must contain guid `PetId` (which needs to match the guid in the url), string `Name`, integer `AnimalTypeId` (see below), and may contain the nullable integer `OwnerId`.
	* Works with both v1.0 and v2.0, with no differences.

* DeletePet: `{domain}/api/pets/{guid}`
	* Deletes a specific pet by id.
	* Works the same with both v1.0 and v2.0.

## Animal Types
I made the choice to turn AnimalTypes into a lookup table, rather than an in-memory enum or struct. The reason for this was that it's conceivable that we'll want to expand on the types of animals that are catered for. I was also going to add a separate table for "food source" as it's also foreseeable that we may want to add multiple food sources for an animal type, or share food sources between multiple animal types (or sub-types), however this seemed to be overkill for the current requirements.

The animal types are defined with the following integer IDs:

1. Dog
2. Cat
3. Chicken
4. Snake

There currently isn't any provided way to expand on the animal types lookup table, but this would probably be available in future versions.
