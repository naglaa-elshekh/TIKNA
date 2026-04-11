public class RegisterDto
{
    // بيانات إجبارية للطرفين
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserType { get; set; } // "Student" أو "Company"
    public string Address { get; set; }

    public string? University { get; set; }     // للطالب
    public string? Faculty { get; set; }        // للطالب

    public string? CompanyDescription { get; set; } // للشركة

    public string? Phone { get; set; } // ضيفي ده عشان يظهر في Swagger
   
    public string? ServiceType { get; set; }
    public string? CompanyRegisterNumber { get; set; }
}