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
        public SolidColorBrush textColour = new SolidColorBrush(Colors.Gold);

        public EventDetail(Events.Event e)
        {
            InitializeComponent();
            Events.Program prog = e.prog;
            txtTitle.Background = Events.Categories[prog.category].colour;
            txtTitle.Background.Opacity = opacity;
            //vbText.Height = Height - (txtTitle.Height + txtInfo.Height);
            //TextBlock txtDesc = new TextBlock() {Background = textColour, Width = this.Width};
            txtDesc.Background = textColour;
            txtDesc.Background.Opacity = opacity;
            txtDesc.Text = Events.Descs[prog.day][prog.id];
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
            double top = this.ActualHeight - (txtTitle.ActualHeight + txtInfo.ActualHeight);
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
            txtToaster.Text = chckFav.IsChecked == true ? "Event added to favourites" : "Event removed from favourites";
            DoubleAnimation anim = new DoubleAnimation(0.0, 1.0, new Duration(new TimeSpan(0, 0, 3)));
            //anim.AutoReverse = true;
            txtToaster.BeginAnimation(TextBox.OpacityProperty, anim);
            anim = new DoubleAnimation(1.0, 0.0, new Duration(new TimeSpan(0, 0, 5)));
            txtToaster.BeginAnimation(TextBox.OpacityProperty, anim);
        }
    }
}
