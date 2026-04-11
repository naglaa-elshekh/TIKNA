using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CartController(ApplicationDbContext context) => _context = context;

    // 1. عرض السلة (يجيب البيانات كاملة بالاسم والصورة للسعر)
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _context.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) return Ok(new List<CartItemReadDTO>());

        var result = cart.CartItems.Select(ci => new CartItemReadDTO
        {
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            Price = ci.Product.Price,
            ImageUrl = ci.Product.ImageUrl,
            Quantity = ci.Quantity
        }).ToList();

        return Ok(result);
    }

    // 2. مزامنة السلة (كلام المعيد: يحفظ كل اللي الطالب عمله في الصفحة مرة واحدة)
    [HttpPost("Sync")]
    public async Task<IActionResult> SyncCart([FromBody] List<CartItemUpdateDTO> items)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. التأكد من وجود سلة لليوزر
        var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        // 2. التحقق من أن كل المنتجات مضافة فعلاً في الداتابيز قبل الحفظ
        foreach (var item in items)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == item.ProductId);
            if (!productExists)
            {
                // لو منتج واحد بس مش موجود، بنوقف العملية ونرجع رسالة خطأ
                return BadRequest($"عفواً، المنتج رقم ({item.ProductId}) غير موجود في المتجر حالياً.");
            }
        }

        // 3. لو كله تمام، نمسح القديم ونضيف الجديد
        _context.CartItems.RemoveRange(cart.CartItems);

        foreach (var item in items)
        {
            _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "تم تحديث السلة بنجاح." });
    }
}