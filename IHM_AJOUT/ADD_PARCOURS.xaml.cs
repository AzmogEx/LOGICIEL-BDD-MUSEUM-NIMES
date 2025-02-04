using LIB_BDD;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

    public partial class ADD_PARCOURS:Window {
        private C_BDD BDD = null;
        private string imagePath;
        private string Path;
        private int Nb_Parcours;
        private List<int> Id_Especes_Parcours;

        public ADD_PARCOURS() {
            BDD = new C_BDD();
            Id_Especes_Parcours = new();
            InitializeComponent();


            var List_Especes = BDD.Get_All_Especes();
            LB_Animaux.ItemsSource = List_Especes;
            LB_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            LB_Animaux_Select.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
        }


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

        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            TB_Credits.Text = string.Empty;
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
        }

        private void BTN_AJOUT_Click(object sender,RoutedEventArgs e) {

            //Verification de si cocher ou non pour l'affichage du parcours
            bool Is_Check;

            if(Check_AfficheParcours.IsChecked == true) {
                Is_Check = true;
            }
            else {
                Is_Check = false;
            }

            try {
                var Parcours = new C_PARCOURS() {
                    nomParcours = TB_NomParcours.Text,
                    descParcours = TB_DescParcours.Text,
                    credits = TB_Credits.Text,
                    afficher = Is_Check,
                    imgPathParcours = Path
                };
                if(Nb_Parcours >= 24) {
                    MessageBox.Show("Veuillez supprimer un parcours avant d'en rajouter davantage. (Vous ne pouvez pas créer plus de 24 parcours.)","Échec",MessageBoxButton.OK,MessageBoxImage.Information);
                } else {
                    BDD.Create_Parcours(Parcours, Id_Especes_Parcours);
                    Nb_Parcours++;
                    MessageBox.Show($"Le parcours '{Parcours.nomParcours}' a été ajouté avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);
                    Close();
                } 
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }

        private void LB_Animaux_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            if(LB_Animaux.SelectedItems.Count > 5) {
                foreach(C_ESPECE espece in e.AddedItems) {
                    LB_Animaux.SelectedItems.Remove(espece);
                }

                MessageBox.Show("Vous ne pouvez pas sélectionner plus de 10 espèces.","Limite de sélection",MessageBoxButton.OK,MessageBoxImage.Warning);
            } else {
                foreach(C_ESPECE espece in e.AddedItems) {
                    int especeId = espece.idEspece;
                    if(!Id_Especes_Parcours.Contains(especeId)) {
                        Id_Especes_Parcours.Add(especeId);
                        if(!LB_Animaux_Select.Items.Contains(espece)) {
                            LB_Animaux_Select.Items.Add(espece);
                        }
                    }
                }
                foreach(C_ESPECE espece in e.RemovedItems) {
                    int especeId = espece.idEspece;
                    if(Id_Especes_Parcours.Contains(especeId)) {
                        Id_Especes_Parcours.Remove(especeId);
                        LB_Animaux_Select.Items.Remove(espece);
                    }
                }
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
