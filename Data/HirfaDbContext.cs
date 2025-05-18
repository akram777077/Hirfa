using System;
using System.Collections.Generic;
using Hirfa.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hirfa.Web.Data;

public partial class HirfaDbContext : DbContext
{
    public HirfaDbContext()
    {
    }

    public HirfaDbContext(DbContextOptions<HirfaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Compte> Comptes { get; set; }

    public virtual DbSet<Demandeclient> Demandeclients { get; set; }

    public virtual DbSet<Demandeprestataire> Demandeprestataires { get; set; }

    public virtual DbSet<Devi> Devis { get; set; }

    public virtual DbSet<Diplome> Diplomes { get; set; }

    public virtual DbSet<Diplomedemande> Diplomedemandes { get; set; }

    public virtual DbSet<Evaluation> Evaluations { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<Matierefacture> Matierefactures { get; set; }

    public virtual DbSet<Matierepremiere> Matierepremieres { get; set; }

    public virtual DbSet<Plainteclient> Plainteclients { get; set; }

    public virtual DbSet<Plainteprestataire> Plainteprestataires { get; set; }

    public virtual DbSet<Prestataire> Prestataires { get; set; }

    public virtual DbSet<Quantitematieredevi> Quantitematieredevis { get; set; }

    public virtual DbSet<Quantitematierefacture> Quantitematierefactures { get; set; }

    public virtual DbSet<Serviceclient> Serviceclients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // The connection string will be provided via dependency injection in Program.cs or Startup.cs
            // No hardcoded connection string here
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Idadmin).HasName("admin_pkey");

            entity.ToTable("admin");

            entity.HasIndex(e => e.Idcompte, "admin_idcompte_key").IsUnique();

            entity.Property(e => e.Idadmin).HasColumnName("idadmin");
            entity.Property(e => e.Idcompte).HasColumnName("idcompte");

            entity.HasOne(d => d.IdcompteNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.Idcompte)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("admin_idcompte_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Idclient).HasName("client_pkey");

            entity.ToTable("client");

            entity.HasIndex(e => e.Idcompte, "client_idcompte_key").IsUnique();

            entity.Property(e => e.Idclient).HasColumnName("idclient");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Datenaissance).HasColumnName("datenaissance");
            entity.Property(e => e.Idcompte).HasColumnName("idcompte");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Numerotelephone)
                .HasMaxLength(10)
                .HasColumnName("numerotelephone");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .HasColumnName("prenom");
            entity.Property(e => e.Sexe)
                .HasMaxLength(1)
                .HasColumnName("sexe");

