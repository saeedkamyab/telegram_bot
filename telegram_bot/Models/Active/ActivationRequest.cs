using System.ComponentModel.DataAnnotations;

namespace telegram_bot.Models.Active
{
    public class ActivationRequest
    {
        [Required]
        public string MobileNumber { get; set; }

        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string AppVersion { get; set; }
        public string ActivationCode { get; set; }
    }
}
