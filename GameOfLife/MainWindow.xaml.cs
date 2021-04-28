using Microsoft.Win32;
using Newtonsoft.Json;
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
        private int rowCount;
        private int columnCount;
        private int moveCount;

        private Cell[][] cells;
        private Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddCells(null);

            // Set default cell values.
            cells[6][16].Value = true;
            cells[7][17].Value = true;
            cells[8][15].Value = true;
            cells[8][16].Value = true;
            cells[8][17].Value = true;
        }

        private void Cell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Get Cell from the array[][] based on the row/column of the Label.
            Label label = (Label)sender;
            Cell cell = cells[(int)label.GetValue(Grid.RowProperty)]
                [(int)label.GetValue(Grid.ColumnProperty)];

            // Reset any color and toggle cell value.
            cell.Color = null;
            cell.Value = !cell.Value;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json";
            // Why do we need "== true", ShowDialog() returns a boolean right?
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
            // Why do we need "== true", ShowDialog() returns a boolean right?
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName,
                    JsonConvert.SerializeObject(cells));
            }
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
                // Parse interval.
                try
                {
                    timer.Interval = int.Parse(tbTimerInterval.Text);
                    if (timer.Interval < 100)
                        throw new Exception();
                }
                catch
                {
                    Debug.WriteLine("Error parsing timer interval");
                    tbTimerInterval.SelectAll();
                    tbTimerInterval.Focus();
                    return;
                }

                Debug.WriteLine("Starting, interval=" + timer.Interval);
                Next();
                timer.Start();
                bStart.Content = "Stop";
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
            // Get row/column count.
            if (cells == null)
            {
                // Parse row/column count when no existing grid is being opened.
                try
                {
                    rowCount = int.Parse(tbRowCount.Text);
                    if (rowCount < 4 || rowCount > 64)
                        throw new Exception();
                }
                catch
                {
                    Debug.WriteLine("Error parsing row count");
                    tbRowCount.SelectAll();
                    tbRowCount.Focus();
                    return;
                }
                try
                {
                    columnCount = int.Parse(tbColumnCount.Text);
                    if (columnCount < 4 || columnCount > 64)
                        throw new Exception();
                }
                catch
                {
                    Debug.WriteLine("Error parsing column count");
                    tbColumnCount.SelectAll();
                    tbColumnCount.Focus();
                    return;
                }
            }
            else
            {
                // Use row/column count from the existing grid.
                rowCount = cells.Length;
                columnCount = cells[0].Length;

                tbRowCount.Text = rowCount.ToString();
                tbColumnCount.Text = columnCount.ToString();
            }
            Debug.WriteLine("rowCount=" + rowCount + ", columnCount=" + columnCount);

            // Stop any running timer.
            if (timer.Enabled)
            {
                Debug.WriteLine("Stopping timer");
                timer.Stop();
                bStart.Content = "Start";
            }

            // Clear any existing cells and row/column definitions.
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            // Add row/column definitions.
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < columnCount; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            // Add cells.
            this.cells = cells == null ? new Cell[rowCount][] : cells;
            for (int row = 0; row < rowCount; row++)
            {
                if (cells == null)
                {
                    this.cells[row] = new Cell[columnCount];
                }
                for (int col = 0; col < columnCount; col++)
                {
                    Label label = new Label();
                    label.SetValue(Grid.RowProperty, row);
                    label.SetValue(Grid.ColumnProperty, col);
                    label.BorderThickness = new Thickness(1);
                    label.BorderBrush = Brushes.DarkSlateGray;
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

            // Reset move count.
            moveCount = 0;
            lMoveCount.Content = "";
        }

        private void Next()
        {
            Debug.WriteLine("Next()");

            // Create a new Cell[][] as we need the cell values
            // specified by the user while they are being updated.
            Cell[][] cells = new Cell[rowCount][];
            for (int row = 0; row < rowCount; row++)
            {
                cells[row] = new Cell[columnCount];
                for (int col = 0; col < columnCount; col++)
                {
                    cells[row][col] = new Cell(this.cells[row][col].Label);
                    ProcessCell(cells, row, col);
                }
            }
            this.cells = cells;

            // Update move count.
            moveCount++;
            lMoveCount.Content = moveCount.ToString();
        }

        private void ProcessCell(Cell[][] cells, int row, int col)
        {
            // Determine the number of populated neighbours for this cell.
            int neighbours = 0;
            for (int iRow = row - 1; iRow <= row + 1; iRow++)
            {
                for (int iCol = col - 1; iCol <= col + 1; iCol++)
                {
                    if (!(iRow == row && iCol == col) &&
                        GetCellValue(iRow, iCol))
                    {
                        neighbours++;
                    }
                }
            }

            if (this.cells[row][col].Value)
            {
                // For a space that is populated:
                if (neighbours == 2 || neighbours == 3)
                {
                    // Each cell with two or three neighbors survives.
                    if (cbFancyColors.IsChecked == true)
                    {
                        // And gets a fancy color.
                        cells[row][col].Color = neighbours == 2 ? Brushes.Red : Brushes.Blue;
                    }
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
                if (neighbours == 3)
                {
                    // Each cell with three neighbors becomes populated.
                    if (cbFancyColors.IsChecked == true)
                    {
                        // And gets a fancy color.
                        cells[row][col].Color = Brushes.Yellow;
                    }
                    cells[row][col].Value = true;
                }
            }
        }

        private bool GetCellValue(int row, int col)
        {
            // Why do we need "== true", IsChecked property is a boolean right?
            if (cbSnake.IsChecked == true)
            {
                // When "snake" mode is used, the grid is treated as "spherical"
                // and "off-grid" indices "continue" on the other edge.
                if (row < 0)
                    row += rowCount;
                else if (row >= rowCount)
                    row -= rowCount;

                if (col < 0)
                    col += columnCount;
                else if (col >= columnCount)
                    col -= columnCount;
            }
            else if (row < 0 || col < 0 || row >= rowCount || col >= columnCount)
            {
                // We can't go "off-grid" when "snake" mode is not used.
                return false;
            }
            return cells[row][col].Value;
        }
    }
}
