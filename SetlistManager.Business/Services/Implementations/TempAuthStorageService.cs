using Microsoft.EntityFrameworkCore;
using SetlistManager.Data;
using System.Security.Cryptography;
namespace SetlistManager.Business.Services.Implementations;

public class TempAuthStorageService : ITempAuthStorageService
{
    private const int _tempAuthSecretLength = 24;
    private readonly AppDbContext _dbContext;

    public TempAuthStorageService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> CreateNewTempAuthSecret(int userId)
    {
        byte[] randomBytes;
        string secret;
        
        do
        {
            randomBytes = RandomNumberGenerator.GetBytes(_tempAuthSecretLength);
            secret = Convert.ToBase64String(randomBytes);
        }
        while (await _dbContext.TempAuthStorage.AnyAsync(x => x.TempSecret == secret));

        await _dbContext.TempAuthStorage.AddAsync(new(){
            UserId = userId,
            TempSecret = secret,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        return secret;
    }
}