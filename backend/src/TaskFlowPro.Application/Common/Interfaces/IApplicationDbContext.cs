using Microsoft.EntityFrameworkCore;
using TaskFlowPro.Domain.Entities;

namespace TaskFlowPro.Application.Common.Interfaces;

// Defined in Application, implemented in Infrastructure
// This keeps Application layer free from EF Core dependency details
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
