namespace HoopLog.Controllers
{
    using Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        } 

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var user = new User
            {
                UserName = input.Email,
                Email = input.Email,
                DisplayName = input.Name
            };

            var result = await _userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(input);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel input, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(input);

            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user is null)
            {
                _logger.LogWarning("Login failed for an unknown account.");
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(input);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, input.Password, input.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl!);
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login locked out for user {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Account temporarily locked, try again later.");
                return View(input);
            }

            _logger.LogWarning("Login failed for user {UserId}.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()         
        {
            return View();
        } 
    }
}
