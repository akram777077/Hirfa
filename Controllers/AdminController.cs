using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;

namespace Hirfa.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly HirfaDbContext _context;
        public AdminController(HirfaDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult RegisterAdmin() => View("~/Views/Admin/Register.cshtml");

        [HttpPost]
        public IActionResult RegisterAdmin(AdminRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Register.cshtml", model);
            }

            if (_context.Comptes.Any(c => c.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View("~/Views/Admin/Register.cshtml", model);
            }

            var compte = new Hirfa.Web.Models.Compte
            {
                Email = model.Email,
                Motdepasse = model.Password
            };
            _context.Comptes.Add(compte);
            _context.SaveChanges();

            var admin = new Hirfa.Web.Models.Admin
            {
                Idcompte = compte.Idcompte
            };
            _context.Admins.Add(admin);
            _context.SaveChanges();

            TempData["SuccessToast"] = "Admin registered successfully.";
            return RedirectToAction("Login", "Account");
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
                .Select(d => new Hirfa.Web.ViewModels.PrestataireDemandeDetailViewModel
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
            return View("~/Views/Admin/Dashboard.cshtml", demandes ?? new List<Hirfa.Web.ViewModels.PrestataireDemandeDetailViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrestataireAccount(int id, string password)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null || demande.Etat != "valide")
                return NotFound();

            if (_context.Comptes.Any(c => c.Email == demande.Email))
            {
                TempData["ErrorToast"] = "An account with this email already exists.";
                return RedirectToAction("AdminDashboard");
            }

            // Create Compte
            var compte = new Hirfa.Web.Models.Compte
            {
                Email = demande.Email,
                Motdepasse = password
            };
            _context.Comptes.Add(compte);
           await _context.SaveChangesAsync();

            // Create Prestataire
            var prestataire = new Hirfa.Web.Models.Prestataire
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

            // Update demand status to 'final valide'
            demande.Etat = "final valide";
           await _context.SaveChangesAsync();

            // Use Uri.EscapeDataString instead of WebUtility.UrlEncode
            var subject = Uri.EscapeDataString("Account Created");
            var body = Uri.EscapeDataString($"Your account has been created. Your password is: {password ?? ""}");
            var mailtoUri = $"mailto:{demande.Email}?subject={subject}&body={body}";

            TempData["SuccessToast"] = "Account created successfully, and demand status updated to 'final valide'.";

            // Pass mailto URI to the view
            ViewData["MailtoUri"] = mailtoUri;
            return View("SendEmail");
        }

        [HttpPost]
        public IActionResult FinalRejectDemandePrestataire(int id)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null)
                return NotFound();

            demande.Etat = "final non valide";
            // Do not overwrite Reason, keep the existing one from service client
            //_context.SaveChanges();

            // Generate mailto URI for rejection with reason in the subject
            var subject = Uri.EscapeDataString($"Hirfa: Account Rejected");
            var body = Uri.EscapeDataString($"Dear {demande.Nom + " " + demande.Prenom}.\nYour account request has been rejected.\nReason: {demande.Reason ?? "Nothing"}.\nPlease contact support for more details.");
            var mailtoUri = $"mailto:{demande.Email}?subject={subject}&body={body}";

            TempData["SuccessToast"] = "Demande prestataire rejected and status set to final non valide. Email link generated.";

            // Pass mailto URI to the view
            ViewData["MailtoUri"] = mailtoUri;
            return View("SendEmail");
        }
    }
}
