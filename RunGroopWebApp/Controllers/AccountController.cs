using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;


namespace RunGroopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }
        // prva verzija implementacije
        //public async task<iactionresult> login(loginviewmodel loginviewmodel)
        //{
        //    if (!modelstate.isvalid) return view(loginviewmodel);

        //    var user = await _usermanager.findbyemailasync(loginviewmodel.emailaddress);

        //    if (user != null)
        //    {
        //        //user is found, check password
        //        var passwordcheck = await _usermanager.checkpasswordasync(user, loginviewmodel.password);
        //        if (passwordcheck)
        //        {
        //            //password correct, sign in
        //            var result = await _signinmanager.passwordsigninasync(user, loginviewmodel.password, false, false);
        //            if (result.succeeded)
        //            {
        //                return redirecttoaction("index", "race");
        //            }
        //        }
        //        //password is incorrect
        //        tempdata["error"] = "wrong credentials. please try again";
        //        return view(loginviewmodel);
        //    }
        //    //user not found
        //    tempdata["error"] = "wrong credentials. please try again";
        //    return view(loginviewmodel);
        //}
        [HttpPost]
        // druga verzija unapredjena
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
            {
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginViewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Race");
            }

            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginViewModel);
        }
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            
            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return RedirectToAction("Index", "Race");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }
    }
}
