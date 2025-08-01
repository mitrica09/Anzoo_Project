﻿using Anzoo.Service.Ad;
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

            ViewBag.Breadcrumbs = new List<(string Text, string Url)>
            {
                ("Acasă", Url.Action("Index", "Home")),
                ("Toate anunțurile", Url.Action("AllAds", "Ad")),
                ("Creează anunț", null)
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = (await _service.GetCategoriesForDropdownMenu()).ToList();
                return View(model);
            }

            var adId = await _service.Create(model);
            if (adId == null)
            {
                ModelState.AddModelError("", "A apărut o eroare la salvarea anunțului.");
                model.Categories = (await _service.GetCategoriesForDropdownMenu()).ToList();
                return View(model);
            }

            TempData["Msg"] = "Anunțul a fost publicat!";
            return RedirectToAction("PromoteAd", new { id = adId });
        }


        [AllowAnonymous]
        [HttpGet("/Ad/View/{id}")]
        public async Task<IActionResult> View(int id)
        {
            var ad = await _service.GetAdById(id);
            if (ad == null)
                return NotFound();

            ViewBag.Breadcrumbs = new List<(string Text, string Url)>
            {
                ("Acasă", Url.Action("Index", "Home")),
                ("Toate anunțurile", Url.Action("AllAds", "Ad")),
                ("Detaliile anunțului", null)
            };

            return View("AdDetail", ad);
        }

        [HttpGet]
        public async Task<IActionResult> MyAds()
        {
            var ads = await _service.GetMyAds();

            ViewBag.Breadcrumbs = new List<(string Text, string Url)>
            {
                ("Acasă", Url.Action("Index", "Home")),
                ("Toate anunțurile", Url.Action("AllAds", "Ad")),
                ("Anunțurile mele", null)
            };

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
            ViewBag.Breadcrumbs = new List<(string Text, string Url)>
            {
                ("Acasă", Url.Action("Index", "Home")),
                ("Toate anunțurile", Url.Action("AllAds", "Ad")),
                ("Anunțurile mele", Url.Action("MyAds", "Ad")),
                ("Editează anunț", null)
            };
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

            ViewBag.Breadcrumbs = new List<(string Text, string Url)>
            {
                ("Acasă", Url.Action("Index", "Home")),
                ("Toate anunțurile", null)
            };

            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> PromoteAd(int id)
        {
            var ad = await _service.GetAdById(id);

            if (ad == null || ad.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View("PromoteAd", ad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteConfirmed(int id, int days)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ✅ Verificăm dacă anunțul aparține userului
            var ad = await _service.GetAdById(id);
            if (ad == null || ad.UserId != userId)
            {
                return Unauthorized(); // sau NotFound() dacă vrei să nu dai niciun indiciu
            }

            var success = await _service.PromoteAd(id, userId, days);
            if (!success)
            {
                TempData["Error"] = $"Nu ai suficiente puncte pentru {days} {(days == 1 ? "zi" : "zile")} de promovare.";
                return RedirectToAction("PromoteAd", new { id });
            }

            TempData["Msg"] = $"Anunțul a fost promovat pentru {days} {(days == 1 ? "zi" : "zile")}!";
            return RedirectToAction("View", new { id });
        }


    }
}
