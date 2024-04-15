using SPPA.Database;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Repository.Users;
using Microsoft.Extensions.Logging;

namespace SPPA.Repository.Users;

public class UserSettingsRepository : BaseRepository<UserSettings>, IUserSettingsRepository
{
    public UserSettingsRepository(
        ApplicationDbContext dbContext,
        ILogger<BaseRepository<UserSettings>> logger
    )
        : base(dbContext, logger)
    {
    }

}

