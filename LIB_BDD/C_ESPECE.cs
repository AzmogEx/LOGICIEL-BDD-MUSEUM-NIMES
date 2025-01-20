using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB_BDD; 

public class C_ESPECE {
    public int idEspece { get; set; }
    public string nomCommun { get; set; }
    public string nomScientifique { get; set; }
    public string statutEspece { get; set; }
    public int tailleMin { get; set; }
    public int tailleMax { get; set; }
    public string uniteTaille { get; set; }
    public int poidsMin { get; set; }
    public int poidsMax { get; set; }
    public string unitePoids { get; set; }
    public int dureeVieMin { get; set; }
    public int dureeVieMax { get; set; }
    public string habitat { get; set; }
    public string embranchement { get; set; }
    public string classe { get; set; }
    public string ordre { get; set; }
    public string famille { get; set; }
    public string description { get; set; }
    public string descUicn { get; set; }
    public string descPres { get; set; }
    public string numInventaire { get; set; }
}
