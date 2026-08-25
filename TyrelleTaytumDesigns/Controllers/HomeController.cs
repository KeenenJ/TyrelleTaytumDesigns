using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TyrelleTaytumDesigns.Models;
using TyrelleTaytumDesigns.Services;

namespace TyrelleTaytumDesigns.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult About() => View();
        public IActionResult FAQ() => View();
        public IActionResult Gallery() => View();

        [HttpGet]
        public IActionResult Contact() => View(new ContactFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _emailService.SendContactEmailAsync(model);
                TempData["SuccessMessage"] = "Thank you. Your message has been sent successfully, and we'll be in touch soon.";
                return RedirectToAction(nameof(Contact));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact enquiry for {Email}", model.EmailAddress);
                ModelState.AddModelError(string.Empty, "We couldn't send your message right now. Please try again or contact us directly by email.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult CustomOrder() => View(new CustomOrderModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(12 * 1024 * 1024)]
        public async Task<IActionResult> CustomOrder(CustomOrderModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _emailService.SendCustomOrderEmailAsync(model);
                TempData["SuccessMessage"] = "Your custom design enquiry has been sent. Tyrelle will review your vision and get back to you soon.";
                return RedirectToAction(nameof(CustomOrder));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send custom order enquiry for {Email}", model.EmailAddress);
                ModelState.AddModelError(string.Empty, "We couldn't send your enquiry right now. Please try again or contact us directly by email.");
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
