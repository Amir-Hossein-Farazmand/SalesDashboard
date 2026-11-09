// Validate login form fields and carry the optional local return URL.
using System.ComponentModel.DataAnnotations;
namespace SalesDashboard.Web.ViewModels;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "نام کاربری را وارد کنید.")]
    [StringLength(50, ErrorMessage = "نام کاربری حداکثر ۵۰ نویسه است.")]
    [Display(Name = "نام کاربری")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
    [StringLength(50, ErrorMessage = "رمز عبور حداکثر ۵۰ نویسه است.")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;
    public string? ReturnURL { get; set; }
}