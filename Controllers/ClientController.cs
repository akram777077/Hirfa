using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Hirfa.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hirfa.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly HirfaDbContext _context;
        public ClientController(HirfaDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult RegisterClient() => View("RegisterClient");

        [HttpPost]
        public async Task<IActionResult> RegisterClient(ClientRegisterViewModel clientModel)
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
            var compte = new Hirfa.Web.Models.Compte
            {
                Email = clientModel.Email,
                Motdepasse = clientModel.Password
            };
            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();
            var client = new Hirfa.Web.Models.Client
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

        [HttpGet]
        public IActionResult ClientDashboard()
        {
            return View("~/Views/Client/Dashboard.cshtml");
        }

        [HttpGet]
        public IActionResult CreateDemand()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDemand(CreateDemandeClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (clientId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var demande = new Demandeclient
            {
                Categorie = model.Categorie,
                Description = model.Description,
                Datedebut = DateTime.SpecifyKind(model.Datedebut, DateTimeKind.Utc),
                Datefin = model.Datefin.HasValue ? DateTime.SpecifyKind(model.Datefin.Value, DateTimeKind.Utc) : (DateTime?)null,
                Idclient = clientId.Value,
                Datedemande = DateTime.UtcNow, // Save the full date and time
                Etat = DemandeclientStatus.Pending
            };

            _context.Demandeclients.Add(demande);
            await _context.SaveChangesAsync();

            TempData["SuccessToast"] = $"Demand created successfully at {demande.Datedemande:yyyy-MM-dd HH:mm:ss}.";
            return RedirectToAction("ClientDashboard");
        }

        [HttpGet]
        public IActionResult AllDemands()
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (clientId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var demands = _context.Demandeclients
                .Include(d => d.Devis) // Ensure Devis collection is eagerly loaded
                .Where(d => d.Idclient == clientId.Value)
                .AsEnumerable() // Convert to in-memory collection to handle null propagation
                .Select(d => new DemandeClientViewModel
                {
                    Id = d.Iddemandeclient,
                    Etat = d.Etat,
                    Description = d.Description,
                    Datedebut = d.Datedebut,
                    Datefin = d.Datefin,
                    Datedemande = d.Datedemande,
                    Categorie = d.Categorie,
                    DevisId = d.Devis.FirstOrDefault()?.Iddevis,
                    Devis = d.Devis.FirstOrDefault() != null ? new DevisViewModel
                    {
                        Iddevis = d.Devis.First().Iddevis,
                        Etat = d.Devis.First().Etat,
                        Montantglobal = d.Devis.First().Montantglobal,
                        Datelimite = d.Devis.First().Datelimite?.ToDateTime(TimeOnly.MinValue),
                        Description = d.Devis.First().Description
                    } : null
                })
                .ToList();

            var debugDemand = demands.FirstOrDefault(d => d.Id == 8);
            if (debugDemand != null)
            {
                Console.WriteLine($"Debug: Demand ID: {debugDemand.Id}, Devis ID: {debugDemand.DevisId}, Devis Description: {debugDemand.Devis?.Description}");
            }

            return View(demands);
        }

        [HttpGet]
        public IActionResult CancelDemand(int id)
        {
            var demand = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demand == null || demand.Etat == DemandeclientStatus.Canceled || demand.Etat == DemandeclientStatus.Complete)
            {
                TempData["ErrorToast"] = "Cannot cancel this demand.";
                return RedirectToAction("AllDemands");
            }

            demand.Etat = DemandeclientStatus.Canceled;
            _context.SaveChanges();

            TempData["SuccessToast"] = "Demand canceled successfully.";
            return RedirectToAction("AllDemands");
        }

        [HttpGet]
        public IActionResult SeeDevis(int id)
        {
            var devis = _context.Devis
                .Include(d => d.Quantitematieredevis) // Include materials
                .Include(d => d.IdprestataireNavigation) // Include prestataire information
                .Where(d => d.Iddevis == id)
                .Select(d => new DevisViewModel
                {
                    Iddevis = d.Iddevis,
                    Etat = d.Etat,
                    Montantglobal = d.Montantglobal,
                    Datelimite = d.Datelimite.HasValue ? d.Datelimite.Value.ToDateTime(TimeOnly.MinValue) : null,
                    Description = d.Description,
                    QuantiteMatieres = d.Quantitematieredevis.Select(q => new QuantiteMatiereDevisViewModel
                    {
                        Idmatierepremiere = q.Idmatierepremiere,
                        MatierePremiereName = q.IdmatierepremiereNavigation.Nommat,
                        PrixUnitaire = q.IdmatierepremiereNavigation.Prixmat,
                        Quantite = q.Quantite
                    }).ToList(),
                    Prestataire = d.IdprestataireNavigation != null ? new PrestataireViewModel
                    {
                        Idprestataire = d.IdprestataireNavigation.Idprestataire,
                        Nom = d.IdprestataireNavigation.Nom,
                        Prenom = d.IdprestataireNavigation.Prenom,
                        Adresse = d.IdprestataireNavigation.Adresse,
                        Numerotelephone = d.IdprestataireNavigation.Numerotelephone,
                        HasRejectedDevis = _context.Devis.Any(de => de.Idprestataire == d.IdprestataireNavigation.Idprestataire && de.Iddemandeclient == d.Iddemandeclient && de.Etat == "Rejected")
                    } : null
                })
                .FirstOrDefault();

            if (devis == null)
            {
                return NotFound();
            }

            return View("SeeDevis", devis);
        }

        [HttpPost]
        public IActionResult AcceptDevis(int id)
        {
            var devis = _context.Devis.Include(d => d.IddemandeclientNavigation).FirstOrDefault(d => d.Iddevis == id);
            if (devis == null || devis.IddemandeclientNavigation == null)
            {
                return NotFound();
            }

            devis.IddemandeclientNavigation.Etat = DemandeclientStatus.WorkOn;
            devis.Etat = "Accepted";
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Devis accepted successfully.";
            return RedirectToAction("SeeDevis", new { id });
        }

        [HttpPost]
        public IActionResult RejectDevis(int id)
        {
            var devis = _context.Devis.Include(d => d.IddemandeclientNavigation).FirstOrDefault(d => d.Iddevis == id);
            if (devis == null || devis.IddemandeclientNavigation == null)
            {
                return NotFound();
            }

            devis.IddemandeclientNavigation.Etat = DemandeclientStatus.Pending;
            devis.IddemandeclientNavigation.Idprestataire = null; // Set Idprestataire to null
            devis.Etat = "Rejected";
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Devis rejected successfully.";
            return RedirectToAction("SeeDevis", new { id });
        }

        [HttpPost]
        public IActionResult CompleteDemand(int id)
        {
            var demand = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demand == null)
            {
                return NotFound();
            }

            demand.Etat = DemandeclientStatus.Complete;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Demand marked as complete successfully.";
            return RedirectToAction("AllDemands");
        }
    }
}
