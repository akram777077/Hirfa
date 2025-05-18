using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hirfa.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compte",
                columns: table => new
                {
                    idcompte = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false),
                    motdepasse = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("compte_pkey", x => x.idcompte);
                });

            migrationBuilder.CreateTable(
                name: "demandeprestataire",
                columns: table => new
                {
                    iddemandeprestataire = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    datenaissance = table.Column<DateOnly>(type: "date", nullable: true),
                    numtel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    nin = table.Column<string>(type: "text", nullable: false),
                    typeservice = table.Column<string>(type: "text", nullable: false),
                    adresse = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false),
                    cv = table.Column<string>(type: "text", nullable: true),
                    etat = table.Column<string>(type: "text", nullable: false),
                    casierjudiciaire = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("demandeprestataire_pkey", x => x.iddemandeprestataire);
                });

            migrationBuilder.CreateTable(
                name: "matierefacture",
                columns: table => new
                {
                    idmatierefacture = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nommat = table.Column<string>(type: "text", nullable: false),
                    prixmat = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("matierefacture_pkey", x => x.idmatierefacture);
                });

            migrationBuilder.CreateTable(
                name: "matierepremiere",
                columns: table => new
                {
                    idmatierepremiere = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nommat = table.Column<string>(type: "text", nullable: false),
                    prixmat = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("matierepremiere_pkey", x => x.idmatierepremiere);
                });

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    idadmin = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idcompte = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("admin_pkey", x => x.idadmin);
                    table.ForeignKey(
                        name: "admin_idcompte_fkey",
                        column: x => x.idcompte,
                        principalTable: "compte",
                        principalColumn: "idcompte",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    idclient = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    numerotelephone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    datenaissance = table.Column<DateOnly>(type: "date", nullable: true),
                    adresse = table.Column<string>(type: "text", nullable: true),
                    sexe = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    idcompte = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("client_pkey", x => x.idclient);
                    table.ForeignKey(
                        name: "client_idcompte_fkey",
                        column: x => x.idcompte,
                        principalTable: "compte",
                        principalColumn: "idcompte",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "serviceclient",
                columns: table => new
                {
                    idserviceclient = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    numerotelephone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    datenaissance = table.Column<DateOnly>(type: "date", nullable: true),
                    adresse = table.Column<string>(type: "text", nullable: true),
                    sexe = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    idcompte = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("serviceclient_pkey", x => x.idserviceclient);
                    table.ForeignKey(
                        name: "serviceclient_idcompte_fkey",
                        column: x => x.idcompte,
                        principalTable: "compte",
                        principalColumn: "idcompte",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "diplomedemande",
                columns: table => new
                {
                    iddiplomedemande = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    institution = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    anneeobtention = table.Column<int>(type: "integer", nullable: true),
                    fiche = table.Column<string>(type: "text", nullable: true),
                    iddemandeprestataire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("diplomedemande_pkey", x => x.iddiplomedemande);
                    table.ForeignKey(
                        name: "diplomedemande_iddemandeprestataire_fkey",
                        column: x => x.iddemandeprestataire,
                        principalTable: "demandeprestataire",
                        principalColumn: "iddemandeprestataire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prestataire",
                columns: table => new
                {
                    idprestataire = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prenom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    numerotelephone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    datenaissance = table.Column<DateOnly>(type: "date", nullable: true),
                    adresse = table.Column<string>(type: "text", nullable: true),
                    sexe = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    estdisponible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    nin = table.Column<string>(type: "text", nullable: false),
                    typeservice = table.Column<string>(type: "text", nullable: false),
                    cv = table.Column<string>(type: "text", nullable: true),
                    casierjudiciaire = table.Column<string>(type: "text", nullable: true),
                    idcompte = table.Column<int>(type: "integer", nullable: false),
                    iddemandeprestataire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("prestataire_pkey", x => x.idprestataire);
                    table.ForeignKey(
                        name: "prestataire_idcompte_fkey",
                        column: x => x.idcompte,
                        principalTable: "compte",
                        principalColumn: "idcompte",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "prestataire_iddemandeprestataire_fkey",
                        column: x => x.iddemandeprestataire,
                        principalTable: "demandeprestataire",
                        principalColumn: "iddemandeprestataire",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "demandeclient",
                columns: table => new
                {
                    iddemandeclient = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    datedemande = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    datedebut = table.Column<DateOnly>(type: "date", nullable: true),
                    datefin = table.Column<DateOnly>(type: "date", nullable: true),
                    etat = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    categorie = table.Column<string>(type: "text", nullable: false),
                    idclient = table.Column<int>(type: "integer", nullable: false),
                    idprestataire = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("demandeclient_pkey", x => x.iddemandeclient);
                    table.ForeignKey(
                        name: "demandeclient_idclient_fkey",
                        column: x => x.idclient,
                        principalTable: "client",
                        principalColumn: "idclient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "demandeclient_idprestataire_fkey",
                        column: x => x.idprestataire,
                        principalTable: "prestataire",
                        principalColumn: "idprestataire",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "diplome",
                columns: table => new
                {
                    iddiplome = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    institution = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    anneeobtention = table.Column<int>(type: "integer", nullable: true),
                    fiche = table.Column<string>(type: "text", nullable: true),
                    idprestataire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("diplome_pkey", x => x.iddiplome);
                    table.ForeignKey(
                        name: "diplome_idprestataire_fkey",
                        column: x => x.idprestataire,
                        principalTable: "prestataire",
                        principalColumn: "idprestataire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "evaluation",
                columns: table => new
                {
                    idevaluation = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    commentaire = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<int>(type: "integer", nullable: false),
                    image = table.Column<string>(type: "text", nullable: true),
                    idclient = table.Column<int>(type: "integer", nullable: false),
                    idprestataire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("evaluation_pkey", x => x.idevaluation);
                    table.ForeignKey(
                        name: "evaluation_idclient_fkey",
                        column: x => x.idclient,
                        principalTable: "client",
                        principalColumn: "idclient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "evaluation_idprestataire_fkey",
                        column: x => x.idprestataire,
                        principalTable: "prestataire",
                        principalColumn: "idprestataire",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plainteclient",
                columns: table => new
                {
                    idplainteclient = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sujet = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    contenu = table.Column<string>(type: "text", nullable: false),
                    idprestataire = table.Column<int>(type: "integer", nullable: false),
                    idclient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("plainteclient_pkey", x => x.idplainteclient);
                    table.ForeignKey(
                        name: "plainteclient_idclient_fkey",
                        column: x => x.idclient,
                        principalTable: "client",
                        principalColumn: "idclient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "plainteclient_idprestataire_fkey",
                        column: x => x.idprestataire,
                        principalTable: "prestataire",
                        principalColumn: "idprestataire",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plainteprestataire",
                columns: table => new
                {
                    idplainteprestataire = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sujet = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    contenu = table.Column<string>(type: "text", nullable: false),
                    idclient = table.Column<int>(type: "integer", nullable: false),
                    idprestataire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("plainteprestataire_pkey", x => x.idplainteprestataire);
                    table.ForeignKey(
                        name: "plainteprestataire_idclient_fkey",
                        column: x => x.idclient,
                        principalTable: "client",
                        principalColumn: "idclient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "plainteprestataire_idprestataire_fkey",
                        column: x => x.idprestataire,
                        principalTable: "prestataire",
                        principalColumn: "idprestataire",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "devis",
                columns: table => new
                {
                    iddevis = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    etat = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    montantglobal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    datelimite = table.Column<DateOnly>(type: "date", nullable: true),
                    avisclient = table.Column<string>(type: "text", nullable: true),
                    idprestataire = table.Column<int>(type: "integer", nullable: false),
                    iddemandeclient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("devis_pkey", x => x.iddevis);
                    table.ForeignKey(
                        name: "devis_iddemandeclient_fkey",
                        column: x => x.iddemandeclient,
                        principalTable: "demandeclient",
                        principalColumn: "iddemandeclient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "devis_idprestataire_fkey",
                        column: x => x.idprestataire,
                        principalTable: "prestataire",
                        principalColumn: "idprestataire",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "facture",
                columns: table => new
                {
                    idfacture = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dateemission = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    dateecheance = table.Column<DateOnly>(type: "date", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    modepaiement = table.Column<string>(type: "text", nullable: true),
                    iddemandeclient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("facture_pkey", x => x.idfacture);
                    table.ForeignKey(
                        name: "facture_iddemandeclient_fkey",
                        column: x => x.iddemandeclient,
                        principalTable: "demandeclient",
                        principalColumn: "iddemandeclient",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quantitematieredevis",
                columns: table => new
                {
                    iddevis = table.Column<int>(type: "integer", nullable: false),
                    idmatierepremiere = table.Column<int>(type: "integer", nullable: false),
                    quantite = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("quantitematieredevis_pkey", x => new { x.iddevis, x.idmatierepremiere });
                    table.ForeignKey(
                        name: "quantitematieredevis_iddevis_fkey",
                        column: x => x.iddevis,
                        principalTable: "devis",
                        principalColumn: "iddevis",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "quantitematieredevis_idmatierepremiere_fkey",
                        column: x => x.idmatierepremiere,
                        principalTable: "matierepremiere",
                        principalColumn: "idmatierepremiere",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quantitematierefacture",
                columns: table => new
                {
                    idfacture = table.Column<int>(type: "integer", nullable: false),
                    idmatierefacture = table.Column<int>(type: "integer", nullable: false),
                    quantite = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("quantitematierefacture_pkey", x => new { x.idfacture, x.idmatierefacture });
                    table.ForeignKey(
                        name: "quantitematierefacture_idfacture_fkey",
                        column: x => x.idfacture,
                        principalTable: "facture",
                        principalColumn: "idfacture",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "quantitematierefacture_idmatierefacture_fkey",
                        column: x => x.idmatierefacture,
                        principalTable: "matierefacture",
                        principalColumn: "idmatierefacture",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "admin_idcompte_key",
                table: "admin",
                column: "idcompte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "client_idcompte_key",
                table: "client",
                column: "idcompte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "compte_email_key",
                table: "compte",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_demandeclient_idclient",
                table: "demandeclient",
                column: "idclient");

            migrationBuilder.CreateIndex(
                name: "IX_demandeclient_idprestataire",
                table: "demandeclient",
                column: "idprestataire");

            migrationBuilder.CreateIndex(
                name: "demandeprestataire_email_key",
                table: "demandeprestataire",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "demandeprestataire_nin_key",
                table: "demandeprestataire",
                column: "nin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_devis_iddemandeclient",
                table: "devis",
                column: "iddemandeclient");

            migrationBuilder.CreateIndex(
                name: "IX_devis_idprestataire",
                table: "devis",
                column: "idprestataire");

            migrationBuilder.CreateIndex(
                name: "IX_diplome_idprestataire",
                table: "diplome",
                column: "idprestataire");

            migrationBuilder.CreateIndex(
                name: "IX_diplomedemande_iddemandeprestataire",
                table: "diplomedemande",
                column: "iddemandeprestataire");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_idclient",
                table: "evaluation",
                column: "idclient");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_idprestataire",
                table: "evaluation",
                column: "idprestataire");

            migrationBuilder.CreateIndex(
                name: "facture_iddemandeclient_key",
                table: "facture",
                column: "iddemandeclient",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_plainteclient_idclient",
                table: "plainteclient",
                column: "idclient");

            migrationBuilder.CreateIndex(
                name: "IX_plainteclient_idprestataire",
                table: "plainteclient",
                column: "idprestataire");

            migrationBuilder.CreateIndex(
                name: "IX_plainteprestataire_idclient",
                table: "plainteprestataire",
                column: "idclient");

            migrationBuilder.CreateIndex(
                name: "IX_plainteprestataire_idprestataire",
                table: "plainteprestataire",
                column: "idprestataire");

            migrationBuilder.CreateIndex(
                name: "IX_prestataire_iddemandeprestataire",
                table: "prestataire",
                column: "iddemandeprestataire");

            migrationBuilder.CreateIndex(
                name: "prestataire_idcompte_key",
                table: "prestataire",
                column: "idcompte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "prestataire_nin_key",
                table: "prestataire",
                column: "nin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quantitematieredevis_idmatierepremiere",
                table: "quantitematieredevis",
                column: "idmatierepremiere");

            migrationBuilder.CreateIndex(
                name: "IX_quantitematierefacture_idmatierefacture",
                table: "quantitematierefacture",
                column: "idmatierefacture");

            migrationBuilder.CreateIndex(
                name: "serviceclient_idcompte_key",
                table: "serviceclient",
                column: "idcompte",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "diplome");

            migrationBuilder.DropTable(
                name: "diplomedemande");

            migrationBuilder.DropTable(
                name: "evaluation");

            migrationBuilder.DropTable(
                name: "plainteclient");

            migrationBuilder.DropTable(
                name: "plainteprestataire");

            migrationBuilder.DropTable(
                name: "quantitematieredevis");

            migrationBuilder.DropTable(
                name: "quantitematierefacture");

            migrationBuilder.DropTable(
                name: "serviceclient");

            migrationBuilder.DropTable(
                name: "devis");

            migrationBuilder.DropTable(
                name: "matierepremiere");

            migrationBuilder.DropTable(
                name: "facture");

            migrationBuilder.DropTable(
                name: "matierefacture");

            migrationBuilder.DropTable(
                name: "demandeclient");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "prestataire");

            migrationBuilder.DropTable(
                name: "compte");

            migrationBuilder.DropTable(
                name: "demandeprestataire");
        }
    }
}
