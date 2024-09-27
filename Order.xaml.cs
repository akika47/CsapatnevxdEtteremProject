using EtteremProject.Migrations;
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
using System.Linq;

namespace EtteremProject
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : Window
    {
        
        
        public Order()
        {
            InitializeComponent();

            //megnezni hogy admin-e a felhazsnalo
            if (false)
            {
                btnAdmin.Visibility = Visibility.Visible;
            }
        }
        
        private void OpenLoginWindow(object sender, RoutedEventArgs e)
        {
            /*
             * 
             * 
             * 
             * 
             * //RestaurantContext nelkul hivja meg nem biztos hogy jo
             * 
             * 
             * 
             * 
             * 
             * 
             */
            MainWindow ujMain = new MainWindow();
            this.Close();
            ujMain.ShowDialog();
            
        }

        Dictionary<string, int> DishQunatitites = new Dictionary<string, int>();
        private void IncreaseDecrease(object sender, RoutedEventArgs e)
        {
            var gomb = sender as Button;
            var stackpanel = gomb.Parent as StackPanel;
            var label = stackpanel.Children[1] as Label;
            if (gomb.Content.ToString() == "+")
            {
                label.Content = Convert.ToInt32(label.Content) + 1;
                if (!DishQunatitites.ContainsKey(stackpanel.Name))
                {
                    DishQunatitites.Add(stackpanel.Name, 1);
                }
                else
                {
                    DishQunatitites[stackpanel.Name] += 1;
                }

            }
            else if (gomb.Content.ToString() == "-" && label.Content.ToString() != "0")
            {
                label.Content = Convert.ToInt32(label.Content) - 1;

                DishQunatitites[stackpanel.Name] -= 1;
            }

            lbCart.Items.Clear();
            foreach (var item in DishQunatitites) {
                if (item.Value != 0)
                    lbCart.Items.Add($"{item.Key}\t \t{item.Value}");
                else
                    DishQunatitites.Remove(item.Key);
                    
            }
      

        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
