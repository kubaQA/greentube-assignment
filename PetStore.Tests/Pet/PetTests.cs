using System.Net;
using Allure.Xunit.Attributes;
using FluentAssertions;
using PetStore.Client.Models;
using PetStore.Tests.Factories;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PetStore.Tests.Pet;

[AllureSuite("Pet API")]
[AllureEpic("Swagger Petstore")]
[AllureOwner("kubaQA")]
public class PetTests : TestBase, IAsyncLifetime
{
    private Client.Models.Pet _payload = default!;
    private long _petId;
    private Client.Models.Pet? _created;

    public PetTests(ITestOutputHelper output) : base(output) { }

    // ===== BEFORE EACH TEST =====
    public async Task InitializeAsync()
    {
        _payload = PetFactory.BuildRandomPet();

        var (resp, pet, err) = await Api.CreatePetAsync(_payload);

        if (resp.StatusCode != HttpStatusCode.OK || pet is null)
        {
            var msg = $"Setup failed: POST /pet returned {(int)resp.StatusCode} {resp.StatusCode} " +
                      $"error={(err?.Message ?? "<null>")}";
            throw new XunitException(msg);
        }

        _created = pet;
        _created.Id.Should().NotBeNull("server should assign Id");
        _petId = _created.Id!.Value;
    }

    // ===== AFTER EACH TEST =====
    public async Task DisposeAsync()
    {
        if (_petId != 0)
        {
            var (resp, _) = await Api.DeletePetAsync(_petId);
            if ((int)resp.StatusCode is < 200 or >= 300)
                Output.WriteLine($"Teardown warning: DELETE /pet/{_petId} -> {(int)resp.StatusCode} {resp.StatusCode}");
        }
    }

    [AllureFeature("Get pet by id")]
    [AllureStory("GET /pet/{id}")]
    [AllureTag("happy-path", "get")]
    [AllureLink("Swagger", "https://petstore.swagger.io/#/pet/getPetById")]
    [Fact]
    public async Task GetPetById()
    {
        var fetched = await TestUtils.RetryUntilAsync(
            async () =>
            {
                var (getResponse, pet, getError) = await Api.GetPetByIdAsync(_petId);
                if (getResponse.StatusCode != HttpStatusCode.OK || pet is null || getError is not null)
                    return null;
                return pet;
            }
        );

        fetched.Should().NotBeNull("created pet should eventually be retrievable by id");
        
        fetched!.Id.Should().Be(_petId);
        fetched.Name.Should().Be(_payload.Name);
        fetched.Status.Should().Be(_payload.Status);
        fetched.Category!.Id.Should().Be(_payload.Category!.Id);
        fetched.Category!.Name.Should().Be(_payload.Category!.Name);
        fetched.PhotoUrls.Should().BeEquivalentTo(_payload.PhotoUrls);
        fetched.Tags!.Should().HaveCount(1);
    }
    
    [AllureFeature("Find all available pets")]
    [AllureStory("GET /pet/findByStatus")]
    [AllureTag("happy-path", "list")]
    [AllureLink("Swagger", "https://petstore.swagger.io/#/pet/findPetsByStatus")]
    [Fact]
    public async Task FindCreatedPetOnAvailablePetsList()
    {
        var matched = await TestUtils.RetryUntilAsync(
            async () =>
            {
                var (listResponse, pets, listError) = await Api.FindPetsByStatusAsync(PetStatus.available.ToString());
                if (listResponse.StatusCode != HttpStatusCode.OK || listError is not null || pets is null)
                    return null;

                return pets.FirstOrDefault(p => p.Id == _petId);
            }
        );

        matched.Should().NotBeNull("freshly created pet should eventually be present in 'available' list");
        
        matched!.Id.Should().Be(_petId);
        matched.Name.Should().Be(_payload.Name);
        matched.Status.Should().Be(_payload.Status);
        matched.Category!.Id.Should().Be(_payload.Category!.Id);
        matched.Category!.Name.Should().Be(_payload.Category!.Name);
        matched.PhotoUrls.Should().BeEquivalentTo(_payload.PhotoUrls);
        matched.Tags!.Should().HaveCount(1);
    }
    
