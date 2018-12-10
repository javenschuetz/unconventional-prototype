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
using System.Windows.Media.Animation;
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
        public const double opacity = 0.5;
        public SolidColorBrush textColour = new SolidColorBrush(Colors.Beige);
        private Events.Event e;
        private bool origFav;

        public EventDetail(Events.Event e)
        {
            InitializeComponent();
            this.e = e;
            origFav = e.Fav;
            Events.Program prog = e.prog;
            chckFav.IsChecked = this.e.Fav;
            grdTitle.Background = Events.Categories[prog.category].colour;
            grdTitle.Background.Opacity = opacity;
            txtTitle.Text = prog.name;
            txtTitle.Background = new SolidColorBrush(Colors.White);
            txtTitle.Background.Opacity = 0.0;
            //vbText.Height = Height - (txtTitle.Height + txtInfo.Height);
            //TextBlock txtDesc = new TextBlock() {Background = textColour, Width = this.Width};
            txtDesc.Background = textColour;
            txtDesc.Background.Opacity = opacity;
            txtDesc.Text = Events.Descs[prog.day][prog.id];
            string content = "";
            if (prog.day == 0)
                content = "Friday, September 21st ";
            else if (prog.day == 1)
                content = "Friday, September 22nd ";
            else
                content = "Friday September 23rd";
            content += prog.start < 26 ? (prog.start / 2) : (prog.start / 2) % 12;
            content += (prog.start % 2) == 0 ? ":00" : ":30";
            content += (prog.start < 24 || prog.start >= 48) ? "AM-" : "PM-";
            content += (prog.start + prog.length) < 26 ? ((prog.start + prog.length) / 2) : ((prog.start + prog.length) / 2) % 12;
            //content += (prog.start + prog.length) / 2;
            content += ((prog.start + prog.length) % 2) == 0 ? ":00" : ":30";
            content += ((prog.start + prog.length) < 24 || (prog.start + prog.length) >= 48) ? "AM" : "PM";
            txtTime.Text = content;

            txtLocation.Text = prog.location;
            if (prog.speakers.Length > 0)
            {
                txtSpeaker.Text = "Speakers: ";
                for (int i = 0; i < prog.speakers.Length - 1; i++)
                    txtSpeaker.Text += prog.speakers[i] + ", ";
                txtSpeaker.Text += prog.speakers[prog.speakers.Length-1];
                txtSpeaker.Height = 25;
            }
            else
                txtSpeaker.Height = 0;
            UpdateLayout();
            /*RowDefinition row = grdContent.RowDefinitions[0];
            while (row.ActualHeight < 500)
            {
                txtDesc.FontSize += 0.25;
                while (row.ActualHeight > 600)
                {
                    txtDesc.FontSize -= 0.25;
                }
            }*/
            //txtDesc.FontSize += 10;
            //grd.RowDefinitions[1].Height = new GridLength(txtDesc.Height);
            //Grid.SetRow(txtDesc, 1);
            //grd.Children.Add(txtDesc);
        }

        private void RowDefinition_Loaded(object sender, RoutedEventArgs e)
        {
            RowDefinition row = grdContent.RowDefinitions[0];
            UpdateLayout();
            double top = this.ActualHeight - (txtTitle.ActualHeight + grdInfo.ActualHeight);
            double bottom = top * 0.85;
            //txtDesc.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //txtDesc.Arrange(new Rect(0, 0, txtDesc.DesiredSize.Width, txtDesc.DesiredSize.Height));
            while (txtDesc.ActualHeight < bottom)
            {
                txtDesc.FontSize += 4;
                UpdateLayout();
                //txtDesc.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //txtDesc.Arrange(new Rect(0, 0, txtDesc.DesiredSize.Width, txtDesc.DesiredSize.Height));
                while (txtDesc.ActualHeight > top)
                {
                    txtDesc.FontSize -= 0.5;
                    UpdateLayout();
                    //txtDesc.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    //txtDesc.Arrange(new Rect(0, 0, txtDesc.DesiredSize.Width, txtDesc.DesiredSize.Height));
                }
            }
        }

        private void chckFav_Click(object sender, RoutedEventArgs e)
        {
            this.e.Fav = !this.e.Fav;
            Events.needsReload = this.e.Fav != origFav && Events.favsFilter;
            txtToaster.Text = chckFav.IsChecked == true ? "Event added to favourites" : "Event removed from favourites";
            //DoubleAnimation anim = new DoubleAnimation(0.0, 1.0, new Duration(new TimeSpan(0, 0, 3)));
            //anim.AutoReverse = true;
            //txtToaster.BeginAnimation(TextBox.OpacityProperty, anim);
            DoubleAnimation anim = new DoubleAnimation(1.0, 0.0, new Duration(new TimeSpan(0, 0, 5)));
            txtToaster.BeginAnimation(TextBox.OpacityProperty, anim);
        }
    }
}
