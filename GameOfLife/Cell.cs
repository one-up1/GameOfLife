using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    public class Cell
    {
        private Label label;
        private bool value;

        public Cell()
        {
        }

        public Cell(Label label)
        {
            this.label = label;
        }

        [JsonIgnore]
        public Label Label
        {
            get
            {
                return label;
            }
            set
            {
                this.label = value;
                SetLabelBackground();
            }
        }

        [JsonProperty("value")]
        public bool Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                SetLabelBackground();
            }
        }

        [JsonIgnore]
        public Brush Color { get; set; }

        private void SetLabelBackground()
        {
            if (label != null)
            {
                label.Background = value ?
                    (Color == null ? Brushes.Black : Color) :
                    Brushes.Transparent;
            }
        }
    }
}
