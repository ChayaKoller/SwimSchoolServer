using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;

namespace SwimSchool.Infrastructure.Persistence;

public class UnitOfWork(SwimSchoolDbContext db) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The data was changed by someone else. Please refresh and try again.");
        }
    }
}
