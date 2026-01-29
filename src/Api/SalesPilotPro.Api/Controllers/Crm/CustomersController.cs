using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Api.Controllers.Base;
using SalesPilotPro.Api.Controllers.Crm.Models;
using SalesPilotPro.Api.Security;
using SalesPilotPro.Infrastructure.Entities.Crm;
using SalesPilotPro.Infrastructure.Persistence;

namespace SalesPilotPro.Api.Controllers.Crm;

[AuthorizeModule("crm")]
[Route("api/crm/customers")]
public sealed class CustomersController : ApiControllerBase
{
    private readonly SalesPilotProDbContext _db;

    public CustomersController(SalesPilotProDbContext db)
    {
        _db = db;
    }

    // GET: api/crm/customers?search=&skip=0&take=20
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var query = _db.Customers
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Code.Contains(search));
        }

        var customers = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return Ok(customers.Select(x => new CustomerDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            IsActive = x.IsActive
        }));
    }

    // GET: api/crm/customers/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        if (customer is null)
            return NotFound();

        return Ok(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            IsActive = customer.IsActive
        });
    }

    // POST: api/crm/customers
    [HttpPost]
    public async Task<IActionResult> Create(CustomerCreateDto input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = _db.TenantId,
            Name = input.Name,
            Code = input.Code
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return Ok(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            IsActive = customer.IsActive
        });
    }

    // PUT: api/crm/customers/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CustomerUpdateDto input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer is null)
            return NotFound();

        customer.Name = input.Name;
        customer.Code = input.Code;
        customer.IsActive = input.IsActive;

        await _db.SaveChangesAsync();

        return Ok(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            IsActive = customer.IsActive
        });
    }

    // DELETE: api/crm/customers/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer is null)
            return NotFound();

        customer.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
