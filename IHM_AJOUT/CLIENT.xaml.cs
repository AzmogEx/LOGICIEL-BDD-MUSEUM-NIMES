using LIB_BDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IHM_BASE {
    /// <summary>
    /// Logique d'interaction pour CLIENT.xaml
    /// </summary>
    public partial class CLIENT :Window {
        private C_BDD BDD = null;
        public CLIENT() {
            BDD = new();
            
            List<C_ESPECE> List_Especes = new();

            var Etat_Connexion = BDD.Test_Connexion();

            if(Etat_Connexion == null) {
                List_Especes = BDD.Get_All_Especes();
            }
            else {
                MessageBox.Show($"La connexion à la base de données a échoué : {Etat_Connexion}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }


            InitializeComponent();
        }

        private void Button_Close_Click(object sender,RoutedEventArgs e) {
            Close();
        }
    }
}
