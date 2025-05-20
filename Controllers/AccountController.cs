using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Models;
using Hirfa.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Hirfa.Web.ViewModels;

namespace Hirfa.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HirfaDbContext _context;
        public AccountController(HirfaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var compte = _context.Comptes.FirstOrDefault(c => c.Email == email && c.Motdepasse == password);
            if (compte == null)
            {
                TempData["ErrorToast"] = "Invalid email or password.";
                return View("~/Views/Account/Login.cshtml");
            }
            // Prestataire login restriction
            if (_context.Prestataires.Any(p => p.Idcompte == compte.Idcompte))
            {
                var prestataire = _context.Prestataires.FirstOrDefault(p => p.Idcompte == compte.Idcompte);
                if (prestataire != null && !prestataire.Estdisponible)
                {
                    TempData["ErrorToast"] = "Your account is not yet approved or is inactive.";
                    return View("~/Views/Account/Login.cshtml");
                }
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "prestataire");
                return RedirectToAction("PrestataireDashboard");
            }
            if (_context.Admins.Any(a => a.Idcompte == compte.Idcompte))
            {
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "admin");
                return RedirectToAction("AdminDashboard");
            }
            if (_context.Clients.Any(c => c.Idcompte == compte.Idcompte))
            {
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "client");
                return RedirectToAction("ClientDashboard");
            }
            if (_context.Serviceclients.Any(s => s.Idcompte == compte.Idcompte))
            {
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "serviceclient");
                return RedirectToAction("ServiceClientDashboard");
            }
            if (_context.Demandeprestataires.Any(d => d.Email == compte.Email && d.Etat == "pending"))
            {
                TempData["ErrorToast"] = "Your account is pending approval. You will be able to log in once approved.";
                return View("~/Views/Account/Login.cshtml");
            }
            TempData["ErrorToast"] = "No role assigned to this account.";
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Redirect to the new prestataire demand registration view
            return RedirectToAction("RegisterPrestataire");
        }

        [HttpGet]
        public IActionResult RegisterAdmin() => View("RegisterAdmin");
        [HttpGet]
        public IActionResult RegisterClient() => View("RegisterClient");
        [HttpGet]
        public IActionResult RegisterServiceClient() => View("RegisterServiceClient");
        [HttpGet]
        public IActionResult RegisterPrestataire() => View("RegisterPrestataireDemande"); // Prestataire demand registration
        [HttpGet]
        public IActionResult RegisterPrestataireDemande() => View("RegisterPrestataireDemande");

        [HttpPost]
        public async Task<IActionResult> Register(string role, AdminRegisterViewModel adminModel, ClientRegisterViewModel clientModel, ServiceClientRegisterViewModel serviceClientModel, PrestataireDemandeViewModel prestataireModel)
        {
            if (role == "admin")
            {
                // Remove validation errors for fields not in AdminRegisterViewModel
                var allowedKeys = new HashSet<string> {
                    nameof(adminModel.Email),
                    nameof(adminModel.Password),
                    "role"
                };
                var keysToRemove = ModelState.Keys.Where(k => !allowedKeys.Contains(k) && !k.StartsWith("adminModel.")).ToList();
                foreach (var key in keysToRemove)
                    ModelState.Remove(key);
                if (!ModelState.IsValid)
                    return View("RegisterAdmin", adminModel);
                if (_context.Comptes.Any(c => c.Email == adminModel.Email))
                {
                    TempData["ErrorToast"] = "Email is already in use.";
                    return View("RegisterAdmin", adminModel);
                }
                var compte = new Compte
                {
                    Email = adminModel.Email,
                    Motdepasse = adminModel.Password
                };
                _context.Comptes.Add(compte);
                await _context.SaveChangesAsync();
                var admin = new Admin { Idcompte = compte.Idcompte };
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
                TempData["SuccessToast"] = "Admin account created successfully. Please log in.";
                return View("RegisterAdmin");
            }
            if (role == "client")
            {
                // Remove validation errors for fields not in ClientRegisterViewModel
                var allowedKeys = new HashSet<string> {
                    nameof(clientModel.Email),
                    nameof(clientModel.Prenom),
                    nameof(clientModel.Nom),
                    nameof(clientModel.Numerotelephone),
                    nameof(clientModel.Datenaissance),
                    nameof(clientModel.Adresse),
                    nameof(clientModel.Sexe),
                    nameof(clientModel.Password),
                    "role"
                };
                var keysToRemove = ModelState.Keys.Where(k => !allowedKeys.Contains(k) && !k.StartsWith("clientModel.")).ToList();
                foreach (var key in keysToRemove)
                    ModelState.Remove(key);
                if (!ModelState.IsValid)
                    return View("RegisterClient", clientModel);
                if (_context.Comptes.Any(c => c.Email == clientModel.Email))
                {
                    TempData["ErrorToast"] = "Email is already in use.";
                    return View("RegisterClient", clientModel);
                }
                var compte = new Compte
                {
                    Email = clientModel.Email,
                    Motdepasse = clientModel.Password
                };
                _context.Comptes.Add(compte);
                await _context.SaveChangesAsync();
                var client = new Client
                {
                    Nom = clientModel.Nom,
                    Prenom = clientModel.Prenom,
                    Numerotelephone = clientModel.Numerotelephone,
                    Datenaissance = DateOnly.FromDateTime(clientModel.Datenaissance),
                    Adresse = clientModel.Adresse,
                    Sexe = clientModel.Sexe,
                    Idcompte = compte.Idcompte
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                TempData["SuccessToast"] = "Account created successfully. Please log in.";
                return View("RegisterClient");
            }
            if (role == "serviceclient")
            {
                // Remove validation errors for fields not in ServiceClientRegisterViewModel
                var allowedKeys = new HashSet<string> {
                    nameof(serviceClientModel.Email),
                    nameof(serviceClientModel.Prenom),
                    nameof(serviceClientModel.Nom),
                    nameof(serviceClientModel.Numerotelephone),
                    nameof(serviceClientModel.Datenaissance),
                    nameof(serviceClientModel.Adresse),
                    nameof(serviceClientModel.Sexe),
                    nameof(serviceClientModel.Password),
                    "role"
                };
                var keysToRemove = ModelState.Keys.Where(k => !allowedKeys.Contains(k) && !k.StartsWith("serviceClientModel.")).ToList();
                foreach (var key in keysToRemove)
                    ModelState.Remove(key);
                if (!ModelState.IsValid)
                    return View("RegisterServiceClient", serviceClientModel);
                if (_context.Comptes.Any(c => c.Email == serviceClientModel.Email))
                {
                    TempData["ErrorToast"] = "Email is already in use.";
                    return View("RegisterServiceClient", serviceClientModel);
                }
                var compte = new Compte
                {
                    Email = serviceClientModel.Email,
                    Motdepasse = serviceClientModel.Password
                };
                _context.Comptes.Add(compte);
                await _context.SaveChangesAsync();
                var serviceclient = new Serviceclient
                {
                    Nom = serviceClientModel.Nom,
                    Prenom = serviceClientModel.Prenom,
                    Numerotelephone = serviceClientModel.Numerotelephone,
                    Datenaissance = DateOnly.FromDateTime(serviceClientModel.Datenaissance),
                    Adresse = serviceClientModel.Adresse,
                    Sexe = serviceClientModel.Sexe,
                    Idcompte = compte.Idcompte
                };
                _context.Serviceclients.Add(serviceclient);
                await _context.SaveChangesAsync();
                TempData["SuccessToast"] = "Account created successfully. Please log in.";
                return View("RegisterServiceClient");
            }
            if (role == "prestataire")
            {
                // Remove validation errors for fields not in PrestataireDemandeViewModel
                var allowedKeys = new HashSet<string> {
                    nameof(prestataireModel.Email),
                    nameof(prestataireModel.Prenom),
                    nameof(prestataireModel.Nom),
                    nameof(prestataireModel.Numerotelephone),
                    nameof(prestataireModel.Datenaissance),
                    nameof(prestataireModel.Adresse),
                    nameof(prestataireModel.Nin),
                    nameof(prestataireModel.Typeservice),
                    nameof(prestataireModel.CvFile),
                    nameof(prestataireModel.CasierjudiciaireFile),
                    nameof(prestataireModel.Diplomes),
                    "role"
                };
                var keysToRemove = ModelState.Keys.Where(k => !allowedKeys.Contains(k) && !k.StartsWith("Diplomes") && !k.StartsWith("prestataireModel.")).ToList();
                foreach (var key in keysToRemove)
                    ModelState.Remove(key);
                try
                {
                    if (!ModelState.IsValid)
                    {
                        // Ensure the diplomas section is initialized for the view if validation fails
                        if (prestataireModel.Diplomes == null || prestataireModel.Diplomes.Count == 0)
                            prestataireModel.Diplomes = new List<DiplomeDemandeInputModel> { new DiplomeDemandeInputModel() };
                        return View("RegisterPrestataireDemande", prestataireModel);
                    }
                    if (_context.Demandeprestataires.Any(d => d.Email == prestataireModel.Email && d.Etat == "pending") || _context.Comptes.Any(c => c.Email == prestataireModel.Email))
                    {
                        TempData["ErrorToast"] = "A demand or account with this email already exists.";
                        if (prestataireModel.Diplomes == null || prestataireModel.Diplomes.Count == 0)
                            prestataireModel.Diplomes = new List<DiplomeDemandeInputModel> { new DiplomeDemandeInputModel() };
                        return View("RegisterPrestataireDemande", prestataireModel);
                    }
                    var demandeprestataire = new Demandeprestataire
                    {
                        Nom = prestataireModel.Nom,
                        Prenom = prestataireModel.Prenom,
                        Numtel = prestataireModel.Numerotelephone,
                        Datenaissance = DateOnly.FromDateTime(prestataireModel.Datenaissance),
                        Adresse = prestataireModel.Adresse,
                        Nin = prestataireModel.Nin,
                        Typeservice = prestataireModel.Typeservice,
                        Email = prestataireModel.Email,
                        Etat = "pending"
                    };
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    if (prestataireModel.CvFile != null && prestataireModel.CvFile.Length > 0)
                    {
                        var cvGuid = Guid.NewGuid().ToString() + Path.GetExtension(prestataireModel.CvFile.FileName);
                        var cvPath = Path.Combine(uploadsFolder, cvGuid);
                        using (var stream = new FileStream(cvPath, FileMode.Create))
                        {
                            await prestataireModel.CvFile.CopyToAsync(stream);
                        }
                        demandeprestataire.Cv = cvGuid;
                    }
                    if (prestataireModel.CasierjudiciaireFile != null && prestataireModel.CasierjudiciaireFile.Length > 0)
                    {
                        var casierGuid = Guid.NewGuid().ToString() + Path.GetExtension(prestataireModel.CasierjudiciaireFile.FileName);
                        var casierPath = Path.Combine(uploadsFolder, casierGuid);
                        using (var stream = new FileStream(casierPath, FileMode.Create))
                        {
                            await prestataireModel.CasierjudiciaireFile.CopyToAsync(stream);
                        }
                        demandeprestataire.Casierjudiciaire = casierGuid;
                    }
                    _context.Demandeprestataires.Add(demandeprestataire);
                    await _context.SaveChangesAsync();
                    if (prestataireModel.Diplomes != null && prestataireModel.Diplomes.Count > 0)
                    {
                        foreach (var diplomaInput in prestataireModel.Diplomes)
                        {
                            if (diplomaInput?.File != null && diplomaInput.File.Length > 0)
                            {
                                var diplomaGuid = Guid.NewGuid().ToString() + Path.GetExtension(diplomaInput.File.FileName);
                                var diplomaPath = Path.Combine(uploadsFolder, diplomaGuid);
                                using (var stream = new FileStream(diplomaPath, FileMode.Create))
                                {
                                    await diplomaInput.File.CopyToAsync(stream);
                                }
                                var diplome = new Diplomedemande
                                {
                                    Institution = diplomaInput.Institution,
                                    Type = diplomaInput.Type,
                                    Anneeobtention = diplomaInput.AnneeObtention,
                                    Fiche = diplomaGuid,
                                    Iddemandeprestataire = demandeprestataire.Iddemandeprestataire
                                };
                                _context.Diplomedemandes.Add(diplome);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                    TempData["SuccessToast"] = "Demande sent successfully. You will be able to log in once approved.";
                    return View("RegisterPrestataireDemande");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    return View("RegisterPrestataireDemande", prestataireModel);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult AdminDashboard(string? status = null)
        {
            var query = _context.Demandeprestataires.AsQueryable();
            query = query.Where(d => d.Etat == "valide" || d.Etat == "non valide");
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.Etat == status);
            }
            var demandes = query
                .Select(d => new ViewModels.PrestataireDemandeDetailViewModel
                {
                    Iddemandeprestataire = d.Iddemandeprestataire,
                    Nom = d.Nom,
                    Prenom = d.Prenom,
                    Typeservice = d.Typeservice,
                    Email = d.Email,
                    Etat = d.Etat,
                    Reason = d.Reason ?? string.Empty // Prevent null
                })
                .ToList();
            return View("~/Views/Admin/AdminDashboard.cshtml", demandes ?? new List<ViewModels.PrestataireDemandeDetailViewModel>());
        }

        [HttpGet]
        public IActionResult ClientDashboard()
        {
            return View("~/Views/Client/ClientDashboard.cshtml");
        }

        [HttpGet]
        public IActionResult ServiceClientDashboard()
        {
            // Add Prestataire Demands to the navbar by using the shared layout or partials in the view
            ViewBag.Demandeclients = _context.Demandeclients.ToList();
            return View("~/Views/ServiceClient/ServiceClientDashboard.cshtml");
        }

        [HttpGet]
        public IActionResult PrestataireDashboard()
        {
            return View("Prestataire/PrestataireDashboard");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult PrestataireDemands()
        {
            var demandes = _context.Demandeprestataires
                .Select(d => new ViewModels.PrestataireDemandeListItemViewModel
                {
                    Iddemandeprestataire = d.Iddemandeprestataire,
                    Nom = d.Nom,
                    Prenom = d.Prenom,
                    Email = d.Email,
                    Typeservice = d.Typeservice,
                    Etat = d.Etat
                })
                .ToList();
            return View("~/Views/Prestataire/PrestataireDemands.cshtml", demandes);
        }

        [HttpGet]
        public IActionResult PrestataireDemandDetails(int id)
        {
            var demande = _context.Demandeprestataires
                .Where(d => d.Iddemandeprestataire == id)
                .Select(d => new ViewModels.PrestataireDemandeDetailViewModel
                {
                    Iddemandeprestataire = d.Iddemandeprestataire,
                    Nom = d.Nom,
                    Prenom = d.Prenom,
                    Datenaissance = d.Datenaissance,
                    Numtel = d.Numtel,
                    Nin = d.Nin,
                    Typeservice = d.Typeservice,
                    Adresse = d.Adresse,
                    Email = d.Email,
                    Cv = d.Cv,
                    Etat = d.Etat,
                    Casierjudiciaire = d.Casierjudiciaire,
                    Reason = d.Reason, // Map Reason
                    Sexe = d.Sexe, // Map Sexe
                    Diplomes = d.Diplomedemandes.Select(di => new ViewModels.DiplomeDemandeDetailViewModel
                    {
                        Institution = di.Institution,
                        Type = di.Type,
                        Anneeobtention = di.Anneeobtention,
                        Fiche = di.Fiche
                    }).ToList()
                })
                .FirstOrDefault();
            if (demande == null)
                return NotFound();
            return View("~/Views/Prestataire/PrestataireDemandDetails.cshtml", demande);
        }

        [HttpPost]
        public IActionResult AcceptDemandeClient(int id)
        {
            var demande = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demande == null)
                return NotFound();
            demande.Etat = Models.DemandeclientStatus.Valide;
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande accepted and status set to Valide.";
            return RedirectToAction("ServiceClientDashboard");
        }

        [HttpPost]
        public IActionResult RejectDemandeClient(int id)
        {
            var demande = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demande == null)
                return NotFound();
            demande.Etat = Models.DemandeclientStatus.NonValide;
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande rejected and status set to NonValide.";
            return RedirectToAction("ServiceClientDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrestataireAccount(int id, string password)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null || demande.Etat != Models.DemandeclientStatus.Valide.ToString())
                return NotFound();
            if (_context.Comptes.Any(c => c.Email == demande.Email))
            {
                TempData["ErrorToast"] = "An account with this email already exists.";
                return RedirectToAction("AdminDashboard");
            }
            // Create Compte
            var compte = new Models.Compte
            {
                Email = demande.Email,
                Motdepasse = password
            };
            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();
            // Create Prestataire
            var prestataire = new Models.Prestataire
            {
                Nom = demande.Nom,
                Prenom = demande.Prenom,
                Numerotelephone = demande.Numtel,
                Datenaissance = demande.Datenaissance,
                Adresse = demande.Adresse,
                Sexe = demande.Sexe,
                Estdisponible = true,
                Nin = demande.Nin,
                Typeservice = demande.Typeservice,
                Cv = demande.Cv,
                Casierjudiciaire = demande.Casierjudiciaire,
                Idcompte = compte.Idcompte,
                Iddemandeprestataire = demande.Iddemandeprestataire
            };
            _context.Prestataires.Add(prestataire);
            // Set demand status to 'final valide' (strong typing)
            demande.Etat = "final valide";
            await _context.SaveChangesAsync();
            // Redirect to Gmail with password in body
            var gmailUrl = $"https://mail.google.com/mail/?view=cm&fs=1&to={demande.Email}&su=Your account has been created&body=Your account has been created and your password is: {System.Net.WebUtility.UrlEncode(password)}";
            return Redirect(gmailUrl);
        }

        [HttpPost]
        public IActionResult FinalRejectDemandePrestataire(int id)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null)
                return NotFound();
            demande.Etat = "final non valide";
            // Do not overwrite Reason, keep the existing one from service client
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande prestataire rejected and status set to final non valide.";
            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public IActionResult AcceptDemandePrestataire(int id, string returnUrl)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null)
                return NotFound();
            demande.Etat = "valide";
            demande.Reason = null;
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande accepted and status set to Valide.";
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("PrestataireDemands");
        }

        [HttpPost]
        public IActionResult RejectDemandePrestataire(int id, string reason, string returnUrl)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null)
                return NotFound();
            demande.Etat = "non valide";
            demande.Reason = reason;
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande rejected and status set to Non Valide.";
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("PrestataireDemands");
        }
    }
}
