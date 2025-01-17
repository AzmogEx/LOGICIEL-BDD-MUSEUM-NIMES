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

    public partial class EDIT:Window {
        C_BDD BDD = null;
        List<string> ListPath = null;
        private string[] imagePaths;
        private string Path;
        private string Path1;
        private string Path2;
        private int IdEspece;

        public EDIT(C_ESPECE Espece) {
            BDD = new();
            ListPath = new();
            IdEspece = Espece.idEspece;
            InitializeComponent();
            imagePaths = BDD.Get_Img_By_ID(Espece.idEspece);
            
            TB_Nom.Text = Espece.nomCommun;
            TB_NomScient.Text = Espece.nomScientifique;
            CB_Statut.Text = Espece.statutEspece;
            TB_Taille.Text = Espece.taille;
            TB_Poids.Text = Espece.poids;
            TB_DureeVie.Text = Espece.dureeVie;
            TB_Habitat.Text = Espece.habitat;
            TB_Embranchement.Text = Espece.embranchement;
            TB_Classe.Text = Espece.classe;
            TB_Ordre.Text = Espece.ordre;
            TB_Famille.Text = Espece.famille;
            TB_Desc.Text = Espece.description;
            TB_DescUICN.Text = Espece.descUicn;
            TB_DescPres.Text = Espece.descPres;

            try {
                foreach(var imagePath in imagePaths) {
                    if(!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath)) {
                        var uri = new Uri(imagePath,UriKind.Absolute);

                        if(ImagePreview.Source == null) {
                            ImagePreview.Source = new BitmapImage(uri);
                            BTN_DeleteImg.IsEnabled = true;
                            Path = imagePath;
                        } else if(ImagePreview1.Source == null) {
                            ImagePreview1.Source = new BitmapImage(uri);
                            BTN_DeleteImg1.IsEnabled = true;
                            Path1 = imagePath;
                        } else if(ImagePreview2.Source == null) {
                            ImagePreview2.Source = new BitmapImage(uri);
                            BTN_DeleteImg2.IsEnabled = true;
                            Path2 = imagePath;
                        }
                    } else { 
                        MessageBox.Show($"Une ou plusieurs images n'ont pas pu être chargées correctement", "Erreur", MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                }
            } catch(Exception ex) {

                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);

            }

        }

        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                    Multiselect = true
                };
                if(openFileDialog.ShowDialog() == true) {
                    imagePaths = openFileDialog.FileNames;
                    foreach(var imagePath in imagePaths) {
                        if(ImagePreview.Source == null) {
                            ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                            BTN_DeleteImg.IsEnabled = true;
                            Path = imagePath;
                        } else {
                            if(ImagePreview1.Source == null) {
                                ImagePreview1.Source = new BitmapImage(new Uri(imagePath));
                                BTN_DeleteImg1.IsEnabled = true;
                                Path1 = imagePath;
                            } else {
                                ImagePreview2.Source = new BitmapImage(new Uri(imagePath));
                                BTN_DeleteImg2.IsEnabled = true;
                                Path2 = imagePath;
                            }
                        }
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void BTN_Edit_Click(object sender,RoutedEventArgs e) {

            var result = MessageBox.Show(
                    "Veuillez vérifier toutes les informations avant de continuer.\n\n" +
                    "Souhaitez-vous modifier cette espèce ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

            if(result == MessageBoxResult.No) {
                return;
            }

            if(Path != null) {
                ListPath.Add(Path);
            }

            if(Path1 != null) {
                ListPath.Add(Path1);
            }

            if(Path2 != null) {
                ListPath.Add(Path2);
            }

            C_ESPECE Espece = new C_ESPECE() {
                idEspece = IdEspece,
                nomCommun = TB_Nom.Text,
                nomScientifique = TB_NomScient.Text,
                statutEspece = CB_Statut.Text,
                taille = TB_Taille.Text,
                poids = TB_Poids.Text,
                dureeVie = TB_DureeVie.Text,
                habitat = TB_Habitat.Text,
                embranchement = TB_Embranchement.Text,
                classe = TB_Classe.Text,
                ordre = TB_Ordre.Text,
                famille = TB_Famille.Text,
                description = TB_Desc.Text,
                descUicn = TB_DescUICN.Text,
                descPres = TB_DescPres.Text,
                numInventaire = TB_NumInv.Text
            };

            BDD.Edit_Espece(Espece,ListPath);

            Close();
        }

        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
            Path = null;
        }

        private void BTN_DeleteImg1_Click(object sender,RoutedEventArgs e) {
            ImagePreview1.Source = null;
            BTN_DeleteImg1.IsEnabled = false;
            Path1 = null;
        }

        private void BTN_DeleteImg2_Click(object sender,RoutedEventArgs e) {
            ImagePreview2.Source = null;
            BTN_DeleteImg2.IsEnabled = false;
            Path2 = null;
        }
    }
}
