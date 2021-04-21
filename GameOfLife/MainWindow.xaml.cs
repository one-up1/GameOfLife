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
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddCells();
        }

        private void AddCells()
        {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < 16; i++)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < 32; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            for (int row = 0; row < 16; row++)
            {
                for (int col = 0; col < 32; col++)
                {
                    Label cell = new Label();
                    cell.SetValue(Grid.RowProperty, row);
                    cell.SetValue(Grid.ColumnProperty, col);
                    cell.BorderThickness = new Thickness(1);
                    cell.BorderBrush = Brushes.Black;
                    cell.Tag = false;
                    cell.MouseUp += Cell_MouseUp;
                    grid.Children.Add(cell);
                }
            }
        }

        private void Cell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Label cell = (Label)sender;
            bool val = (bool)cell.Tag;
            if (val)
            {
                cell.Background = Brushes.Transparent;
                val = false;
            }
            else
            {
                cell.Background = Brushes.Red;
                val = true;
            }
            cell.Tag = val;
        }
    }
}
