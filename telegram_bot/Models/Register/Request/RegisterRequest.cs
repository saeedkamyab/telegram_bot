using System.ComponentModel.DataAnnotations;

namespace telegram_bot.Models.Register.Request
{
    public class RegisterRequest
    {
        [Required]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "Invalid mobile number format")]
        [Key]
        public string MobileNumber { get; set; }

        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string AppVersion { get; set; }
    }
}
