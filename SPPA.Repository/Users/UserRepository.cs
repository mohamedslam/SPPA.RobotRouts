using SPPA.Database;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Repository.Users;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Users;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<User>> logger
    )
        : base(dbContext, logger)
    {
    }

}

