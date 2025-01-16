using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;


namespace LIB_BDD;

public class C_BDD {

    const string Chaine_Connexion = "SERVER=localhost;DATABASE=animaux;UserID=admin;PASSWORD=admin;";

    public void Test() {
        using(MySqlConnection Connection = new MySqlConnection(Chaine_Connexion)) {
            try {
                Connection.Open();
                Console.WriteLine("Connexion réussie !");
            } catch(Exception ex) {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }

    public List<C_ESPECE> Get_All_Especes() {
        using MySqlConnection Connexion = new MySqlConnection(Chaine_Connexion);

        return Connexion.Query<C_ESPECE>("select * from Especes").ToList();
    }

    public void Delete_Espece(C_ESPECE P_Espece) {

        using MySqlConnection Connexion = new MySqlConnection(Chaine_Connexion);


        Connexion.Query<C_ESPECE>("delete from Especes where idEspece = @IDESPECE",new { IDESPECE = P_Espece.idEspece });
    }

    public void Add_Espece(C_ESPECE P_Espece, List<string> P_ImgPath) {

        try {
            using MySqlConnection Connexion = new MySqlConnection(Chaine_Connexion);
            Connexion.Execute($"insert into especes(nomCommun, nomScientifique, statutEspece, taille, poids, dureeVie, habitat, embranchement, classe, ordre, famille, description, descUicn, descPres, numInventaire) " +
                $"VALUES (@NOMCOMMUN, @NOMSCIENT, @STATUTESPECE, @TAILLE, @POIDS, @DUREEVIE, @HABITAT, @EMBRANCHEMENT, @CLASSE, @ORDRE, @FAMILLE, @DESCRIPTION, @DESCUICN, @DESCPRES, @NUMINVENTAIRE);",
                new {
                    NOMCOMMUN = P_Espece.nomCommun,
                    NOMSCIENT = P_Espece.nomScient,
                    STATUTESPECE = P_Espece.statutEspece,
                    TAILLE = P_Espece.taille,
                    POIDS = P_Espece.poids,
                    DUREEVIE = P_Espece.dureeVie,
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
            int ID = Connexion.QuerySingle<int>("SELECT LAST_INSERT_ID();");
            Add_Image(ID,P_ImgPath);
        } catch(Exception) {
            throw;
        }

    }

    public void Add_Image(int P_idEspece,List<string> P_ListPath) {
        using(MySqlConnection connexion = new MySqlConnection(Chaine_Connexion)) {
            foreach(var Path in P_ListPath) {
                connexion.Execute("INSERT INTO images (idEspece, imgPath) VALUES (@IDESPECE, @IMGPATH)",
                new { IDESPECE = P_idEspece,IMGPATH = Path });
            }
        }
    }


    public void Edit_Espece(C_ESPECE P_Espece) {
        using MySqlConnection Connexion = new MySqlConnection(Chaine_Connexion);

        Connexion.Query<C_ESPECE>("update Especes set nomCommun = @NOMCOMMUN, nomScientifique = @NOMSCIENT, statutEspece = @STATUTESPECE, taille = @TAILLE, poids = @POIDS, dureeVie = @DUREEVIE, habitat = @HABITAT, embranchement = @EMBRANCHEMENT, classe = @CLASSE, ordre = @ORDRE, famille = @FAMILLE, description = @DESCRIPTION, descUicn = @DESCUICN, descPres = @DESCPRES, numInventaire = @NUMINVENTAIRE where idEspece = @IDESPECE",
            new {
                NOMCOMMUN = P_Espece.nomCommun,
                NOMSCIENT = P_Espece.nomScient,
                STATUTESPECE = P_Espece.statutEspece,
                TAILLE = P_Espece.taille,
                POIDS = P_Espece.poids,
                DUREEVIE = P_Espece.dureeVie,
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
    }


}
