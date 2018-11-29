using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Media.Effects;

namespace unconventional
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : Page
    {
        bool filters = false;
        const int interval = 30;
        const double eventHeight = 48.0;
        const double timeWidth = 100.0;
        const double timeHeader = 40.0;
        const double whiteSpace = 10.0;

        static Brush favColour = Brushes.Gold;
        static Brush notFavColour = Brushes.White;

        int maxCol = 24 * (60 / interval);

        public class Category
        {
            public static int count = 0;
            public string name;
            public int num;
            public SolidColorBrush colour;

            public Category(string Name, SolidColorBrush Colour)
            {
                name = Name;
                num = count;
                colour = Colour;
                count++;
            }
        }

        public static BrushConverter converter = new BrushConverter();
        public static Category[] Categories = { new Category("18+ Only", (SolidColorBrush)converter.ConvertFrom("#FF0000")),
            new Category("AMV Event", (SolidColorBrush)converter.ConvertFrom("#44F4C4")),
            new Category("Anime Showing", (SolidColorBrush)converter.ConvertFrom("#B7FBFF")),
            new Category("Community Panel", (SolidColorBrush)converter.ConvertFrom("#7F97F3")),
            new Category("Dedicated Space", (SolidColorBrush)converter.ConvertFrom("#B1B1B1")),
            new Category("Gaming Event", (SolidColorBrush)converter.ConvertFrom("#FFBC57")),
            new Category("Guest Event", (SolidColorBrush)converter.ConvertFrom("#FFFD67")),
            new Category("Otafest Event", (SolidColorBrush)converter.ConvertFrom("#EFC8FE"))
        };

        CheckBox[] chckFilters = new CheckBox[Category.count];

        //Program[][] Progs = new Program[3][];
        List<Program>[] Progs = new List<Program>[3];
        List<string>[] Descs = new List<string>[3];

        string[] date = { "Friday September 21, 2018", "Saturday September 22, 2018", "Sunday September 23, 2018" };

        bool[] filterCat;


        public Events()
        {
            InitializeComponent();

            for(int i = 0; i < Progs.Length; i++)
            {
                Progs[i] = new List<Program>();
                Descs[i] = new List<String>();
            }

            List<ProgramJSON> progs = JsonConvert.DeserializeObject<List<ProgramJSON>>(File.ReadAllText(@"..\..\otafest.json"));

            foreach(ProgramJSON p in progs)
            {
                Progs[p.day].Add(new Program(p));
                Descs[p.day].Add(p.desc);
            }

            //AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl));
            AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(HandleMouseDown));
            Schedule.ColumnDefinitions.Add(new ColumnDefinition());
            //string description = Enumerations.GetEnumDescription((MyEnum)value);

            grdFilters.ColumnDefinitions.Add(new ColumnDefinition());
            //FieldInfo[] fields = typeof(Categories).GetFields();
            //filterCat = new bool[fields.Length];
            filterCat = new bool[Categories.Length];
            for(int i = 0; i < Categories.Length; i++)
            {
                grdFilters.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
                CheckBox chck = new CheckBox() { Content = Categories[i].name, IsChecked = true};
                chck.Click += chckCategory_Clicked;
                Grid.SetColumn(chck, 0);
                Grid.SetRow(chck, i);
                grdFilters.Children.Add(chck);
                chckFilters[i] = chck;
                chckFilters[i].Tag = i;
                filterCat[i] = true;
            }

            chckAll.IsChecked = true;
            for(int i = 0; i < date.Length; i++)
            {
                CreateDay(date[i], Progs[i]);
            }
            //Schedule.ShowGridLines = true;
            //CreateDay("Friday September 21, 2018", FriProg);
            //CreateDay("Saturday September 22, 2018", SatProg);
            //CreateDay("Sunday September 23, 2018", SunProg);
        }

        public class ProgSched
        {
            public Node head = null;
            public int depth = 1;

            public ProgSched(Node h)
            {
                head = h;
            }

            public void Add(Node node)
            {
                depth++;
                Node current = head;
                if (current.data.length <= node.data.length)
                {
                    node.next = current;
                    head = node;
                    return;
                }
                while (current.next != null)
                {
                    if (current.next.data.length <= node.data.length)
                    {
                        node.next = current.next;
                        current.next = node;
                        return;
                    }
                    current = current.next;
                }
                current.next = node;
            }
        }

        public class Node
        {
            public Node next = null;
            public Program data = null;
            public Node(Program prog)
            {
                data = prog;
            }
        }

        public class Program
        {
            public string name;
            public int start;
            public int length;
            public int category;
            public bool fav = false;
            
            public Program(ProgramJSON js)
            {
                name = js.name;
                start = js.start;
                length = js.length;
                category = js.category;
            }
        }

        public class ProgramJSON
        {
            public string name;
            public int start;
            public int length;
            public int day;
            public int category;
            public string desc;
            /*public int End
            {
                set {
                    int quotient = (int)(value / 100);
                    length = ((quotient * 60 + (value - quotient * 100)) / interval) - start; }
            }*/
            [JsonConstructor]
            public ProgramJSON(string Name, int Start, int End, int Day, int Category, string Description)
            {
                name = Name;
                int quotient = (int)(Start / 100);
                start = (quotient * 60 + (Start - quotient * 100)) / interval;
                quotient = (int)(End / 100);
                length = ((quotient * 60 + (End - quotient * 100)) / interval) - start;
                day = Day;
                category = Category;
                desc = Description;
            }
            /*public Program(string Name, int Start, int End, Category cat)
            {
                name = Name;
                int quotient = (int)(Start / 100);
                start = (quotient*60 + (Start - quotient*100)) / interval;
                quotient = (int)(End / 100);
                length = ((quotient * 60 + (End - quotient * 100)) / interval) - start;
                category = cat;
            }
            /*public Program(string Name, int Start, int End, Category cat, bool Fav) : this(Name, Start, End, cat)
            {
                fav = Fav;
            }*/
        }

        public void CreateDay(string Date, List<Program> programs)
        {
            Label date = new Label();
            date.Content = Date;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            Schedule.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
            Grid.SetColumn(date, 0);
            Grid.SetRow(date, Schedule.RowDefinitions.Count-1);
            Grid sched = CreateSched(programs);
            ScrollViewer scroll = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, Height = sched.Height};
            scroll.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scroll.Content = sched;
            Schedule.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(scroll.Height) });
            Grid.SetColumn(scroll, 0);
            Grid.SetRow(scroll, Schedule.RowDefinitions.Count-1);

            Schedule.Children.Add(date);
            Schedule.Children.Add(scroll);

        }

        public Grid CreateSched(List<Program> programs)
        {
            Grid grid = new Grid();
            //grid.ShowGridLines = true;

            ProgSched[] progSched = new ProgSched[maxCol];
            for (int i = 0; i < programs.Count; i++)
            {
                Node node = new Node(programs[i]);
                if (progSched[node.data.start] == null)
                {
                    progSched[node.data.start] = new ProgSched(node);
                }
                else
                {
                    progSched[node.data.start].Add(node);
                }
            }


            var coldef = grid.ColumnDefinitions;
            var rowdef = grid.RowDefinitions;

            int progStart = progSched.Length;
            for (int i = 0; i < progSched.Length; i++)
            {
                if (progSched[i] != null)
                {
                    progStart = i;
                    break;
                }
            }


            int progEnd = progStart-1;
            int maxRow = 1;
            for (int i = progStart; i < progSched.Length; i++)
            {
                if (progSched[i] != null)
                {
                    if (maxRow < progSched[i].depth)
                        maxRow = progSched[i].depth;
                    progEnd = i;
                }
            }

            //Provide a buffer of up to 4 more conflictions that may be "piled up" from other timeslots
            maxRow += 10;

            bool[,] filled = new bool[1 + progEnd - progStart, maxRow];

            rowdef.Add(new RowDefinition() { Name = "rowHead", Height = new GridLength(timeHeader) });

            int maxDepth = 1;
            int newColMax = progEnd;
            for (int i = progStart; i <= progEnd; i++)
            {
                int index = i - progStart;
                if (progSched[i] == null)
                    continue;
                Node current = progSched[i].head;
                int depth = 1;
                while (current != null)
                {
                    while (filled[index, depth] == true)
                    {
                        depth++;
                        if(depth >= maxRow)
                        {
                            maxRow += 5;
                            bool[,] tempFilled = new bool[1 + progEnd - progStart, maxRow];
                            for (int j = 0; j < progEnd - progStart; j++)
                            {
                                for (int k = 0; k < maxRow - 5; k++)
                                {
                                    tempFilled[j,k] = filled[j,k];
                                }
                            }
                            filled = tempFilled;
                        }
                    }
                    if (depth >= rowdef.Count)
                    {
                        rowdef.Add(new RowDefinition() { Name = "row" + (index), Height = new GridLength(eventHeight) });
                    }
                    Button program = new Button() {
                            Style = (Style)this.FindResource("MyButtonStyle"),
                            Background = Categories[current.data.category].colour,
                            BorderBrush = current.data.fav ? favColour : notFavColour,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            HorizontalContentAlignment = HorizontalAlignment.Center};
                    
                    // couldn't get this to work in time
                    /*UIElement uie = new UIElement();
                    uie.Effect = new DropShadowEffect
                        {
                            Color = new Color { A = 255, R = 255, G = 255, B = 0 },
                            Direction = 320,
                            ShadowDepth = 1,
                            Opacity = 1
                        };
                        */
                    program.Content = current.data.name;
                    program.Click += Program_Click;
                    Grid.SetColumn(program, index);
                    Grid.SetRow(program, depth);
                    Grid.SetColumnSpan(program, current.data.length);
                    grid.Children.Add(program);

                    for (int j = 0; (index + j) <= (progEnd - progStart) && j < current.data.length; j++)
                    {
                        filled[index + j, depth] = true;
                    }

                    if (newColMax < i + current.data.length)
                        newColMax = i + current.data.length;
                    current = current.next;
                    depth++;
                }
                if (maxDepth < depth)
                {
                    maxDepth = depth;
                }
            }

            for (int i = progStart; i <= newColMax; i++)
            {
                coldef.Add(new ColumnDefinition() { Name = "col" + i, Width = new GridLength(timeWidth) });
                int index = i - progStart;
                Button header = new Button();
                header.IsEnabled = false;
                int quotient = (int)(i / (60 / interval));
                header.Content = quotient % 25 + ":" + ((i % (60 / interval)) * interval).ToString().PadLeft(2, '0');
                Grid.SetColumn(header, index);
                Grid.SetRow(header, 0);
                grid.Children.Add(header);
            }

            grid.Height = (eventHeight * maxDepth) + timeHeader;
            return grid;
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - e.Delta);
            //scrollviewer.LineLeft();
            else
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - e.Delta);
                //scrollviewer.LineRight();
            e.Handled = true;
        }

        private void chckAll_Clicked(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < chckFilters.Length; i++)
            {
                chckFilters[i].IsChecked = ((CheckBox)sender).IsChecked;
                filterCat[i] = (bool)chckFilters[i].IsChecked;
            }
            ConstructWithFilters();
        }

        private void ConstructWithFilters()
        {
            Schedule.Children.Clear();
            Schedule.ColumnDefinitions.Clear();
            Schedule.RowDefinitions.Clear();
            Schedule.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < date.Length; i++)
            {
                List<Program> tempProg = new List<Program>();
                //tempProg.Capacity = Progs[i].Count;
                //int k = 0;
                for(int j = 0; j < Progs[i].Count; j++)
                {
                    if (filterCat[Progs[i][j].category])
                    {
                        tempProg.Add(Progs[i][j]);
                        //k++;
                    }
                }
                //Array.Resize(ref tempProg, k);
                if(tempProg.Count != 0)
                {
                    CreateDay(date[i], tempProg);
                }
            }
        }

        private void chckCategory_Clicked(object sender, RoutedEventArgs e)
        {
            CheckBox chckBox = (CheckBox)sender;
            filterCat[(int)chckBox.Tag] = (bool)chckBox.IsChecked;
            ConstructWithFilters();
        }

        /*private void chckCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            filterCat[(int)((CheckBox)sender).Tag] = false;
            //ConstructWithFilters();
        }*/

        private void ShowHideMenu(string storyboard, Grid pnl)
        {
            Storyboard sb = Resources[storyboard] as Storyboard;
            sb.Begin(pnl);
            this.filters = !this.filters;
        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {
            if (!this.filters)
            {
                ShowHideMenu("sbShowFilters", Filters);
                //Mouse.Capture(Filters);
            }
        }

        /*private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            if (this.filters)
            {
                ShowHideMenu("sbHideFilters", Filters);
                Mouse.Capture(null);
            }
        }*/

        //List to store all the elements under the cursor
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        private void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {

            Point pt = e.GetPosition((UIElement)sender);
            hitResultsList.Clear();

            //Retrieving all the elements under the cursor
            VisualTreeHelper.HitTest(this, null,
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(pt));

            //Testing if the grdFilters is under the cursor
            if (!hitResultsList.Contains(this.Filters) && this.filters)
            {
                ShowHideMenu("sbHideFilters", Filters);
            }
        }

        //Necessary callback function
        private HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            hitResultsList.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }

        public event EventHandler EventClick;

        private void Program_Click(object sender, RoutedEventArgs e)
        {
            if (EventClick != null)
                EventClick(this, e);
        }
    }
}
