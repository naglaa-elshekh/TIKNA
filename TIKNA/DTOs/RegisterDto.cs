using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    public string Name { get; set; }

    [Required(ErrorMessage = "الإيميل مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة الإيميل غير صحيحة")]
    public string Email { get; set; }

    [Required(ErrorMessage = "كلمة السر مطلوبة")]
    public string Password { get; set; }

    [Required(ErrorMessage = "تأكيد كلمة السر مطلوب")]
    [Compare("Password", ErrorMessage = "كلمة السر وتأكيدها مش متطابقين")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "نوع الحساب مطلوب")]
    public string UserType { get; set; }

    // اجعلي العناوين اختيارية بوضع علامة استفهام أو قيمة افتراضية
    public string? Address { get; set; } = "Not Specified";

    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? CompanyDescription { get; set; }
    public string? Phone { get; set; }
    public string? ServiceType { get; set; }
    public string? CompanyRegisterNumber { get; set; }
}