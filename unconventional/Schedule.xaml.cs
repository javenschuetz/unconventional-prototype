using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace unconventional
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Schedule : Page
    {
        bool filters = false;
        static int interval = 30;
        static double eventHeight = 48.0;
        static double timeWidth = 100.0;
        static double timeHeader = 40.0;

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
        public class Categories
        {
            public static Category houseKeeping = new Category("House Keeping", Brushes.Green);
            public static Category contest = new Category("Contest", Brushes.Pink);
            public static Category game = new Category("Game", Brushes.Aqua);
            public static Category him = new Category("How It's Made", Brushes.Crimson);
            public static Category guests = new Category("Guests", Brushes.AliceBlue);
            public static Category trivia = new Category("Trivia", Brushes.Cyan);
            public static Category showings = new Category("Showings", Brushes.Teal);
            public static Category community = new Category("Community", Brushes.Yellow);
        }

        CheckBox[] chckFilters = new CheckBox[Category.count];

        Program[][] Progs = new Program[][] {
                new Program[] {
                    new Program("Cosplay 101", 900, 1100, Categories.houseKeeping),
                    new Program("Cosplay Contest", 930, 1100, Categories.contest),
                    new Program("Cosplay Chess", 930, 1400, Categories.game),
                    new Program("Art Contest", 1200, 1300, Categories.contest),
                    new Program("Autographs with Leah Clark", 1500, 1630, Categories.guests),
                    new Program("SCT - All Ages Improv Show", 1230, 1430, Categories.guests),
                    new Program("My Hero Academia Season 2", 1000, 1400, Categories.showings),
                    new Program("Death March", 1400, 1800, Categories.showings),
                    new Program("Convention Etiquette 101", 1000, 1130, Categories.houseKeeping),
                    new Program("Coming To A Theatre Near You", 1600, 1800, Categories.showings),
                    new Program("SCT - 18+ Improv", 2000, 2200, Categories.guests),
                },

                new Program[]
                {
                    new Program("Saturday Morning Cartoons", 900, 1300, Categories.showings),
                    new Program("Autographs with Matt Mercer", 1500, 1630, Categories.guests),
                    new Program("Gym Battles", 1200, 1330, Categories.contest),
                    new Program("Leah Clark Phones A Friend", 1700, 1830, Categories.guests),
                    new Program("RWBY Vs. JNPR", 1100, 1200, Categories.trivia),
                    new Program("Capcom Live", 1900, 2100, Categories.guests),
                    new Program("Cosplay Chess", 930, 1400, Categories.game),
                    new Program("Animethon Idol 2018", 1600, 1800, Categories.contest),
                    new Program("Leah Clar: Act With Me", 1400, 1600, Categories.guests),
                },

                new Program[]
                {
                    new Program("Fate Stay Night Mythos", 1330, 1530, Categories.trivia),
                    new Program("Cards Against Animethon", 1500, 1700, Categories.game),
                    new Program("Animethon Night Festival", 1800, 2200, Categories.houseKeeping),
                    new Program("Lolita Fashion Show", 1100, 1400, Categories.community),
                    new Program("How It's Made Cosplay", 1400, 1530, Categories.him)
                }
            };

        string[] date = { "Friday September 21, 2018", "Saturday September 22, 2018", "Sunday September 23, 2018" };

        bool[] filterCat;


        public Schedule()
        {
            InitializeComponent();
            //AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl));
            AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(HandleMouseDown));
            Sched.ColumnDefinitions.Add(new ColumnDefinition());
            //string description = Enumerations.GetEnumDescription((MyEnum)value);

            grdFilters.ColumnDefinitions.Add(new ColumnDefinition());
            FieldInfo[] fields = typeof(Categories).GetFields();
            filterCat = new bool[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                grdFilters.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
                CheckBox chck = new CheckBox() { Content = ((Category)fields[i].GetValue(null)).name, IsChecked = true };
                chck.Click += chckCategory_Clicked;
                Grid.SetColumn(chck, 0);
                Grid.SetRow(chck, i);
                grdFilters.Children.Add(chck);
                chckFilters[i] = chck;
                chckFilters[i].Tag = i;
                filterCat[i] = true;
            }

            chckAll.IsChecked = true;
            for (int i = 0; i < date.Length; i++)
            {
                CreateDay(date[i], Progs[i]);
            }
            //Sched.ShowGridLines = true;
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
            public Category category;
            public Program(string Name, int Start, int End, Category cat)
            {
                name = Name;
                int quotient = (int)(Start / 100);
                start = (quotient * 60 + (Start - quotient * 100)) / interval;
                quotient = (int)(End / 100);
                length = ((quotient * 60 + (End - quotient * 100)) / interval) - start;
                category = cat;
            }
        }

        public void CreateDay(string Date, Program[] programs)
        {
            Label date = new Label();
            date.Content = Date;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            Sched.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
            Grid.SetColumn(date, 0);
            Grid.SetRow(date, Sched.RowDefinitions.Count - 1);
            Grid sched = CreateSched(programs);
            ScrollViewer scroll = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Height = sched.Height
            };
            scroll.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scroll.Content = sched;
            Sched.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(scroll.Height) });
            Grid.SetColumn(scroll, 0);
            Grid.SetRow(scroll, Sched.RowDefinitions.Count - 1);

            Sched.Children.Add(date);
            Sched.Children.Add(scroll);

        }

        public Grid CreateSched(Program[] programs)
        {
            Grid grid = new Grid();
            //grid.ShowGridLines = true;

            ProgSched[] progSched = new ProgSched[maxCol];
            for (int i = 0; i < programs.Length; i++)
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


            int progEnd = progStart - 1;
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
            maxRow += 5;

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
                        if (depth >= maxRow)
                        {
                            maxRow += 5;
                            bool[,] tempFilled = new bool[1 + progEnd - progStart, maxRow];
                            for (int j = 0; j < progEnd - progStart; j++)
                            {
                                for (int k = 0; k < maxRow - 5; k++)
                                {
                                    tempFilled[j, k] = filled[j, k];
                                }
                            }
                            filled = tempFilled;
                        }
                    }
                    if (depth >= rowdef.Count)
                    {
                        rowdef.Add(new RowDefinition() { Name = "row" + (index), Height = new GridLength(eventHeight) });
                    }
                    Button program = new Button()
                    {
                        Style = (Style)this.FindResource("MyButtonStyle"),
                        Background = current.data.category.colour,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center
                    };
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
                header.Content = quotient + ":" + ((i % (60 / interval)) * interval).ToString().PadLeft(2, '0');
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
            for (int i = 0; i < chckFilters.Length; i++)
            {
                chckFilters[i].IsChecked = ((CheckBox)sender).IsChecked;
                filterCat[i] = (bool)chckFilters[i].IsChecked;
            }
            ConstructWithFilters();
        }

        private void ConstructWithFilters()
        {
            Sched.Children.Clear();
            Sched.ColumnDefinitions.Clear();
            Sched.RowDefinitions.Clear();
            Sched.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < date.Length; i++)
            {
                Program[] tempProg = new Program[Progs[i].Length];
                int k = 0;
                for (int j = 0; j < tempProg.Length; j++)
                {
                    if (filterCat[Progs[i][j].category.num])
                    {
                        tempProg[k] = Progs[i][j];
                        k++;
                    }
                }
                Array.Resize(ref tempProg, k);
                if (tempProg.Length != 0)
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
