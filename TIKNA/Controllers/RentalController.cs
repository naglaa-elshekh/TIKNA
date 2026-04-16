using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIKNA.Models;
using System.Security.Claims;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RentalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("RequestRent")]
        public async Task<IActionResult> RequestRent([FromForm] RentalDto dto)
        {
            int duration = (dto.EndDate - dto.StartDate).Days;

            if (duration <= 0)
                return BadRequest("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return BadRequest("هذا الجهاز غير موجود");

            var isBooked = await _context.RentalRequests.AnyAsync(r =>
                r.ProductId == dto.ProductId &&
                r.Status == "Accepted" &&
                ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
                 (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate)));

            if (isBooked)
                return BadRequest("الجهاز محجوز بالفعل في هذه الفترة");

            decimal finalPrice = duration * product.Price;

            var rental = new RentalRequest
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ProductId = dto.ProductId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = finalPrice,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.RentalRequests.Add(rental);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Success", totalPrice = finalPrice });
        }
    }

    public class RentalDto
    {
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}