using System.ComponentModel.DataAnnotations;

namespace TIKNA.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "الإيميل مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة الإيميل غير صحيحة")]
        public string Email { get; set; }
    }
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } // التوكن المشفر اللي جاي من اللينك

        [Required]
        [EmailAddress]
        public string Email { get; set; } // الإيميل اللي جاي من اللينك

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "كلمة المرور غير متطابقة")]
        public string ConfirmPassword { get; set; }
    }

}
