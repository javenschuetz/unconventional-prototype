/*using System;
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
        public enum Categories
        {
            [Description("House Keeping")]
            houseKeeping,
            [Description("Cosplay Contest")]
            cosplayContest
        }

        //string description = Enumerations.GetEnumDescription((MyEnum)value);

        public Events()
        {
            InitializeComponent();


            FriGrid.ShowGridLines = true;


            Program[] FriProg = {
                new Program("Animethon 101", 900, 1000, Categories.houseKeeping),
                new Program("Animethon 102", 900, 1100, Categories.houseKeeping),
                new Program("Cosplay Contest", 930, 1100, Categories.cosplayContest)
            };
            int maxCol = 24 * (60 / interval);
            ProgSched[] progSched = new ProgSched[maxCol];
            for (int i = 0; i < FriProg.Length; i++)
            {
                Node node = new Node(FriProg[i]);
                if (progSched[node.data.start] == null)
                {
                    progSched[node.data.start] = new ProgSched(node);
                }
                else
                {
                    progSched[node.data.start].Add(node);
                }
            }


            var coldef = FriGrid.ColumnDefinitions;
            var rowdef = FriGrid.RowDefinitions;

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
            maxRow += 4;

            bool[,] filled = new bool[1 + progEnd - progStart, maxRow];

            for (int i = progStart; i <= progEnd; i++)
            {
                coldef.Add(new ColumnDefinition() { Name = "col" + i, Width = new GridLength(100.0) });
            }

            int maxDepth = 0;
            for (int i = progStart; i <= progEnd; i++)
            {
                int index = i - progStart;
                if (progSched[i] == null)
                    continue;
                Node current = progSched[i].head;
                int depth = 0;
                while (current != null)
                {
                    while (filled[index, depth] == true)
                        depth++;
                    if (depth >= rowdef.Count)
                    {
                        rowdef.Add(new RowDefinition() { Name = "row" + (index), Height = new GridLength(100.0) });
                    }
                    Button program = new Button();
                    program.Content = current.data.name;
                    Grid.SetColumn(program, index);
                    Grid.SetRow(program, depth);
                    Grid.SetColumnSpan(program, current.data.length);
                    FriGrid.Children.Add(program);

                    for (int j = 0; (index + j) <= (progEnd - progStart) && j < current.data.length; j++)
                    {
                        filled[index + j, depth] = true;
                    }

                    current = current.next;
                    depth++;
                }
                if (maxDepth < depth)
                {
                    maxDepth = depth;
                }
            }
            FriGrid.Height = 100.0 * maxDepth;
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
                start = (quotient * 60 + (Start - quotient * 100)) / interval;
                quotient = (int)(End / 100);
                length = ((quotient * 60 + (End - quotient * 100)) / interval) - start;
                category = cat;
            }
        }
    }
}
*/