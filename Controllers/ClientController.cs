using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;

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
            return View("~/Views/Client/ClientDashboard.cshtml");
        }
    }
}
