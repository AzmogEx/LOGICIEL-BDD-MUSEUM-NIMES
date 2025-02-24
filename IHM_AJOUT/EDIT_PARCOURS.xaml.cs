using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using LIB_BDD;
using IHM_AJOUT;
using System.Windows.Controls;
//test
namespace IHM_BASE {
    public partial class EDIT_PARCOURS :Window {
        // Instance de la base de données
        C_BDD BDD = new C_BDD();
        private C_PARCOURS selectedParcours;
        private List<C_ESPECE> Especes_Parcours;
        private List<int> Id_Especes_Parcours;
        private string[] cheminImage;
        private string Path;
        private readonly string imageDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"IMAGES");


        public EDIT_PARCOURS() {
            Id_Especes_Parcours = new();
            Especes_Parcours = new();
            InitializeComponent();

            // Assurer que le dossier pour les images existe
            if(!Directory.Exists(imageDirectory)) {
                Directory.CreateDirectory(imageDirectory);
            }

            var List_Especes = BDD.Get_All_Especes();
            LB_Animaux.ItemsSource = List_Especes;
            LB_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);

            LoadParcoursList();
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

        private void LoadParcoursImage(string imageFileName) {
            if(!string.IsNullOrEmpty(imageFileName)) {
                string imagePath = System.IO.Path.Combine(imageDirectory,imageFileName);
                if(File.Exists(imagePath)) {
                    ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                    BTN_DeleteImg.IsEnabled = true;
                }
            }
        }

        // Sélectionner un parcours dans la liste
        private void LB_Parcours_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            if(LB_Parcours.SelectedItem != null) {
                selectedParcours = (C_PARCOURS)LB_Parcours.SelectedItem;

                TB_NomParcours.Text = selectedParcours.nomParcours;
                TB_DescParcours.Text = selectedParcours.descParcours;

                // Charger l'image
                LoadParcoursImage(selectedParcours.imgPathParcours);
            }
        }

        // Importer une image
        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if(openFileDialog.ShowDialog() == true) {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                string destinationPath = System.IO.Path.Combine(imageDirectory,fileName);

                try {
                    File.Copy(openFileDialog.FileName,destinationPath,true);
                    ImagePreview.Source = new BitmapImage(new Uri(destinationPath));
                    BTN_DeleteImg.IsEnabled = true;

                    // Sauvegarde uniquement du nom du fichier
                    selectedParcours.imgPathParcours = fileName;
                }
                catch(Exception ex) {
                    MessageBox.Show($"Erreur lors de l'importation de l'image : {ex.Message}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                }
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
            if(string.IsNullOrWhiteSpace(TB_NomParcours.Text) || string.IsNullOrWhiteSpace(TB_DescParcours.Text)) {
                MessageBox.Show("Veuillez remplir tous les champs avant de modifier le parcours.");
                return;
            }
            else if(selectedParcours != null) {
                selectedParcours.nomParcours = TB_NomParcours.Text;
                selectedParcours.descParcours = TB_DescParcours.Text;

                BDD.Edit_Parcours(selectedParcours);
                BDD.Edit_Parcours_Especes(selectedParcours.idParcours,Id_Especes_Parcours);
                MessageBox.Show("Parcours modifié avec succès.");
            }
        }

        // Supprimer un parcours
        private void BTN_SUPPR_Click(object sender,RoutedEventArgs e) {
            selectedParcours = LB_Parcours.SelectedItem as C_PARCOURS;
            if(selectedParcours != null) {
                // Supprimer le parcours de la base de données
                BDD.Delete_Parcours(selectedParcours.idParcours);
                var parcoursList = BDD.Get_All_Parcours();
                LB_Parcours.SelectionChanged -= LB_Parcours_SelectionChanged;
                LB_Parcours.ItemsSource = parcoursList;
                LB_Parcours.SelectionChanged += LB_Parcours_SelectionChanged;

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
            foreach(C_ESPECE espece in e.AddedItems) {
                int especeId = espece.idEspece; // Assuming C_ESPECE has an Id property of type int
                if(!Id_Especes_Parcours.Contains(especeId)) {
                    Id_Especes_Parcours.Add(especeId);
                }
            }
            foreach(C_ESPECE espece in e.RemovedItems) {
                int especeId = espece.idEspece; // Assuming C_ESPECE has an Id property of type int
                if(Id_Especes_Parcours.Contains(especeId)) {
                    Id_Especes_Parcours.Remove(especeId);
                }
            }
        }

    }
}