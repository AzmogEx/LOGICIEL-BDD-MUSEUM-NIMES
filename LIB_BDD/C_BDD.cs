using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using BCrypt.Net;


namespace LIB_BDD;

public class C_BDD {

    const string Chaine_Connexion = "Server=tcp:service.adam-marzuk.fr;Initial Catalog=animaux;Persist Security Info=False;User ID=stage;Password=Museum123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
    public Exception Test_Connexion() {
        Exception ok = null;
        using(SqlConnection Connection = new SqlConnection(Chaine_Connexion)) {
            try {
                Connection.Open();
                return ok;
            }
            catch(Exception ex) {
                return ex;
            }
        }
    }
    public bool Connexion(string nomUtilisateur,string motDePasse) {
        using(SqlConnection connexion = new SqlConnection(Chaine_Connexion)) {
            try {
                connexion.Open();
                // Requête pour récupérer le hash du mot de passe correspondant
                string hashPassword = connexion.QuerySingleOrDefault<string>("SELECT MotDePasse FROM Utilisateurs WHERE NomUtilisateur = @NomUtilisateur",new { NomUtilisateur = nomUtilisateur });

                if(hashPassword != null && BCrypt.Net.BCrypt.Verify(motDePasse,hashPassword)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch(Exception) {
                return false;
            }
        }
    }

    public void Add_User(string nomUtilisateur,string motDePasse) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        // Hacher le mot de passe avec BCrypt.Net
        string motDePasseHache = BCrypt.Net.BCrypt.HashPassword(motDePasse);

        // Insérer dans la base de données
        connexion.Execute(
            "INSERT INTO Utilisateurs (NomUtilisateur, MotDePasse) VALUES (@NomUtilisateur, @MotDePasse)",
            new { NomUtilisateur = nomUtilisateur,MotDePasse = motDePasseHache }
        );
    }

    public List<C_ESPECE> Get_All_Especes() {

        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);

        return Connexion.Query<C_ESPECE>("select * from especes").ToList();

    }

    public string[] Get_Img_By_ID(int P_ID) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        string query = "SELECT imgPath FROM images WHERE idEspece = @ID";

