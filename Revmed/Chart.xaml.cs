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
using System.ComponentModel;

namespace Revmed
{
    public partial class Chart : PhoneApplicationPage, INotifyPropertyChanged
    {
        private List<ChartData> list = new List<ChartData>();
        public string CurrentPseudo
        {
            get
            {
                return _currentpseudo;
            }
            set
            {
                _currentpseudo = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentPseudo"));
            }
        }
        List<ScoresData> _scores;
        private string _currentpseudo;
        public Chart()
        {
            _scores = ScoresData.GetScores();
            InitializeComponent();
            Loaded += new RoutedEventHandler((o, e) =>
            {
                int counter = 1;
                foreach (ScoresData data in _scores)
                {

                    if (data.Pseudo.Equals(_currentpseudo))
                    {
                        list.Add(new ChartData() { Score = Double.Parse(data.Score), Tries = counter++ });
                    }
                }
                CurrentPseudo = "Pseudo: " + _currentpseudo;
                Plot.DataSource = list;
                DataContext = this;
            });
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _currentpseudo = this.NavigationContext.QueryString["pseudo"];
            
        }

        public class ChartData
        {
            
            public double Tries { get; set; }
            public double Score { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}