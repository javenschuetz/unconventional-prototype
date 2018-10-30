using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : Page
    {
        static int interval = 30;
        static double eventHeight = 25.0;
        static double timeWidth = 100.0;
        static double timeHeader = 40.0;

        int maxCol = 24 * (60 / interval);
        public enum Categories
        {
            [Description("House Keeping")]
            houseKeeping,
            [Description("Contest")]
            contest,
            [Description("Game")]
            game,
            [Description("How it's Made")]
            him,
            [Description("Guests")]
            guests,
            [Description("Trivia")]
            trivia,
            [Description("Showings")]
            showings,
            [Description("Community")]
            community
        }

        SolidColorBrush[] categoryColours = { Brushes.Green, Brushes.Pink, Brushes.Blue, Brushes.Crimson, Brushes.Indigo, Brushes.Cyan, Brushes.Teal, Brushes.Yellow };

        //string description = Enumerations.GetEnumDescription((MyEnum)value);


        public Events()
        {
            InitializeComponent();
            Schedule.ColumnDefinitions.Add(new ColumnDefinition());

            Program[] FriProg = {
                new Program("Opening Ceremonies", 900, 1000, Categories.houseKeeping),
                new Program("Animethon 101", 900, 930, Categories.houseKeeping),
                new Program("Cosplay 101", 900, 1100, Categories.houseKeeping),
                new Program("Cosplay Contest", 930, 1100, Categories.contest),
                new Program("Cosplay Chess", 930, 1400, Categories.game),
                new Program("Art Contest", 1200, 1300, Categories.contest),
                new Program("Overwatch Championship", 1100, 1600, Categories.contest),
                new Program("Open Video Gaming", 900, 1600, Categories.game),
                new Program("Autographs with Leah Clark", 1500, 1630, Categories.guests),
                new Program("What They Did Right", 1300, 1430, Categories.community),
                new Program("Choose Your Own AMV Adventure", 1430, 1600, Categories.community),
                new Program("SCT - All Ages Improv Show", 1230, 1430, Categories.guests),
                new Program("My Hero Academia Season 2", 1000, 1400, Categories.showings),
                new Program("Death March", 1400, 1800, Categories.showings),
                new Program("Convention Etiquette 101", 1000, 1130, Categories.houseKeeping),
                new Program("Coming To A Theatre Near You", 1600, 1800, Categories.showings),
                new Program("SCT - 18+ Improv", 2000, 2200, Categories.guests),
                new Program("Black Clover", 1800, 2200, Categories.showings),
                new Program("Zap Brannigans \"How to Panel\" Panel", 1100, 1330, Categories.him),
                new Program("Voice Over Adventure", 1030, 1200, Categories.community)
            };

            Program[] SatProg =
            {
                new Program("Saturday Morning Cartoons", 900, 1300, Categories.showings),
                new Program("Autographs with Matt Mercer", 1500, 1630, Categories.guests),
                new Program("Open Video Gaming", 900, 1600, Categories.game),
                new Program("Gym Battles", 1200, 1330, Categories.contest),
                new Program("Leah Clark Phones A Friend", 1700, 1830, Categories.guests),
                new Program("RWBY Vs. JNPR", 1100, 1200, Categories.trivia),
                new Program("Capcom Live", 1900, 2100, Categories.guests),
                new Program("Cosplay Chess", 930, 1400, Categories.game),
                new Program("Pokemon Go Walk", 1100, 1300, Categories.community),
                new Program("Light Novels 101", 1300, 1400, Categories.him),
                new Program("Animethon AMV Contest", 1500, 1600, Categories.contest),
                new Program("Animethon Idol 2018", 1600, 1800, Categories.contest),
                new Program("Leah Clar: Act With Me", 1400, 1600, Categories.guests),
                new Program("Wheel of Anime", 1000, 1200, Categories.trivia)
            };

            Program[] SunProg =
            {
                new Program("Fate Stay Night Mythos", 1330, 1530, Categories.trivia),
                new Program("Live Action Mario Cart", 1400, 1530, Categories.game),
                new Program("Open Video Gaming", 900, 1600, Categories.game),
                new Program("Cards Against Animethon", 1500, 1700, Categories.game),
                new Program("Fire Emblem Jeopardy", 1400, 1500, Categories.trivia),
                new Program("Cosplay Chess", 930, 1400, Categories.game),
                new Program("Animethon Night Festival", 1800, 2200, Categories.houseKeeping),
                new Program("Maid Cafe", 1500, 1700, Categories.houseKeeping),
                new Program("AMV Mortal Kombat", 1200, 1500, Categories.showings),
                new Program("Lolita Fashion Show", 1100, 1400, Categories.community),
                new Program("How It's Made Cosplay", 1400, 1530, Categories.him)
            };
            //Schedule.ShowGridLines = true;
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
            public Categories category;
            public Program(string Name, int Start, int End, Categories cat)
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
            Schedule.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25.0) });
            Grid.SetColumn(date, 0);
            Grid.SetRow(date, Schedule.RowDefinitions.Count-1);
            Grid sched = CreateSched(programs);
            ScrollViewer scroll = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, Height = sched.Height};
            scroll.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scroll.Content = sched;
            Schedule.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(scroll.Height) });
            Grid.SetColumn(scroll, 0);
            Grid.SetRow(scroll, Schedule.RowDefinitions.Count-1);

            Schedule.Children.Add(date);
            Schedule.Children.Add(scroll);

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
                    Button program = new Button() { Style = (Style)this.FindResource("MyButtonStyle"), Background = categoryColours[(int)current.data.category]};
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
    }
}
