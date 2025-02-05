using LIB_BDD;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;


namespace IHM_BASE {
    /// <summary>
    /// Logique d'interaction pour LOGIN.xaml
    /// </summary>
    public partial class LOGIN :Window {
        private DispatcherTimer _timer;
        public LOGIN() {
            InitializeComponent();

            // Initialisation du Timer
            _timer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(30) // Définir l'intervalle à 30 secondes
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Button_Connect_Click(object sender,RoutedEventArgs e) {
            C_BDD La_Base = new C_BDD();
            
            try {
                var Ok = La_Base.Connexion(Tbx_User.Text,Tbx_Password.Password);
                if(Ok == true) {
                    _timer.Stop(); // Arrêter le Timer à la connexion réussie
                    MessageBox.Show("Vous êtes connecté");
                    var deplace = new Menu();
                    deplace.Show();
                    this.Close();
                }
                else {
                    MessageBox.Show("Le mots de passe ou le nom d'utilisateur est incorrecte !");
                }

            }
            catch(Exception ex) {
                MessageBox.Show($"Une erreur est survenue: {ex}");
            }
        }

        private void Timer_Tick(object sender,EventArgs e) {
            _timer.Stop(); // Arrête le Timer

            // Redirection vers la page client
            CLIENT clientPage = new CLIENT();
            clientPage.Show();
            this.Close();
        }
    }
}
