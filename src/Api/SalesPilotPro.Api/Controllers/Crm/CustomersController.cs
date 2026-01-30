using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Api.Controllers.Base;
using SalesPilotPro.Api.Controllers.Crm.Models;
using SalesPilotPro.Api.Security;
using SalesPilotPro.Api.Shared;
using SalesPilotPro.Infrastructure.Entities.Crm;
using SalesPilotPro.Infrastructure.Persistence;

namespace SalesPilotPro.Api.Controllers.Crm;

[ApiVersion("1.0")]
[AuthorizeModule("CRM")]
[Route("api/v{version:apiVersion}/crm/customers")]
public sealed class CustomersController : ApiControllerBase
{
    private readonly SalesPilotProDbContext _db;

    public CustomersController(SalesPilotProDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] string? sortBy = "name",
        [FromQuery] string? sortDir = "asc",
        [FromQuery] string? status = "active")
    {
        var query = _db.Customers.AsNoTracking();

        query = status?.ToLower() switch
        {
            "inactive" => query.Where(x => !x.IsActive),
            "all" => query,
            _ => query.Where(x => x.IsActive)
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Code.Contains(search));
        }

        query = (sortBy?.ToLower(), sortDir?.ToLower()) switch
        {
            ("code", "desc") => query.OrderByDescending(x => x.Code),
            ("code", _) => query.OrderBy(x => x.Code),
            ("name", "desc") => query.OrderByDescending(x => x.Name),
            _ => query.OrderBy(x => x.Name)
        };

        var total = await query.CountAsync();

        var customers = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var items = customers.Select(x => new CustomerDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            IsActive = x.IsActive
        });

        return Ok(ApiResponse<object>.Ok(new
        {
            total,
            skip,
            take,
            items
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (customer is null)
            return NotFound(ApiResponse<CustomerDto>.Fail("Customer not found"));

        return Ok(ApiResponse<CustomerDto>.Ok(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            IsActive = customer.IsActive
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerCreateDto input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data"));

        var userId = Guid.Parse(User.FindFirst("sub")!.Value);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = _db.TenantId,
            Name = input.Name,
            Code = input.Code,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = userId,
            IsActive = true
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id, version = "1" },
            ApiResponse<CustomerDto>.Ok(new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Code = customer.Code,
                IsActive = customer.IsActive
            })
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CustomerUpdateDto input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data"));

        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer is null)
            return NotFound(ApiResponse<object>.Fail("Customer not found"));

        var userId = Guid.Parse(User.FindFirst("sub")!.Value);

        customer.Name = input.Name;
        customer.Code = input.Code;
        customer.IsActive = input.IsActive;
        customer.UpdatedAtUtc = DateTime.UtcNow;
        customer.UpdatedBy = userId;

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<CustomerDto>.Ok(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            IsActive = customer.IsActive
        }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer is null)
            return NotFound(ApiResponse<object>.Fail("Customer not found"));

        var userId = Guid.Parse(User.FindFirst("sub")!.Value);

        customer.IsActive = false;
        customer.UpdatedAtUtc = DateTime.UtcNow;
        customer.UpdatedBy = userId;

        await _db.SaveChangesAsync();

        return NoContent();
    }
}
