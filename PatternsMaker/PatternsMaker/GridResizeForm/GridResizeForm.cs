using Syncfusion.Windows.Forms.Grid;
using System;
using System.Windows.Forms;

namespace PatternsMaker
{
    public partial class GridResizeForm : Form
    {
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private GridControl gridControl1;

        public GridResizeForm(GridControl gridControl1)
        {
            this.gridControl1 = gridControl1;
            InitializeComponent();

            textBox1.Text = gridControl1.ColCount.ToString();
            textBox2.Text = gridControl1.RowCount.ToString();
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "X: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y: ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(44, 9);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(159, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(43, 42);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(159, 22);
            this.textBox2.TabIndex = 3;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(210, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 58);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GridResizeForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 77);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GridResizeForm";
            this.ResumeLayout(false);
            this.PerformLayout();


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // x
            int x = int.Parse(textBox1.Text);
            gridControl1.ColCount = x;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // y

            int y = int.Parse(textBox2.Text);
            gridControl1.RowCount = y;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // exit form
            this.Close();
        }
    }
}
