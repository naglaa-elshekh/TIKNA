namespace TIKNA.DTOs
{
    // الكلاس اللي هيشيل البيانات اللي جاية من الفرونت
    public class UpdateProfileDto
    {
        public string CurrentPassword { get; set; } // لازم عشان التأكد
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? Faculty { get; set; }
    }

}