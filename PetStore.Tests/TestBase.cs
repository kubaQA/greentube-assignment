using PetStore.Client.API;
using Xunit.Abstractions;
using Bogus;

namespace PetStore.Tests;

public abstract class TestBase
{
    protected readonly PetApiClient Api;
    protected readonly Faker Faker;
    protected readonly ITestOutputHelper Output;

    protected const string BaseUrl = "https://petstore.swagger.io/v2";

    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        Api = new PetApiClient(BaseUrl, output.WriteLine);
        Faker = new Faker("en");
    }
}