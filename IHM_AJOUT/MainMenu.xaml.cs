using IHM_AJOUT;
using LIB_BDD;
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

namespace IHM_BASE {

    public partial class MainMenu:Window {
        private C_BDD BDD = null;
        private string[] imagePaths;
        private string Path;
        private string Path1;
        private string Path2;
        private List<string> ListPath;
        private List<C_ESPECE> List_Especes;

        public MainMenu() {
            BDD = new();
            ListPath = new();
            List_Especes = new();
            InitializeComponent();

            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
            LB_Especes.DisplayMemberPath = nameof(C_ESPECE.nomCommun);
            LB_Especes.SelectedIndex = 0;
        }

        private void BTN_ADD_Click(object sender,RoutedEventArgs e) {
            var addWindow = new ADD();
            addWindow.ShowDialog();
        }

        private void BTN_EDIT_Click(object sender,RoutedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;
            var addWindow = new EDIT(Espece_Select);
            addWindow.ShowDialog();
        }

        private void LB_Especes_SelectionChanged(object sender,SelectionChangedEventArgs e) {
            C_ESPECE Espece_Select = LB_Especes.SelectedItem as C_ESPECE;
        }

        private void BTN_REFRESH_Click(object sender,RoutedEventArgs e) {
            List_Especes = BDD.Get_All_Especes();
            LB_Especes.ItemsSource = List_Especes;
        }
    }
}
