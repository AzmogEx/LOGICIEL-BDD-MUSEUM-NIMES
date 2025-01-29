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
        private string imgPath;
        private string Path;
        private int Nb_Parcours;

        public ADD_PARCOURS() {
            BDD = new C_BDD();
            InitializeComponent();


            var List_Especes = BDD.Get_All_Especes();
            LB_Animaux.ItemsSource = List_Especes;
            LB_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
        }


        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                };
                if(openFileDialog.ShowDialog() == true) {
                    imgPath = openFileDialog.FileName;
                    Path = imgPath;
                    if(ImagePreview.Source == null) {
                        ImagePreview.Source = new BitmapImage(new Uri(imgPath));
                        BTN_DeleteImg.IsEnabled = true;
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
        }

        private void BTN_AJOUT_Click(object sender,RoutedEventArgs e) {
            try {
                var Parcours = new C_PARCOURS() {
                    nomParcours = TB_NomParcours.Text,
                    descParcours = TB_DescParcours.Text,
                    imgPathParcours = Path
                };
                if(Nb_Parcours >= 24) { 
                    MessageBox.Show("Veuillez supprimer un parcours avant d'en rajouter davantage. (Vous ne pouvez pas créer plus de 24 parcours.)","Échec",MessageBoxButton.OK,MessageBoxImage.Information);
                } else {
                    BDD.Create_Parcours(Parcours);
                    Nb_Parcours++;
                    MessageBox.Show($"Le parcours '{Parcours.nomParcours}' a été ajouté avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);
                    Close();
                } 
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }

        private void SearchBox_TextChanged(object sender,TextChangedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Espece_By_Name(SearchBox.Text);
            LB_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }
    }
}
