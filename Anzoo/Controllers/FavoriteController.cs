using Anzoo.Models;
using Anzoo.Service.Favorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Anzoo.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _service;

        public FavoriteController(IFavoriteService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int adId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _service.AddToFavoritesAsync(userId, adId);

            // Dacă e AJAX, răspunde simplu
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Ok(new { success = true });
            }

            // Dacă e form clasic, redirect
            return RedirectToAction("View", "Ad", new { id = adId });
        }


        [Authorize]
        public async Task<IActionResult> MyFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var favorites = await _service.GetUserFavoritesAsync(userId);

            return View(favorites);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromFavorites(int adId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _service.RemoveFromFavoritesAsync(userId, adId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Ok(new { success = true });
            }

            return RedirectToAction("MyFavorites");
        }

    }
}