    [AllureFeature("Update pet")]
    [AllureStory("PUT /pet")]
    [AllureTag("happy-path", "put")]
    [AllureLink("Swagger", "https://petstore.swagger.io/#/pet/updatePet")]
    [Fact]
    public async Task UpdatePetStatusToSoldAndVerifyOnLists()
    {
        _payload.Status = PetStatus.sold.ToString();
        var (updateResponse, updatedPet, updateError) = await Api.UpdatePetAsync(_payload);
        
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateError.Should().BeNull();
        updatedPet.Should().NotBeNull();
        updatedPet!.Status.Should().Be(PetStatus.sold.ToString());
        
        var notInAvailable = await TestUtils.RetryUntilAsync(
            async () =>
            {
                var (availableResponse, availablePets, availableError) = await Api.FindPetsByStatusAsync(PetStatus.available.ToString());
                if (availableResponse.StatusCode != HttpStatusCode.OK || availableError is not null || availablePets is null)
                    return false;

                return availablePets.All(p => p.Id != _petId);
            }
        );
        notInAvailable.Should().BeTrue("pet updated to sold should eventually disappear from 'available' list");
        
        var matchedSold = await TestUtils.RetryUntilAsync(
            async () =>
            {
                var (soldResponse, soldPets, soldError) = await Api.FindPetsByStatusAsync(PetStatus.sold.ToString());
                if (soldResponse.StatusCode != HttpStatusCode.OK || soldError is not null || soldPets is null)
                    return null;

                return soldPets.FirstOrDefault(p => p.Id == _petId);
            }
        );

        matchedSold.Should().NotBeNull("pet updated to sold should eventually appear in 'sold' list'");
        matchedSold!.Id.Should().Be(_petId);
        matchedSold.Name.Should().Be(_payload.Name);
        matchedSold.Status.Should().Be(PetStatus.sold.ToString());
        matchedSold.Category!.Id.Should().Be(_payload.Category!.Id);
        matchedSold.Category!.Name.Should().Be(_payload.Category!.Name);
        matchedSold.PhotoUrls.Should().BeEquivalentTo(_payload.PhotoUrls);
        matchedSold.Tags!.Should().HaveCount(1);
    }
    
    [AllureFeature("Update pet – Negative 400")]
    [AllureStory("PUT /pet returns 400 for invalid id")]
    [AllureTag("negative", "put")]
    [AllureLink("Swagger", "https://petstore.swagger.io/#/pet/updatePet")]
    [Fact]
    public async Task PutShouldReturn400ForInvalidId()
    {
        var invalid = PetFactory.BuildPetWithInvalidId();

        var (resp, _, err) = await Api.UpdatePetAsync(invalid);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        err.Should().NotBeNull();
        err!.Code.Should().Be(400);
        err.Message.Should().Be("Invalid ID supplied");
    }
    
    [AllureFeature("Update pet – Negative 404")]
    [AllureStory("PUT /pet returns 404 when pet not found")]
    [AllureTag("negative", "put")]
    [AllureLink("Swagger", "https://petstore.swagger.io/#/pet/updatePet")]
    [Fact]
    public async Task UpdateShouldReturn404WhenPetNotFound()
    {
        var nonexistent = PetFactory.BuildRandomPet();
        nonexistent.Id = 99_999_999_999_999;

        var (resp, _, err) = await Api.UpdatePetAsync(nonexistent);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        err.Should().NotBeNull();
        err!.Code.Should().Be(404);
        err.Message.Should().Be("Pet not found");
    }
    
    [AllureFeature("Update pet – Negative 405")]
    [AllureStory("PUT /pet returns 405 validation exception")]
    [AllureTag("negative", "put")]
    [AllureLink("Swagger", "https://petstore.swagger.io/#/pet/updatePet")]
    [Fact]
    public async Task PutShouldReturn405ForValidationException()
    {
        var invalid = PetFactory.BuildPetForValidationError();
        var (resp, _, err) = await Api.UpdatePetAsync(invalid);

        ((int)resp.StatusCode).Should().Be(405);
        err.Should().NotBeNull();
        err!.Code.Should().Be(405);
        err.Message.Should().Be("Validation exception");
    }
}
