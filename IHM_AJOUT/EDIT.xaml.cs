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
        private List<C_IMAGE> images;
        private string[] imagePaths;
        private byte[] Image;
        private byte[] Image1;
        private byte[] Image2;
        private string Path;
        private string Path1;
        private string Path2;
        private int IDEspece;
        private C_ESPECE P_Espece;

        public EDIT(C_ESPECE Espece) {
            BDD = new();
            ListPath = new();
            IDEspece = Espece.idEspece;
            InitializeComponent();
            images = BDD.Get_Img_By_ID(Espece.idEspece);
            
            TB_Nom.Text = Espece.nomCommun;
            TB_NomScient.Text = Espece.nomScientifique;
            CB_Statut.Text = Espece.statutEspece;
            TB_TailleMin.Text = Espece.tailleMin.ToString();
            TB_TailleMax.Text = Espece.tailleMax.ToString();
            CB_Unite_Taille.Text = Espece.uniteTaille;
            TB_PoidsMin.Text = Espece.poidsMin.ToString();
            TB_PoidsMax.Text = Espece.poidsMax.ToString();
            CB_Unite_Poids.Text = Espece.unitePoids;
            TB_AgeMin.Text = Espece.dureeVieMin.ToString();
            TB_AgeMax.Text = Espece.dureeVieMax.ToString();
            TB_Habitat.Text = Espece.habitat;
            TB_Embranchement.Text = Espece.embranchement;
            TB_Classe.Text = Espece.classe;
            TB_Ordre.Text = Espece.ordre;
            TB_Famille.Text = Espece.famille;
            TB_Desc.Text = Espece.description;
            TB_DescUICN.Text = Espece.descUicn;
            TB_DescPres.Text = Espece.descPres;
            TB_NumInv.Text = Espece.numInventaire;

            try {
                foreach(var image in images) // imagePaths est maintenant un byte[][]
                {
                    if(image.ImgData != null && image.ImgData.Length > 0) {
                        // Convertir les données binaires en image
                        using(var ms = new MemoryStream(image.ImgData)) {
                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();

                            // Affecter l'image au premier contrôle disponible
                            if(ImagePreview.Source == null) {
                                ImagePreview.Source = bitmapImage;
                                BTN_DeleteImg.IsEnabled = true;
                                Path = image.ImgPath;
                                ListPath.Add(Path);
                            } else if(ImagePreview1.Source == null) {
                                ImagePreview1.Source = bitmapImage;
                                BTN_DeleteImg1.IsEnabled = true;
                                Path1 = image.ImgPath;
                                ListPath.Add(Path1);
                            } else if(ImagePreview2.Source == null) {
                                ImagePreview2.Source = bitmapImage;
                                BTN_DeleteImg2.IsEnabled = true;
                                Path2 = image.ImgPath;
                                ListPath.Add(Path2);
                            }
                        }
                    } else {
                        MessageBox.Show($"Une ou plusieurs images n'ont pas pu être chargées correctement","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
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

            try {
                int.TryParse(TB_TailleMin.Text,out int TailleMin);
                int.TryParse(TB_TailleMax.Text,out int TailleMax);
                try {
                    int.TryParse(TB_PoidsMin.Text,out int PoidsMin);
                    int.TryParse(TB_PoidsMax.Text,out int PoidsMax);
                    try {
                        int.TryParse(TB_AgeMin.Text,out int DureeVieMin);
                        int.TryParse(TB_AgeMax.Text,out int DureeVieMax);

                        P_Espece = new C_ESPECE() {
                            nomCommun = TB_Nom.Text,
                            nomScientifique = TB_NomScient.Text,
                            statutEspece = CB_Statut.Text,
                            tailleMin = TailleMin,
                            tailleMax = TailleMax,
                            uniteTaille = CB_Unite_Taille.Text,
                            poidsMin = PoidsMin,
                            poidsMax = PoidsMax,
                            unitePoids = CB_Unite_Poids.Text,
                            dureeVieMin = DureeVieMin,
                            dureeVieMax = DureeVieMax,
                            habitat = TB_Habitat.Text,
                            embranchement = TB_Embranchement.Text,
                            classe = TB_Classe.Text,
                            ordre = TB_Ordre.Text,
                            famille = TB_Famille.Text,
                            description = TB_Desc.Text,
                            descUicn = TB_DescUICN.Text,
                            descPres = TB_DescPres.Text,
                            numInventaire = TB_NumInv.Text,
                            idEspece = IDEspece
                        };

                    } catch(Exception) {
                        MessageBox.Show("Erreur : Veuillez vérifier les données entrées sur la longévité");
                    }
                } catch(Exception) {
                    MessageBox.Show("Erreur : Veuillez vérifier les données entrées sur le poids");
                }
            } catch(Exception) {
                MessageBox.Show("Erreur : Veuillez vérifier les données entrées sur la taille");
            }

            try {
                BDD.Edit_Espece(P_Espece,ListPath);
            } catch(Exception ex) {
                MessageBox.Show($"Erreur lors de la modification de l'espèce : {ex.Message}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("L'espèce a été modifiée avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);

            Close();
        }

        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
            ListPath.Remove(Path);
            Path = null;
        }

        private void BTN_DeleteImg1_Click(object sender,RoutedEventArgs e) {
            ImagePreview1.Source = null;
            BTN_DeleteImg1.IsEnabled = false;
            ListPath.Remove(Path1);
            Path1 = null;
        }

        private void BTN_DeleteImg2_Click(object sender,RoutedEventArgs e) {
            ImagePreview2.Source = null;
            BTN_DeleteImg2.IsEnabled = false;
            ListPath.Remove(Path2);
            Path2 = null;
        }
    }
}
