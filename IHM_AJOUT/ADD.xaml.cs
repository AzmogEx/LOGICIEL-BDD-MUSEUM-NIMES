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
        private List<C_IMAGE> ListPath;
        private List<string> Regions;
        private C_ESPECE Espece;

        public ADD() {
            BDD = new();
            ListPath = new();
            Regions = new();

            InitializeComponent();

            //Alimentation de la comboBox des parcours
            CB_PARCOURS.ItemsSource = BDD.Get_All_Parcours();
            CB_PARCOURS.DisplayMemberPath = nameof(C_PARCOURS.nomParcours);
        }

        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            try {
                // Configuration du dialogue d'importation
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                    Multiselect = true
                };

                // Répertoire cible pour les images importées
                string imageDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"IMAGES");

                // Crée le répertoire s'il n'existe pas
                if(!Directory.Exists(imageDirectory)) {
                    Directory.CreateDirectory(imageDirectory);
                }

                if(openFileDialog.ShowDialog() == true) {
                    imagePaths = openFileDialog.FileNames;

                    foreach(var imagePath in imagePaths) {
                        string fileName = System.IO.Path.GetFileName(imagePath);
                        string destinationPath = System.IO.Path.Combine(imageDirectory,fileName);
                        // Copie du fichier dans le dossier Resources/Images
                        File.Copy(imagePath,destinationPath,true);

                        // Mise à jour des aperçus et stockage des chemins
                        if(ImagePreview.Source == null) {
                            ImagePreview.Source = new BitmapImage(new Uri(destinationPath));
                            BTN_DeleteImg.IsEnabled = true;
                            Path = fileName;
                        }
                        else if(ImagePreview1.Source == null) {
                            ImagePreview1.Source = new BitmapImage(new Uri(destinationPath));
                            BTN_DeleteImg1.IsEnabled = true;
                            Path1 = fileName;
                        }
                        else {
                            ImagePreview2.Source = new BitmapImage(new Uri(destinationPath));
                            BTN_DeleteImg2.IsEnabled = true;
                            Path2 = fileName;
                        }
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        
        private void BTN_Add_Click(object sender,RoutedEventArgs e) {
            try {
                //Vérifications que les champs ne contiennent pas de valeurs inappropriées 
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

                //si des images sont chargées, et donc que les chemins ne sont pas nuls, alors on les sauvegarde dans une liste

                if(Path == null) { Path = " "; }
                if(Path == null) { Path1 = " "; }
                if(Path == null) { Path2 = " "; }

                ListPath.Add(new C_IMAGE() { ImgPath = Path });
                ListPath.Add(new C_IMAGE() { ImgPath = Path1 });
                ListPath.Add(new C_IMAGE() { ImgPath = Path2 });

                try {
                    float.TryParse(TB_TailleMin.Text,out float TailleMin);
                    float.TryParse(TB_TailleMax.Text,out float TailleMax);
                    try {
                        float.TryParse(TB_PoidsMin.Text,out float PoidsMin);
                        float.TryParse(TB_PoidsMax.Text,out float PoidsMax);
                        try {
                            int.TryParse(TB_AgeMin.Text,out int DureeVieMin);
                            int.TryParse(TB_AgeMax.Text,out int DureeVieMax);

                            //initialisation du type C_ESPECE avec les données rentrées par l'utilisateur
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

                var Parcours_Selectionnee = CB_PARCOURS.SelectedItem as C_PARCOURS; //permet de lié un parcours à une espèce

                BDD.Add_Espece(Espece, ListPath, Regions, Parcours_Selectionnee);
                MessageBox.Show("L'espèce a été ajoutée avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);
                Close();
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        //supprimer une image chargée précédemment
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


        //assignation d'une ou plusieurs régions à une espèce
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