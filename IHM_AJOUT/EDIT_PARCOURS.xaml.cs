using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using LIB_BDD;

namespace IHM_BASE {
    public partial class EDIT_PARCOURS :Window {
        // Instance de la base de données
        C_BDD La_Base = new C_BDD();
        private C_PARCOURS selectedParcours;

        public EDIT_PARCOURS() {
            InitializeComponent();

            // Charger tous les parcours dans la ListBox
            LoadParcoursList();
        }

        // Charger la liste des parcours dans le ListBox
        private void LoadParcoursList() {
            // Récupérer tous les parcours de la base de données
            var parcoursList = La_Base.Get_All_Parcours();

            // Lier les parcours à la ListBox
            LB_Parcours.ItemsSource = parcoursList;
            LB_Parcours.DisplayMemberPath = nameof(C_PARCOURS.nomParcours);
        }

        // Sélectionner un parcours dans la liste
        private void LB_Parcours_SelectionChanged(object sender,System.Windows.Controls.SelectionChangedEventArgs e) {
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
                La_Base.Edit_Parcours(updatedParcours);
            }
        }

        // Supprimer un parcours
        private void BTN_SUPPR_Click(object sender,RoutedEventArgs e) {
            if(selectedParcours != null) {
                // Supprimer le parcours de la base de données
                La_Base.Delete_Parcours(selectedParcours.idParcours);
                MessageBox.Show("Delete Reussi");
            }
        }
    }
}