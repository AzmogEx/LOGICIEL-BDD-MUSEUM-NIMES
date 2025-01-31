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
        List<C_IMAGE> ListPath = null;
        private List<C_IMAGE> imagePaths = null;
        private string[] imagePath;
        private string Path;
        private string Path1;
        private string Path2;
        private int IDEspece;
        private List<string> Regions;
        private string[] GetRegions;
        private C_ESPECE P_Espece;

        public EDIT(C_ESPECE Espece) {
            BDD = new();
            ListPath = new();
            IDEspece = Espece.idEspece;
            Regions = new();
            InitializeComponent();

            //Alimentation de la comboBox des parcours
            CB_PARCOURS.ItemsSource = BDD.Get_All_Parcours();
            CB_PARCOURS.DisplayMemberPath = nameof(C_PARCOURS.nomParcours);

            //Alimentation de la listBox des régions
            imagePaths = BDD.Get_Img_By_ID(IDEspece);
            GetRegions = BDD.Get_Region_By_ID(IDEspece);

            //Alimentation des informations de l'espèce
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
            C_PARCOURS Parcours = BDD.Get_Parcours_By_ID(Espece.idParcours);
            if(Parcours!=null) {
                CB_PARCOURS.Text = Parcours.nomParcours;
            }

            try {
                LB_Region.SelectionChanged -= LB_Region_SelectionChanged; // Désactiver temporairement l'événement

                foreach(var item in LB_Region.Items) {
                    if(item != null && GetRegions.Contains(item.ToString())) {
                        LB_Region.SelectedItems.Add(item);
                        Regions.Add(item.ToString());
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}");
            } finally {
                LB_Region.SelectionChanged += LB_Region_SelectionChanged; // Réactiver l'événement
            }

            try {
                foreach(var imagePath in imagePaths) {
                    if(!string.IsNullOrWhiteSpace(imagePath.ImgPath) && File.Exists(imagePath.ImgPath)) {
                        var uri = new Uri(imagePath.ImgPath,UriKind.Absolute);

                        if(ImagePreview.Source == null) {
                            ImagePreview.Source = new BitmapImage(uri);
                            BTN_DeleteImg.IsEnabled = true;
                            Path = imagePath.ImgPath;
                        } else if(ImagePreview1.Source == null) {
                            ImagePreview1.Source = new BitmapImage(uri);
                            BTN_DeleteImg1.IsEnabled = true;
                            Path1 = imagePath.ImgPath;
                        } else if(ImagePreview2.Source == null) {
                            ImagePreview2.Source = new BitmapImage(uri);
                            BTN_DeleteImg2.IsEnabled = true;
                            Path2 = imagePath.ImgPath;
                        }
                    } else if(!string.IsNullOrWhiteSpace(imagePath.ImgPath)) {
                        MessageBox.Show($"Une ou plusieurs images n'ont pas pu être chargées correctement","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                }

            } catch(Exception ex) {

                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);

            }

        }

        //Glisser déposer d'image
        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog() {Multiselect=true};
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

        private void BTN_Edit_Click(object sender,RoutedEventArgs e) {

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
                    "Souhaitez-vous modifier cette espèce ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

            if(result == MessageBoxResult.No) {
                return;
            }

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

            var Item_Selectionnee = CB_PARCOURS.SelectedItem as C_PARCOURS;

            try {
                BDD.Edit_Espece(P_Espece,ListPath, Regions,Item_Selectionnee);
            } catch(Exception ex) {
                MessageBox.Show($"Erreur lors de la modification de l'espèce : {ex.Message}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("L'espèce a été modifiée avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);

            Close();
        }

        //Premier boutton de suppression d'image
        private void BTN_DeleteImg_Click(object sender,RoutedEventArgs e) {
            ImagePreview.Source = null;
            BTN_DeleteImg.IsEnabled = false;
            Path = null;
        }

        //Deuxieme boutton de suppression d'image
        private void BTN_DeleteImg1_Click(object sender,RoutedEventArgs e) {
            ImagePreview1.Source = null;
            BTN_DeleteImg1.IsEnabled = false;
            Path1 = null;
        }

        //Troisieme boutton de suppression d'image
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
