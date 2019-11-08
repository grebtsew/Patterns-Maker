using System;
using System.Drawing;
using System.Windows.Forms;

namespace PatternsMaker.ColorPicker
{

    public static class OwnColorDialog
    {
        public static Color ShowDialog()
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 500;
            prompt.Text = "Select a Color and press OK";
            Button confirmation = new Button() { Text = "Ok", Height = 40, Width = 500, Top = 420 };
            DmcListView listview = new DmcListView();
            listview.Size = new Size(500, 420);
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(listview);

            prompt.ShowDialog();
            try
            {
                return listview.SelectedItems[0].BackColor;
            }
            catch (Exception)
            {
                return Color.White;
            }
        }
    }
}
