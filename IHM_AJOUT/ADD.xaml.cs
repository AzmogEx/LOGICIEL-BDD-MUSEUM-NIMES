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
        private string imagePath;
        private List<string> ListPath = new();

        public ADD() {
            BDD = new();
            InitializeComponent();
        }

        private void ImportImage_Click(object sender,RoutedEventArgs e) {
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

                if(openFileDialog.ShowDialog() == true) {
                    imagePath = openFileDialog.FileName;
                    ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                    ListPath.Add(imagePath);
                }
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }

        }

        private void Image_Drop(object sender,DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(files.Length > 0) {
                    imagePath = files[0];
                    ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                }
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

                C_ESPECE Espece = new C_ESPECE() {
                    nomCommun = TB_Nom.Text,
                    nomScient = TB_NomScient.Text,
                    statutEspece = CB_Statut.Text,
                    taille = $"De {TB_TailleMin.Text} à {TB_TailleMax.Text} {CB_Unite_Taille.Text}",
                    poids = $"De {TB_PoidsMin.Text} à {TB_PoidsMax.Text} {CB_Unite_Poids.Text}",
                    dureeVie = $"De {TB_AgeMin.Text} à {TB_AgeMax.Text} ans",
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

                BDD.Add_Espece(Espece, ListPath);

                MessageBox.Show("L'espèce a été ajoutée avec succès.","Succès",MessageBoxButton.OK,MessageBoxImage.Information);
            } catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}\n{ex.StackTrace}","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

    }
}