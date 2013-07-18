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
using System.Collections.ObjectModel;

namespace Revmed
{
    public partial class Specialities : PhoneApplicationPage
    {

        private const string ConnectionString = "Data Source = 'appdata:/data.sdf'; File Mode = read only;";
        private RevmedDataContext context = new RevmedDataContext(ConnectionString);
        public ObservableCollection<Speciality> SpecialtiesCollection
        {
            get;
            set;
        }


        public Specialities()
        {
            InitializeComponent();
            if (!context.DatabaseExists())
            {
                // create database if it does not exist
                context.CreateDatabase();
            }

            Loaded += new RoutedEventHandler((o, e) =>
            {
                GetSpecialities();
                SpecialityListBox.ItemsSource = SpecialtiesCollection;

            });

            SpecialityListBox.SelectionChanged += new SelectionChangedEventHandler((o, e) =>
            {
                Speciality spec = (o as ListBox).SelectedItem as Speciality;
                if(spec != null)
                    NavigationService.Navigate(new Uri(string.Format("/Objectives.xaml?SpecId={0}",
                        spec.Id), UriKind.Relative));
            }
            );
        }


        public void GetSpecialities()
        {
            SpecialtiesCollection = new ObservableCollection<Speciality>(
                context.Specialities.OrderBy(x => x.Speciality1)
             );
        }




    }
}