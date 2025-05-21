using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace Hirfa.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly HirfaDbContext _context;
        public AdminController(HirfaDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult RegisterAdmin() => View("RegisterAdmin");

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
            return View("~/Views/Admin/AdminDashboard.cshtml", demandes ?? new List<Hirfa.Web.ViewModels.PrestataireDemandeDetailViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrestataireAccount(int id, string password)
        {
            var demande = _context.Demandeprestataires.FirstOrDefault(d => d.Iddemandeprestataire == id);
            if (demande == null || demande.Etat != Hirfa.Web.Models.DemandeclientStatus.Valide.ToString())
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
            return RedirectToAction("PrestataireDemands", "Prestataire");
        }
    }
}
