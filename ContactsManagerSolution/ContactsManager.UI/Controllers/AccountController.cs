using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            // Check for validation erros
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return View(registerDTO);
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                PersonName = registerDTO.PersonName
                
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
                //return RedirectToAction(nameof(PersonsController.Index), nameof(PersonsController));
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerDTO);
            }
        }
    }
}
