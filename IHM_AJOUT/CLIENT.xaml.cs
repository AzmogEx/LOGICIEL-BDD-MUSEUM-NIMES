using Dapper;
using LIB_BDD;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IHM_BASE {
    /// <summary>
    /// Logique d'interaction pour CLIENT.xaml
    /// </summary>
    public partial class CLIENT:Window {
        private C_BDD BDD = null;
        private string Desc1;
        private string Desc2;
        private List<C_ESPECE> EspecesParcours;
        private bool canClose = false;

        public CLIENT() {
            InitializeComponent();

            BDD = new();
            List<C_ESPECE> List_Especes = new();
            EspecesParcours = new();

            KeyboardHook.Start();
            this.Closing += (s,e) => KeyboardHook.Stop();

            InitialiserConnexion();

            //Recuperation des especes de la base dans la listbox
            //Lstbx_Animaux.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            //Lstbx_Animaux.ItemsSource = List_Especes;

            Grid_Info.Visibility = Visibility.Hidden;
            Border_Info.Visibility = Visibility.Hidden;

            Grid_Parcours.Visibility = Visibility.Visible;
            Border_Parcours.Visibility = Visibility.Visible;
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            if(!canClose) {
                e.Cancel = true; // Bloque la fermeture de la fenêtre
            }
            base.OnClosing(e);
        }

        private void InitialiserConnexion() {
            var Etat_Connexion = BDD.Test_Connexion();
            if(Etat_Connexion == null) {
                ChargerEspeces();
                ChargerParcours();
            } else {
                MessageBox.Show($"La connexion à la base de données a échoué : {Etat_Connexion}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void ChargerEspeces() {
            try {
                var List_Especes = BDD.Get_All_Especes();
                Lstbx_Animaux.ItemsSource = List_Especes;
            } catch {
                MessageBox.Show("Aucune espèce n'est récupérer");
            }
        }

        private void ChargerParcours() {
            try {
                var List_Parcours = BDD.Get_All_Parcours_Affichable();
                List_Parcours.Add(new C_PARCOURS() { idParcours = 0,nomParcours = "Carte",imgPathParcours = "RESSOURCES/world.png",TextButton = "Voir la carte" });
                ParcoursList.ItemsSource = List_Parcours;
            } catch {
                MessageBox.Show("Aucun parcours n'est affichable");
            }
        }

        private void Button_Close_Click(object sender,RoutedEventArgs e) {
            Grid_Info.Visibility = Visibility.Hidden;
            Border_Info.Visibility = Visibility.Hidden;
            Button_Close.Visibility = Visibility.Collapsed;

            Txt_UICN_Black.Width = 20;
            Txt_UICN_Black.Height = 10;

            Txt_UICN_Red.Width = 20;
            Txt_UICN_Red.Height = 10;

            Txt_UICN_Orange.Width = 20;
            Txt_UICN_Orange.Height = 10;

            Txt_UICN_Yellow.Width = 20;
            Txt_UICN_Yellow.Height = 10;

            Txt_UICN_LawnGreen.Width = 20;
            Txt_UICN_LawnGreen.Height = 10;

            Txt_UICN_YellowGreen.Width = 20;
            Txt_UICN_YellowGreen.Height = 10;

            Txt_UICN_Green.Width = 20;
            Txt_UICN_Green.Height = 10;
            Img_Animal_1.Source = null;
            Img_Animal_2.Source = null;
            Img_Animal_3.Source = null;

            Label_Credits.Content = string.Empty;
            Label_Credits2.Content = string.Empty;
            Label_Credits3.Content = string.Empty;

            Grid_Especes_Parcours.Visibility = Visibility.Visible;
            Border_Especes.Visibility = Visibility.Visible;
        }

        private void Show_Map() {
            Grid_Parcours.Visibility = Visibility.Hidden;
            Border_Parcours.Visibility = Visibility.Hidden;
            Grid_Recherche.Visibility = Visibility.Visible;
            Border_Recherche_Animal.Visibility = Visibility.Visible;
        }

        private void Lstbx_Animaux_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
            Grid_Recherche.Visibility = Visibility.Hidden;
            Border_Recherche_Animal.Visibility = Visibility.Hidden;
            BorderDescParcours.Visibility = Visibility.Hidden;

            Grid_Info.Visibility = Visibility.Visible;
            Border_Info.Visibility = Visibility.Visible;
            Button_Back_Carte.Visibility = Visibility.Visible;

            // Récupération des infos de l'animal sélectionné
            C_ESPECE Espece_Select = Lstbx_Animaux.SelectedItem as C_ESPECE;
            List<C_IMAGE> Images = new();

            if(Espece_Select != null) {
                Images = BDD.Get_Img_By_ID(Espece_Select.idEspece);
                try {
                    foreach(var imagePath in Images) {
                        if(!string.IsNullOrWhiteSpace(imagePath.ImgPath) && File.Exists(imagePath.ImgPath)) {
                            var uri = new Uri(imagePath.ImgPath,UriKind.Absolute);

                            if(Img_Animal_1.Source == null) {
                                Img_Animal_1.Source = new BitmapImage(uri);
                                Label_Credits.Content = imagePath.Credits;
                            } else if(Img_Animal_2.Source == null) {
                                Img_Animal_2.Source = new BitmapImage(uri);
                                Label_Credits2.Content = imagePath.Credits;
                            } else if(Img_Animal_3.Source == null) {
                                Img_Animal_3.Source = new BitmapImage(uri);
                                Label_Credits3.Content = imagePath.Credits;
                            }
                        }
                    }
                } catch(Exception) {
                    MessageBox.Show("Erreur sur le chargement des images");
                }

                if(!string.IsNullOrEmpty(Espece_Select.description)) {
                    int splitPoint = Espece_Select.description.IndexOf(' ',Espece_Select.description.Length / 2);
                    Desc1 = Espece_Select.description.Substring(0,splitPoint).Trim();
                    Desc2 = Espece_Select.description.Substring(splitPoint).Trim();
                } else {
                    Desc1 = " ";
                    Desc2 = " ";
                }

                // Titre de l'animal
                Label_Nom_Animal.Content = Espece_Select.nomCommun;
                Label_Nom_Scientifique_Animal.Content = Espece_Select.nomScientifique;

                Label_Taille.Content = $"{Espece_Select.tailleMin} - {Espece_Select.tailleMax} {Espece_Select.uniteTaille}";
                Label_Poids.Content = $"{Espece_Select.poidsMin} - {Espece_Select.poidsMax} {Espece_Select.unitePoids}";
                Label_Duree_Vie.Content = $"{Espece_Select.dureeVieMin} - {Espece_Select.dureeVieMax} {Espece_Select.uniteVie}";
                Label_Habitat.Text = Espece_Select.habitat;

                // Infos complémentaires
                Label_Embranchement.Content = Espece_Select.embranchement;
                Label_Classe.Content = Espece_Select.classe;
                Label_Ordre.Content = Espece_Select.ordre;
                Label_Famille.Content = Espece_Select.famille;

                // Description en bas
                Tbx_Description_Global.Text = Desc1;
                Tbx_Description_Global1.Text = Desc2;
                Text_Info_Pratique.Text = Espece_Select.descPres;
                Text_Critere_Menace.Text = Espece_Select.statutEspece;
                Text_UICN.Text = Espece_Select.descUicn;
                Text_DescParcours.Text = Espece_Select.descParcours;

                // Critère de danger d'extinction UICN rectangle de couleur
                switch(Text_Critere_Menace.Text) {
                    case "Eteinte (EX)":
                        Txt_UICN_Black.Width = 40;
                        Txt_UICN_Black.Height = 20;
                        break;
                    case "Eteinte à l’état sauvage (EW)":
                        Txt_UICN_Red.Width = 40;
                        Txt_UICN_Red.Height = 20;
                        break;
                    case "En danger critique (CR)":
                        Txt_UICN_Orange.Width = 40;
                        Txt_UICN_Orange.Height = 20;
                        break;
                    case "En danger (EN)":
                        Txt_UICN_Yellow.Width = 40;
                        Txt_UICN_Yellow.Height = 20;
                        break;
                    case "Vulnérable (VU)":
                        Txt_UICN_LawnGreen.Width = 40;
                        Txt_UICN_LawnGreen.Height = 20;
                        break;
                    case "Quasi menacée (NT)":
                        Txt_UICN_YellowGreen.Width = 40;
                        Txt_UICN_YellowGreen.Height = 20;
                        break;
                    case "Préoccupation mineure (LC)":
                        Txt_UICN_Green.Width = 40;
                        Txt_UICN_Green.Height = 20;
                        break;
                }
            }
        }


        private void Zone_Click_Amerique_Nord(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Amérique du Nord");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Amerique_Sud(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Amérique du Sud");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Afrique(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Afrique");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Europe(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Europe");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Asie(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Asie");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;

        }

        private void Zone_Click_Oceanie(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Océanie");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Zone_Click_Antartique(object sender,RoutedEventArgs e) {
            var Liste_Animaux_Recuperer = BDD.Get_Especes_By_Region("Antarctique");
            Lstbx_Animaux.ItemsSource = Liste_Animaux_Recuperer;
        }

        private void Btn_Admin_Click(object sender,RoutedEventArgs e) {
            Menu mainMenu = new Menu();
            mainMenu.Show();
            canClose = true; // Autorise la fermeture
            Close();
        }

        private void Btn_Open_Parcours_Click(object sender,RoutedEventArgs e) {
            var button = sender as Button;
            EspecesParcours = new();
            if(button != null) {
                var parcours = button.DataContext as C_PARCOURS;

                if(parcours.idParcours == 0) {
                    Show_Map();
                } else {
                    if(parcours != null) {
                        int parcoursId = parcours.idParcours;
                        var BackgroundColor = parcours.colorBg;
                        var TextColor = parcours.textColor;
                        try {
                            Border_Especes.Background = (Brush)new BrushConverter().ConvertFromString(BackgroundColor);
                            Label_Nom_Parcours.Foreground = (Brush)new BrushConverter().ConvertFromString(TextColor);
                            Label_Description_Parcours.Foreground = (Brush)new BrushConverter().ConvertFromString(TextColor);

                        } catch {
                            Border_Especes.Background = Brushes.Transparent; // Valeur par défaut en cas d'erreur
                            Label_Nom_Parcours.Foreground = Brushes.Transparent;
                            Label_Description_Parcours.Foreground = Brushes.Transparent;
                        }
                        // Affichage du titre et de la description du parcours
                        Label_Nom_Parcours.Content = $"Parcours {parcours.nomParcours}";
                        Label_Description_Parcours.Content = parcours.descParcours;

                        // Récupérer les espèces associées au parcours
                        var especes = BDD.Get_All_Especes_By_IdParcours(parcoursId);

                        // Créer une liste combinée d'espèces et d'images
                        List<C_ESPECES_PARCOURS> Especes_Parcours = new();

                        foreach(var espece in especes) {
                            // Récupérer l'image associée à l'espèce
                            var image = BDD.Get_Img_By_ID(espece.idEspece)?.FirstOrDefault();
                            EspecesParcours.Add(espece);

                            if(image != null) {


                                Especes_Parcours.Add(new C_ESPECES_PARCOURS {
                                    NomCommun = espece.nomCommun,
                                    NomScientifique = espece.nomScientifique,
                                    StatutEspece = espece.statutEspece,
                                    ImgPath = image.ImgPath,
                                    CardColor = parcours.cardColor,
                                    Credits = image.Credits,
                                    TextColor = parcours.textColor
                                });
                            }
                        }

                        // Lier la liste combinée au ItemsControl
                        EspecesList.ItemsSource = Especes_Parcours;

                        // Basculer l'affichage
                        Grid_Especes_Parcours.Visibility = Visibility.Visible;
                        Border_Especes.Visibility = Visibility.Visible;

                        Grid_Parcours.Visibility = Visibility.Hidden;
                        Border_Parcours.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void Btn_Open_Espece_Click(object sender,RoutedEventArgs e) {
            var button = sender as Button; // Récupère le bouton cliqué
            C_ESPECE Espece_Select;
            List<C_IMAGE> Images = new();
            if(button != null) {
                // Récupère l'espèce associée au bouton
                var especes = button.DataContext as C_ESPECES_PARCOURS;

                if(especes != null) {
                    // Récupération des données de l'espèce sélectionnée
                    foreach(var Espece in EspecesParcours) {

                        if(especes.NomScientifique == Espece.nomScientifique) {
                            Espece_Select = Espece;
                            Images = BDD.Get_Img_By_ID(Espece_Select.idEspece);

                            try {
                                foreach(var imagePath in Images) {
                                    if(!string.IsNullOrWhiteSpace(imagePath.ImgPath) && File.Exists(imagePath.ImgPath)) {
                                        var uri = new Uri(imagePath.ImgPath,UriKind.Absolute);

                                        if(Img_Animal_1.Source == null) {
                                            Img_Animal_1.Source = new BitmapImage(uri);
                                            Label_Credits.Content = imagePath.Credits;
                                        } else if(Img_Animal_2.Source == null) {
                                            Img_Animal_2.Source = new BitmapImage(uri);
                                            Label_Credits2.Content = imagePath.Credits;
                                        } else if(Img_Animal_3.Source == null) {
                                            Img_Animal_3.Source = new BitmapImage(uri);
                                            Label_Credits3.Content = imagePath.Credits;

                                        }
                                    }
                                }
                            } catch(Exception) {

                                MessageBox.Show("Erreur sur le chargement des images");
                            }

                            if(Espece_Select.description != string.Empty) {
                                int splitPoint = Espece_Select.description.IndexOf(' ',Espece_Select.description.Length / 2);
                                Desc1 = Espece_Select.description.Substring(0,splitPoint).Trim();
                                Desc2 = Espece_Select.description.Substring(splitPoint).Trim();
                            } else {
                                Desc1 = " ";
                                Desc2 = " ";
                            }

                            //Titre de l'animal
                            Label_Nom_Animal.Content = Espece_Select.nomCommun;
                            Label_Nom_Scientifique_Animal.Content = Espece_Select.nomScientifique;

                            //Affichage des informations de l'animal
                            Label_Taille.Content = $"{Espece_Select.tailleMin} - {Espece_Select.tailleMax} {Espece_Select.uniteTaille}";
                            Label_Poids.Content = $"{Espece_Select.poidsMin} - {Espece_Select.poidsMax} {Espece_Select.unitePoids}";
                            Label_Duree_Vie.Content = $"{Espece_Select.dureeVieMin} - {Espece_Select.dureeVieMax} {Espece_Select.uniteVie}";
                            Label_Habitat.Text = Espece_Select.habitat;

                            //Info complémentaire
                            Label_Embranchement.Content = Espece_Select.embranchement;
                            Label_Classe.Content = Espece_Select.classe;
                            Label_Ordre.Content = Espece_Select.ordre;
                            Label_Famille.Content = Espece_Select.famille;

                            //Description en bas
                            Tbx_Description_Global.Text = Desc1;
                            Tbx_Description_Global1.Text = Desc2;
                            Text_Info_Pratique.Text = Espece_Select.descPres;
                            Text_Critere_Menace.Text = Espece_Select.statutEspece;
                            Text_UICN.Text = Espece_Select.descUicn;
                            Text_DescParcours.Text = Espece_Select.descParcours;

                            //Critere de danger d'extinction uicn rectangle de couleur 
                            switch(Text_Critere_Menace.Text) {
                                case "Eteinte (EX)":
                                    Txt_UICN_Black.Width = 40;
                                    Txt_UICN_Black.Height = 20;
                                    break;
                                case "Eteinte à l’état sauvage (EW)":
                                    Txt_UICN_Red.Width = 40;
                                    Txt_UICN_Red.Height = 20;
                                    break;
                                case "En danger critique (CR)":
                                    Txt_UICN_Orange.Width = 40;
                                    Txt_UICN_Orange.Height = 20;
                                    break;
                                case "En danger (EN)":
                                    Txt_UICN_Yellow.Width = 40;
                                    Txt_UICN_Yellow.Height = 20;
                                    break;
                                case "Vulnérable (VU)":
                                    Txt_UICN_LawnGreen.Width = 40;
                                    Txt_UICN_LawnGreen.Height = 20;
                                    break;
                                case "Quasi menacée (NT)":
                                    Txt_UICN_YellowGreen.Width = 40;
                                    Txt_UICN_YellowGreen.Height = 20;
                                    break;
                                case "Préoccupation mineure (LC)":
                                    Txt_UICN_Green.Width = 40;
                                    Txt_UICN_Green.Height = 20;
                                    break;
                            }
                            Grid_Info.Visibility = Visibility.Visible;
                            Border_Info.Visibility = Visibility.Visible;
                            Button_Close.Visibility = Visibility.Visible;

                            Grid_Especes_Parcours.Visibility = Visibility.Hidden;
                            Border_Especes.Visibility = Visibility.Hidden;
                            return;
                        }
                    }
                }
            }
        }


        private void Button_Close2_Click(object sender,RoutedEventArgs e) {
            Grid_Especes_Parcours.Visibility = Visibility.Hidden;
            Border_Especes.Visibility = Visibility.Hidden;
            Border_Recherche_Animal.Visibility = Visibility.Hidden;
            Grid_Recherche.Visibility = Visibility.Hidden;

            Grid_Parcours.Visibility = Visibility.Visible;
            Border_Parcours.Visibility = Visibility.Visible;
        }

        private void Button_Back_Carte_Click(object sender,RoutedEventArgs e) {
            Border_Info.Visibility = Visibility.Hidden;
            Grid_Info.Visibility = Visibility.Hidden;
            Button_Back_Carte.Visibility = Visibility.Collapsed;

            Txt_UICN_Black.Width = 20;
            Txt_UICN_Black.Height = 10;

            Txt_UICN_Red.Width = 20;
            Txt_UICN_Red.Height = 10;

            Txt_UICN_Orange.Width = 20;
            Txt_UICN_Orange.Height = 10;

            Txt_UICN_Yellow.Width = 20;
            Txt_UICN_Yellow.Height = 10;

            Txt_UICN_LawnGreen.Width = 20;
            Txt_UICN_LawnGreen.Height = 10;

            Txt_UICN_YellowGreen.Width = 20;
            Txt_UICN_YellowGreen.Height = 10;

            Txt_UICN_Green.Width = 20;
            Txt_UICN_Green.Height = 10;
            Img_Animal_1.Source = null;
            Img_Animal_2.Source = null;
            Img_Animal_3.Source = null;

            Label_Credits.Content = string.Empty;
            Label_Credits2.Content = string.Empty;
            Label_Credits3.Content = string.Empty;

            Border_Recherche_Animal.Visibility = Visibility.Visible;
            Grid_Recherche.Visibility = Visibility.Visible;
            BorderDescParcours.Visibility = Visibility.Visible;
        }
    }
}
