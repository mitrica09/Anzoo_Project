using Anzoo.Service.Ad;
using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Anzoo.Controllers
{
    [Authorize]
    public class AdController : Controller
    {
        private readonly IAdService _service;

        public AdController(IAdService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateAdViewModel
            {
                Categories = (await _service.GetCategoriesForDropdownMenu()).ToList()
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reîncarcă dropdown-ul pentru validare
                model.Categories = (await _service.GetCategoriesForDropdownMenu()).ToList();
                return View(model);
            }

            var success = await _service.Create(model);
            if (!success)
            {
                ModelState.AddModelError("", "A apărut o eroare la salvarea anunțului.");
                model.Categories = (await _service.GetCategoriesForDropdownMenu()).ToList();
                return View(model);
            }

            TempData["Msg"] = "Anunțul a fost publicat!";
            return RedirectToAction("MyAds");
        }

        [HttpGet("/Ad/View/{id}")]
        public async Task<IActionResult> View(int id)
        {
            var ad = await _service.GetAdById(id);
            if (ad == null)
                return NotFound();

            return View("AdDetail", ad);
        }

        [HttpGet]
        public async Task<IActionResult> MyAds()
        {
            var ads = await _service.GetMyAds();
            return View("MyAds", ads);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var form = await _service.GetAdForEditAsync(id, userId);
            if (form == null)
                return NotFound();

            ViewBag.Categories = await _service.GetCategoriesForDropdownMenu(); // ✅ Așteaptă corect

            return View(form);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAdViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = (await _service.GetCategoriesForDropdownMenu()).ToList();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.UpdateAdAsync(model, userId);

            if (!success)
            {
                ModelState.AddModelError("", "Eroare la actualizare.");
                model.Categories = (await _service.GetCategoriesForDropdownMenu()).ToList();
                return View(model);
            }

            TempData["Msg"] = "Modificările au fost salvate!";
            return RedirectToAction("MyAds");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.DeleteAdAsync(id, userId);

            if (!success)
            {
                TempData["Error"] = "Eroare la ștergerea anunțului.";
                return RedirectToAction("MyAds");
            }

            TempData["Msg"] = "Anunțul a fost șters.";
            return RedirectToAction("MyAds");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AllAds([FromQuery] AdFilterViewModel filter)
        {
            var result = await _service.GetAllAdsFilteredAsync(filter);
            ViewBag.Categories = await _service.GetCategoriesForDropdownMenu();
            ViewBag.Filter = filter;
            return View(result);
        }

    }
}
