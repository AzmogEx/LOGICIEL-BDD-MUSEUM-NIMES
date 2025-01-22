using LIB_BDD;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace IHM_AJOUT {
    public partial class ADD:Window {
        private C_BDD BDD = null;
        private string[] imagePaths;
        private string Path;
        private string Path1;
        private string Path2;
        private List<string> ListPath;
        private List<string> Regions;
        private C_ESPECE Espece;

        public ADD() {
            BDD = new();
            ListPath = new();
            Regions = new();
            InitializeComponent();
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

        private void BTN_Add_Click(object sender,RoutedEventArgs e) {
            try {
                if(string.IsNullOrEmpty(TB_Nom.Text) || string.IsNullOrEmpty(TB_NomScient.Text)) {
                    MessageBox.Show("Le nom commun et le nom scientifique sont obligatoires.","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                if(!decimal.TryParse(TB_TailleMin.Text,out _) || !decimal.TryParse(TB_TailleMax.Text,out _)) {
                    MessageBox.Show("Veuillez entrer des valeurs numériques valides pour la taille.","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                if(!decimal.TryParse(TB_PoidsMin.Text,out _) || !decimal.TryParse(TB_PoidsMax.Text,out _)) {
                    MessageBox.Show("Veuillez entrer des valeurs numériques valides pour le poids.","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                if(!int.TryParse(TB_AgeMin.Text,out _) || !int.TryParse(TB_AgeMax.Text,out _)) {
                    MessageBox.Show("Veuillez entrer des valeurs numériques valides pour la durée de vie.","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    "Veuillez vérifier toutes les informations avant de continuer.\n\n" +
                    "Souhaitez-vous ajouter cette espèce ?",
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

                try {
                    int.TryParse(TB_TailleMin.Text,out int TailleMin);
                    int.TryParse(TB_TailleMax.Text,out int TailleMax);
                    try {
                        int.TryParse(TB_PoidsMin.Text,out int PoidsMin);
                        int.TryParse(TB_PoidsMax.Text,out int PoidsMax);
                        try {
                            int.TryParse(TB_AgeMin.Text,out int DureeVieMin);
                            int.TryParse(TB_AgeMax.Text,out int DureeVieMax);

                            Espece = new C_ESPECE() {
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
                                numInventaire = TB_NumInv.Text
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


                BDD.Add_Espece(Espece, ListPath, Regions);
                ListPath = new();
                MessageBox.Show("L'espèce a été ajoutée avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
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

        private void LB_Region_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            foreach(var item in e.AddedItems) {
                string region = item.ToString();
                if(!Regions.Contains(region)) {
                    Regions.Add(region);
                }
            }

            foreach(var item in e.RemovedItems) {
                string region = item.ToString();
                if(Regions.Contains(region)) {
                    Regions.Remove(region);
                }
            }
        }

    }
}