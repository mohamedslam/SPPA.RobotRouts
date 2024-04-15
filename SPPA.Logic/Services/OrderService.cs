using Microsoft.Extensions.Logging;
using System.Text.Json;
using AutoMapper;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Orders.Files;
using SPPA.Logic.Dto.Orders;
using SPPA.Domain.Repository.Orders;
using SPPA.Domain.Repository.Reports;

namespace SPPA.Logic.Services;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IMapper _mapper;
    private readonly IReportStore _reportStore;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderFileRepository _orderFileRepository;

    public OrderService(
        ILogger<OrderService> logger,
        IMapper mapper,
        IReportStore reportStore,
        IOrderRepository orderRepository,
        IOrderFileRepository orderFileRepository
    )

    {
        _logger = logger;
        _mapper = mapper;
        _reportStore = reportStore;
        _orderRepository = orderRepository;
        _orderFileRepository = orderFileRepository;
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto, Guid workspaceId, Guid userId)
    {
        try
        {
            Order model = _mapper.Map<Order>(dto);
            model.WorkspaceId = workspaceId;
            _orderRepository.Add(model);
            await _orderRepository.SaveChangesAsync();

            model = await _orderRepository.GetOrderWithRolesAndUsersAsync(model.Id);

            var modelDto = _mapper.Map<OrderDto>(model);
            modelDto.UserRole = model.GetUserRole(userId);
            return modelDto;
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on create Order entity. DTO: " + json);
            throw;
        }
    }

    public async Task<OrderPreviewDto[]> GetAllForUserAsync(Guid workspaceId, Guid userId)
    {
        try
        {
            var list = await _orderRepository.GetAllForUserAsync(workspaceId, userId);
            return _mapper.Map<OrderPreviewDto[]>(list);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read Order list entity");
            throw;
        }
    }

    public async Task<OrderDto> GetAsync(Guid orderId, Guid userId)
    {
        var model = await _orderRepository.GetOrderWithRolesAndUsersAsync(orderId);
        var ifcFiles = await _orderFileRepository.GetAllForOrderAsync(orderId, OrderFileTypeEnum.Ifc);
        model.OrderFiles = ifcFiles.ToList();

        var modelDto = _mapper.Map<OrderDto>(model);
        modelDto.UserRole = model.GetUserRole(userId);
        return modelDto;
    }

    public async Task<OrderDto> UpdateAsync(OrderUpdateDto dto, Guid orderId, Guid userId)
    {
        var model = await _orderRepository.GetOrderWithRolesAndUsersAsync(orderId);
        _mapper.Map(dto, model);

        try
        {
            _orderRepository.Update(model);
            await _orderRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on update OrderDto entity. DTO: " + json);
            throw;
        }

        var ifcFiles = await _orderFileRepository.GetAllForOrderAsync(orderId, OrderFileTypeEnum.Ifc);
        model.OrderFiles = ifcFiles.ToList();
        var modelDto = _mapper.Map<OrderDto>(model);
        modelDto.UserRole = model.GetUserRole(userId);
        return modelDto;
    }

    public async Task DeleteAsync(Guid orderId)
    {
        try
        {
            var model = await _orderRepository.GetByIdAsync(orderId);
            var files = await _orderFileRepository.GetAllForOrderAsync(orderId);
            foreach (var file in files)
            {
                _orderFileRepository.DeleteFile(file);
            }

            _orderRepository.Remove(model);
            await _orderRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on delete Orders entity. OrderId:{orderId}");
            throw;
        }
    }

    public IEnumerable<OrderReportInfoDto> GetReports(Guid orderId)
    {
        var result = new List<OrderReportInfoDto>();
        var reports = _reportStore.GetReportList();
        foreach (var report in reports)
        {
            var url = $"/reports/{orderId.ToString()}/{report.Id}";
            var reportDto = new OrderReportInfoDto(report.DisplayName, url);
            result.Add(reportDto);
        }

        return result;
    }

}
