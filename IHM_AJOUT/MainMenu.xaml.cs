using IHM_AJOUT;
using LIB_BDD;
using System.Windows;
using System.Windows.Controls;

namespace IHM_BASE {

    public partial class Menu :Window {
        private C_BDD BDD = null;
        private List<C_ESPECE> List_Especes;

        public Menu() {
            BDD = new();
            List_Especes = new();
            InitializeComponent();
            var Etat_Connexion = BDD.Test_Connexion();

            if(Etat_Connexion == null) {
                List_Especes = BDD.Get_All_Especes();
            }
            else {
                MessageBox.Show($"La connexion à la base de données a échoué : {Etat_Connexion}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            //Recuperation des especes de la base dans la listbox
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            LB_Especes.SelectedIndex = 0;
        }

        //Button Ajout d'une espèce
        private void BTN_ADD_Click(object sender,RoutedEventArgs e) {
            var addWindow = new ADD();
            bool? result = addWindow.ShowDialog();
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }

        //Button edit d'une espèce
        private void BTN_EDIT_Click(object sender,RoutedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;
            var addWindow = new EDIT(Espece_Select);
            bool? result = addWindow.ShowDialog();
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }

        //Changement de l'espèce sélectionnée
        private void LB_Especes_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;

            if(Espece_Select == null) {
                BTN_ADD.IsEnabled = false;
                BTN_SUPPR.IsEnabled = false;
                BTN_EDIT.IsEnabled = false;
            } else {
                BTN_ADD.IsEnabled = true;
                BTN_EDIT.IsEnabled = true;
                BTN_SUPPR.IsEnabled = true;
            }
        }

        //Button Suppression d'une espèce
        private void BTN_SUPPR_Click(object sender,RoutedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;
            var result = MessageBox.Show(
                    $"Voulez vous vraiment supprimer {Espece_Select.nomCommun}? Cette action est irréversible.",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

            if(result == MessageBoxResult.No) {
                return;
            }

            BDD.Delete_Espece(Espece_Select.idEspece);
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }

        //Button Creation de compte utilisateur
        private void BTN_CREATE_USER_Click(object sender,RoutedEventArgs e) {
            var Deplace = new INSCRIPTION();
            Deplace.Show();
        }

        private void BTN_COUCOU_CLICK(object sender,RoutedEventArgs e) {
            CLIENT Coucou = new();
            Coucou.Show();
            Close();
        }

        private void BTN_PARCOURS_Click(object sender,RoutedEventArgs e) {
            var addWindow = new ADD_PARCOURS();
            bool? result = addWindow.ShowDialog();
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }

        private void BTN_EDIT_PARCOURS_Click(object sender,RoutedEventArgs e) {
            EDIT_PARCOURS Edit_Parcours = new();

            Edit_Parcours.Show();
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }

        private void SearchBox_Espece_TextChanged(object sender,TextChangedEventArgs e) {
            // Récupérer les espèces correspondant au texte de recherche
            var resultatRecherche = BDD.Get_Espece_By_Name(SearchBox_Espece.Text);

            // Mettre à jour la source de la ListBox
            LB_Especes.ItemsSource = resultatRecherche;
        }

        private void MenuItem_Modifier_Click(object sender,RoutedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;
            var addWindow = new EDIT(Espece_Select);
            bool? result = addWindow.ShowDialog();
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }

        private void MenuItem_Supprimer_Click(object sender,RoutedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;
            var result = MessageBox.Show(
                    $"Voulez vous vraiment supprimer {Espece_Select.nomCommun}? Cette action est irréversible.",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

            if(result == MessageBoxResult.No) {
                return;
            }

            BDD.Delete_Espece(Espece_Select.idEspece);
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.SelectedIndex = 0;
        }
    }
}
