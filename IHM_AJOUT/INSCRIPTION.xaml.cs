using System;
using System.Windows;
using LIB_BDD;

namespace IHM_BASE {
    public partial class INSCRIPTION :Window {
        C_BDD maBDD = new C_BDD();

        public INSCRIPTION() {
            InitializeComponent();
        }

        private void Button_Create_Click(object sender,RoutedEventArgs e) {
            string nomUtilisateur = Tbx_User.Text.Trim();
            string Confirmation = Tbx_Confirm.Password.Trim();
            string motDePasse = Tbx_Password.Password.Trim();
            if(Confirmation == motDePasse) {
                if(!string.IsNullOrEmpty(nomUtilisateur) && !string.IsNullOrEmpty(motDePasse)) {
                    try {
                        maBDD.Add_User(nomUtilisateur,motDePasse);
                        MessageBox.Show("Compte créé avec succès !");
                        // Rediriger ou vider les champs, selon ton besoin
                        Tbx_User.Clear();
                        Tbx_Confirm.Clear();
                        Tbx_Password.Clear();
                    }
                    catch(Exception ex) {
                        MessageBox.Show("Une erreur est survenue : " + ex.Message);
                    }
                }
                else {
                    MessageBox.Show("Veuillez remplir tous les champs !");
                }
            }
            else {
                MessageBox.Show("Les deux champs de mots de passe sont différent !");
            }
        }
    }
}