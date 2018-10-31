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
    /// Interaction logic for Schedule.xaml
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
            public static Category game = new Category("Game", Brushes.Blue);
            public static Category him = new Category("How It's Made", Brushes.Crimson);
            public static Category guests = new Category("Guests", Brushes.Indigo);
            public static Category trivia = new Category("Trivia", Brushes.Cyan);
            public static Category showings = new Category("Showings", Brushes.Teal);
            public static Category community = new Category("Community", Brushes.Yellow);
        }

        CheckBox[] chckFilters = new CheckBox[Category.count];


        public Schedule()
        {
            InitializeComponent();
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl));
            Schedules.ColumnDefinitions.Add(new ColumnDefinition());
            //string description = Enumerations.GetEnumDescription((MyEnum)value);

            grdFilters.ColumnDefinitions.Add(new ColumnDefinition());
            FieldInfo[] fields = typeof(Categories).GetFields();
            for(int i = 0; i < fields.Length; i++)
            {
                grdFilters.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
                CheckBox chck = new CheckBox() { Content = ((Category)fields[i].GetValue(null)).name };
                Grid.SetColumn(chck, 0);
                Grid.SetRow(chck, i);
                grdFilters.Children.Add(chck);
                chckFilters[i] = chck;
            }
            //for(int i = 0; i < chckFilters.Length; i++)
            //{
            //    chckFilters[i] = new CheckBox() { Content= }
            //}

            Program[] FriProg = {
                new Program("Opening Ceremonies", 900, 1000, Categories.houseKeeping),
                new Program("Cosplay Contest", 930, 1100, Categories.contest),
                new Program("Overwatch Championship", 1100, 1600, Categories.contest),
                new Program("My Hero Academia Season 2", 1000, 1400, Categories.showings),
                new Program("Death March", 1400, 1800, Categories.showings),
            };

            Program[] SatProg =
            {
                new Program("Saturday Morning Cartoons", 900, 1300, Categories.showings),
                new Program("Open Video Gaming", 900, 1600, Categories.game),
                new Program("Animethon Idol 2018", 1600, 1800, Categories.contest),
                new Program("Leah Clar: Act With Me", 1400, 1600, Categories.guests),
                new Program("Wheel of Anime", 1000, 1200, Categories.trivia)
            };

            Program[] SunProg =
            {
                new Program("Fate Stay Night Mythos", 1330, 1530, Categories.trivia),
                new Program("Live Action Mario Cart", 1400, 1530, Categories.game),
                new Program("Cards Against Animethon", 1500, 1700, Categories.game),
                new Program("How It's Made Cosplay", 1400, 1530, Categories.him)
            };
            //Schedules.ShowGridLines = true;
            CreateDay("Friday September 21, 2018", FriProg);
            CreateDay("Saturday September 22, 2018", SatProg);
            CreateDay("Sunday September 23, 2018", SunProg);
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
                start = (quotient*60 + (Start - quotient*100)) / interval;
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
            Schedules.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
            Grid.SetColumn(date, 0);
            Grid.SetRow(date, Schedules.RowDefinitions.Count-1);
            Grid sched = CreateSched(programs);
            ScrollViewer scroll = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, Height = sched.Height};
            scroll.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scroll.Content = sched;
            Schedules.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(scroll.Height) });
            Grid.SetColumn(scroll, 0);
            Grid.SetRow(scroll, Schedules.RowDefinitions.Count-1);

            Schedules.Children.Add(date);
            Schedules.Children.Add(scroll);

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

            int progStart = -1;
            for (int i = 0; i < progSched.Length; i++)
            {
                if (progSched[i] != null)
                {
                    progStart = i;
                    break;
                }
            }


            int progEnd = progStart;
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
                    Button program = new Button() { Style = (Style)this.FindResource("MyButtonStyle"), Background = current.data.category.colour};
                    program.Content = current.data.name;
                    //program.HorizontalContentAlignment = HorizontalAlignment.Center;
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

        private void chckAll_Checked(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < chckFilters.Length; i++)
            {
                chckFilters[i].IsChecked = true;
            }
        }

        private void chckAll_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < chckFilters.Length; i++)
            {
                chckFilters[i].IsChecked = false;
            }
        }

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
                Mouse.Capture(Filters);
            }
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            if (this.filters)
            {
                ShowHideMenu("sbHideFilters", Filters);
                Mouse.Capture(null);
            }
        }
    }
}