            entity.HasOne(d => d.IdcompteNavigation).WithOne(p => p.Client)
                .HasForeignKey<Client>(d => d.Idcompte)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("client_idcompte_fkey");
        });

        modelBuilder.Entity<Compte>(entity =>
        {
            entity.HasKey(e => e.Idcompte).HasName("compte_pkey");

            entity.ToTable("compte");

            entity.HasIndex(e => e.Email, "compte_email_key").IsUnique();

            entity.Property(e => e.Idcompte).HasColumnName("idcompte");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Motdepasse).HasColumnName("motdepasse");
        });

        modelBuilder.Entity<Demandeclient>(entity =>
        {
            entity.HasKey(e => e.Iddemandeclient).HasName("demandeclient_pkey");

            entity.ToTable("demandeclient");

            entity.Property(e => e.Iddemandeclient).HasColumnName("iddemandeclient");
            entity.Property(e => e.Categorie).HasColumnName("categorie");
            entity.Property(e => e.Datedebut).HasColumnName("datedebut");
            entity.Property(e => e.Datedemande)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("datedemande");
            entity.Property(e => e.Datefin).HasColumnName("datefin");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Etat)
                .HasConversion<string>()
                .HasColumnName("etat");
            entity.Property(e => e.Idclient).HasColumnName("idclient");
            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");

            entity.HasOne(d => d.IdclientNavigation).WithMany(p => p.Demandeclients)
                .HasForeignKey(d => d.Idclient)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("demandeclient_idclient_fkey");

            entity.HasOne(d => d.IdprestataireNavigation).WithMany(p => p.Demandeclients)
                .HasForeignKey(d => d.Idprestataire)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("demandeclient_idprestataire_fkey");
        });

        modelBuilder.Entity<Demandeprestataire>(entity =>
        {
            entity.HasKey(e => e.Iddemandeprestataire).HasName("demandeprestataire_pkey");

            entity.ToTable("demandeprestataire");

            entity.HasIndex(e => e.Email, "demandeprestataire_email_key").IsUnique();

            entity.HasIndex(e => e.Nin, "demandeprestataire_nin_key").IsUnique();

            entity.Property(e => e.Iddemandeprestataire).HasColumnName("iddemandeprestataire");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Casierjudiciaire).HasColumnName("casierjudiciaire");
            entity.Property(e => e.Cv).HasColumnName("cv");
            entity.Property(e => e.Datenaissance).HasColumnName("datenaissance");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Etat).HasColumnName("etat");
            entity.Property(e => e.Nin).HasColumnName("nin");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Numtel)
                .HasMaxLength(10)
                .HasColumnName("numtel");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .HasColumnName("prenom");
            entity.Property(e => e.Typeservice).HasColumnName("typeservice");
            entity.Property(e => e.Reason)
                .HasColumnName("reason")
                .HasMaxLength(500)
                .IsRequired(false);
        });

        modelBuilder.Entity<Devi>(entity =>
        {
            entity.HasKey(e => e.Iddevis).HasName("devis_pkey");

            entity.ToTable("devis");

            entity.Property(e => e.Iddevis).HasColumnName("iddevis");
            entity.Property(e => e.Avisclient).HasColumnName("avisclient");
            entity.Property(e => e.Datelimite).HasColumnName("datelimite");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Etat).HasColumnName("etat");
            entity.Property(e => e.Iddemandeclient).HasColumnName("iddemandeclient");
            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");
            entity.Property(e => e.Montantglobal)
                .HasPrecision(10, 2)
                .HasColumnName("montantglobal");

            entity.HasOne(d => d.IddemandeclientNavigation).WithMany(p => p.Devis)
                .HasForeignKey(d => d.Iddemandeclient)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("devis_iddemandeclient_fkey");

            entity.HasOne(d => d.IdprestataireNavigation).WithMany(p => p.Devis)
                .HasForeignKey(d => d.Idprestataire)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("devis_idprestataire_fkey");
        });

        modelBuilder.Entity<Diplome>(entity =>
        {
            entity.HasKey(e => e.Iddiplome).HasName("diplome_pkey");

            entity.ToTable("diplome");

            entity.Property(e => e.Iddiplome).HasColumnName("iddiplome");
            entity.Property(e => e.Anneeobtention).HasColumnName("anneeobtention");
            entity.Property(e => e.Fiche).HasColumnName("fiche");
            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");
            entity.Property(e => e.Institution).HasColumnName("institution");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.IdprestataireNavigation).WithMany(p => p.Diplomes)
                .HasForeignKey(d => d.Idprestataire)
                .HasConstraintName("diplome_idprestataire_fkey");
        });

        modelBuilder.Entity<Diplomedemande>(entity =>
        {
            entity.HasKey(e => e.Iddiplomedemande).HasName("diplomedemande_pkey");

            entity.ToTable("diplomedemande");

            entity.Property(e => e.Iddiplomedemande).HasColumnName("iddiplomedemande");
            entity.Property(e => e.Anneeobtention).HasColumnName("anneeobtention");
            entity.Property(e => e.Fiche).HasColumnName("fiche");
            entity.Property(e => e.Iddemandeprestataire).HasColumnName("iddemandeprestataire");
            entity.Property(e => e.Institution).HasColumnName("institution");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.IddemandeprestataireNavigation).WithMany(p => p.Diplomedemandes)
                .HasForeignKey(d => d.Iddemandeprestataire)
                .HasConstraintName("diplomedemande_iddemandeprestataire_fkey");
        });

        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.HasKey(e => e.Idevaluation).HasName("evaluation_pkey");

            entity.ToTable("evaluation");

            entity.Property(e => e.Idevaluation).HasColumnName("idevaluation");
            entity.Property(e => e.Commentaire).HasColumnName("commentaire");
            entity.Property(e => e.Idclient).HasColumnName("idclient");
            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Note).HasColumnName("note");

            entity.HasOne(d => d.IdclientNavigation).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.Idclient)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("evaluation_idclient_fkey");

            entity.HasOne(d => d.IdprestataireNavigation).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.Idprestataire)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("evaluation_idprestataire_fkey");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.Idfacture).HasName("facture_pkey");

            entity.ToTable("facture");

            entity.HasIndex(e => e.Iddemandeclient, "facture_iddemandeclient_key").IsUnique();

            entity.Property(e => e.Idfacture).HasColumnName("idfacture");
            entity.Property(e => e.Dateecheance).HasColumnName("dateecheance");
            entity.Property(e => e.Dateemission)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("dateemission");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Iddemandeclient).HasColumnName("iddemandeclient");
            entity.Property(e => e.Modepaiement).HasColumnName("modepaiement");

            entity.HasOne(d => d.IddemandeclientNavigation).WithOne(p => p.Facture)
                .HasForeignKey<Facture>(d => d.Iddemandeclient)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("facture_iddemandeclient_fkey");
        });

        modelBuilder.Entity<Matierefacture>(entity =>
        {
            entity.HasKey(e => e.Idmatierefacture).HasName("matierefacture_pkey");

            entity.ToTable("matierefacture");

            entity.Property(e => e.Idmatierefacture).HasColumnName("idmatierefacture");
            entity.Property(e => e.Nommat).HasColumnName("nommat");
            entity.Property(e => e.Prixmat)
                .HasPrecision(10, 2)
                .HasColumnName("prixmat");
        });

        modelBuilder.Entity<Matierepremiere>(entity =>
        {
            entity.HasKey(e => e.Idmatierepremiere).HasName("matierepremiere_pkey");

            entity.ToTable("matierepremiere");

            entity.Property(e => e.Idmatierepremiere).HasColumnName("idmatierepremiere");
            entity.Property(e => e.Nommat).HasColumnName("nommat");
            entity.Property(e => e.Prixmat)
                .HasPrecision(10, 2)
                .HasColumnName("prixmat");
        });

        modelBuilder.Entity<Plainteclient>(entity =>
        {
            entity.HasKey(e => e.Idplainteclient).HasName("plainteclient_pkey");

            entity.ToTable("plainteclient");

            entity.Property(e => e.Idplainteclient).HasColumnName("idplainteclient");
            entity.Property(e => e.Contenu).HasColumnName("contenu");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Idclient).HasColumnName("idclient");
            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");
            entity.Property(e => e.Sujet).HasColumnName("sujet");

            entity.HasOne(d => d.IdclientNavigation).WithMany(p => p.Plainteclients)
                .HasForeignKey(d => d.Idclient)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("plainteclient_idclient_fkey");

            entity.HasOne(d => d.IdprestataireNavigation).WithMany(p => p.Plainteclients)
                .HasForeignKey(d => d.Idprestataire)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("plainteclient_idprestataire_fkey");
        });

        modelBuilder.Entity<Plainteprestataire>(entity =>
        {
            entity.HasKey(e => e.Idplainteprestataire).HasName("plainteprestataire_pkey");

            entity.ToTable("plainteprestataire");

            entity.Property(e => e.Idplainteprestataire).HasColumnName("idplainteprestataire");
            entity.Property(e => e.Contenu).HasColumnName("contenu");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Idclient).HasColumnName("idclient");
            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");
            entity.Property(e => e.Sujet).HasColumnName("sujet");

            entity.HasOne(d => d.IdclientNavigation).WithMany(p => p.Plainteprestataires)
                .HasForeignKey(d => d.Idclient)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("plainteprestataire_idclient_fkey");

            entity.HasOne(d => d.IdprestataireNavigation).WithMany(p => p.Plainteprestataires)
                .HasForeignKey(d => d.Idprestataire)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("plainteprestataire_idprestataire_fkey");
        });

        modelBuilder.Entity<Prestataire>(entity =>
        {
            entity.HasKey(e => e.Idprestataire).HasName("prestataire_pkey");

            entity.ToTable("prestataire");

            entity.HasIndex(e => e.Idcompte, "prestataire_idcompte_key").IsUnique();

            entity.HasIndex(e => e.Nin, "prestataire_nin_key").IsUnique();

            entity.Property(e => e.Idprestataire).HasColumnName("idprestataire");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Casierjudiciaire).HasColumnName("casierjudiciaire");
            entity.Property(e => e.Cv).HasColumnName("cv");
            entity.Property(e => e.Datenaissance).HasColumnName("datenaissance");
            entity.Property(e => e.Estdisponible)
                .HasDefaultValue(true)
                .HasColumnName("estdisponible");
            entity.Property(e => e.Idcompte).HasColumnName("idcompte");
            entity.Property(e => e.Iddemandeprestataire).HasColumnName("iddemandeprestataire");
            entity.Property(e => e.Nin).HasColumnName("nin");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Numerotelephone)
                .HasMaxLength(10)
                .HasColumnName("numerotelephone");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .HasColumnName("prenom");
            entity.Property(e => e.Sexe)
                .HasMaxLength(1)
                .HasColumnName("sexe");
            entity.Property(e => e.Typeservice).HasColumnName("typeservice");

            entity.HasOne(d => d.IdcompteNavigation).WithOne(p => p.Prestataire)
                .HasForeignKey<Prestataire>(d => d.Idcompte)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("prestataire_idcompte_fkey");

            entity.HasOne(d => d.IddemandeprestataireNavigation).WithMany(p => p.Prestataires)
                .HasForeignKey(d => d.Iddemandeprestataire)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("prestataire_iddemandeprestataire_fkey");
        });

        modelBuilder.Entity<Quantitematieredevi>(entity =>
        {
            entity.HasKey(e => new { e.Iddevis, e.Idmatierepremiere }).HasName("quantitematieredevis_pkey");

            entity.ToTable("quantitematieredevis");

            entity.Property(e => e.Iddevis).HasColumnName("iddevis");
            entity.Property(e => e.Idmatierepremiere).HasColumnName("idmatierepremiere");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.IddevisNavigation).WithMany(p => p.Quantitematieredevis)
                .HasForeignKey(d => d.Iddevis)
                .HasConstraintName("quantitematieredevis_iddevis_fkey");

            entity.HasOne(d => d.IdmatierepremiereNavigation).WithMany(p => p.Quantitematieredevis)
                .HasForeignKey(d => d.Idmatierepremiere)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("quantitematieredevis_idmatierepremiere_fkey");
        });

        modelBuilder.Entity<Quantitematierefacture>(entity =>
        {
            entity.HasKey(e => new { e.Idfacture, e.Idmatierefacture }).HasName("quantitematierefacture_pkey");

            entity.ToTable("quantitematierefacture");

            entity.Property(e => e.Idfacture).HasColumnName("idfacture");
            entity.Property(e => e.Idmatierefacture).HasColumnName("idmatierefacture");
            entity.Property(e => e.Quantite).HasColumnName("quantite");

            entity.HasOne(d => d.IdfactureNavigation).WithMany(p => p.Quantitematierefactures)
                .HasForeignKey(d => d.Idfacture)
                .HasConstraintName("quantitematierefacture_idfacture_fkey");

            entity.HasOne(d => d.IdmatierefactureNavigation).WithMany(p => p.Quantitematierefactures)
                .HasForeignKey(d => d.Idmatierefacture)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("quantitematierefacture_idmatierefacture_fkey");
        });

        modelBuilder.Entity<Serviceclient>(entity =>
        {
            entity.HasKey(e => e.Idserviceclient).HasName("serviceclient_pkey");

            entity.ToTable("serviceclient");

            entity.HasIndex(e => e.Idcompte, "serviceclient_idcompte_key").IsUnique();

            entity.Property(e => e.Idserviceclient).HasColumnName("idserviceclient");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Datenaissance).HasColumnName("datenaissance");
            entity.Property(e => e.Idcompte).HasColumnName("idcompte");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Numerotelephone)
                .HasMaxLength(10)
                .HasColumnName("numerotelephone");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .HasColumnName("prenom");
            entity.Property(e => e.Sexe)
                .HasMaxLength(1)
                .HasColumnName("sexe");

            entity.HasOne(d => d.IdcompteNavigation).WithOne(p => p.Serviceclient)
                .HasForeignKey<Serviceclient>(d => d.Idcompte)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("serviceclient_idcompte_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
