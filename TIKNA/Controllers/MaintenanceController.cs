using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIKNA.Models;
using System.Security.Claims;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
       public MaintenanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateRequest([FromForm] MaintenanceRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderNumber = "TIKNA-M-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            var request = new MaintenanceRequest
            {
                UserId = userId,
                DeviceType = dto.DeviceType,
                ModelName = dto.ModelName,
                ProblemDescription = dto.ProblemDescription,
                OrderNumber = orderNumber,
                Status = MaintenanceStatus.Pending,
                RequestDate = DateTime.Now
            };

            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Success", orderNumber = orderNumber });
        }

        [HttpGet("MyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = await _context.MaintenanceRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return Ok(requests);
        }
    }

    public class MaintenanceRequestDto
    {
        public string DeviceType { get; set; }
        public string ModelName { get; set; }
        public string ProblemDescription { get; set; }
    }
}
