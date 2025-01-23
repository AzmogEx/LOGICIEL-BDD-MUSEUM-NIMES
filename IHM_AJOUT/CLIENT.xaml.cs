using LIB_BDD;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private string Desc1;
        private string Desc2;

        public CLIENT() {
            BDD = new();
            List<C_ESPECE> List_Especes = new();

            //Verification de la connexion à la base
            var Etat_Connexion = BDD.Test_Connexion();

            if(Etat_Connexion == null) {
                List_Especes = BDD.Get_All_Especes();
            }
            else {
                MessageBox.Show($"La connexion à la base de données a échoué : {Etat_Connexion}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }


            InitializeComponent();

            //Recuperation des especes de la base dans la listbox
            //Lstbx_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            //Lstbx_Animaux.ItemsSource = List_Especes;

            Grid_Info.Visibility = Visibility.Hidden;
            Grid_Recherche.Visibility = Visibility.Visible;
            Lstbx_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            

        }

        private void Button_Close_Click(object sender,RoutedEventArgs e) {
            Grid_Info.Visibility = Visibility.Hidden;
            Grid_Recherche.Visibility = Visibility.Visible;
        }

        private void Lstbx_Animaux_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
            Grid_Recherche.Visibility = Visibility.Hidden;

            Grid_Info.Visibility = Visibility.Visible;

            //Recuperation des infos de l'animal selectionné
            C_ESPECE Espece_Select = Lstbx_Animaux.SelectedItem as C_ESPECE;

            int splitPoint = Espece_Select.description.IndexOf(' ',Espece_Select.description.Length / 2);
            Desc1 = Espece_Select.description.Substring(0,splitPoint).Trim();
            Desc2 = Espece_Select.description.Substring(splitPoint).Trim();

            //Titre de l'animal
            Label_Nom_Animal.Content = Espece_Select.nomCommun;
            Label_Nom_Scientifique_Animal.Content = Espece_Select.nomScientifique;

            //Affichage des informations de l'animal
            Label_Taille.Content = $"{Espece_Select.tailleMin} - {Espece_Select.tailleMax} {Espece_Select.uniteTaille}";
            Label_Poids.Content = $"{Espece_Select.poidsMin} - {Espece_Select.poidsMax} {Espece_Select.unitePoids}";
            Label_Duree_Vie.Content = $"{Espece_Select.dureeVieMin} - {Espece_Select.dureeVieMax} ans";
            Label_Habitat.Text = Espece_Select.habitat;

            //Info complémentaire
            Label_Embranchement.Content = Espece_Select.embranchement;
            Label_Classe.Content = Espece_Select.classe;
            Label_Ordre.Content = Espece_Select.ordre;
            Label_Famille.Content = Espece_Select.famille;

            //Description en bas
            Tbx_Description_Global.Text = Desc1;
            Tbx_Description_Global1.Text = Desc2;
            Text_Info_Pratique.Text = Espece_Select.descPres;
            Text_Critere_Menace.Text = Espece_Select.statutEspece;
            Text_UICN.Text = Espece_Select.descUicn;

            //Critere de danger d'extinction uicn rectangle de couleur 
            switch(Text_Critere_Menace.Text) {
                case "Eteinte (EX)":
                    Txt_UICN_Black.Width = 40;
                    Txt_UICN_Black.Height = 20;
                    break;
                case "Eteinte à l’état sauvage (EW)":
                    Txt_UICN_Red.Width = 40;
                    Txt_UICN_Red.Height = 20; 
                    break;
                case "En danger critique (CR)":
                    Txt_UICN_Orange.Width = 40;
                    Txt_UICN_Orange.Height = 20; 
                    break;
                case "En danger (EN)":
                    Txt_UICN_Yellow.Width = 40;
                    Txt_UICN_Yellow.Height = 20; 
                    break;
                case "Vulnérable (VU)":
                    Txt_UICN_LawnGreen.Width = 40;
                    Txt_UICN_LawnGreen.Height = 20; 
                    break;
                case "Quasi menacée (NT)":
                    Txt_UICN_YellowGreen.Width = 40;
                    Txt_UICN_YellowGreen.Height = 20;
                    break;
                case "Préoccupation mineure (LC)":
                    Txt_UICN_Green.Width = 40;
                    Txt_UICN_Green.Height = 20;
                    break;
            }
        }
        private void Zone_Click_Amerique_Nord(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Amérique du Nord");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Amerique_Sud(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Amérique du Sud");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Afrique(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Afrique");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Europe(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Europe");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Asie(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Asie");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;

        }

        private void Zone_Click_Oceanie(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Océanie");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;

        }

        private void Zone_Click_Antartique(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Antarctique");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void SearchBox_TextChanged(object sender,TextChangedEventArgs e) {

            if(SearchBox.Text.IsNullOrEmpty()) {
                Lstbx_Animaux.Items.Clear();
            }

            var Liste_Animaux_Recuperer = BDD.Get_Espece_By_Name(SearchBox.Text);
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }
    }
}
