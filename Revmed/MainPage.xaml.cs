using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

namespace Revmed
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructeur

      
        public MainPage()
        {
            InitializeComponent();
          
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
           
            (App.Current as App).Mode = 1; //Mode Exam
            NavigationService.Navigate(new Uri(string.Format("/Specialities.xaml"), UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).Mode = 0; //Mode revision
            NavigationService.Navigate(new Uri(string.Format("/Specialities.xaml"), UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri(string.Format("/Chart.xaml"), UriKind.Relative));
            NavigationService.Navigate(new Uri(string.Format("/Scores.xaml"), UriKind.Relative));
        }
    }
}