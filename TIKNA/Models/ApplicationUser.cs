using Microsoft.AspNetCore.Identity;
using TIKNA.Models;

public class ApplicationUser : IdentityUser
{
    // الاسم الكامل متاح للأدمن والطالب
    public string? FullName { get; set; }

    public string UserType { get; set; } = "Individual";

    // حالة القبول: الشركات بتبدأ بـ false والأدمن يخليها true
    public string? ApprovalStatus { get; set; } = "Pending";


    // الربط مع جدول العميل (للطالب فقط)
    public Customer? Customer { get; set; }
}