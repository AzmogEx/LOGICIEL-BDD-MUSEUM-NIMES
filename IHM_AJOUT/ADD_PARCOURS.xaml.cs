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

        public ADD_PARCOURS() {
            BDD = new C_BDD();
            InitializeComponent();

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
                BDD.Create_Parcours(Parcours);
                MessageBox.Show($"Le parcours '{Parcours.nomParcours}' a été ajouté avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);
                Close();
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }

        private void LB_Parcours_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            C_PARCOURS Parcours_Select = LB_Parcours.SelectedItem as C_PARCOURS;
        }

        private void BTN_SUPPR_Click(object sender,RoutedEventArgs e) {
            C_PARCOURS Parcours_Select = LB_Parcours.SelectedItem as C_PARCOURS;
            var result = MessageBox.Show(
                    $"Voulez vous vraiment supprimer {Parcours_Select.nomParcours}? Cette action est irréversible.",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

            if(result == MessageBoxResult.No) {
                return;
            }

            BDD.Delete_Espece(Parcours_Select.idEspece);
            var List_Parcours = BDD.Get_All_Especes();
            LB_Parcours.ItemsSource = List_Parcours;
            LB_Parcours.SelectedIndex = 0;
        }
    }
}
