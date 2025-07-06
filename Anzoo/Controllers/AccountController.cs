using Anzoo.Migrations;
using Anzoo.Models;
using Anzoo.Service.SendGrid;
using Anzoo.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anzoo.Controllers
{
    [Authorize] // tot controllerul protejat
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ISendGridService _mailService;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ISendGridService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mailService = mailService;
        }

        /* ------------------- LOGIN ------------------- */
        [HttpGet, AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _signInManager.PasswordSignInAsync(
                vm.Email, vm.Password, vm.RememberMe, false);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(vm);
        }

        /* ------------------- REGISTER ------------------- */
        [HttpGet, AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new User
            {
                FullName = vm.Name,
                Email = vm.Email,
                UserName = vm.Email,
                Location = "N/A"
            };

            var res = await _userManager.CreateAsync(user, vm.Password);

            if (res.Succeeded) return RedirectToAction(nameof(Login));

            foreach (var e in res.Errors)
                ModelState.AddModelError(string.Empty, e.Description);

            return View(vm);
        }


        /* ------------- FORGOT-PASSWORD ----------------- */
        [HttpGet, AllowAnonymous]
        public IActionResult VerifyEmail() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                var tok = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action(nameof(ChangePassword), "Account",
                                        new { token = tok, email = user.Email }, Request.Scheme);

                await _mailService.SendPasswordResetEmailAsync(user.Email, link);
            }
            // mesaj generic, fără leak de existență cont
            return View("VerifyEmailConfirmation");
        }

        /* --------------- RESET-PASSWORD --------------- */
        [HttpGet, AllowAnonymous]
        public IActionResult ChangePassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(VerifyEmail));

            return View(new ChangePasswordViewModel { Email = email, Token = token });
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found.");
                return View(vm);
            }

            var res = await _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword);
            if (res.Succeeded) return RedirectToAction(nameof(Login));

            foreach (var e in res.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View(vm);
        }

        /* ------------------- PROFILE ------------------ */
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Location = user.Location
            };

            ViewBag.Breadcrumbs = new List<(string Text, string Url)>
            {
                ("Acasă", Url.Action("Index", "Home")),
                ("Profilul meu", null)
            };


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.GetUserAsync(User);
            user.FullName = vm.FullName;
            user.PhoneNumber = vm.PhoneNumber;
            user.Location = vm.Location;

            var res = await _userManager.UpdateAsync(user);
            ViewBag.Message = res.Succeeded ? "Modificările au fost salvate." : null;

            if (!res.Succeeded)
                foreach (var e in res.Errors) ModelState.AddModelError(string.Empty, e.Description);

            return View(vm);
        }

        /* ------------------- LOGOUT ------------------- */
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



    }
}
