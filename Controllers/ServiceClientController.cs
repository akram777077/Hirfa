using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hirfa.Web.Controllers
{
    public class ServiceClientController : Controller
    {
        private readonly HirfaDbContext _context;
        public ServiceClientController(HirfaDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult RegisterServiceClient() => View("~/Views/ServiceClient/Register.cshtml");

        [HttpPost]
        public async Task<IActionResult> RegisterServiceClient(ServiceClientRegisterViewModel serviceClientModel)
        {
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
                return View("~/Views/ServiceClient/Register.cshtml", serviceClientModel);
            if (_context.Comptes.Any(c => c.Email == serviceClientModel.Email))
            {
                TempData["ErrorToast"] = "Email is already in use.";
                return View("~/Views/ServiceClient/Register.cshtml", serviceClientModel);
            }
            var compte = new Hirfa.Web.Models.Compte
            {
                Email = serviceClientModel.Email,
                Motdepasse = serviceClientModel.Password
            };
            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();
            var serviceclient = new Hirfa.Web.Models.Serviceclient
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
            return View("~/Views/Account/Login.cshtml", serviceClientModel);
        }

        [HttpGet]
        public IActionResult ServiceClientDashboard()
        {
            ViewBag.Demandeclients = _context.Demandeclients.ToList();
            return View("~/Views/ServiceClient/Dashboard.cshtml");
        }

        [HttpPost]
        public IActionResult AcceptDemandeClient(int id)
        {
            var demande = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demande == null)
                return NotFound();
            demande.Etat = Hirfa.Web.Models.DemandeclientStatus.Matching;
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande accepted and status set to Matching.";
            return RedirectToAction("ServiceClientDashboard");
        }

        [HttpPost]
        public IActionResult RejectDemandeClient(int id)
        {
            var demande = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demande == null)
                return NotFound();
            demande.Etat = Hirfa.Web.Models.DemandeclientStatus.NotFound;
            _context.SaveChanges();
            TempData["SuccessToast"] = "Demande rejected and status set to NotFound.";
            return RedirectToAction("ClientDemands");
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

        [HttpGet]
        public IActionResult ClientDemands()
        {
            var demands = _context.Demandeclients
                .Include(d => d.IdclientNavigation) // Eagerly load the client navigation property
                .Select(d => new DemandeClientViewModel
                {
                    Id = d.Iddemandeclient,
                    Description = d.Description,
                    Categorie = d.Categorie,
                    DateDemande = d.Datedemande,
                    DateDebut = d.Datedebut,
                    DateFin = d.Datefin,
                    Etat = d.Etat,
                    ClientAddress = d.IdclientNavigation.Adresse,
                    MatchingPrestatairesCount = _context.Prestataires.Count(p => (p.Typeservice == d.Categorie || p.Typeservice.Contains(d.Categorie)) && p.Estdisponible) // Ensure only available Prestataires are counted
                })
                .ToList();

            return View(demands);
        }

        [HttpGet]
        public IActionResult ViewPrestataires(int id)
        {
            var demand = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demand == null)
            {
                return NotFound();
            }

            var relatedPrestataires = _context.Prestataires
                .Where(p => (p.Typeservice == demand.Categorie || p.Typeservice.Contains(demand.Categorie)) && p.Estdisponible)
                .Select(p => new PrestataireViewModel
                {
                    Idprestataire = p.Idprestataire,
                    Nom = p.Nom,
                    Prenom = p.Prenom,
                    Numerotelephone = p.Numerotelephone,
                    Adresse = p.Adresse,
                    Typeservice = p.Typeservice,
                    Estdisponible = p.Estdisponible,
                    Sexe = p.Sexe, // Map gender
                    Stars = 0, // Placeholder for stars as Evaluation.Stars is not defined
                    TotalDemands = p.Demandeclients.Count,
                    HasRejectedDevis = _context.Devis.Any(de => de.Idprestataire == p.Idprestataire && de.Iddemandeclient == demand.Iddemandeclient && de.Etat == "Rejected") // Check for rejected devis
                })
                .ToList();

            ViewBag.DemandId = id;
            return View("PrestataireList", relatedPrestataires);
        }

        [HttpPost]
        public IActionResult MatchPrestataire(int demandId, int prestataireId)
        {
            var demand = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == demandId);
            if (demand == null)
            {
                return NotFound();
            }

            demand.Idprestataire = prestataireId;
            demand.Etat = Hirfa.Web.Models.DemandeclientStatus.Matching; // Update status to Matching
            _context.SaveChanges();

            TempData["SuccessToast"] = "Prestataire matched successfully, and status updated to Matching.";
            return RedirectToAction("ClientDemands");
        }

        private IEnumerable<DemandeClientViewModel> GetUpdatedDemands()
        {
            return _context.Demandeclients
                .Include(d => d.IdclientNavigation)
                .Select(d => new DemandeClientViewModel
                {
                    Id = d.Iddemandeclient,
                    Description = d.Description,
                    Categorie = d.Categorie,
                    DateDemande = d.Datedemande,
                    DateDebut = d.Datedebut,
                    DateFin = d.Datefin,
                    ClientAddress = d.IdclientNavigation.Adresse,
                    Etat = d.Etat,
                    MatchingPrestatairesCount = _context.Prestataires.Count(p => p.Typeservice == d.Categorie)
                })
                .ToList();
        }
    }
}
