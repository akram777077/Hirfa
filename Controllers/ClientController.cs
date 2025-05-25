using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Hirfa.Web.Models;

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
                .Where(d => d.Idclient == clientId.Value)
                .ToList();

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
    }
}
