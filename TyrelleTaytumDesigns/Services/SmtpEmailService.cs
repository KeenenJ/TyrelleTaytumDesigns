using System.Net;
using System.Net.Mail;
using System.Text;
using TyrelleTaytumDesigns.Models;

namespace TyrelleTaytumDesigns.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IConfiguration configuration)
        {
            _settings = configuration.GetSection("Email").Get<EmailSettings>() ?? new EmailSettings();
        }

        public async Task SendContactEmailAsync(ContactFormModel model)
        {
            var body = $"""
                <h2>New Contact Enquiry</h2>
                <p><strong>Name:</strong> {Encode(model.FullName)}</p>
                <p><strong>Email:</strong> {Encode(model.EmailAddress)}</p>
                <p><strong>Phone:</strong> {Encode(model.PhoneNumber ?? "Not provided")}</p>
                <p><strong>Reason:</strong> {Encode(model.ReasonForContact)}</p>
                <p><strong>Preferred contact:</strong> {Encode(model.PreferredContactMethod)}</p>
                <hr />
                <p><strong>Message:</strong></p>
                <p>{Encode(model.Message).Replace("\n", "<br />")}</p>
                """;

            await SendAsync($"Website enquiry — {model.ReasonForContact}", body, model.EmailAddress);
        }

        public async Task SendCustomOrderEmailAsync(CustomOrderModel model)
        {
            var measurementUnit = string.IsNullOrWhiteSpace(model.MeasurementUnit) ? "cm" : model.MeasurementUnit;
            var measurements = $"""
                <h3>5. Measurements</h3>
                <p><strong>Measurements available:</strong> {Encode(model.HasMeasurements)}</p>
                """;

            if (model.HasMeasurements.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                measurements += $"""
                    <p><strong>Unit:</strong> {Encode(measurementUnit)}</p>
                    <p><strong>Height:</strong> {Value(model.Height, measurementUnit)}</p>
                    <p><strong>Bust / Chest:</strong> {Value(model.BustChest, measurementUnit)}</p>
                    <p><strong>Waist:</strong> {Value(model.Waist, measurementUnit)}</p>
                    <p><strong>Hips:</strong> {Value(model.Hips, measurementUnit)}</p>
                    <p><strong>Shoulder to Shoulder:</strong> {Value(model.ShoulderToShoulder, measurementUnit)}</p>
                    <p><strong>Shoulder to Waist:</strong> {Value(model.ShoulderToWaist, measurementUnit)}</p>
                    <p><strong>Neck:</strong> {Value(model.Neck, measurementUnit)}</p>
                    <p><strong>Arm Length:</strong> {Value(model.ArmLength, measurementUnit)}</p>
                    <p><strong>Upper Arm:</strong> {Value(model.UpperArm, measurementUnit)}</p>
                    <p><strong>Wrist:</strong> {Value(model.Wrist, measurementUnit)}</p>
                    <p><strong>Waist to Floor:</strong> {Value(model.WaistToFloor, measurementUnit)}</p>
                    <p><strong>Full Garment Length:</strong> {Value(model.FullGarmentLength, measurementUnit)}</p>
                    <p><strong>Inseam:</strong> {Value(model.Inseam, measurementUnit)}</p>
                    """;
            }
            else
            {
                measurements += "<p><em>Client does not currently have measurements.</em></p>";
            }

            var body = $"""
                <h2>New Custom Design Enquiry</h2>

                <h3>1. About the Client</h3>
                <p><strong>Name:</strong> {Encode(model.FullName)}</p>
                <p><strong>Email:</strong> {Encode(model.EmailAddress)}</p>
                <p><strong>Phone:</strong> {Encode(model.PhoneNumber)}</p>
                <p><strong>Preferred contact:</strong> {Encode(model.PreferredContact)}</p>

                <h3>2. Design Details</h3>
                <p><strong>Garment:</strong> {Encode(model.GarmentType)}</p>
                <p><strong>Occasion:</strong> {Encode(model.Occasion)}</p>
                <p><strong>Event date:</strong> {(model.EventDate.HasValue ? model.EventDate.Value.ToString("dd MMMM yyyy") : "Not provided")}</p>
                <p><strong>Preferred colours:</strong> {Encode(model.PreferredColours ?? "Not provided")}</p>
                <p><strong>Vision:</strong></p>
                <p>{Encode(model.Vision).Replace("\n", "<br />")}</p>

                <h3>3. Budget</h3>
                <p><strong>Estimated budget:</strong> {Encode(model.Budget)}</p>

                <h3>4. Inspiration</h3>
                <p><strong>Inspiration file:</strong> {(model.InspirationImages?.FileName is null ? "Not provided" : Encode(model.InspirationImages.FileName))}</p>

                {measurements}
                """;

            await SendAsync($"Custom design enquiry — {model.FullName}", body, model.EmailAddress, model.InspirationImages);
        }

        private async Task SendAsync(string subject, string htmlBody, string replyTo, Microsoft.AspNetCore.Http.IFormFile? attachment = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.Username) || string.IsNullOrWhiteSpace(_settings.Password) || string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                throw new InvalidOperationException("Email settings are not configured. Add the Email settings using User Secrets or environment variables before testing form submissions.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, "TyrelleTaytum Designs Website"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            message.To.Add(_settings.ToEmail);
            message.ReplyToList.Add(new MailAddress(replyTo));

            if (attachment is not null && attachment.Length > 0)
            {
                if (attachment.Length > 10 * 1024 * 1024)
                    throw new InvalidOperationException("The inspiration file is too large. Please upload a file smaller than 10 MB.");

                await using var stream = attachment.OpenReadStream();
                var memory = new MemoryStream();
                await stream.CopyToAsync(memory);
                memory.Position = 0;
                message.Attachments.Add(new Attachment(memory, attachment.FileName, attachment.ContentType));
            }

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            await client.SendMailAsync(message);
        }

        private static string Value(string? value, string unit)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "Not provided"
                : $"{Encode(value)} {Encode(unit)}";
        }

        private static string Encode(string value) => WebUtility.HtmlEncode(value);
    }
}
