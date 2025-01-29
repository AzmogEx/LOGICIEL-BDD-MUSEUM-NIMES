using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using LIB_BDD;
using IHM_AJOUT;
using System.Windows.Controls;

namespace IHM_BASE {
    public partial class EDIT_PARCOURS :Window {
        // Instance de la base de données
        C_BDD BDD = new C_BDD();
        private C_PARCOURS selectedParcours;
        private List<C_ESPECE> Especes_Parcours;
        private List<int> Id_Especes_Parcours;


        public EDIT_PARCOURS() {
            Id_Especes_Parcours = new();
            Especes_Parcours = new();
            InitializeComponent();


            var List_Especes = BDD.Get_All_Especes();
            LB_Animaux.ItemsSource = List_Especes;
            LB_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);

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
        private void LB_Parcours_SelectionChanged(object sender,System.Windows.Controls.SelectionChangedEventArgs e) {
            var Parcours_Select = LB_Parcours.SelectedItem as C_PARCOURS;
            Id_Especes_Parcours.Clear();
            LB_Animaux.SelectedItems.Clear();
            Especes_Parcours = BDD.Get_All_Especes_By_IdParcours(Parcours_Select.idParcours);
            foreach(var espece in Especes_Parcours) {
                Id_Especes_Parcours.Add(espece.idEspece);
            }

            if(LB_Parcours.SelectedItem != null) {
                selectedParcours = (C_PARCOURS)LB_Parcours.SelectedItem;

                // Afficher les informations du parcours sélectionné
                TB_NomParcours.Text = selectedParcours.nomParcours;
                TB_DescParcours.Text = selectedParcours.descParcours;

                // Charger l'image si elle existe
                if(File.Exists(selectedParcours.imgPathParcours)) {
                    ImagePreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(selectedParcours.imgPathParcours));
                    BTN_DeleteImg.IsEnabled = true;
                }
            }
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

        // Importer une image
        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true) {
                selectedParcours.imgPathParcours = openFileDialog.FileName;
                ImagePreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(selectedParcours.imgPathParcours));
                BTN_DeleteImg.IsEnabled = true;
            }
        }

        // Supprimer l'image
        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            selectedParcours.imgPathParcours = string.Empty;
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
        }

        // Modifier le parcours
        private void BTN_EDIT_PARCOURS_Click(object sender,RoutedEventArgs e) {
            if(selectedParcours != null) {
                // Récupérer les nouvelles valeurs depuis les TextBox
                string newNom = TB_NomParcours.Text;
                string newDesc = TB_DescParcours.Text;

                // Créer un objet C_PARCOURS avec les nouvelles valeurs
                C_PARCOURS updatedParcours = new C_PARCOURS {
                    idParcours = selectedParcours.idParcours,
                    nomParcours = newNom,
                    imgPathParcours = selectedParcours.imgPathParcours,  // Garder l'ancien chemin d'image
                    descParcours = newDesc
                };

                // Appeler la méthode Edit_Parcours pour mettre à jour le parcours
                BDD.Edit_Parcours(updatedParcours);
                BDD.Edit_Parcours_Especes(updatedParcours.idParcours,Id_Especes_Parcours);
                MessageBox.Show("Parcours modifié");
            }
        }

        // Supprimer un parcours
        private void BTN_SUPPR_Click(object sender,RoutedEventArgs e) {
            if(selectedParcours != null) {
                // Supprimer le parcours de la base de données
                BDD.Delete_Parcours(selectedParcours.idParcours);
                MessageBox.Show("Parcours supprimé avec succès.");
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
            foreach(C_ESPECE espece in e.AddedItems.Cast<C_ESPECE>()) {
                if(!Id_Especes_Parcours.Contains(espece.idEspece)) // Utilise espece.idEspece
                {
                    Id_Especes_Parcours.Add(espece.idEspece);
                }
            }

            foreach(C_ESPECE espece in e.RemovedItems.Cast<C_ESPECE>()) {
                if(Id_Especes_Parcours.Contains(espece.idEspece)) // Utilise espece.idEspece
                {
                    Id_Especes_Parcours.Remove(espece.idEspece);
                }
            }
        }

    }
}