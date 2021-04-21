using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    public class Cell
    {
        private Label label;
        private bool value;

        public Cell(Label label)
        {
            this.label = label;
        }

        public Label Label
        {
            get
            {
                return label;
            }
        }

        public bool Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                label.Background = value ? Brushes.Red : Brushes.Transparent;
            }
        }
    }
}
