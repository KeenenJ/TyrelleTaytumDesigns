using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TyrelleTaytumDesigns.Models
{
    public class CustomOrderModel
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string PreferredContact { get; set; } = "Email";

        [Required, StringLength(80)]
        public string GarmentType { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Vision { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Occasion { get; set; } = string.Empty;

        [StringLength(150)]
        public string? PreferredColours { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EventDate { get; set; }

        [Required, StringLength(50)]
        public string Budget { get; set; } = string.Empty;

        public IFormFile? InspirationImages { get; set; }

        [Required, StringLength(10)]
        public string HasMeasurements { get; set; } = "No";

        [StringLength(20)]
        public string MeasurementUnit { get; set; } = "cm";

        [StringLength(30)]
        public string? Height { get; set; }

        [StringLength(30)]
        public string? BustChest { get; set; }

        [StringLength(30)]
        public string? Waist { get; set; }

        [StringLength(30)]
        public string? Hips { get; set; }

        [StringLength(30)]
        public string? ShoulderToShoulder { get; set; }

        [StringLength(30)]
        public string? ShoulderToWaist { get; set; }

        [StringLength(30)]
        public string? Neck { get; set; }

        [StringLength(30)]
        public string? ArmLength { get; set; }

        [StringLength(30)]
        public string? UpperArm { get; set; }

        [StringLength(30)]
        public string? Wrist { get; set; }

        [StringLength(30)]
        public string? WaistToFloor { get; set; }

        [StringLength(30)]
        public string? FullGarmentLength { get; set; }

        [StringLength(30)]
        public string? Inseam { get; set; }
    }
}
