using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_DBFirst.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            if (User.Identity is { IsAuthenticated: true })
                return RedirectToAction("Index", "Products");

            ViewBag.PageTitle = "Create an Account";
            return View();
        }

        [HttpPost]
        [Route("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            ViewBag.PageTitle = "Create an Account";

            if (!ModelState.IsValid)
                return View(registerDto);

            var user = new ApplicationUser
            {
                FullName = registerDto.CustomerName,
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            try
            {
                // UserManager hashes the password internally - no PasswordHasher needed anymore.
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    _logger.LogWarning("Registration failed. Email={Email}, Errors={Errors}", registerDto.Email, string.Join("; ", result.Errors.Select(e => e.Description)));

                    return View(registerDto);
                }

                // Ensure the "Customer" role exists, then assign it to every new user by default.
                if (!await _roleManager.RoleExistsAsync("Customer"))
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));

                await _userManager.AddToRoleAsync(user, "Customer");

                _logger.LogInformation("New user registered. Email={Email}", registerDto.Email);

                TempData["SuccessMessage"] = "Account created successfully. Please log in. ✅";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration. Email={Email}", registerDto.Email);
                ModelState.AddModelError(string.Empty, "Something went wrong while creating your account❌");
                return View(registerDto);
            }
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity is { IsAuthenticated: true })
                return RedirectToAction("Index", "Products");

            ViewBag.PageTitle = "Log In";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDto, string? returnUrl = null)
        {
            ViewBag.PageTitle = "Log In";
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(loginDto);

            try
            {
                // SignInManager handles password verification AND cookie sign-in in one call.
                var result = await _signInManager.PasswordSignInAsync(
                    loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed login attempt. Email={Email}", loginDto.Email);
                    ViewBag.ErrorMessage = "Invalid email or password ❌";
                    return View(loginDto);
                }

                _logger.LogInformation("User logged in. Email={Email}", loginDto.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login. Email={Email}", loginDto.Email);
                ViewBag.ErrorMessage = "Something went wrong while logging in❌";
                return View(loginDto);
            }
        }

        [HttpPost]
        [Route("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var email = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out. Email={Email}", email);
            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        [Route("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            ViewBag.PageTitle = "My Account";

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}