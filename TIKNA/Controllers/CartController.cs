using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CartController(ApplicationDbContext context) => _context = context;

    // 1. إضافة منتج للسلة (أو زيادة الكمية لو موجود)
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartDTO dto)
    {
        var cart = await _context.Carts.Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

        if (cart == null)
        {
            cart = new Cart { CustomerId = dto.CustomerId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = dto.ProductId, Quantity = dto.Quantity });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "تمت الإضافة" });
    }

    // 2. عرض محتويات السلة لعميل معين
    [HttpGet("{customerId}")]
    public async Task<ActionResult<IEnumerable<CartItemReadDTO>>> GetCart(int customerId)
    {
        var items = await _context.CartItems
            .Where(ci => ci.Cart.CustomerId == customerId)
            .Select(ci => new CartItemReadDTO
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Price = ci.Product.Price,
                ImageUrl = ci.Product.ImageUrl,
                Quantity = ci.Quantity
            }).ToListAsync();

        return Ok(items);
    }

    // 3. تعديل الكمية (مثلاً من علامة + و - في الفرونت إند)
    [HttpPut("update-quantity")]
    public async Task<IActionResult> UpdateQuantity(UpdateQuantityDTO dto)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Cart.CustomerId == dto.CustomerId && ci.ProductId == dto.ProductId);

        if (item == null) return NotFound("المنتج غير موجود في السلة");

        if (dto.NewQuantity <= 0)
        {
            _context.CartItems.Remove(item); // لو نزل للصفر نمسحه خالص
        }
        else
        {
            item.Quantity = dto.NewQuantity;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "تم تحديث الكمية" });
    }

    // 4. حذف منتج واحد من السلة
    [HttpDelete("remove/{customerId}/{productId}")]
    public async Task<IActionResult> RemoveItem(int customerId, int productId)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Cart.CustomerId == customerId && ci.ProductId == productId);

        if (item == null) return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok(new { message = "تم الحذف" });
    }

    // 5. مسح السلة بالكامل (Clear Cart)
    [HttpDelete("clear/{customerId}")]
    public async Task<IActionResult> ClearCart(int customerId)
    {
        var items = await _context.CartItems.Where(ci => ci.Cart.CustomerId == customerId).ToListAsync();
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
        return Ok(new { message = "تم تفريغ السلة" });
    }
}