using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using LIB_BDD;
using IHM_AJOUT;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IHM_BASE {
    public partial class EDIT_PARCOURS :Window {
        // Instance de la base de données
        C_BDD BDD = new C_BDD();
        private C_PARCOURS selectedParcours;
        private List<C_ESPECE> Especes_Parcours;
        private List<int> Id_Especes_Parcours;
        private string imagePath;
        private string Path;


        public EDIT_PARCOURS() {
            Id_Especes_Parcours = new();
            Especes_Parcours = new();
            InitializeComponent();


            var List_Especes = BDD.Get_All_Especes();
            LB_Animaux.ItemsSource = List_Especes;
            LB_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            LB_Animaux_Select.DisplayMemberPath = nameof(C_ESPECE.nomCommun);

            // Charger tous les parcours dans la ListBox
            LoadParcoursList();
            var Parcours_Select = LB_Parcours.SelectedItem as C_PARCOURS;
            Especes_Parcours = BDD.Get_All_Especes_By_IdParcours(Parcours_Select.idParcours);
            foreach(var espece in Especes_Parcours) {
                Id_Especes_Parcours.Add(espece.idEspece);
            }
            try {
                foreach(C_ESPECE espece in LB_Animaux.Items) {
                    if(espece != null && Id_Especes_Parcours.Contains(espece.idEspece)) {
                        LB_Animaux.SelectedItems.Add(espece);
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}");
            }
        }

        // Charger la liste des parcours dans le ListBox
        private void LoadParcoursList() {
            // Récupérer tous les parcours de la base de données
            var parcoursList = BDD.Get_All_Parcours();

            // Lier les parcours à la ListBox
            LB_Parcours.ItemsSource = parcoursList;
            LB_Parcours.DisplayMemberPath = nameof(C_PARCOURS.nomParcours);
            LB_Parcours.SelectedIndex = 0;
        }

        // Sélectionner un parcours dans la liste
        private void LB_Parcours_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            if(LB_Parcours.SelectedItem is C_PARCOURS parcours) {
                selectedParcours = parcours; // Mise à jour de la variable selectedParcours
            }
            else {
                selectedParcours = null;
            }

            // Réinitialisation de la sélection
            LB_Animaux.SelectedItems.Clear();
            Id_Especes_Parcours.Clear();

            // Charger les espèces associées au parcours sélectionné
            if(selectedParcours != null) {
                Especes_Parcours = BDD.Get_All_Especes_By_IdParcours(selectedParcours.idParcours);

                foreach(var espece in LB_Animaux.Items) {
                    if(espece is C_ESPECE especeItem && Especes_Parcours.Any(e => e.idEspece == especeItem.idEspece)) {
                        LB_Animaux.SelectedItems.Add(especeItem);
                    }
                }

                // Mise à jour des informations du parcours sélectionné
                TB_NomParcours.Text = selectedParcours.nomParcours;
                TB_DescParcours.Text = selectedParcours.descParcours;
                TB_Credits.Text = selectedParcours.credits;
                Check_AfficheParcours.IsChecked = selectedParcours.afficher;

                // Charger l'image si elle existe
                if(File.Exists(selectedParcours.imgPathParcours)) {
                    ImagePreview.Source = new BitmapImage(new Uri(selectedParcours.imgPathParcours));
                    BTN_DeleteImg.IsEnabled = true;
                }
            }
        }

        // Importer une image
        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

                if(openFileDialog.ShowDialog() == true) {
                    imagePath = openFileDialog.FileName;
                    if(ImagePreview.Source == null) {
                        ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                        BTN_DeleteImg.IsEnabled = true;
                        Path = imagePath;
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        // Supprimer l'image
        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            TB_Credits.Text = string.Empty;
            selectedParcours.imgPathParcours = string.Empty;
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
        }

        // Modifier le parcours
        private void BTN_EDIT_PARCOURS_Click(object sender,RoutedEventArgs e) {

            //Verification de si cocher ou non pour l'affichage du parcours
            bool Is_Check;

            if(Check_AfficheParcours.IsChecked == true) {
                Is_Check = true;
            }
            else {
                Is_Check = false;
            }

            if(selectedParcours != null) {
                // Récupérer les nouvelles valeurs depuis les TextBox
                string newNom = TB_NomParcours.Text;
                string newDesc = TB_DescParcours.Text;
                string newCredits = TB_Credits.Text;

                // Mettre à jour le chemin de l'image si une nouvelle image est sélectionnée
                if(!string.IsNullOrEmpty(imagePath)) {
                    selectedParcours.imgPathParcours = imagePath;
                }

                // Créer un objet C_PARCOURS avec les nouvelles valeurs
                C_PARCOURS updatedParcours = new C_PARCOURS {
                    idParcours = selectedParcours.idParcours,
                    nomParcours = newNom,
                    imgPathParcours = selectedParcours.imgPathParcours,
                    credits = newCredits,
                    descParcours = newDesc,
                    afficher = Is_Check
                };

                // Appeler la méthode Edit_Parcours pour mettre à jour le parcours
                BDD.Edit_Parcours(updatedParcours);
                BDD.Edit_Parcours_Especes(updatedParcours.idParcours,Id_Especes_Parcours);
                MessageBox.Show("Parcours modifié");
                Close();
            }
            else {
                MessageBox.Show("Veuillez sélectionner un parcours à modifier");
            }
        }

        // Supprimer un parcours
        private void BTN_SUPPR_Click(object sender,RoutedEventArgs e) {
            var result = MessageBox.Show("Voulez-vous vraiment supprimer ce parcours ?","Confirmation",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes) {

                if(selectedParcours != null) {
                    // Supprimer le parcours de la base de données
                    BDD.Delete_Parcours(selectedParcours.idParcours);
                    LB_Parcours.SelectionChanged -= LB_Parcours_SelectionChanged;
                    var parcoursList = BDD.Get_All_Parcours();
                    LB_Parcours.ItemsSource = parcoursList;
                    LB_Parcours.SelectionChanged += LB_Parcours_SelectionChanged;

                    MessageBox.Show("Parcours supprimé avec succès.");
                }
            }
            else {
                return;
            }
        }

        private void SearchBox_TextChanged(object sender,TextChangedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Espece_By_Name(SearchBox.Text);
            LB_Animaux.ItemsSource = Liste_Animaux_Recuperer;

            try {
                LB_Animaux.SelectionChanged -= LB_Animaux_SelectionChanged; // Désactiver temporairement l'événement

                foreach(C_ESPECE espece in LB_Animaux.Items) {
                    if(espece != null && Id_Especes_Parcours.Contains(espece.idEspece)) {
                        LB_Animaux.SelectedItems.Add(espece);
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}");
            } finally {
                LB_Animaux.SelectionChanged += LB_Animaux_SelectionChanged; // Réactiver l'événement
            }
        }

        private void LB_Animaux_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            foreach(C_ESPECE espece in e.AddedItems) {
                if(!Id_Especes_Parcours.Contains(espece.idEspece)) {
                    Id_Especes_Parcours.Add(espece.idEspece);
                }
                if(!LB_Animaux_Select.Items.Contains(espece)) {
                    LB_Animaux_Select.Items.Add(espece);
                }
            }

            foreach(C_ESPECE espece in e.RemovedItems) {
                Id_Especes_Parcours.Remove(espece.idEspece);
                LB_Animaux_Select.Items.Remove(espece);
            }
        }

        private void BTN_Retour_Click(object sender,RoutedEventArgs e) {
            var result = MessageBox.Show("Voulez-vous annuler ?","Confirmation",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes) {
                Close();
            }
            else {
                return;
            }
        }
    }
}