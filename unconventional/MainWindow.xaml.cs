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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace unconventional
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region init
        Events eve = new Events();
        Schedule sched = new Schedule();

        private static Brush navColour = Brushes.White;
        private static Brush navSelectColour = Brushes.LightGray;

        public MainWindow()
        {
            InitializeComponent();
            
            
            
            this.NavBar.NavToMap.Click += NavToMap_Click;
            this.NavBar.NavToEvents.Click += NavToEvents_Click;
            this.NavBar.NavToSchedule.Click += NavToSchedule_Click;
            this.NavBar.NavToSettings.Click += NavToSettings_Click;
            this.NavBar.NavToNews.Click += NavToNews_Click;
            this.NavBar.NavToSocial.Click += NavToSocial_Click;
            SetInitialVisibilities();
            this.main_frame.Navigate(new News()); // loads mocked news interface
            ResetButtonColours(); // resets colour
            this.NavBar.NavToNews.Background = navSelectColour; // set button colour
            eve.EventClick += new EventHandler(NavEventDetails);
            sched.EventClick += new EventHandler(NavEventDetails);
        }

        private void NavEventDetails(object sender, EventArgs e)
        {
            this.main_frame.Navigate(new EventDetail());
            ResetButtonColours();
        }

        #endregion
        
        #region click handlers

        private void NavToMap_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(new Map()); // need to use a stored 'map' if we want persisted changes
            ResetButtonColours();
            this.NavBar.NavToMap.Background = navSelectColour;
        }
		
        private void NavToSettings_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(new Settings()); // need to use a stored 'map' if we want persisted changes
            ResetButtonColours(); // resets colour
            this.NavBar.NavToSettings.Background = navSelectColour; // highlight button
		}
		
        private void NavToNews_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(new News()); // loads mocked news interface
            ResetButtonColours(); // resets colour
            this.NavBar.NavToNews.Background = navSelectColour; // set button colour
        }

        private void NavToEvents_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(eve); // need to use a stored 'map' if we want persisted changes
            ResetButtonColours();
            this.NavBar.NavToEvents.Background = navSelectColour;
        }

        private void NavToSocial_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(new Social()); // need to use a stored 'map' if we want persisted changes
            ResetButtonColours();
            this.NavBar.NavToSocial.Background = navSelectColour;
        }
        private void NavToSchedule_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(sched); // need to use a stored 'map' if we want persisted changes
            ResetButtonColours();
            this.NavBar.NavToSchedule.Background = navSelectColour;
        }

        // OrangeRed is the default colour
        private void ResetButtonColours() {
            this.NavBar.NavToMap.Background = navColour; 
            this.NavBar.NavToSocial.Background = navColour;
            this.NavBar.NavToNews.Background = navColour;
            this.NavBar.NavToSchedule.Background = navColour;
            this.NavBar.NavToEvents.Background = navColour;
            this.NavBar.NavToSettings.Background = navColour;
        }

        private Boolean keyboard_showing = false;

        private void SetInitialVisibilities() {
            // stub - delete later if we dont need it
        }

        

        /*private void Show_keyboard_btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.keyboard_showing == false)
            {
                ShowHideMenu("sbShowBottomMenu", show_keyboard_btn, show_keyboard_btn, sliding_keyboard);
            }
            else {
                ShowHideMenu("sbHideBottomMenu", show_keyboard_btn, show_keyboard_btn, sliding_keyboard);
            }
        }*/

        #endregion

        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, UserControl pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);
            if (Storyboard.Contains("Show"))
            {
                this.keyboard_showing = true;
                btnHide.Visibility = System.Windows.Visibility.Visible;                
            }
            else if (Storyboard.Contains("Hide"))
            {
                this.keyboard_showing = false;                
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void NavBar_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
