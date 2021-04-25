using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int ROW_COUNT = 32;
        private const int COLUMN_COUNT = 64;

        private Cell[][] cells;
        private Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            timer = new Timer(500);
            timer.Elapsed += Timer_Elapsed;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddCells(null);

            // Set default values.
            cells[12][32].Value = true;
            cells[13][33].Value = true;
            cells[14][30].Value = true;
            cells[14][32].Value = true;
            cells[14][33].Value = true;
        }

        private void Cell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            Cell cell = cells[(int)label.GetValue(Grid.RowProperty)]
                [(int)label.GetValue(Grid.ColumnProperty)];
            cell.Value = !cell.Value;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (timer.Enabled)
            {
                Debug.WriteLine("Stopping");
                timer.Stop();
                bStart.Content = "Start";
            }
            else
            {
                Debug.WriteLine("Starting");
                timer.Start();
                bStart.Content = "Stop";
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json";
            if (ofd.ShowDialog() == true)
            {
               AddCells(JsonConvert.DeserializeObject<Cell[][]>(
                    File.ReadAllText(ofd.FileName)));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON files (*.json)|*.json";
            // Why do we need "== true", ShowDialog() return a boolean right?
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName,
                    JsonConvert.SerializeObject(cells));
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            AddCells(null);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // The Elapsed event is not called from the UI thread.
            Dispatcher.Invoke((Action)delegate()
            {
                Next();
            });
        }

        private void AddCells(Cell[][] cells)
        {
            // Clear any existing cells and row/column definitions.
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            // Add row/column definitions.
            for (int i = 0; i < ROW_COUNT; i++)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < COLUMN_COUNT; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            // Add cells.
            this.cells = cells == null ? new Cell[ROW_COUNT][] : cells;
            for (int row = 0; row < ROW_COUNT; row++)
            {
                if (cells == null)
                {
                    this.cells[row] = new Cell[COLUMN_COUNT];
                }
                for (int col = 0; col < COLUMN_COUNT; col++)
                {
                    Label label = new Label();
                    label.SetValue(Grid.RowProperty, row);
                    label.SetValue(Grid.ColumnProperty, col);
                    label.BorderThickness = new Thickness(1);
                    label.BorderBrush = Brushes.Black;
                    label.MouseUp += Cell_MouseUp;
                    grid.Children.Add(label);

                    if (cells == null)
                    {
                        this.cells[row][col] = new Cell(label);
                    }
                    else
                    {
                        this.cells[row][col].Label = label;
                    }
                }
            }
        }

        private void Next()
        {
            Debug.WriteLine("Next()");
            Cell[][] cells = new Cell[ROW_COUNT][];
            for (int row = 0; row < ROW_COUNT; row++)
            {
                cells[row] = new Cell[COLUMN_COUNT];
                for (int col = 0; col < COLUMN_COUNT; col++)
                {
                    cells[row][col] = new Cell(this.cells[row][col].Label);
                    ProcessCell(cells, row, col);
                }
            }
            this.cells = cells;
        }

        private void ProcessCell(Cell[][] cells, int row, int col)
        {
            int neighbors = 0;
            for (int iRow = row - 1; iRow <= row + 1; iRow++)
            {
                for (int iCol = col - 1; iCol <= col + 1; iCol++)
                {
                    if (iRow >= 0 && iCol >= 0 && iRow < ROW_COUNT && iCol < COLUMN_COUNT
                        && !(iRow == row && iCol == col) && this.cells[iRow][iCol].Value)
                    {
                        neighbors++;
                    }
                }
            }

            if (this.cells[row][col].Value)
            {
                // For a space that is populated:
                if (neighbors == 2 || neighbors == 3)
                {
                    // Each cell with two or three neighbors survives.
                    cells[row][col].Value = true;
                }
                else
                {
                    // Each cell with one or no neighbors dies, as if by solitude.
                    // Each cell with four or more neighbors dies, as if by overpopulation.
                    cells[row][col].Value = false;
                }
            }
            else
            {
                // For a space that is empty or unpopulated:
                if (neighbors == 3)
                {
                    // Each cell with three neighbors becomes populated.
                    cells[row][col].Value = true;
                }
            }
        }
    }
}
