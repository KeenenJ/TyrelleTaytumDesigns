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
            var body = $"""
                <h2>New Custom Design Enquiry</h2>
                <p><strong>Name:</strong> {Encode(model.FullName)}</p>
                <p><strong>Email:</strong> {Encode(model.EmailAddress)}</p>
                <p><strong>Phone:</strong> {Encode(model.PhoneNumber)}</p>
                <p><strong>Preferred contact:</strong> {Encode(model.PreferredContact)}</p>
                <hr />
                <p><strong>Garment:</strong> {Encode(model.GarmentType)}</p>
                <p><strong>Occasion:</strong> {Encode(model.Occasion)}</p>
                <p><strong>Event date:</strong> {(model.EventDate.HasValue ? model.EventDate.Value.ToString("dd MMMM yyyy") : "Not provided")}</p>
                <p><strong>Preferred colours:</strong> {Encode(model.PreferredColours ?? "Not provided")}</p>
                <p><strong>Estimated budget:</strong> {Encode(model.Budget)}</p>
                <p><strong>Measurements available:</strong> {Encode(model.HasMeasurements)}</p>
                <hr />
                <p><strong>Vision:</strong></p>
                <p>{Encode(model.Vision).Replace("\n", "<br />")}</p>
                <p><strong>Inspiration file:</strong> {(model.InspirationImages?.FileName is null ? "Not provided" : Encode(model.InspirationImages.FileName))}</p>
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

        private static string Encode(string value) => System.Net.WebUtility.HtmlEncode(value);
    }
}
