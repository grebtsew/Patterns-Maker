using System.Drawing;
using System.Windows.Forms;

namespace PatternsMaker
{
    public partial class DmcListView : ListView
    {
        string path_to_dmc = @"../../Data/dmc.txt";
        public DmcListView()
        {
            initiate_dmc_colorpicker();
        }

        private void initiate_dmc_colorpicker()
        {
            int counter = 0;
            string line;

            char[] del = { '	' };

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(path_to_dmc);
            while ((line = file.ReadLine()) != null)
            {
                if (counter > 0)
                {
                    string[] splitted = line.Split(del);
                    this.Items.Add(line);
                    this.Items[this.Items.Count - 1].BackColor = ColorTranslator.FromHtml("0x" + splitted[splitted.Length - 2]);
                }
                counter++;
            }
            file.Close();
        }
    }
}
