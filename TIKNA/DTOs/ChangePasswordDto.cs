namespace TIKNA.DTOs
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } // الباسوورد القديم
        public string NewPassword { get; set; }     // الباسوورد الجديد
        public string ConfirmPassword { get; set; } // تأكيد الباسوورد الجديد
    }
}