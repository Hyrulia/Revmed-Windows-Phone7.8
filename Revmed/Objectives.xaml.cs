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
    public partial class Objectives : PhoneApplicationPage
    {

        private const string ConnectionString = "Data Source = 'appdata:/data.sdf'; File Mode = read only;";
        private RevmedDataContext context = new RevmedDataContext(ConnectionString);
        private int? _specId;
        public ObservableCollection<Objective> ObjectivesCollection
        {
            get;
            set;
        }
        public Objectives()
        {
            InitializeComponent();
            if (!context.DatabaseExists())
            {
                // create database if it does not exist
                context.CreateDatabase();
            }

            Loaded += new RoutedEventHandler((o, e) =>
            {
                GetObjectives();
                ObjectivesListBox.ItemsSource = ObjectivesCollection;

            });

            ObjectivesListBox.SelectionChanged += new SelectionChangedEventHandler((o, e) =>
            {
                Objective obj = (o as ListBox).SelectedItem as Objective;
                if (obj != null)
                    NavigationService.Navigate(new Uri(string.Format("/Questions.xaml?ObjId={0}",
                        obj.Id), UriKind.Relative));
            }
            );

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string id = this.NavigationContext.QueryString["SpecId"];
            _specId = int.Parse(id);
        }

        public void GetObjectives()
        {
            ObjectivesCollection = new ObservableCollection<Objective>(
                context.Objectives.Where(x => x.Speciality_id == _specId).OrderBy(x => x.Objective1)
            );
        }
    }
}