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
        public MainWindow()
        {
            InitializeComponent();
            this.debugLabel.Content = this.NavBar.DebugString;
            this.updateDebugLabelBtn.Click += UpdateDebugLabelBtn_Click;
            this.show_keyboard_btn.Click += Show_keyboard_btn_Click;
            this.NavBar.NavToMap.Click += NavToMap_Click;
            this.NavBar.NavToNews.Click += NavToNews_Click;
            SetInitialVisibilities();
        }

        private void NavToMap_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(new Map()); // need to use a stored 'map' if we want persisted changes
            ResetButtonColours();
            this.NavBar.NavToMap.Background = Brushes.LawnGreen;
        }

        private void NavToNews_Click(object sender, RoutedEventArgs e)
        {
            this.main_frame.Navigate(new News()); // loads mocked news interface
            ResetButtonColours(); // resets colour
            this.NavBar.NavToNews.Background = Brushes.LawnGreen; // set button colour
        }

        // OrangeRed is the default colour
        private void ResetButtonColours() {
            this.NavBar.NavToMap.Background = Brushes.OrangeRed; 
            this.NavBar.NavToSocial.Background = Brushes.OrangeRed;
            this.NavBar.NavToNews.Background = Brushes.OrangeRed;
            this.NavBar.NavToSchedule.Background = Brushes.OrangeRed;
            this.NavBar.NavToEvents.Background = Brushes.OrangeRed;
            this.NavBar.NavToSettings.Background = Brushes.OrangeRed;
        }

        private Boolean keyboard_showing = false;

        private void SetInitialVisibilities() {
            // stub - delete later if we dont need it
        }

        #endregion

        #region click handlers

        private void UpdateDebugLabelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.debugLabel.Content = this.NavBar.DebugString;
        }

        private void Show_keyboard_btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.keyboard_showing == false)
            {
                ShowHideMenu("sbShowBottomMenu", show_keyboard_btn, show_keyboard_btn, sliding_keyboard);
            }
            else {
                ShowHideMenu("sbHideBottomMenu", show_keyboard_btn, show_keyboard_btn, sliding_keyboard);
            }
        }

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
    }
}
