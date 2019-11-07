using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram.Controls;
using Syncfusion.Windows.Forms.Grid;

namespace PatternsMaker
{
    public struct CellItem
    {
        public int occurence;
        public Color? color;
        public Bitmap symbol;
        public float needed;
        public List<Point> used_cells;

        public CellItem(int occurence, List<Point> used_cells, Color? color, Bitmap symbol, float needed)
        {
            this.used_cells = used_cells;
            this.needed = needed;
            this.occurence = occurence;
            this.color = color;
            this.symbol = symbol;
        }
    }

    public partial class PurchaseForm : Form
    {
        float needed_embroidery = 0;
        private GridControl gridControl1;
        private GroupBox groupBox1;
        private Label label1;
        private TextBox textBox1;
        private ListView listView1;
        private Label label2;
        private Diagram diagram1;
        private int hole_per_cm = 10;
        public PurchaseForm(GridControl gridControl1, Diagram diagram1)
        {
            this.gridControl1 = gridControl1;
            this.diagram1 = diagram1;
            InitializeComponent();
            initiate_dmc();
            calculate();

        }

        private void calculate()
        {
            calculate_diagram();
            calculate_embroidery();
            calculate_gridcontrol();
        }


        private void calculate_embroidery()
        {

            List<CellItem> colorlist = new List<CellItem>();
            for (int i = 0; i < this.gridControl1.ColCount; i++)
            {
                for (int j = 0; j < this.gridControl1.RowCount; j++)
                {

                    bool colorexist = false;
                    for (int c = 0; c < colorlist.Count; c++)
                    {
                        if (colorlist.Count > 0)
                        {

                            if (colorlist[c].color == this.gridControl1[i, j].BackColor)
                            {
                                colorexist = true;
                                CellItem tmp = colorlist[c];
                                colorlist.RemoveAt(c);
                                tmp.occurence++;
                                tmp.used_cells.Add(new Point(i, j));
                                colorlist.Add(tmp);
                                
                            }
                        }
                    }

                    if (!colorexist)
                    {
                        List<Point> plist = new List<Point>();
                        plist.Add(new Point(i, j));
                        colorlist.Add(new CellItem(1, plist, this.gridControl1[i, j].BackColor, null, (float)(14+hole_per_cm*0.01)));
                    }
                }
            }

            // sort colorlist
            colorlist.Sort((c1, c2) => c2.occurence.CompareTo(c1.occurence));

            listView1.Items.Clear();
            // add to listview
            foreach (CellItem c in colorlist)
            {
                ListViewItem item = new ListViewItem();
                item.BackColor = c.color ?? Color.White;
               // Console.WriteLine("doing color " + c);
                float needed = (float)(get_distance_to_closest_cells(c.used_cells));
                //Console.WriteLine("done");
                item.Text ="DMC "+get_dmc_color_name(item.BackColor)+ "Nr of usages " + c.occurence + " needed in m "+needed; // todo
                listView1.Items.Add(item);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
          
    }

        public double factorial_WhileLoop(int number)
        {
            double result = 1;
            while (number != 1)
            {
                result = result * number;
                number = number - 1;
            }
            return result;
        }
        private float get_distance_to_closest_cells( List<Point> active_cells)
        {
            List<Point> used_cells = new List<Point>();
            Point closest_cell = Point.Empty;
            float closest_dist = float.PositiveInfinity;
            float xdif, ydif, tmp_dist;
            float total_dist = 0.14f; // size for start and end
            int tot = (int)(factorial_WhileLoop(active_cells.Count));
            if (active_cells.Count > 10000)
            {
                return 0;
            }
            int i = 0;
            foreach (Point current in active_cells) {
              //  Console.WriteLine(i + " Out of " + tot);
                used_cells.Add(current);
                i++;
                    foreach(Point next in active_cells)
                    {
                    if (!used_cells.Contains(next))
                    {

                        if (closest_cell.IsEmpty)
                        {
                            closest_cell = next;
                            xdif = Math.Abs(current.X - closest_cell.X);
                            ydif = Math.Abs(current.Y - closest_cell.Y);
                            closest_dist=(float)Math.Sqrt(xdif * xdif + ydif * ydif); // pythagoras
                        } else
                        {
                            xdif = Math.Abs(current.X - closest_cell.X);
                            ydif = Math.Abs(current.Y - closest_cell.Y);
                            tmp_dist = (float)Math.Sqrt(xdif * xdif + ydif * ydif); // pythagoras
                            if (closest_dist > tmp_dist)
                            {
                                closest_cell = next;
                                closest_dist = tmp_dist;
                            }
                        }
                    }
                    }
                    if(closest_dist != float.PositiveInfinity)
                    {
                    

                    total_dist += (float)(closest_dist* (0.01 / (hole_per_cm * 0.1)) + (0.01 / (hole_per_cm * 0.1)));
                    closest_dist = float.PositiveInfinity;
                    closest_cell = Point.Empty;
                    } else
                {
                    total_dist += (float)(0.01 / (hole_per_cm * 0.1));
                }
            }
            return total_dist;
        }

        List<string> all_dmc_names = new List<string>();

        List<Color> all_dmc_colors = new List<Color>();

        private void initiate_dmc()
        {
            string path_to_dmc = @"../../Data/dmc.txt";

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
                    all_dmc_names.Add(line);
                    all_dmc_colors.Add(ColorTranslator.FromHtml("0x" + splitted[splitted.Length - 2]));
                }
                counter++;
            }

            file.Close();

        }
        private string get_dmc_color_name(Color c)
        {
            int i = 0;
            foreach(Color col in all_dmc_colors)
            {
                if(col == c)
                {
                    return all_dmc_names[i];
                }
                i++;
            }
            return "";
        }

        private void calculate_gridcontrol()
        {

        }

        private void calculate_diagram()
        {

        }

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(883, 280);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grid Pattern";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(223, 72);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(654, 202);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Needed Embroidery :";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(223, 38);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(241, 22);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "10";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Embroidery (holes per 10 cm) :";
            // 
            // PurchaseForm
            // 
            this.ClientSize = new System.Drawing.Size(907, 304);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PurchaseForm";
            this.Load += new System.EventHandler(this.PurchaseForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void PurchaseForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // embrodery holes per cm
            try
            {
                hole_per_cm = int.Parse(textBox1.Text);
                calculate_embroidery();
            }
            catch (Exception)
            {
                textBox1.Text = "10";
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // embrodeire list
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
