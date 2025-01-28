using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using BCrypt.Net;
using System.Reflection;


namespace LIB_BDD;

public class C_BDD {

    const string Chaine_Connexion = "Server=tcp:service.adam-marzuk.fr;Initial Catalog=animaux;Persist Security Info=False;User ID=stage;Password=Museum123.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
    List<C_ESPECE> Les_Especes = new List<C_ESPECE>();
    List<C_PARCOURS> Les_Parcours = new List<C_PARCOURS>();
    private List<C_PARCOURS> parcoursList = new List<C_PARCOURS>();

    public Exception Test_Connexion() {
        Exception ok = null;
        using(SqlConnection Connection = new SqlConnection(Chaine_Connexion)) {
            try {
                Connection.Open();
                return ok;
            } catch(Exception ex) {
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
                } else {
                    return false;
                }
            } catch(Exception) {
                return false;
            }
        }
    }

    public void Add_User(string nomUtilisateur,string motDePasse) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        // Hachage du mot de passe avec BCrypt.Net
        string motDePasseHache = BCrypt.Net.BCrypt.HashPassword(motDePasse);

        // Insertion dans la base de données
        connexion.Execute(
            "INSERT INTO Utilisateurs (NomUtilisateur, MotDePasse) VALUES (@NomUtilisateur, @MotDePasse)",
            new { NomUtilisateur = nomUtilisateur,MotDePasse = motDePasseHache }
        );
    }

    public List<C_ESPECE> Get_All_Especes() {

        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
        Les_Especes = Connexion.Query<C_ESPECE>("select * from especes").ToList();
        return Les_Especes;

    }

    public List<C_ESPECE> Get_Especes_By_Region(string P_Region) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);
        string query = @"SELECT * FROM especes WHERE idEspece IN (SELECT idEspece FROM region WHERE nomRegion = @Region)";

        return connexion.Query<C_ESPECE>(query,new { Region = P_Region }).ToList();
    }

    public List<C_ESPECE> Get_Espece_By_Name(string P_Nom) {
        P_Nom = P_Nom.ToLower();
        var Especes_Found = new List<C_ESPECE>();
        foreach(var Espece in Les_Especes) {
            if(Espece.nomCommun.ToLower().Contains(P_Nom)) Especes_Found.Add(Espece);
        }
        return Especes_Found;
    }

    public List<C_ESPECE> Get_Espece_By_Name_Scient(string P_Nom) {
        P_Nom = P_Nom.ToLower();
        var Especes_Found = new List<C_ESPECE>();
        foreach(var Espece in Les_Especes) {
            if(Espece.nomScientifique.ToLower().Contains(P_Nom)) Especes_Found.Add(Espece);
        }
        return Especes_Found;
    }

    public C_ESPECE Get_Espece_By_ID(int especeID) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        string query = "SELECT * FROM espece WHERE idEspece = @ID";

        return connexion.QuerySingleOrDefault<C_ESPECE>(query,new { ID = especeID });
    }

    public List<C_IMAGE> Get_Img_By_ID(int P_ID) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        string query = "SELECT imgPath FROM images WHERE idEspece = @ID";

        return connexion.Query<C_IMAGE>(query,new { ID = P_ID }).ToList();
    }

    public C_PARCOURS Get_Parcours_By_ID(int P_ID) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        string query = "SELECT * FROM parcours WHERE idParcours = @ID";

        return connexion.QuerySingleOrDefault<C_PARCOURS>(query,new { ID = P_ID });
    }

    public string[] Get_Region_By_ID(int P_ID) {
        using SqlConnection connexion = new SqlConnection(Chaine_Connexion);

        string query = "SELECT nomRegion FROM region WHERE idEspece = @ID";

        return connexion.Query<string>(query,new { ID = P_ID }).ToArray();
    }


    public void Delete_Espece(int P_Espece) {

        try {
            using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
            Connexion.Execute("delete from images where images.idEspece = @IDESPECE",new { IDESPECE = P_Espece });
            Connexion.Execute("delete from especes where idEspece = @IDESPECE",new { IDESPECE = P_Espece });
        } catch(Exception) {
            throw;
        }
    }

    public void Delete_Parcours(int P_Parcours) {
        try {
            using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
            Connexion.Execute("update especes set idParcours = NULL where especes.idParcours = @IDPARCOURS",new { IDPARCOURS = P_Parcours });
            Connexion.Execute("delete from parcours where idParcours = @IDPARCOURS",new { IDPARCOURS = P_Parcours });
        } catch(Exception) {
            throw;
        }
    }

    public void Add_Espece(C_ESPECE P_Espece,List<C_IMAGE> P_ImgPath, List<string> P_Regions, C_PARCOURS P_Parcours) {

        try {
            using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
            Connexion.Execute($"insert into especes(nomCommun, nomScientifique, statutEspece, tailleMin, tailleMax, uniteTaille, poidsMin, poidsMax, unitePoids, dureeVieMin, dureeVieMax, habitat, embranchement, classe, ordre, famille, description, descUicn, descPres, numInventaire, idParcours) " +
                $"VALUES (@NOMCOMMUN, @NOMSCIENT, @STATUTESPECE, @TAILLEMIN, @TAILLEMAX, @UNITETAILLE, @POIDSMIN, @POIDSMAX, @UNITEPOIDS, @DUREEVIEMIN, @DUREEVIEMAX, @HABITAT, @EMBRANCHEMENT, @CLASSE, @ORDRE, @FAMILLE, @DESCRIPTION, @DESCUICN, @DESCPRES, @NUMINVENTAIRE, @IDPARCOURS);",
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
                    IDPARCOURS = P_Parcours.idParcours
                });
            int ID = Connexion.QuerySingle<int>("SELECT TOP 1 idEspece FROM especes ORDER BY idEspece DESC;");

            Add_Image(ID,P_ImgPath);
            Add_Region(ID,P_Regions);
        } catch(Exception) {
            throw;
        }
    }

    public void Add_Image(int P_idEspece,List<C_IMAGE> P_ListPath) {
        using(SqlConnection connexion = new SqlConnection(Chaine_Connexion)) {
            foreach(var Path in P_ListPath) {
                connexion.Execute("INSERT INTO images (idEspece, imgPath) VALUES (@IDESPECE, @IMGPATH)",
                new { IDESPECE = P_idEspece,IMGPATH = Path.ImgPath });
            }
        }
    }

    public void Add_Region(int P_idEspece, List<string> P_ListRegion) {
        using(SqlConnection connexion = new SqlConnection(Chaine_Connexion)) {
            foreach(var Region in P_ListRegion) {
                connexion.Execute("INSERT INTO region (idEspece, nomRegion) VALUES (@IDESPECE, @NOMREGION)",
                new { IDESPECE = P_idEspece,NOMREGION = Region });
            }
        }
    }
    public void Edit_Parcours(C_PARCOURS P_Parcours) {
        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);

        // Mise à jour des informations du parcours (nom et description)
        Connexion.Execute("UPDATE Parcours SET nomParcours = @NOMPARCOURS, descParcours = @DESCPARCOURS " +
                          "WHERE idParcours = @IDPARCOURS",
            new {
                NOMPARCOURS = P_Parcours.nomParcours,
                DESCPARCOURS = P_Parcours.descParcours,
                IDPARCOURS = P_Parcours.idParcours
            });
    }

    public void Edit_Espece(C_ESPECE P_Espece,List<C_IMAGE> P_ImgPaths, List<string> P_Regions, C_PARCOURS P_Parcours) {
        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
        List<C_IMAGE> OldImgPaths = Get_Img_By_ID(P_Espece.idEspece);
        string[] OldListRegion = Get_Region_By_ID(P_Espece.idEspece);

        Connexion.Execute("update especes set nomCommun = @NOMCOMMUN, nomScientifique = @NOMSCIENT, statutEspece = @STATUTESPECE, " +
            "tailleMin = @TAILLEMIN, tailleMax = @TAILLEMAX, uniteTaille = @UNITETAILLE, " +
            "poidsMin = @POIDSMIN, poidsMax = @POIDSMAX, unitePoids = @UNITEPOIDS, dureeVieMin = @DUREEVIEMIN, " +
            "dureeVieMax = @DUREEVIEMAX, habitat = @HABITAT, embranchement = @EMBRANCHEMENT, classe = @CLASSE, " +
            "ordre = @ORDRE, famille = @FAMILLE, description = @DESCRIPTION, descUicn = @DESCUICN, descPres = @DESCPRES, " +
            "numInventaire = @NUMINVENTAIRE, idParcours = @IDPARCOURS where idEspece = @IDESPECE",
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
                IDESPECE = P_Espece.idEspece,
                IDPARCOURS = P_Parcours.idParcours
            });

        if(OldImgPaths != P_ImgPaths) {
            Connexion.Execute("delete from images where images.idEspece = @IDESPECE",new { IDESPECE = P_Espece.idEspece });
            Add_Image(P_Espece.idEspece,P_ImgPaths);
        }
        if(OldListRegion != P_Regions.ToArray()) {
            Connexion.Execute("delete from region where region.idEspece = @IDESPECE",new { IDESPECE = P_Espece.idEspece });
            Add_Region(P_Espece.idEspece,P_Regions);
        }
    }

    public void Create_Parcours(C_PARCOURS P_Parcours) {
        using(SqlConnection connexion = new SqlConnection(Chaine_Connexion)) {
            connexion.Execute("INSERT INTO parcours (nomParcours, imgPathParcours, descParcours) VALUES (@NOMPARCOURS, @IMGPATHPARCOURS, @DESCPARCOURS)",
            new { NOMPARCOURS = P_Parcours.nomParcours,IMGPATHPARCOURS = P_Parcours.imgPathParcours,DESCPARCOURS = P_Parcours.descParcours});
        }
    }

    public List<C_PARCOURS> Get_All_Parcours() {

        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);
        Les_Parcours = Connexion.Query<C_PARCOURS>("select * from parcours").ToList();
        return Les_Parcours;

    }

    public List<C_ESPECE> Get_All_Especes_By_IdParcours(int parcoursId) {
        using SqlConnection Connexion = new SqlConnection(Chaine_Connexion);

        // Récupération des espèces pour un parcours donné
        return Connexion.Query<C_ESPECE>(
            "SELECT * FROM especes WHERE idParcours = @IDPARCOURS",
            new { IDPARCOURS = parcoursId }).ToList();
    }
}
