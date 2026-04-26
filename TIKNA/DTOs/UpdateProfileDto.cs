namespace TIKNA.DTOs
{
    // الكلاس اللي هيشيل البيانات اللي جاية من الفرونت
    public class UpdateProfileDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string University { get; set; }
        public string Faculty { get; set; }

    }
}