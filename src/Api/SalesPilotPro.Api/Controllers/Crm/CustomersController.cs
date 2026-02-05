using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Infrastructure.Persistence;

namespace SalesPilotPro.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/crm/customers")]
[ApiVersion("1.0")]
[Authorize(Policy = "MODULE_CRM")]
public sealed class CustomersController : ControllerBase
{
    private readonly SalesPilotProDbContext _db;

    public CustomersController(SalesPilotProDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var customers = await _db.Customers
            .AsNoTracking()
            .ToListAsync(ct);

        return Ok(customers);
    }
}
