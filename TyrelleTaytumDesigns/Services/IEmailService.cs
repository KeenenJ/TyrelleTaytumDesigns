using TyrelleTaytumDesigns.Models;

namespace TyrelleTaytumDesigns.Services
{
    public interface IEmailService
    {
        Task SendContactEmailAsync(ContactFormModel model);
        Task SendCustomOrderEmailAsync(CustomOrderModel model);
    }
}
