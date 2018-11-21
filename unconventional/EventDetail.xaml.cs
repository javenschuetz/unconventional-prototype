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
    /// Interaction logic for EventDetail.xaml
    /// </summary>
    public partial class EventDetail : Page
    {
        public EventDetail()
        {
            InitializeComponent();
            SolidColorBrush trans = new SolidColorBrush(Colors.LawnGreen);
            trans.Opacity = 0.15f;
            txtTitle.Background = trans;
        }

        private void chckFav_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
