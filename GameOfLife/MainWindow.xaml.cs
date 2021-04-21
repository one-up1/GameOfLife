using System;
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
        private const int ROW_COUNT = 16;
        private const int COL_COUNT = 32;

        private Cell[][] cells;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddCells();
        }

        private void Cell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            Cell cell = cells[(int)label.GetValue(Grid.RowProperty)][(int)label.GetValue(Grid.ColumnProperty)];
            cell.Value = !cell.Value;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Cell[][] cells = new Cell[ROW_COUNT][];
            for (int row = 0; row < ROW_COUNT; row++)
            {
                cells[row] = new Cell[COL_COUNT];
                for (int col = 0; col < COL_COUNT; col++)
                {
                    cells[row][col] = new Cell(this.cells[row][col].Label);
                    ProcessCell(cells, row, col);
                }
            }
            this.cells = cells;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            AddCells();
        }

        private void AddCells()
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
            for (int i = 0; i < COL_COUNT; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            // Add cells.
            cells = new Cell[ROW_COUNT][];
            for (int row = 0; row < ROW_COUNT; row++)
            {
                cells[row] = new Cell[COL_COUNT];
                for (int col = 0; col < COL_COUNT; col++)
                {
                    Label label = new Label();
                    label.SetValue(Grid.RowProperty, row);
                    label.SetValue(Grid.ColumnProperty, col);
                    label.BorderThickness = new Thickness(1);
                    label.BorderBrush = Brushes.Black;
                    label.MouseUp += Cell_MouseUp;

                    grid.Children.Add(label);
                    cells[row][col] = new Cell(label);
                }
            }
        }

        private void ProcessCell(Cell[][] cells, int row, int col)
        {
            int neighbors = 0;
            for (int iRow = row == 0 ? 0 : row - 1; iRow <= (row == ROW_COUNT - 1 ? row : row + 1); iRow++)
            {
                for (int iCol = col == 0? 0 : col - 1; iCol <= (col == COL_COUNT - 1 ? col : col + 1); iCol++)
                {
                    if ((iRow != row || iCol != col) && this.cells[iRow][iCol].Value)
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
                else
                {
                    cells[row][col].Value = false;
                }
            }
        }
    }
}
