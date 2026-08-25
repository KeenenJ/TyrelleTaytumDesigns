using System.ComponentModel.DataAnnotations;

namespace TyrelleTaytumDesigns.Models
{
    public class ContactFormModel
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(100)]
        public string ReasonForContact { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string PreferredContactMethod { get; set; } = "Email";

        [Required, StringLength(3000)]
        public string Message { get; set; } = string.Empty;
    }
}
