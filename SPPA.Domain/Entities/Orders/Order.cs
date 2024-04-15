using SPPA.Domain.Entities.Orders.Files;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Entities.WorkSpaces;
using SPPA.Domain.Interfaces;

namespace SPPA.Domain.Entities.Orders;

/// <summary>
/// Заказ или проект
/// </summary>
public class Order : IEntityGuid
{
    public Guid Id { get; set; }

    /// <summary>
    /// Название заказа
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Шифр
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Заказчик проекта
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// Перемещено в архив
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Статус выполнения
    /// </summary>
    public OrderProgressStatusEnum Status { get; set; }

    /// <summary>
    /// Пользовательские свойства
    /// </summary>
    public List<OrderCustomField> CustomFields { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; }

    public List<UserRole> Roles { get; set; }

    public List<UserInvite> Invites { get; set; }

    public List<OrderFile> OrderFiles { get; set; }

    public Order(
        string name,
        string? code,
        string? customer,
        Guid workspaceId

      )
    : this()
    {
        Id = Guid.Empty;
        Name = name;
        Code = code;
        Customer = customer;
        Archived = false;
        Status = OrderProgressStatusEnum.Create;
        WorkspaceId = workspaceId;

    }

    private Order()
    {
        Roles = new List<UserRole>();
        Invites = new List<UserInvite>();
        OrderFiles = new List<OrderFile>();
        CustomFields = new List<OrderCustomField>();
    }

    public int GetUserCount()
    {
        var usersId = new List<Guid>();
        usersId.AddRange(this.Roles?.Select(x => x.UserId) ?? new List<Guid>());
        usersId.AddRange(this.Workspace?.Roles?.Select(x => x.UserId) ?? new List<Guid>());
        return usersId.Distinct().Count();
    }

    public UserRoleTypeEnum? GetUserRole(Guid userId)
    {
        var orderRole = this.Roles?.SingleOrDefault(x => x.UserId == userId);
        if (orderRole != null)
        {
            return orderRole.RoleType;
        }

        var workspaceRole = this.Workspace?.Roles?.SingleOrDefault(x => x.UserId == userId);
        if (workspaceRole != null)
        {
            return workspaceRole.RoleType;
        }

        return null;
    }
}