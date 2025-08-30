using Bogus;
using PetStore.Client.Models;

namespace PetStore.Tests.Factories;

public static class PetFactory
{
    private static readonly Faker Faker = new("en");

    public static Client.Models.Pet BuildRandomPet(PetStatus status = PetStatus.available)
    {
        var id = Faker.Random.Long(1_000_000_000_000, 9_000_000_000_000);
        var category = new Category
        {
            Id = Faker.Random.Int(1, 10),
            Name = Faker.Commerce.Categories(1)[0]
        };
        var tag = new Tag
        {
            Id = Faker.Random.Int(1, 1000),
            Name = Faker.Hacker.Noun()
        };
        var photoUrl = Faker.Internet.Url();

        return new Client.Models.Pet
        {
            Id = id,
            Name = Faker.Random.Word(),
            Status = status.ToString(),
            Category = category,
            PhotoUrls = new List<string> { photoUrl },
            Tags = new List<Tag> { tag }
        };
    }

    /// <summary>
    /// Build payload with invalid ID (0 or negative).
    /// Should trigger 400 Invalid ID on API.
    /// </summary>
    public static Client.Models.Pet BuildPetWithInvalidId()
    {
        var pet = BuildRandomPet();
        pet.Id = -200; // invalid id for swagger
        return pet;
    }

    /// <summary>
    /// Build payload missing required fields (name, photoUrls).
    /// Should trigger 405 Validation Exception on API.
    /// </summary>
    public static Client.Models.Pet BuildPetForValidationError()
    {
        return new Client.Models.Pet
        {
            Id = Faker.Random.Long(1_000_000_000_000, 9_000_000_000_000),
            Name = null!,              // required → null
            Status = PetStatus.available.ToString(),
            Category = null,           // optional, zostawiamy null
            PhotoUrls = null!,         // required → null
            Tags = null                // optional
        };
    }
}