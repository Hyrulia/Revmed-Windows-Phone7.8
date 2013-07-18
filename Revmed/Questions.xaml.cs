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
using System.ComponentModel;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Phone.Tasks;

namespace Revmed
{
    public partial class Questions : PhoneApplicationPage, INotifyPropertyChanged
    {

        private const string ConnectionString = "Data Source = 'appdata:/data.sdf'; File Mode = read only;";
        private IEnumerable<Choice> _choices;
        private Question _currentQuestion;
        private int _currentIdx = 0;
        private float _score = 0.0f;
        private RevmedDataContext context = new RevmedDataContext(ConnectionString);
        private int _objectiveId;
        public string _titleText;
        private bool _isValidate = false;
        public string _questionText;
        public string _commentText;
        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                _titleText = value;
                if(PropertyChanged!= null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TitleText"));
            }
        }
        public string QuestionText
        {
            get
            {
                return _questionText;
            }
            set
            {
                _questionText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("QuestionText"));
            }
        }
        public string CommentText
        {
            get
            {
                return _commentText;
            }
            set
            {
                _commentText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CommentText"));
            }
        }
        public float Score
        {
            get
            {
                
                return _score;
            }
            set
            {
                _score = value;
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ScoreString"));
            }
        }
        public string ScoreString
        {
            get { return Score.ToString("0.00"); }
            set { }
            
        }
     
        public ObservableCollection<Choice> ChoicesCollection
        {
            get;
            set;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public Questions()
        {
            InitializeComponent();
            if (!context.DatabaseExists())
            {
                // create database if it does not exist
                context.CreateDatabase();
            }
            if ((App.Current as App).Mode == 0)
            {
                ScoreLabel.Visibility = System.Windows.Visibility.Collapsed;
                ScoreText.Visibility = System.Windows.Visibility.Collapsed;
            }

            Loaded += new RoutedEventHandler((o, e) =>
            {
                Init();
                Comment.DataContext = this;
                Question.DataContext = this;
                Titlez.DataContext = this;
                ScoreText.DataContext = this;
            
                ChoicesListBox.ItemsSource = ChoicesCollection;
                
            });

            ChoicesListBox.SelectionChanged += new SelectionChangedEventHandler((o, e) =>
            {
                Choice c = (o as ListBox).SelectedItem as Choice;
                if (c != null && !_isValidate)
                {
                    c.Color = "#81C2FF";
                }
            });

            ValiderButton.Click += new RoutedEventHandler((o, e) =>
            {
                calculateScore();

                foreach (Choice c in ChoicesCollection)
                    if (c.Color == "#81C2FF") //blue
                        if (c.State == true)
                        {
                            c.Color = "#ADFF81"; //green
                            c.Image = "/Revmed;component/Images/img_correct.png";
                        }
                        else
                        {
                            c.Color = "#FF9281"; //red
                            c.Image = "/Revmed;component/Images/img_wrong.png";
                        }
                    else
                        if (c.State == true)
                        {
                            c.Color = "#ADFF81"; //red
                            c.Image = "/Revmed;component/Images/img_wrong.png";
                        }

                _isValidate = true;
                ValiderButton.Visibility = System.Windows.Visibility.Collapsed;
                NextButton.Visibility = System.Windows.Visibility.Visible;
                if ((App.Current as App).Mode == 0)
                    CommentBorder.Visibility = System.Windows.Visibility.Visible;
            });

            NextButton.Click += new RoutedEventHandler((o, e) =>
            {
                NextQuestion();
                GetChoices();
                _isValidate = false;
                NextButton.Visibility = System.Windows.Visibility.Collapsed;
                ValiderButton.Visibility = System.Windows.Visibility.Visible;
                CommentBorder.Visibility = System.Windows.Visibility.Collapsed;
            });

        }


        private void GetString(IAsyncResult res)
        {
            string test = Guide.EndShowKeyboardInput(res);
            if (test != null && !test.Equals(""))
            {
                ScoresData s = new ScoresData();
                s.Pseudo = test;
                s.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                s.Score = ScoreString;
                s.Objectif = context.Objectives.Where(x => x.Id == _objectiveId).ToArray()[0].Objective1;
                ScoresData.AddScore(s);
                
                //Share
                ShareStatusTask shareStatusTask = new ShareStatusTask();
                shareStatusTask.Status = "J'ai eu un nouveau score de " + s.Score + "/5 en " + s.Objectif;
                shareStatusTask.Show();

            }

        }

        public void NextQuestion()
        {
            if (_currentIdx >= 5)
            {
                if ((App.Current as App).Mode == 1)
                {
                    Guide.BeginShowKeyboardInput(Microsoft.Xna.Framework.PlayerIndex.One, "Share et Sauvegarder votre score?", "Votre score final est de " + ScoreString + " / 5", 
                        "Unnamed", new AsyncCallback(GetString), null);
       
                }
                else
                    MessageBox.Show("Révision finie!");
                NavigationService.Navigate(new Uri(string.Format("/MainPage.xaml"), 
                    UriKind.Relative));
            }
            Random rand = new Random();
            int toSkip = rand.Next(0, context.Questions.Count());
            _currentQuestion = context.Questions.Skip(toSkip).Take(1).First();
            TitleText = "Question N°" + (++_currentIdx) + "/5";
            QuestionText = "Question: \n" + _currentQuestion.Question1;
            CommentText = "Commentaire: \n"+ _currentQuestion.Revision;
        }

        public void Init()
        {
            int i = 0;
            string[] abc = new string[]{ "A. ", "B. ", "C. ", "D. ", "E. " };
            NextQuestion();
            
            _choices = context.Choices.Where(x => x.Question_id == _currentQuestion.Id);
            foreach (Choice c in _choices)
            {
                c.Color = "Black";
                c.Choice1 = abc[i++] + c.Choice1;
            }
            ChoicesCollection = new ObservableCollection<Choice>(_choices);
        }

        public void GetChoices()
        {
            int i = 0;
            string[] abc = new string[] { "A. ", "B. ", "C. ", "D. ", "E. " };
            _choices = context.Choices.Where(x => x.Question_id == _currentQuestion.Id);
            ChoicesCollection.Clear();
            foreach (Choice c in _choices)
            {
                c.Color = "Black";
                c.Choice1 = abc[i++] + c.Choice1;
                ChoicesCollection.Add(c);
            }
        }

        public void calculateScore()
        {
            int totalCorrect = 0;
            foreach(Choice c in _choices)
                if(c.State == true)
                    totalCorrect++;
		int correct = 0;
		int wrong = 0;
		foreach(Choice c in _choices) {
            if (c.Color == "#81C2FF")
				if(c.State == true)
					correct++;
				else
					wrong++;			
		}
		Score += (float)( correct * (1 / (float) totalCorrect) ) * ( 1 / (float)(wrong + 1) );
	}

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string id = this.NavigationContext.QueryString["ObjId"];
            _objectiveId = int.Parse(id);
        }


    }
}