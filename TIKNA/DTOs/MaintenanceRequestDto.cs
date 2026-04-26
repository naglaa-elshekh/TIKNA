namespace TIKNA.DTOs
{
    public class MaintenanceRequestDto
    {
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string DeviceAge { get; set; }
        public string ProblemType { get; set; }
        public string Description { get; set; }
        public string ServiceType { get; set; }
        public DateTime PreferredDate { get; set; }
        public string PreferredTimeSlot { get; set; }
    }
}