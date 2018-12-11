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
        bool schedFocus = false;
        public static bool favsFilter = false;
        public string nameFilter = "";
        public static bool needsReload = false;
        public const int interval = 30;
        const double eventHeight = 48.0;
        const double timeWidth = 100.0;
        const double timeHeader = 40.0;
        //const double whiteSpace = 10.0;
        const double eventOpac = 0.5;
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
        public static List<string>[] Descs = new List<string>[3];

        string[] date = { "Friday September 21, 2018", "Saturday September 22, 2018", "Sunday September 23, 2018" };

        bool[] filterCat;

        public class Event : Button
        {
            public Program prog;
            private bool fav = false;

            public bool Fav
            {
                get { return fav; }
                set
                {
                    fav = value;
                    prog.fav = value;
                    if (fav)
                        //BorderBrush = favColour;
                        this.FontWeight = FontWeights.UltraBold;
                    //this.Style.Setters.Add(new Setter(TextBlock.FontWeightProperty, "Bold"));
                    else
                        //BorderBrush = notFavColour;
                        //this.Style.Setters.Add(new Setter(TextBlock.FontWeightProperty, "Normal"));
                        this.FontWeight = FontWeights.Normal;
                }
            }
        }


        public Events()
        {
            InitializeComponent();

            //swFav.Background = new SolidColorBrush(Colors.LightGray);
            //swFav.Opacity = 0.5;

            txtSearch.Background = new SolidColorBrush(Colors.White);
            txtSearch.Foreground = new SolidColorBrush(Colors.LightGray);
            txtSearch.Background.Opacity = 0.25;
            txtSearch.Foreground.Opacity = 0.5;

            for(int i = 0; i < Progs.Length; i++)
            {
                Progs[i] = new List<Program>();
                Descs[i] = new List<String>();
            }
            Program.count = new int[Progs.Length];

            List<ProgramJSON> progs = JsonConvert.DeserializeObject<List<ProgramJSON>>(File.ReadAllText(@"..\..\otafest.json"));

            for(int i = 0; i < progs.Count; i++)
            {
                ProgramJSON p = progs[i];
                Progs[p.day].Add(new Program(p, p.day));
                Descs[p.day].Add(p.desc);
            }

            //AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl));
            AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(HandleMouseDown));
            //Schedule.ColumnDefinitions.Add(new ColumnDefinition());
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
            public static int[] count;
            public int id;
            public int day;
            public string name;
            public int start;
            public int length;
            public int category;
            public bool fav = false;
            public string[] speakers;
            public string location;

            public Program(ProgramJSON js, int Day)
            {
                day = Day;
                id = count[day]++;
                name = js.name;
                start = js.start;
                length = js.length;
                category = js.category;
                speakers = js.speakers;
                location = js.location;
            }
        }

        public class ProgramJSON
        {
            public string name;
            public int start;
            public int length;
            public int day;
            public int category;
            public string[] speakers;
            public string location;
            public string desc;
            /*public int End
            {
                set {
                    int quotient = (int)(value / 100);
                    length = ((quotient * 60 + (value - quotient * 100)) / interval) - start; }
            }*/
            [JsonConstructor]
            public ProgramJSON(string Name, int Start, int End, int Day, int Category, string[] Speakers, string Location, string Description)
            {
                name = Name;
                int quotient = (int)(Start / 100);
                start = (quotient * 60 + (Start - quotient * 100)) / interval;
                quotient = (int)(End / 100);
                length = ((quotient * 60 + (End - quotient * 100)) / interval) - start;
                day = Day;
                category = Category;
                speakers = Speakers;
                location = Location;
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

        public class PairedScrollViewer : ScrollViewer
        {
            public ScrollViewer pair;
        }

        public void CreateDay(string Date, List<Program> programs)
        {
            Label date = new Label();
            date.Content = Date;
            date.FontSize = 16;
            date.FontWeight = FontWeights.Bold;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            Total.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(date, Total.RowDefinitions.Count - 1);
            Total.Children.Add(date);
            //ScrollViewer sc = new ScrollViewer() { Margin = new Thickness(10.0, 0.0, 0.0, 0.0)};
            //sc.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //sc.PreviewMouseWheel += ScheduleScroll_PreviewMouseWheel;
            //Grid Schedule = new Grid() { Margin = new Thickness(10.0, 0.0, 0.0, 0.0)};
            //Schedule.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30.0) });
            //Grid.SetRow(date, Total.RowDefinitions.Count-1);
            //Grid holder = new Grid() { Margin = new Thickness(10.0, 0.0, 0.0, 0.0) };
            //holder.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(625.0) });
            Grid sched = CreateSched(programs);
            //sched.Margin = new Thickness(10.0, 0.0, 0.0, 0.0);
            PairedScrollViewer scroll = new PairedScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = new Thickness(10.0, 0.0, 0.0, 0.0), pair = (ScrollViewer)Total.Children[Total.RowDefinitions.Count - 1]
            };
            scroll.MouseEnter += schedule_MouseEnter;
            scroll.MouseLeave += schedule_MouseLeave;
            scroll.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scroll.Content = sched;
            ScrollViewer vertScroll = new ScrollViewer() {VerticalScrollBarVisibility = ScrollBarVisibility.Hidden }; //{ Margin = new Thickness(10.0, 0.0, 0.0, 0.0) };
            vertScroll.Content = scroll;
            //vertScroll.MouseWheel += ScheduleScroll_MouseWheel;
            vertScroll.PreviewMouseWheel += ScheduleScroll_PreviewMouseWheel;
            //Schedule.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(scroll.Height) });
            //Grid.SetColumn(scroll, 0);
            //Grid.SetRow(scroll, Schedule.RowDefinitions.Count-1);


            //Grid.SetRow(vertScroll, 0);
            //holder.Children.Add(vertScroll);

            Total.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(600) });
            Grid.SetRow(vertScroll, Total.RowDefinitions.Count - 1);
            Total.Children.Add(vertScroll);
            //Grid.SetRow(holder, Total.RowDefinitions.Count - 1);
            //Total.Children.Add(holder);

            //Schedule.Children.Add(scroll);
            //Total.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(625.0) });
            //Grid.SetRow(Schedule, Total.RowDefinitions.Count - 1);
            //Total.Children.Add(Schedule)
            //sc.Content = Schedule;
            //Grid.SetRow(sc, Total.RowDefinitions.Count - 1);
            //Total.Children.Add(sc);
            //Total.Children.Add(date);

        }

        public Grid CreateSched(List<Program> programs)
        {
            Grid grid = new Grid();
            //grid.MouseEnter += schedule_MouseEnter;
            //grid.MouseLeave += schedule_MouseLeave;
            Grid timeGrid = new Grid() { Margin = new Thickness(10.0, 0.0, 0.0, 0.0) };
            //timeGrid.PreviewMouseWheel += Time_PreviewMouseWheel;
            ScrollViewer sc = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Width = Total.Width
            };
            //grid.ShowGridLines = true;
            sc.Content = timeGrid;
            sc.PreviewMouseWheel += Time_PreviewMouseWheel;

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

            timeGrid.RowDefinitions.Add(new RowDefinition() { Name = "rowHead", Height = new GridLength(timeHeader) });

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
                    Event program = new Event() {
                            Style = (Style)this.FindResource("MyButtonStyle"),
                            Background = Categories[current.data.category].colour,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            BorderBrush = notFavColour,
                        prog = current.data, Fav = current.data.fav};
                    program.Background.Opacity = eventOpac;

                    program.Content = new TextBlock() { Text = current.data.name, TextTrimming = TextTrimming.WordEllipsis,
                        Margin = new Thickness(10.0, 0.0, 10.0, 0.0) };
                    program.Click += Program_Click;
                    Grid.SetColumn(program, index);
                    Grid.SetRow(program, depth-1);
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
            Total.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(sc, Total.RowDefinitions.Count - 1);
            Total.Children.Add(sc);

            for (int i = progStart; i <= newColMax; i++)
            {
                coldef.Add(new ColumnDefinition() { Name = "col" + i, Width = new GridLength(timeWidth) });
                timeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Name = "col" + i, Width = new GridLength(timeWidth) });
                int index = i - progStart;
                Button header = new Button();
                header.IsEnabled = false;
                header.Foreground = Brushes.Black;
                int quotient = (int)(i / (60 / interval));
                header.Content = quotient % 25 + ":" + ((i % (60 / interval)) * interval).ToString().PadLeft(2, '0');
                Grid.SetColumn(header, index);
                Grid.SetRow(header, 0);
                timeGrid.Children.Add(header);
            }

            grid.Height = (eventHeight * maxDepth) + timeHeader;
            return grid;
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PairedScrollViewer scrollviewer = sender as PairedScrollViewer;
            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - e.Delta);
            scrollviewer.pair.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - e.Delta);

            /*if (e.Delta > 0)
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - e.Delta);
            //scrollviewer.LineLeft();
            else
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - e.Delta);*/
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

        public void ConstructWithFilters()
        {
            Total.Children.Clear();
            Total.ColumnDefinitions.Clear();
            Total.RowDefinitions.Clear();
            Total.ColumnDefinitions.Add(new ColumnDefinition());
            if(swFav.IsChecked == true)
            {
                if(nameFilter.Length == 0)
                {
                    for (int i = 0; i < date.Length; i++)
                    {
                        List<Program> tempProg = new List<Program>();
                        //tempProg.Capacity = Progs[i].Count;
                        //int k = 0;
                        for (int j = 0; j < Progs[i].Count; j++)
                        {
                            if (filterCat[Progs[i][j].category] && Progs[i][j].fav == true)
                            {
                                tempProg.Add(Progs[i][j]);
                                //k++;
                            }
                        }
                        //Array.Resize(ref tempProg, k);
                        if (tempProg.Count != 0)
                        {
                            CreateDay(date[i], tempProg);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < date.Length; i++)
                    {
                        List<Program> tempProg = new List<Program>();
                        //tempProg.Capacity = Progs[i].Count;
                        //int k = 0;
                        for (int j = 0; j < Progs[i].Count; j++)
                        {
                            if (filterCat[Progs[i][j].category] && Progs[i][j].fav == true && Progs[i][j].name.Contains(nameFilter))
                            {
                                tempProg.Add(Progs[i][j]);
                                //k++;
                            }
                        }
                        //Array.Resize(ref tempProg, k);
                        if (tempProg.Count != 0)
                        {
                            CreateDay(date[i], tempProg);
                        }
                    }
                }
            }
            else
            {
                if (nameFilter.Length == 0)
                {
                    for (int i = 0; i < date.Length; i++)
                    {
                        List<Program> tempProg = new List<Program>();
                        //tempProg.Capacity = Progs[i].Count;
                        //int k = 0;
                        for (int j = 0; j < Progs[i].Count; j++)
                        {
                            if (filterCat[Progs[i][j].category])
                            {
                                tempProg.Add(Progs[i][j]);
                                //k++;
                            }
                        }
                        //Array.Resize(ref tempProg, k);
                        if (tempProg.Count != 0)
                        {
                            CreateDay(date[i], tempProg);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < date.Length; i++)
                    {
                        List<Program> tempProg = new List<Program>();
                        //tempProg.Capacity = Progs[i].Count;
                        //int k = 0;
                        for (int j = 0; j < Progs[i].Count; j++)
                        {
                            if (filterCat[Progs[i][j].category] && Progs[i][j].name.Contains(nameFilter))
                            {
                                tempProg.Add(Progs[i][j]);
                                //k++;
                            }
                        }
                        //Array.Resize(ref tempProg, k);
                        if (tempProg.Count != 0)
                        {
                            CreateDay(date[i], tempProg);
                        }
                    }
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

        public event EventHandler<Event> EventClick;

        private void Program_Click(object sender, RoutedEventArgs e)
        {
            if (EventClick != null)
                EventClick(this, ((Event)e.Source));
        }

        private void swFav_Click(object sender, RoutedEventArgs e)
        {
            favsFilter = !favsFilter;
            ConstructWithFilters();
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.FontStyle = FontStyles.Normal;
            txtSearch.Foreground = new SolidColorBrush(Colors.Black);
            txtSearch.Foreground.Opacity = 1.0;
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text.Trim().Length == 0)
            {
                txtSearch.Text = "Search event by name";
                txtSearch.FontStyle = FontStyles.Italic;
                txtSearch.Foreground = new SolidColorBrush(Colors.LightGray);
                txtSearch.Foreground.Opacity = 0.5;
                nameFilter = "";
            }
            else
            {
                nameFilter = txtSearch.Text.Trim();
            }
            ConstructWithFilters();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                if (txtSearch.Text.Trim().Length == 0)
                {
                    txtSearch.Text = "Search event by name";
                    txtSearch.FontStyle = FontStyles.Italic;
                    txtSearch.Foreground = new SolidColorBrush(Colors.LightGray);
                    txtSearch.Foreground.Opacity = 0.5;
                    nameFilter = "";
                }
                else
                {
                    nameFilter = txtSearch.Text.Trim();
                }
                ConstructWithFilters();
            }
        }

        private void ScheduleScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(!schedFocus)
            {
                ScrollViewer scrollviewer = sender as ScrollViewer;
                //Point relativePoint = Total.Children[0].TransformToAncestor(this).Transform(new Point(0, 0));
                Point relativePoint = scrollviewer.TransformToAncestor(this).Transform(new Point(0, 0));
                //if((e.Delta < 0 && scrollviewer.VerticalOffset == scrollviewer.ScrollableHeight) || (e.Delta > 0 && scrollviewer.VerticalOffset == 0))
                if ((e.Delta < 0 && (scrollviewer.VerticalOffset == scrollviewer.ScrollableHeight || relativePoint.Y > 50)) || (e.Delta > 0 && (scrollviewer.VerticalOffset == 0 || relativePoint.Y < 50)))
                    TotalScroll.ScrollToVerticalOffset(TotalScroll.VerticalOffset - (e.Delta >> 1));
                else
                    scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - (e.Delta >> 1));
                e.Handled = true;
            }
        }

        void schedule_MouseLeave(object sender, EventArgs e)
        {
            schedFocus = false;
        }

        void schedule_MouseEnter(object sender, EventArgs e)
        {
            schedFocus = true;
        }

        private void Time_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TotalScroll.ScrollToVerticalOffset(TotalScroll.VerticalOffset - (e.Delta >> 1));
        }
    }
}
