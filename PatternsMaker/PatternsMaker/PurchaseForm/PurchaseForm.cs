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

        public CellItem(int occurence, Color? color, Bitmap symbol)
        {

            this.occurence = occurence;
            this.color = color;
            this.symbol = symbol;
        }
    }

    public partial class PurchaseForm : Form
    {
        private GridControl gridControl1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox textBox1;
        private Label label3;
        private ListView listView1;
        private Label label2;
        private ListView listView3;
        private Label label4;
        private ListView listView2;
        private Diagram diagram1;
        private int hole_per_cm = 10;
        public PurchaseForm(GridControl gridControl1, Diagram diagram1)
        {
            this.gridControl1 = gridControl1;
            this.diagram1 = diagram1;
            InitializeComponent();
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
                                colorlist.Add(tmp);
                                
                            }
                        }
                    }

                    if (!colorexist)
                    {
                        colorlist.Add(new CellItem(1, this.gridControl1[i, j].BackColor, null));
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
                item.Text ="DMC "+"TODO dmc here"+ "Nr of usages " + c.occurence + " needed in m "+"nr"; // todo
                listView1.Items.Add(item);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
          
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.listView2 = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.listView3 = new System.Windows.Forms.ListView();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(501, 196);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grid Pattern";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 214);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(501, 80);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Flow Pattern";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Embroidery (holes per cm) :";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(223, 38);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(272, 22);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "10";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
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
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(223, 72);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(272, 52);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(206, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Needed Knitting and Crochet  : ";
            // 
            // listView2
            // 
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(235, 142);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(272, 52);
            this.listView2.TabIndex = 5;
            this.listView2.UseCompatibleStateImageBehavior = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Needed Knitting and Crochet : ";
            // 
            // listView3
            // 
            this.listView3.HideSelection = false;
            this.listView3.Location = new System.Drawing.Point(216, 18);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(272, 52);
            this.listView3.TabIndex = 6;
            this.listView3.UseCompatibleStateImageBehavior = false;
            // 
            // PurchaseForm
            // 
            this.ClientSize = new System.Drawing.Size(523, 304);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PurchaseForm";
            this.Load += new System.EventHandler(this.PurchaseForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
    }
}
