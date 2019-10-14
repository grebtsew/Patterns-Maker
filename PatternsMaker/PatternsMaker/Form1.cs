using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatternsMaker
{
    public partial class Form1 : Form
    {

       
        public Form1()
        {
            InitializeComponent();
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // add selected image to each selected cell

            if (listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0];
                var img = item.ImageList.Images[item.ImageIndex];
                Console.WriteLine(img);

                var selected_cells = gridControl1.Selections.Ranges;
                Image image = (Image)(new Bitmap(img, new Size(25, 25)));
                //gridControl1[1, 1].CellValue = img as Bitmap;
                //gridControl1[1, 1].BackgroundImage = image;

                Console.WriteLine(gridControl1[1, 1].CellValue);
                // gridControl1[2, 2].CellValue = "yest";

                if (selected_cells.ToString().Length > 0) { 
                char[] delcol = { 'C' };
                char[] delrow = { 'R' };
                char[] delsep = { ':' };
                Console.WriteLine(selected_cells.ToString().Split(delcol)[1].Split(delsep)[0]);
                int min_cols = int.Parse(selected_cells.ToString().Split(delcol)[1].Split(delsep)[0]);
                int max_cols = int.Parse(selected_cells.ToString().Split(delcol)[2].Split(delcol)[0]);

                int min_rows = int.Parse(selected_cells.ToString().Split(delrow)[1].Split(delcol)[0]);
                int max_rows = int.Parse(selected_cells.ToString().Split(delrow)[2].Split(delcol)[0]);

                Console.WriteLine("Cols: " +min_cols + " " +max_cols+ "  Rows : " +min_rows + " "+ max_rows);


                for (int i = min_cols; i <= max_cols; i++)
                    {
                        for (int j = min_rows; j <= max_rows; j++)
                        {
                            Console.WriteLine(j + " " + i);
                            gridControl1[j,i].BackgroundImage = image;
                        }
                    }
                } else
                {
                    if(gridControl1.CurrentCell != null)
                    {
                        // TODO
                       // gridControl1.cell.BackgroundImage = image;
                    }
                }



                Console.WriteLine(selected_cells);
                foreach (var cell in selected_cells)
                {
                    Console.WriteLine(cell);
                }

            }

        }

        private void gridControl1_CellClick(object sender, Syncfusion.Windows.Forms.Grid.GridCellClickEventArgs e)
        {

        }

        private void colorUIControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
