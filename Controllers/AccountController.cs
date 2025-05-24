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
                return RedirectToAction("PrestataireDashboard", "Prestataire");
            }
            if (_context.Admins.Any(a => a.Idcompte == compte.Idcompte))
            {
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "admin");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            if (_context.Clients.Any(c => c.Idcompte == compte.Idcompte))
            {
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "client");
                return RedirectToAction("ClientDashboard", "Client");
            }
            if (_context.Serviceclients.Any(s => s.Idcompte == compte.Idcompte))
            {
                HttpContext.Session.SetString("UserName", compte.Email);
                HttpContext.Session.SetString("UserRole", "serviceclient");
                return RedirectToAction("ServiceClientDashboard", "ServiceClient");
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
        public IActionResult RegisterAdmin() => RedirectToAction("RegisterAdmin", "Admin");
        [HttpGet]
        public IActionResult RegisterClient() => RedirectToAction("RegisterClient", "Client");
        [HttpGet]
        public IActionResult RegisterServiceClient() => RedirectToAction("RegisterServiceClient", "ServiceClient");
        [HttpGet]
        public IActionResult RegisterPrestataire() => RedirectToAction("RegisterPrestataireDemande", "Prestataire");
        [HttpGet]
        public IActionResult RegisterPrestataireDemande() => RedirectToAction("RegisterPrestataireDemande", "Prestataire");

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
