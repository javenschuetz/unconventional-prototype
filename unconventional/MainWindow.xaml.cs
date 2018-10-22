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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
            this.debugLabel.Content = this.NavBar.DebugString;
            this.updateDebugLabelBtn.Click += UpdateDebugLabelBtn_Click;
        }

        #region click handlers

        private void UpdateDebugLabelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.debugLabel.Content = this.NavBar.DebugString;
        }

        #endregion
    }
}
