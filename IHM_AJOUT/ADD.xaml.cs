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
        private string[] imagePath;
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
                OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
                openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

                if(openFileDialog.ShowDialog() == true) {
                    imagePath = openFileDialog.FileNames;
                    foreach(var imagePath in imagePath) {
                        if(ImagePreview.Source == null) {
                            ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                            BTN_DeleteImg.IsEnabled = true;
                            Path = imagePath;
                        }
                        else {
                            if(ImagePreview1.Source == null) {
                                ImagePreview1.Source = new BitmapImage(new Uri(imagePath));
                                BTN_DeleteImg1.IsEnabled = true;
                                Path1 = imagePath;
                            }
                            else {
                                ImagePreview2.Source = new BitmapImage(new Uri(imagePath));
                                BTN_DeleteImg2.IsEnabled = true;
                                Path2 = imagePath;
                            }
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
                C_PARCOURS Selected_Parcours = CB_PARCOURS.SelectedItem as C_PARCOURS;
                var Nb_Espece_Parcours = BDD.Get_All_Especes_By_IdParcours(Selected_Parcours.idParcours);
                if(Nb_Espece_Parcours.Count > 9) {
                    MessageBox.Show("Vous ne pouvez pas ajouter une espèce supplémentaire dans le parcours sélectionné (limite maximum : 10)","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                //Vérifications que les champs ne contiennent pas de valeurs inappropriées 
                if(string.IsNullOrEmpty(TB_Nom.Text) || string.IsNullOrEmpty(TB_NomScient.Text)) {
                    MessageBox.Show("Le nom commun et le nom scientifique sont obligatoires.","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                if(!float.TryParse(TB_TailleMin.Text,out _) || !float.TryParse(TB_TailleMax.Text,out _)) {
                    MessageBox.Show("Veuillez entrer des valeurs numériques valides pour la taille.","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }

                if(!float.TryParse(TB_PoidsMin.Text,out _) || !float.TryParse(TB_PoidsMax.Text,out _)) {
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
                if(Path1 == null) { Path1 = " "; }
                if(Path2 == null) { Path2 = " "; }

                ListPath.Add(new C_IMAGE() { ImgPath = Path, Credits = TB_Credits.Text});
                ListPath.Add(new C_IMAGE() { ImgPath = Path1, Credits = TB_Credits2.Text });
                ListPath.Add(new C_IMAGE() { ImgPath = Path2, Credits = TB_Credits3.Text });

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
                                uniteVie = CB_Unite_Date.Text,
                                habitat = TB_Habitat.Text,
                                embranchement = TB_Embranchement.Text,
                                classe = TB_Classe.Text,
                                ordre = TB_Ordre.Text,
                                famille = TB_Famille.Text,
                                description = TB_Desc.Text,
                                descUicn = TB_DescUICN.Text,
                                descPres = TB_DescPres.Text,
                                descParcours = TB_DescParcours.Text,
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
            TB_Credits.Text = string.Empty;
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
            Path = null;
        }

        private void BTN_DeleteImg1_Click(object sender,RoutedEventArgs e) {
            TB_Credits2.Text = string.Empty;
            ImagePreview1.Source = null;
            BTN_DeleteImg1.IsEnabled = false;
            Path1 = null;
        }

        private void BTN_DeleteImg2_Click(object sender,RoutedEventArgs e) {
            TB_Credits3.Text = string.Empty;
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