        return connexion.Query<string>(query,new { ID = P_ID }).ToArray();
    }


    public void Delete_Espece(int P_Espece) {

        try {
            using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
            Connexion.Execute("delete from images where images.idEspece = @IDESPECE",new { IDESPECE = P_Espece });
            Connexion.Execute("delete from especes where idEspece = @IDESPECE",new { IDESPECE = P_Espece });
        }
        catch(Exception) {
            throw;
        }

    }

    public void Add_Espece(C_ESPECE P_Espece,List<string> P_ImgPath) {

        try {
            using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
            Connexion.Execute($"insert into especes(nomCommun, nomScientifique, statutEspece, tailleMin, tailleMax, uniteTaille, poidsMin, poidsMax, unitePoids, dureeVieMin, dureeVieMax, habitat, embranchement, classe, ordre, famille, description, descUicn, descPres, numInventaire) " +
                $"VALUES (@NOMCOMMUN, @NOMSCIENT, @STATUTESPECE, @TAILLEMIN, @TAILLEMAX, @UNITETAILLE, @POIDSMIN, @POIDSMAX, @UNITEPOIDS, @DUREEVIEMIN, @DUREEVIEMAX, @HABITAT, @EMBRANCHEMENT, @CLASSE, @ORDRE, @FAMILLE, @DESCRIPTION, @DESCUICN, @DESCPRES, @NUMINVENTAIRE);",
                new {
                    NOMCOMMUN = P_Espece.nomCommun,
                    NOMSCIENT = P_Espece.nomScientifique,
                    STATUTESPECE = P_Espece.statutEspece,
                    TAILLEMIN = P_Espece.tailleMin,
                    TAILLEMAX = P_Espece.tailleMax,
                    UNITETAILLE = P_Espece.uniteTaille,
                    POIDSMIN = P_Espece.poidsMin,
                    POIDSMAX = P_Espece.poidsMax,
                    UNITEPOIDS = P_Espece.unitePoids,
                    DUREEVIEMIN = P_Espece.dureeVieMin,
                    DUREEVIEMAX = P_Espece.dureeVieMax,
                    HABITAT = P_Espece.habitat,
                    EMBRANCHEMENT = P_Espece.embranchement,
                    CLASSE = P_Espece.classe,
                    ORDRE = P_Espece.ordre,
                    FAMILLE = P_Espece.famille,
                    DESCRIPTION = P_Espece.description,
                    DESCUICN = P_Espece.descUicn,
                    DESCPRES = P_Espece.descPres,
                    NUMINVENTAIRE = P_Espece.numInventaire
                });
            int ID = Connexion.QuerySingle<int>("SELECT TOP 1 idEspece FROM especes ORDER BY idEspece DESC;");

            Add_Image(ID,P_ImgPath);
        }
        catch(Exception) {
            throw;
        }

    }

    public void Add_Image(int P_idEspece,List<string> P_ListPath) {
        using(SqlConnection connexion = new SqlConnection(Chaine_Connexion)) {
            foreach(var Path in P_ListPath) {
                connexion.Execute("INSERT INTO images (idEspece, imgPath) VALUES (@IDESPECE, @IMGPATH)",
                new { IDESPECE = P_idEspece,IMGPATH = Path });
            }
        }
    }


    public void Edit_Espece(C_ESPECE P_Espece,List<string> P_ImgPaths) {
        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
        string[] OldImgPaths = Get_Img_By_ID(P_Espece.idEspece);

        Connexion.Execute("update especes set nomCommun = @NOMCOMMUN, nomScientifique = @NOMSCIENT, statutEspece = @STATUTESPECE, tailleMin = @TAILLEMIN, tailleMax = @TAILLEMAX, uniteTaille = @UNITETAILLE, poidsMin = @POIDSMIN, poidsMax = @POIDSMAX, unitePoids = @UNITEPOIDS, dureeVieMin = @DUREEVIEMIN, dureeVieMax = @DUREEVIEMAX, habitat = @HABITAT, embranchement = @EMBRANCHEMENT, classe = @CLASSE, ordre = @ORDRE, famille = @FAMILLE, description = @DESCRIPTION, descUicn = @DESCUICN, descPres = @DESCPRES, numInventaire = @NUMINVENTAIRE where idEspece = @IDESPECE",
            new {
                NOMCOMMUN = P_Espece.nomCommun,
                NOMSCIENT = P_Espece.nomScientifique,
                STATUTESPECE = P_Espece.statutEspece,
                TAILLEMIN = P_Espece.tailleMin,
                TAILLEMAX = P_Espece.tailleMax,
                UNITETAILLE = P_Espece.uniteTaille,
                POIDSMIN = P_Espece.poidsMin,
                POIDSMAX = P_Espece.poidsMax,
                UNITEPOIDS = P_Espece.unitePoids,
                DUREEVIEMIN = P_Espece.dureeVieMin,
                DUREEVIEMAX = P_Espece.dureeVieMax,
                HABITAT = P_Espece.habitat,
                EMBRANCHEMENT = P_Espece.embranchement,
                CLASSE = P_Espece.classe,
                ORDRE = P_Espece.ordre,
                FAMILLE = P_Espece.famille,
                DESCRIPTION = P_Espece.description,
                DESCUICN = P_Espece.descUicn,
                DESCPRES = P_Espece.descPres,
                NUMINVENTAIRE = P_Espece.numInventaire,
                IDESPECE = P_Espece.idEspece
            });

        if(OldImgPaths != P_ImgPaths.ToArray()) {
            Connexion.Execute("delete from images where images.idEspece = @IDESPECE",new { IDESPECE = P_Espece.idEspece });
            Add_Image(P_Espece.idEspece,P_ImgPaths);
        }
    }


}
