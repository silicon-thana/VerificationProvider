using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VerificationProvider.Data.Contexts;

namespace VerificationProvider.Services;

public class VerificationCleanerService(ILogger<VerificationCleanerService> logger, DataContext context) : IVerificationCleanerService
{
    private readonly ILogger<VerificationCleanerService> _logger = logger;
    private readonly DataContext _context = context;


    public async Task RemoveExpiredRecordAsync()
    {
        try
        {
            var expired = await _context.VerificationRequest.Where(x => x.ExpirationDate <= DateTime.Now).ToListAsync();
            _context.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationCleanerService.RemoveExpiredRecordAsync :: {ex.Message}");
        }
    }

}
