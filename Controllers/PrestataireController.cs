using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;

namespace Hirfa.Web.Controllers
{
    public class PrestataireController : Controller
    {
        private readonly HirfaDbContext _context;
        public PrestataireController(HirfaDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult RegisterPrestataireDemande() => View("RegisterPrestataireDemande");

        [HttpPost]
        public async Task<IActionResult> RegisterPrestataireDemande(PrestataireDemandeViewModel prestataireModel)
        {
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
                var demandeprestataire = new Hirfa.Web.Models.Demandeprestataire
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
                            var diplome = new Hirfa.Web.Models.Diplomedemande
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

        [HttpGet]
        public IActionResult PrestataireDashboard()
        {
            return View("Prestataire/PrestataireDashboard");
        }

        [HttpGet]
        public IActionResult PrestataireDemands()
        {
            var demandes = _context.Demandeprestataires
                .Select(d => new Hirfa.Web.ViewModels.PrestataireDemandeListItemViewModel
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
                .Select(d => new Hirfa.Web.ViewModels.PrestataireDemandeDetailViewModel
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
                    Reason = d.Reason,
                    Sexe = d.Sexe,
                    Diplomes = d.Diplomedemandes.Select(di => new Hirfa.Web.ViewModels.DiplomeDemandeDetailViewModel
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
