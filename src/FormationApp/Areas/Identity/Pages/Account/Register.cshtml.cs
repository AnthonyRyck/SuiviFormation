using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AccessData;
using AccessData.Models;
using FormationApp.Codes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.Logging;

namespace FormationApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
		public SqlContext SqlService { get; set; }

		private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            SqlContext mySql)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            SqlService = mySql;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Un minimum de 6 caractères, et maximum 100.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le Mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne sont pas les mêmes.")]
            public string ConfirmPassword { get; set; }


            [Required]
            [StringLength(32)]
            [Display(Name = "Nom")]
            public string Nom { get; set; }

            [Required]
            [StringLength(32)]
            [Display(Name = "Prénom")]
            public string Prenom { get; set; }

            [Required]
            [StringLength(32)]
            [Display(Name = "Login de connexion")]
            public string Login { get; set; }

            [Required]
            [StringLength(32)]
            [Display(Name = "Service")]
            public string Service { get; set; }

            [Required]
            [Display(Name = "Est externe MinDef ?")]
            public bool IsExterneMindef { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Login, Email = Input.Email };

                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur crée un nouveau compt avec mot de passe.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirmez votre Email",
                        $"Confirmez votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.");


                    // Ajout dans la table Personnelle
                    await AddToPersonnel(user.Id, Input);


                    // S'il faut une confirmation d'email pour activer le compte.
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                //IdentityResult resultRole = await _userManager.AddToRoleAsync(user, Role.Agent.ToString());

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private async Task AddToPersonnel(string IdUser, InputModel inputModel)
		{
            // Création du nouveau personnel
            Personnel personnel = new Personnel()
            {
                IdPersonnel = IdUser,
                IsActif = true,
                IsExterne = inputModel.IsExterneMindef,
                Nom = inputModel.Nom,
                Prenom = inputModel.Prenom,
                Login = inputModel.Login,
                Service = inputModel.Service
            };

            await SqlService.AddPersonnel(personnel);
		}


    }
}
