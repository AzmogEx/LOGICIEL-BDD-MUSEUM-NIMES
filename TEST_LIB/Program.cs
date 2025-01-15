using LIB_BDD;

namespace TEST_LIB;

internal class Program {


    static void Main(string[] args) {
        var BDD = new C_BDD();

        BDD.Test();

        //BDD.Delete_Espece(new C_ESPECE() { nomCommun = "axolotl"});

        var Especes = BDD.Get_All_Especes();

        foreach (var especes in Especes) {
            Console.WriteLine(especes.nomCommun);
        }
    }

}
