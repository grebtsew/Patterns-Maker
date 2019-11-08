using PatternsMaker.ColorPicker;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PatternsMaker
{
    public partial class ColorBox : PictureBox
    {
        public ColorBox()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Size = new Size(10, 10);
            this.Click += new EventHandler(ColorBoxClickEvent);
        }

        private void ColorBoxClickEvent(object sender, EventArgs e)
        {
            // open color selection dialog
            BackColor = OwnColorDialog.ShowDialog();

        }
    }
}

