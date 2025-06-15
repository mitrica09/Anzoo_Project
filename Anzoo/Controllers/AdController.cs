using Anzoo.Service.Ad;
using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anzoo.Controllers
{
    [Authorize]                               // - doar utilizatori logaţi
    public class AdController : Controller
    {
        private readonly IAdService _service;

        public AdController(IAdService service)
        {
            _service = service;
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateAdViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);

            var ok = await _service.Create(form);
            if (ok)
            {
                TempData["Msg"] = "Anunţul a fost publicat ✔";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Eroare la salvarea anunţului");
            return View(form);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AllAds()
        {
            var ads = await _service.GetAllAds();
            return View(ads);
        }

        [HttpGet("/Ad/View/{id}")]
        public async Task<IActionResult> View(int id)
        {
            var ad = await _service.GetAdById(id);
            if (ad == null)
                return NotFound();

            return View("AdDetail", ad);
        }


    }
}
