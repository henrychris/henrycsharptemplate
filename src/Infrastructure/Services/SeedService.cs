namespace HenryCsharpTemplate.Infrastructure.Services;

public interface ISeedService
{
    Task SeedDataAsync();
}

public class SeedService : ISeedService
{
    public Task SeedDataAsync()
    {
        return Task.CompletedTask;
    }
}
