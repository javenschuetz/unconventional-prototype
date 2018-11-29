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

namespace unconventional 
{
    /// <summary>
    /// Interaction logic for nav_bar.xaml
    /// </summary>
    public partial class NavBar : UserControl
    {
        #region debug string definition

        private string debug_string = "I am a debug string";
        public string DebugString
        {
            get
            {
                return this.debug_string.ToString();
            }
            set
            {
                this.debug_string = value;
            }
        }

        #endregion

        public NavBar()
        {
            InitializeComponent();
            // surely these can be combined into a single handler, but that would require 
            // looking up how the 'sender' and 'e' arguments work
            this.NavToEvents.Click += NavToEvents_Click;
            this.NavToNews.Click += NavToNews_Click;
            this.NavToMap.Click += NavToMap_Click;
            this.NavToSettings.Click += NavToSettings_Click;
            this.NavToSocial.Click += NavToSocial_Click;
        }

        #region click handlers
        private void NavToSocial_Click(object sender, RoutedEventArgs e)
        {
            this.debug_string = "social";
        }

        private void NavToSettings_Click(object sender, RoutedEventArgs e)
        {
            this.debug_string = "settings";
        }

        private void NavToMap_Click(object sender, RoutedEventArgs e)
        {
            this.debug_string = "map";
        }

        private void NavToNews_Click(object sender, RoutedEventArgs e)
        {
            this.debug_string = "news";
        }

        private void NavToSchedule_Click(object sender, RoutedEventArgs e)
        {
            this.debug_string = "schedule";
        }

        private void NavToEvents_Click(object sender, RoutedEventArgs e)
        {
            this.debug_string = "events";
        }

        #endregion
    }
}
