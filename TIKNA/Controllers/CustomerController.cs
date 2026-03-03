using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.DTOs;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context) => _context = context;

    // --- قسم الـ Admin ---

    [HttpGet("Admin/All")]
    [Authorize(Roles = "Admin")] // مسموح للأدمن فقط
    public async Task<IActionResult> GetAll(string? search)
    {
        var query = _context.Customers.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Name.Contains(search) || c.Phone.Contains(search));

        return Ok(await query.ToListAsync());
    }

    // --- قسم الـ User (Dashboard) ---

    [HttpGet("MyProfile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        return customer == null ? NotFound() : Ok(customer);
    }

    [HttpPut("UpdateMyProfile")]
    public async Task<IActionResult> Update(CustomerDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (customer == null) return NotFound();

        customer.Name = dto.Name;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;

        await _context.SaveChangesAsync();
        return Ok(customer);
    }
}