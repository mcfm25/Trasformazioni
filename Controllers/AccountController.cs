using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione dell'autenticazione (Login, Logout, Register)
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRepartoService _repartoService;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;

        public AccountController(
            IUserService userService,
            ILogger<AccountController> logger,
            IRepartoService repartoService,
            IEmailService emailService)
        {
            _userService = userService;
            _logger = logger;
            _repartoService = repartoService;
            _emailService = emailService;
        }

        #region Login

        /// <summary>
        /// GET: /Account/Login
        /// Mostra la pagina di login
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // Se l'utente è già autenticato, reindirizza alla home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: /Account/Login
        /// Elabora il login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _userService.LoginAsync(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Login effettuato con successo per: {Email}", model.Email);

                    // Reindirizza all'URL di ritorno o alla home
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Account bloccato per: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Account bloccato per troppi tentativi di accesso. Riprova più tardi.");
                    return View(model);
                }

                if (result.IsNotAllowed)
                {
                    _logger.LogWarning("Accesso non consentito per: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Accesso non consentito. L'account potrebbe essere disattivato.");
                    return View(model);
                }

                // Login fallito
                ModelState.AddModelError(string.Empty, "Email o password non validi.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il login per: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante il login. Riprova.");
                return View(model);
            }
        }

        #endregion

        #region Logout

        /// <summary>
        /// POST: /Account/Logout
        /// Effettua il logout
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _userService.LogoutAsync();
                _logger.LogInformation("Logout effettuato");
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il logout");
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Register

        /// <summary>
        /// GET: /Account/Register
        /// Mostra la pagina di registrazione
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            // Se l'utente è già autenticato, reindirizza alla home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel
            {
                RepartiSelectList = await _repartoService.GetSelectListAsync()
            };

            return View(model);
        }

        /// <summary>
        /// POST: /Account/Register
        /// Elabora la registrazione
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);
                return View(model);
            }

            try
            {
                // Verifica se l'email esiste già
                if (await _userService.EmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Questa email è già registrata.");
                    model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);
                    return View(model);
                }

                var result = await _userService.RegisterAsync(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Nuovo utente registrato: {Email}", model.Email);

                    TempData["SuccessMessage"] = "Registrazione completata con successo! Puoi ora effettuare il login.";
                    return RedirectToAction("Login", "Account");
                }

                // Aggiungi gli errori al ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione per: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante la registrazione. Riprova.");
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);
                return View(model);
            }
        }

        #endregion

        #region Access Denied

        /// <summary>
        /// GET: /Account/AccessDenied
        /// Mostra la pagina di accesso negato
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        //#region Forgot Password (Optional - da implementare)

        ///// <summary>
        ///// GET: /Account/ForgotPassword
        ///// Mostra la pagina per il recupero password
        ///// </summary>
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ForgotPassword()
        //{
        //    // TODO: Implementare logica recupero password
        //    return View();
        //}

        ///// <summary>
        ///// POST: /Account/ForgotPassword
        ///// Elabora la richiesta di recupero password
        ///// </summary>
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(string email)
        //{
        //    // TODO: Implementare logica recupero password
        //    // 1. Verifica se l'email esiste
        //    // 2. Genera token di reset
        //    // 3. Invia email con link di reset

        //    TempData["InfoMessage"] = "Se l'email esiste nel sistema, riceverai un'email con le istruzioni per il recupero password.";
        //    return RedirectToAction("Login");
        //}

        //#endregion

        #region Forgot Password

        /// <summary>
        /// GET: /Account/ForgotPassword
        /// Mostra la pagina per il recupero password
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        /// <summary>
        /// POST: /Account/ForgotPassword
        /// Elabora la richiesta di recupero password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Genera token (restituisce null se utente non esiste, è eliminato o disattivato)
                var token = await _userService.GeneratePasswordResetTokenAsync(model.Email);

                if (token != null)
                {
                    // Costruisci il link di reset
                    var resetLink = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { email = model.Email, token = token },
                        protocol: Request.Scheme);

                    // Invia email
                    var emailBody = GenerateResetPasswordEmailHtml(model.Email, resetLink!);
                    var (success, error) = await _emailService.SendEmailAsync(
                        model.Email,
                        "Recupero Password",
                        emailBody);

                    if (success)
                    {
                        _logger.LogInformation(
                            "Email di reset password inviata a: {Email}",
                            model.Email);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Errore invio email reset password a {Email}: {Error}",
                            model.Email, error);
                    }
                }
                else
                {
                    // Log per debug, ma non rivelare all'utente se l'email esiste
                    _logger.LogWarning(
                        "Richiesta reset password per email non valida/eliminata/disattivata: {Email}",
                        model.Email);
                }

                // Sempre redirect alla conferma (per sicurezza non rivelare se l'email esiste)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante la richiesta di reset password per: {Email}",
                    model.Email);

                // Anche in caso di errore, redirect alla conferma per sicurezza
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
        }

        /// <summary>
        /// GET: /Account/ForgotPasswordConfirmation
        /// Mostra la conferma dell'invio email
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Reset Password

        /// <summary>
        /// GET: /Account/ResetPassword
        /// Mostra la pagina per impostare la nuova password
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? email, string? token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Tentativo di accesso a ResetPassword senza email o token");
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        /// <summary>
        /// POST: /Account/ResetPassword
        /// Elabora il reset della password
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _userService.ResetPasswordAsync(
                    model.Email,
                    model.Token,
                    model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "Password resettata con successo per: {Email}",
                        model.Email);

                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                // Aggiungi errori al ModelState
                foreach (var error in result.Errors)
                {
                    // Traduci alcuni errori comuni
                    var errorMessage = error.Code switch
                    {
                        "InvalidToken" => "Il link di reset è scaduto o non valido. Richiedi un nuovo link.",
                        "UserDisabled" => "L'account è disattivato. Contatta l'amministratore.",
                        "InvalidEmail" => "Email non valida.",
                        _ => error.Description
                    };

                    ModelState.AddModelError(string.Empty, errorMessage);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il reset password per: {Email}",
                    model.Email);

                ModelState.AddModelError(string.Empty,
                    "Si è verificato un errore. Riprova o richiedi un nuovo link.");

                return View(model);
            }
        }

        /// <summary>
        /// GET: /Account/ResetPasswordConfirmation
        /// Mostra la conferma del reset password
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Genera l'HTML per l'email di reset password
        /// </summary>
        private string GenerateResetPasswordEmailHtml(string email, string resetLink)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""it"">
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Recupero Password</title>
                </head>
                <body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f6f9; line-height: 1.6;"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f4f6f9; padding: 20px;"">
                        <tr>
                            <td align=""center"">
                                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                                    <!-- Header -->
                                    <tr>
                                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px 20px; text-align: center;"">
                                            <h1 style=""color: #ffffff; margin: 0; font-size: 24px; font-weight: 600;"">
                                                🔐 Recupero Password
                                            </h1>
                                        </td>
                                    </tr>
                    
                                    <!-- Content -->
                                    <tr>
                                        <td style=""padding: 30px 20px; color: #333333;"">
                                            <p style=""font-size: 16px; margin-bottom: 20px;"">
                                                Ciao,
                                            </p>
                                            <p style=""font-size: 16px; margin-bottom: 20px;"">
                                                Abbiamo ricevuto una richiesta di reset password per l'account associato a <strong>{email}</strong>.
                                            </p>
                                            <p style=""font-size: 16px; margin-bottom: 25px;"">
                                                Clicca sul pulsante qui sotto per impostare una nuova password:
                                            </p>
                            
                                            <!-- Button -->
                                            <div style=""text-align: center; margin: 30px 0;"">
                                                <a href=""{resetLink}"" 
                                                   style=""display: inline-block; padding: 14px 35px; background-color: #667eea; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: 600; font-size: 16px;"">
                                                    Reimposta Password
                                                </a>
                                            </div>
                            
                                            <!-- Warning Box -->
                                            <div style=""background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 25px 0; border-radius: 4px;"">
                                                <p style=""margin: 0; font-size: 14px; color: #856404;"">
                                                    <strong>⚠️ Attenzione:</strong> Questo link scadrà tra 24 ore. 
                                                    Se non hai richiesto il reset della password, puoi ignorare questa email.
                                                </p>
                                            </div>
                            
                                            <p style=""font-size: 14px; color: #6c757d; margin-top: 25px;"">
                                                Se il pulsante non funziona, copia e incolla questo link nel browser:
                                            </p>
                                            <p style=""font-size: 12px; color: #6c757d; word-break: break-all; background-color: #f8f9fa; padding: 10px; border-radius: 4px;"">
                                                {resetLink}
                                            </p>
                                        </td>
                                    </tr>
                    
                                    <!-- Footer -->
                                    <tr>
                                        <td style=""background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #6c757d; border-top: 1px solid #e9ecef;"">
                                            <p style=""margin: 0;""><strong>Sistema Gestione</strong></p>
                                            <p style=""margin: 5px 0 0 0;"">Questa è una email automatica. Si prega di non rispondere.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
        }

        #endregion
    }
}