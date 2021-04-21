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

        private bool[][] cells;

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
            Label cell = (Label)sender;
            int row = (int)cell.GetValue(Grid.RowProperty);
            int col = (int)cell.GetValue(Grid.ColumnProperty);

            if (cells[row][col])
            {
                cell.Background = Brushes.Transparent;
                cells[row][col] = false;
            }
            else
            {
                cell.Background = Brushes.Red;
                cells[row][col] = true;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < ROW_COUNT; row++)
            {
                for (int col = 0; col < COL_COUNT; col++)
                {
                    if (cells[row][col])
                    {
                        ProcessCell(row, col);
                    }
                }
            }
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
            cells = new bool[ROW_COUNT][];
            for (int row = 0; row < ROW_COUNT; row++)
            {
                cells[row] = new bool[COL_COUNT];
                for (int col = 0; col < COL_COUNT; col++)
                {
                    Label cell = new Label();
                    cell.SetValue(Grid.RowProperty, row);
                    cell.SetValue(Grid.ColumnProperty, col);
                    cell.BorderThickness = new Thickness(1);
                    cell.BorderBrush = Brushes.Black;
                    cell.MouseUp += Cell_MouseUp;
                    grid.Children.Add(cell);
                }
            }
        }

        private void ProcessCell(int row, int col)
        {

        }
    }
}
