using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Interfaces;

namespace SPPA.Domain.Entities.WorkSpaces;

/// <summary>
/// Рабочее пространство отдельной компании
/// </summary>
public class Workspace : IEntityGuid
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<Order> Orders { get; set; }

    public List<UserRole> Roles { get; set; }

    public List<UserInvite> Invites { get; set; }

    public Workspace(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Orders = new List<Order>();
        Roles = new List<UserRole>();
        Invites = new List<UserInvite>();
    }

    public int GetUserCount()
    {
        return Roles?.Count ?? 0;
    }



}

