using System.ComponentModel.DataAnnotations;

public class CustomerDto
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    public string Name { get; set; }

    [Required(ErrorMessage = "رقم التليفون مطلوب")]
    public string Phone { get; set; }

    // العنوان اختياري عشان الأدمن ملوش عنوان عميل
    public string? Address { get; set; }
}