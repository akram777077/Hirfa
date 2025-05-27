using Microsoft.AspNetCore.Mvc;
using Hirfa.Web.Data;
using System.Threading.Tasks;
using Hirfa.Web.ViewModels;
using Hirfa.Web.Models; // Added missing namespace import

namespace Hirfa.Web.Controllers
{
    public class PrestataireController : Controller
    {
        private readonly HirfaDbContext _context;
        public PrestataireController(HirfaDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult RegisterPrestataireDemande() => View("~/Views/Prestataire/Register.cshtml");

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
                // Log ModelState errors for debugging
                if (!ModelState.IsValid)
                {

                    if (prestataireModel.Diplomes == null || prestataireModel.Diplomes.Count == 0)
                        prestataireModel.Diplomes = new List<DiplomeDemandeInputModel> { new DiplomeDemandeInputModel() };
                    return View("~/Views/Prestataire/Register.cshtml", prestataireModel);
                }

                if (_context.Demandeprestataires.Any(d => d.Email == prestataireModel.Email && d.Etat == "pending") || _context.Comptes.Any(c => c.Email == prestataireModel.Email))
                {
                    TempData["ErrorToast"] = "A demand or account with this email already exists.";
                    if (prestataireModel.Diplomes == null || prestataireModel.Diplomes.Count == 0)
                        prestataireModel.Diplomes = new List<DiplomeDemandeInputModel> { new DiplomeDemandeInputModel() };
                    return View("~/Views/Prestataire/Register.cshtml", prestataireModel);
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
                    Etat = "pending",
                    Sexe = prestataireModel.Sexe,
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
                return View("~/Views/Account/Login.cshtml");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("~/Views/Prestataire/Register.cshtml", prestataireModel);
            }
        }

        [HttpGet]
        public IActionResult PrestataireDashboard()
        {
            return View("~/Views/Prestataire/Dashboard.cshtml");
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
            return View("~/Views/Prestataire/Demands.cshtml", demandes);
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
            return View("~/Views/Prestataire/DemandDetails.cshtml", demande);
        }

        [HttpGet]
        public IActionResult AssignedDemands()
        {
            if (HttpContext.Session.GetString("UserRole") != "prestataire")
            {
                return Unauthorized();
            }

            int? prestataireId = HttpContext.Session.GetInt32("PrestataireId");
            if (prestataireId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var assignedDemands = _context.Demandeclients
                .Where(d => d.Idprestataire == prestataireId)
                .Select(d => new DemandeClientViewModel
                {
                    Id = d.Iddemandeclient,
                    Etat = d.Etat, // Directly assign the DemandeclientStatus
                    Description = d.Description,
                    DateDebut = d.Datedebut,
                    ClientAddress = d.IdclientNavigation.Adresse ?? "N/A"
                })
                .ToList();

            return View("AssignedDemands", assignedDemands);
        }

        [HttpPost]
        public IActionResult SubmitDevis(DevisViewModel model)
        {
            // Log if the action is hit
            Console.WriteLine("SubmitDevis action invoked.");

            // Check if ModelState is valid

            // Existing logic...
            var newDevis = new Devi
            {
                Montantglobal = model.Montantglobal,
                Datelimite = model.Datelimite.HasValue ? DateOnly.FromDateTime(model.Datelimite.Value) : null,
                Etat = model.Etat,
                Idprestataire = model.Idprestataire,
                Iddemandeclient = model.Iddemandeclient,
                Description = model.Description
            };

            _context.Devis.Add(newDevis);
            _context.SaveChanges();

            if (model.QuantiteMatieres != null)
            {
                foreach (var matiere in model.QuantiteMatieres)
                {
                    var existingMatiere = _context.Matierepremieres.FirstOrDefault(m => m.Idmatierepremiere == matiere.Idmatierepremiere);

                    if (existingMatiere == null)
                    {
                        var newMatiere = new Matierepremiere
                        {
                            Idmatierepremiere = matiere.Idmatierepremiere,
                            Nommat = matiere.MatierePremiereName,
                            Prixmat = matiere.PrixUnitaire
                        };
                        _context.Matierepremieres.Add(newMatiere);
                        _context.SaveChanges();
                        existingMatiere = newMatiere;
                    }

                    var newQuantiteMatiere = new Quantitematieredevi
                    {
                        Iddevis = newDevis.Iddevis,
                        Idmatierepremiere = existingMatiere.Idmatierepremiere,
                        Quantite = matiere.Quantite
                    };
                    _context.Quantitematieredevis.Add(newQuantiteMatiere);
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Devis created successfully.";
            return RedirectToAction("AssignedDemands");
        }

        [HttpGet]
        public IActionResult FillDevis(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "prestataire")
            {
                return Unauthorized();
            }

            var devis = _context.Demandeclients
                .Where(d => d.Iddemandeclient == id)
                .Select(d => new DevisViewModel
                {
                    Iddevis = 0, // Default value for new Devis
                    Etat = d.Etat.ToString(),
                    Montantglobal = 0, // Default value
                    Datelimite = null, // Default value
                    Avisclient = null, // Default value
                    Idprestataire = HttpContext.Session.GetInt32("PrestataireId") ?? 0,
                    Iddemandeclient = d.Iddemandeclient,
                    Description = "", // Default empty description for the devis
                    DemandDetails = new DemandeClientViewModel
                    {
                        Id = d.Iddemandeclient,
                        Etat = d.Etat,
                        Description = d.Description,
                        DateDebut = d.Datedebut,
                        ClientAddress = d.IdclientNavigation.Adresse ?? "N/A",
                        ClientName = d.IdclientNavigation.Prenom + " " + d.IdclientNavigation.Nom,
                        ClientGender = d.IdclientNavigation.Sexe != null ? d.IdclientNavigation.Sexe.ToString() : "Unknown"
                    }
                })
                .FirstOrDefault();

            if (devis == null)
            {
                return NotFound();
            }

            return View("FillDevis", devis);
        }

        [HttpPost]
        public IActionResult RejectDemande(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "prestataire")
            {
                return Unauthorized();
            }

            var demande = _context.Demandeclients.FirstOrDefault(d => d.Iddemandeclient == id);
            if (demande == null)
            {
                return NotFound();
            }

            demande.Etat = DemandeclientStatus.Pending;
            demande.Idprestataire = null;

            _context.SaveChanges();

            return RedirectToAction("AssignedDemands");
        }

        [HttpPost]
        public IActionResult ToggleActiveStatus()
        {
            var prestataireId = HttpContext.Session.GetInt32("PrestataireId");
            if (prestataireId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var prestataire = _context.Prestataires.FirstOrDefault(p => p.Idprestataire == prestataireId);
            if (prestataire != null)
            {
                prestataire.Estdisponible = !prestataire.Estdisponible;
                _context.SaveChanges();
                HttpContext.Session.SetString("IsActive", prestataire.Estdisponible.ToString());

                if (!prestataire.Estdisponible) // If status is changed to Non Active
                {
                    var assignedDemands = _context.Demandeclients.Where(d => d.Idprestataire == prestataireId).ToList();
                    foreach (var demand in assignedDemands)
                    {
                        demand.Etat = DemandeclientStatus.Pending;
                        demand.Idprestataire = null;
                    }
                    _context.SaveChanges();
                }

                TempData["SuccessToast"] = $"Your status has been changed to {(prestataire.Estdisponible ? "Active" : "Non Active")} successfully.";
            }

            return RedirectToAction("Dashboard", "Prestataire");
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return RedirectToAction("PrestataireDashboard");
        }
    }
}
