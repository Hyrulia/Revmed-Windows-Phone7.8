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
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Phone.Tasks;

namespace Revmed
{
    public partial class Scores : PhoneApplicationPage
    {

        public Scores()
        {
            InitializeComponent();
            List<ScoresData> list = ScoresData.GetScores();
            list.ForEach(x => { x.Pseudo += " a eu "; x.Score += "/5 le "; });
            ScoresListBox.ItemsSource = list;
            ScoresListBox.SelectionChanged += new SelectionChangedEventHandler((o, e) =>
            {
                ScoresData obj = (o as ListBox).SelectedItem as ScoresData;
                if (obj != null)
                    NavigationService.Navigate(new Uri(string.Format("/Chart.xaml?pseudo={0}",
                        obj.RealPseudo), UriKind.Relative));
            }
             );
        }


    }
}