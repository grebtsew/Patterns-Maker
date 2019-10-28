using PatternsMaker.GeneratorTab;
using Syncfusion.Windows.Forms.Diagram;
using Syncfusion.Windows.Forms.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// TODO LIST

// show how much jarn/thread is needed of which color
// use palette view!


// MetroModernUI
// Fancy form
// ImageEditor


// low prio
// fix nicer gui placeing and size
// fix nicer gui colors
// fix README


namespace PatternsMaker
{
    public struct Cell
    {
        public int x, y;
        public Color? color;
        public Bitmap symbol;

        public Cell(int x, int y, Color? color, Bitmap symbol)
        {

            this.x = x;
            this.y = y;
            this.color = color;
            this.symbol = symbol;
        }
    }



    public partial class Form1 : Form
    {
        char[] delitem = { ';' }; char[] delcol = { 'C' };
        char[] delrow = { 'R' };
        char[] delsep = { ':' };
        List<Cell> to_cells = new List<Cell>();
        List<Color> all_dmc_colors = new List<Color>();
        List<string> all_dmc_names = new List<string>();

        int listview_image_size = 200;
        Bitmap loaded_image;
        int x = 50;
        int y = 50;
        int nr_colors_in = 2;
        int nr_colors_out = 2;
        int fill_tresh = 10000;

        List<int> sortedOccurence = new List<int>();
        List<Color> sortedPixelColors = new List<Color>();
        List<ListViewItem> rec_in = new List<ListViewItem>();
        List<ListViewItem> rec_out = new List<ListViewItem>();

        List<Cell> tmp_saved = new List<Cell>();
        List<Cell> tmp_latest = new List<Cell>();

        Size cell_size = new Size(25, 25);
        string path_to_dmc = @"../../Data/dmc.txt";
        string path_to_symbol = "../../Symbols/";

        private ToolStripProgressBar toolStripProgressBar; // todo add this progressbar when necessary

        public Form1()
        {
            InitializeComponent();

            initiate_dmc_colorpicker();

            //To prevent the selection from particular range of cells
            this.gridControl1.SelectionChanging += gridControl1_SelectionChanging;

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            toolStripProgressBar = new ToolStripProgressBar();

            this.gridControl1.CommandStack.Enabled = true; // for ctrl-z function

            // Set MergeCells direction for the GridControl.
            this.gridControl1.TableStyle.MergeCell = GridMergeCellDirection.Both;
            

            // Set merge cells behavior for the Grid.
            //  this.gridControl1.Model.Options.MergeCellsMode = GridMergeCellsMode.OnDemandCalculation
            //     | GridMergeCellsMode.MergeColumnsInRow | GridMergeCellsMode.MergeRowsInColumn;
            // this.gridControl1.Model.Options.MergeCellsLayout = GridMergeCellsLayout.Grid;

            init_flowlayouts();
            init_symbollistviews();
            init_flowchart();
        }

        private void init_symbollistviews()
        {
            ImageList ilist = new ImageList();
            foreach (string dirName in Directory.GetDirectories(path_to_symbol))
            {
               // Console.WriteLine(Path.GetFileName(dirName));
                ListViewGroup group = new ListViewGroup(Path.GetFileName(dirName));
                
                listView1.Groups.Add(group);

                  //  Console.WriteLine(dirName);
                    foreach (string fileName in Directory.GetFiles(dirName ))
                    {
                
                 //   Console.WriteLine(fileName);
                    ilist.Images.Add(Image.FromFile(fileName));
                    
                    ListViewItem item = new ListViewItem();
                    item.ImageIndex = ilist.Images.Count-1;
                    item.Group = group;
                    this.listView1.Items.Add(item);
                    
                }
                
            }
            
            this.listView1.View = System.Windows.Forms.View.SmallIcon;
            ilist.ImageSize = new Size(50, 50);
            listView1.ShowGroups = true;
            this.listView1.SmallImageList = ilist;
        }


        private void init_flowlayouts()
        {

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel2.AutoScroll = true;

            foreach (int i in Enumerable.Range(0, nr_colors_in))
            {
                flowLayoutPanel1.Controls.Add(new ColorBox());

            }
            foreach (int i in Enumerable.Range(0, nr_colors_out))
            {
                flowLayoutPanel2.Controls.Add(new ColorBox());
            }
            //Enable diagram rulers
            diagram1.ShowRulers = true;

        }

        private void add_border(List<Point> cells)
        {
            foreach (Point p in cells)
            {
                gridControl1[p.Y, p.X].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.Black, GridBorderWeight.Medium);
            }
        }

        private void add_border_all()
        {
            // add border to all cells

            foreach (int i in Enumerable.Range(0, gridControl1.ColStyles.Count))
            {
                gridControl1.ColStyles[i].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.Black, GridBorderWeight.Medium);
            }
        }

        private List<Point> get_selected_cells()
        {
            List<Point> res = new List<Point>();
            var selected_cells = gridControl1.Selections.Ranges;
            foreach (var selected_cell_block in selected_cells.ToString().Split(delitem))
            {
                if (selected_cells.ToString().Length > 0)
                {
                    if (selected_cell_block.Contains(":"))
                    {
                        int min_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1].Split(delsep)[0]);
                        int max_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[2].Split(delcol)[0]);

                        int min_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                        int max_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[2].Split(delcol)[0]);

                        for (int i = min_cols; i <= max_cols; i++)
                        {
                            for (int j = min_rows; j <= max_rows; j++)
                            {
                                res.Add(new Point(i, j));
                            }
                        }
                    }
                    else
                    {
                        int cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1]);
                        int rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                        res.Add(new Point(rows, cols));
                    }
                }
                else
                {
                    // get current active cell
                    if (gridControl1.CurrentCell.ColIndex > 0)
                    {
                        res.Add(new Point(gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex));
                    }
                }

            }
            return res;
        }


        private void do_changes(List<Cell> changes)
        {
            if (changes == null)
            {
                return;
            }

            foreach (Cell cell in changes)
            {
                gridControl1[cell.x, cell.y].BackgroundImage = cell.symbol; // something wrong with this type!
                gridControl1[cell.x, cell.y].BackColor = (Color)cell.color;
            }

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
                    listView3.Items.Add(line);
                    all_dmc_names.Add(line);
                    all_dmc_colors.Add(ColorTranslator.FromHtml("0x" + splitted[splitted.Length - 2]));
                    listView3.Items[listView3.Items.Count - 1].BackColor = ColorTranslator.FromHtml("0x" + splitted[splitted.Length - 2]);
                }
                counter++;
            }

            file.Close();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Save pattern gridControl1

            // open save dialog

            // Save to xml
            gridControl1.FileName = "test.xml";
            gridControl1.SaveXml();

            // Save to Excel


        }

        private void showimage(Bitmap img)
        {
            // show image in new form
            using (Form form = new Form())
            {

                form.StartPosition = FormStartPosition.CenterScreen;
                form.Size = img.Size;

                PictureBox pb = new PictureBox();
                pb.Dock = DockStyle.Fill;
                pb.Image = img;

                form.Controls.Add(pb);
                form.ShowDialog();
            }
        }

        public static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return bmp1 == bmp2;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                if (b1bytes[n] != b2bytes[n])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // add selected image to each selected cell

            if (listView1.SelectedItems.Count > 0)
            {
                var item = listView1.SelectedItems[0]; // item to set
                Bitmap img = (Bitmap)item.ImageList.Images[item.ImageIndex];

                img = new Bitmap(img, cell_size);
                pictureBox2.Image = img;
                img.MakeTransparent();

                if (FillCheckBox.Checked)
                {
                    //item to replace
                    Bitmap img_to_replace = (Bitmap)gridControl1[gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex].BackgroundImage;
                    List<Point> fill_points = new List<Point>();
                    bool all_found = false;
                    Point current = new Point(gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex);
                    fill_points.Add(current);
                    int i = 0;


                    while (!all_found)
                    {
                        if (i >= fill_points.Count || i > fill_tresh)
                        {
                            all_found = true;
                            continue;
                        }

                        current = fill_points[i];
                        if (current.X > 0)
                        {
                            if (!fill_points.Contains(new Point(current.X - 1, current.Y)))
                            {
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X - 1, current.Y].BackgroundImage, img_to_replace))
                                {
                                    fill_points.Add(new Point(current.X - 1, current.Y));
                                }
                            }
                        }

                        if (current.X > 0 && current.Y > 0)
                            if (!fill_points.Contains(new Point(current.X - 1, current.Y - 1)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X - 1, current.Y - 1].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X - 1, current.Y - 1));
                                }

                        if (current.Y > 0)
                            if (!fill_points.Contains(new Point(current.X, current.Y - 1)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X, current.Y - 1].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X, current.Y - 1));
                                }
                        if (current.Y > 0 && current.X < gridControl1.RowCount)
                            if (!fill_points.Contains(new Point(current.X + 1, current.Y - 1)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X + 1, current.Y - 1].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X + 1, current.Y - 1));
                                }
                        if (current.X < gridControl1.RowCount)
                            if (!fill_points.Contains(new Point(current.X + 1, current.Y)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X + 1, current.Y].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X + 1, current.Y));
                                }
                        if (current.X < gridControl1.RowCount && current.Y < gridControl1.ColCount)
                            if (!fill_points.Contains(new Point(current.X + 1, current.Y + 1)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X + 1, current.Y + 1].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X + 1, current.Y + 1));
                                }
                        if (current.Y < gridControl1.ColCount)
                            if (!fill_points.Contains(new Point(current.X, current.Y + 1)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X, current.Y + 1].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X, current.Y + 1));
                                }
                        if (current.X > 0 && current.Y < gridControl1.ColCount)
                            if (!fill_points.Contains(new Point(current.X - 1, current.Y + 1)))
                                if (CompareBitmapsFast((Bitmap)gridControl1[current.X - 1, current.Y + 1].BackgroundImage, img_to_replace))
                                {

                                    fill_points.Add(new Point(current.X - 1, current.Y + 1));
                                }

                        i++;
                    }


                    foreach (Point p in fill_points)
                    {
                        img = new Bitmap(img, gridControl1[p.X, p.Y].CellModel.GetCellSize(p.X, p.Y));
                        gridControl1[p.X, p.Y].BackgroundImage = img;
                    }

                }
                else
                {
                    var selected_cells = gridControl1.Selections.Ranges;
                    foreach (var selected_cell_block in selected_cells.ToString().Split(delitem))
                    {
                        if (selected_cells.ToString().Length > 0)
                        {
                            if (selected_cell_block.Contains(":"))
                            {
                                int min_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1].Split(delsep)[0]);
                                int max_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[2].Split(delcol)[0]);

                                int min_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                                int max_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[2].Split(delcol)[0]);

                                for (int i = min_cols; i <= max_cols; i++)
                                {
                                    for (int j = min_rows; j <= max_rows; j++)
                                    {
                                        img = new Bitmap(img, gridControl1[j,i].CellModel.GetCellSize(j,i));
                                        gridControl1[j, i].BackgroundImage = img;
                                    }
                                }
                            }
                            else
                            {
                                int cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1]);
                                int rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                               
                                img = new Bitmap(img, gridControl1[rows, cols].CellModel.GetCellSize(rows, cols));
                                gridControl1[rows, cols].BackgroundImage = img;
                            }
                        }
                        else
                        {
                            // get current active cell
                            if (gridControl1.CurrentCell.ColIndex > 0)
                            {
                                img = new Bitmap(img, gridControl1[gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex].CellModel.GetCellSize(gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex));

                                gridControl1[gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex].BackgroundImage = img;
                            }
                        }
                    }
                }
            }
        }
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        void gridControl1_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            // Update selected Size
            var selected_cells = gridControl1.Selections.Ranges;
            foreach (var selected_cell_block in selected_cells.ToString().Split(delitem))
            {
                if (selected_cells.ToString().Length > 0)
                {
                    if (selected_cell_block.Contains(":"))
                    {
                        int min_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1].Split(delsep)[0]);
                        int max_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[2].Split(delcol)[0]);

                        int cols = max_cols - min_cols + 1;

                        int min_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                        int max_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[2].Split(delcol)[0]);

                        int rows = max_rows - min_rows + 1;

                        SelectedSize.Text = "x = " + cols + ", y = " + rows;
                    }
                    else
                    {
                        // todo fix when a row or columns is selected!
                        try
                        {
                            int cols = 1 + int.Parse(selected_cell_block.ToString().Split(delcol)[1]);
                            int rows = 1 + int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                            SelectedSize.Text = "x = " + cols + ", y = " + rows;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("WARNING: TODO fix when row or column is selected!");
                        }
                    }

                }
                else
                {
                    SelectedSize.Text = "x = " + 1 + ", y = " + 1;
                }
            }
        }

        private void gridControl1_CellClick(object sender, Syncfusion.Windows.Forms.Grid.GridCellClickEventArgs e)
        {
            label13.Text = "x = " + e.ColIndex + ", y = " + e.RowIndex;
        }

        private void colorUIControl1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set selected color
            // add selected image to each selected cell

            if (listView3.SelectedItems.Count > 0)
            {
                var item = listView3.SelectedItems[0];
                Color color = item.BackColor;
                pictureBox3.BackColor = color;


                if (FillCheckBox.Checked)
                {
                    //item to replace
                    Color color_to_replace = gridControl1[gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex].BackColor;
                    List<Point> fill_points = new List<Point>();
                    bool all_found = false;
                    Point current = new Point(gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex);
                    fill_points.Add(current);
                    int i = 0;

                    while (!all_found)
                    {
                        if (i >= fill_points.Count || i > fill_tresh)
                        {
                            all_found = true;
                            continue;
                        }
                        current = fill_points[i];
                        if (current.X > 0)
                        {
                            if (!fill_points.Contains(new Point(current.X - 1, current.Y)))
                            {
                                if (gridControl1[current.X - 1, current.Y].BackColor == color_to_replace)
                                {
                                    fill_points.Add(new Point(current.X - 1, current.Y));
                                }
                            }
                        }

                        if (current.X > 0 && current.Y > 0)
                            if (!fill_points.Contains(new Point(current.X - 1, current.Y - 1)))
                                if (gridControl1[current.X - 1, current.Y - 1].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X - 1, current.Y - 1));
                                }

                        if (current.Y > 0)
                            if (!fill_points.Contains(new Point(current.X, current.Y - 1)))
                                if (gridControl1[current.X, current.Y - 1].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X, current.Y - 1));
                                }
                        if (current.Y > 0 && current.X < gridControl1.RowCount)
                            if (!fill_points.Contains(new Point(current.X + 1, current.Y - 1)))
                                if (gridControl1[current.X + 1, current.Y - 1].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X + 1, current.Y - 1));
                                }
                        if (current.X < gridControl1.RowCount)
                            if (!fill_points.Contains(new Point(current.X + 1, current.Y)))
                                if (gridControl1[current.X + 1, current.Y].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X + 1, current.Y));
                                }
                        if (current.X < gridControl1.RowCount && current.Y < gridControl1.ColCount)
                            if (!fill_points.Contains(new Point(current.X + 1, current.Y + 1)))
                                if (gridControl1[current.X + 1, current.Y + 1].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X + 1, current.Y + 1));
                                }
                        if (current.Y < gridControl1.ColCount)
                            if (!fill_points.Contains(new Point(current.X, current.Y + 1)))
                                if (gridControl1[current.X, current.Y + 1].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X, current.Y + 1));
                                }
                        if (current.X > 0 && current.Y < gridControl1.ColCount)
                            if (!fill_points.Contains(new Point(current.X - 1, current.Y + 1)))
                                if (gridControl1[current.X - 1, current.Y + 1].BackColor == color_to_replace)
                                {

                                    fill_points.Add(new Point(current.X - 1, current.Y + 1));
                                }

                        i++;
                    }


                    foreach (Point p in fill_points)
                    {
                        gridControl1[p.X, p.Y].BackColor = color;
                    }

                }
                else
                {
                    var selected_cells = gridControl1.Selections.Ranges;
                    char[] delitem = { ';' };
                    foreach (var selected_cell_block in selected_cells.ToString().Split(delitem))
                    {
                        if (selected_cells.ToString().Length > 0)
                        {
                            if (selected_cell_block.Contains(":"))
                            {

                                int min_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1].Split(delsep)[0]);
                                int max_cols = int.Parse(selected_cell_block.ToString().Split(delcol)[2].Split(delcol)[0]);

                                int min_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                                int max_rows = int.Parse(selected_cell_block.ToString().Split(delrow)[2].Split(delcol)[0]);


                                for (int i = min_cols; i <= max_cols; i++)
                                {
                                    for (int j = min_rows; j <= max_rows; j++)
                                    {
                                        gridControl1[j, i].BackColor = color;
                                    }
                                }
                            }
                            else
                            {
                                int cols = int.Parse(selected_cell_block.ToString().Split(delcol)[1]);
                                int rows = int.Parse(selected_cell_block.ToString().Split(delrow)[1].Split(delcol)[0]);
                                gridControl1[rows, cols].BackColor = color;

                            }
                        }
                        else
                        {
                            if (gridControl1.CurrentCell.ColIndex > 0)
                            {
                                gridControl1[gridControl1.CurrentCell.RowIndex, gridControl1.CurrentCell.ColIndex].BackColor = color;

                            }
                        }
                    }
                }
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void SelectedSize_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // load our selected image
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string loaded_image_path = openFileDialog1.FileName;

                using (Stream bmpStream = System.IO.File.Open(loaded_image_path, System.IO.FileMode.Open))
                {
                    Image image = Image.FromStream(bmpStream);

                    loaded_image = new Bitmap(image);

                    label11.Text = loaded_image.Width.ToString() + "x" + loaded_image.Height.ToString();
                    pictureBox1.Image = loaded_image;

                }

                // loaded_image

                get_recommended_colors();
            }

        }

        private void get_recommended_colors()
        {
            int pixel_tresh = 10000;
            int img_resize = 100;
            List<Color> pixelColors = new List<Color>();
            List<int> occurence = new List<int>();

            //Console.WriteLine("Will now read "+ loaded_image.Width +"x"+ loaded_image.Height + " pixels!");
            Bitmap tmp = loaded_image;
            if (loaded_image.Width*loaded_image.Height > pixel_tresh)
            {
               tmp = new Bitmap( loaded_image, new Size(img_resize, img_resize));
            }
            LockBitmap lockBitmap = new LockBitmap(tmp);
            lockBitmap.LockBits();

            float count = 0;
            for (int i = 0; i < lockBitmap.Width; i++)
            {
                for (int j = 0; j < lockBitmap.Height; j++)
                {
                    // Console.WriteLine("Handle pixel " +count + " out of " + lockBitmap.Width * lockBitmap.Height);
                    progressBar1.Value =(int) ((count / (lockBitmap.Width * lockBitmap.Height)) * 100);
                   // Console.WriteLine((float)(count / (lockBitmap.Width * lockBitmap.Height)));
                    Color currentPixel = lockBitmap.GetPixel(i, j);
                    if (pixelColors.Contains(currentPixel))
                    {
                        occurence[pixelColors.IndexOf(currentPixel)]++;
                    }
                    else
                    {
                        occurence.Add(1);
                        pixelColors.Add(currentPixel);
                    }
                    /* Might help to sleep here but it will slow down the implementation
                    System.Threading.Thread.Sleep(1);
                    */
                    count++;
                }
            }
            lockBitmap.UnlockBits();
            // insertsort implementation
            sortedOccurence = new List<int>();
            sortedPixelColors = new List<Color>();

            progressBar1.Value = 100;
            for (int i = 0; i < occurence.Count; i++)
            {

                progressBar1.Value = (int) (((float)i /(occurence.Count))*100);
                if (sortedOccurence.Count == 0)
                {
                    sortedOccurence.Add(occurence[i]);
                    sortedPixelColors.Add(pixelColors[i]);
                }
                else
                {
                    for (int index = 0; index <= sortedOccurence.Count; index++)
                    {
                        if (index == sortedOccurence.Count)
                        {
                            sortedOccurence.Insert(index, occurence[i]);
                            sortedPixelColors.Insert(index, pixelColors[i]);
                            break;
                        }
                        else
                        {
                            if (occurence[i] >= sortedOccurence[index])
                            {
                                sortedOccurence.Insert(index, occurence[i]);
                                sortedPixelColors.Insert(index, pixelColors[i]);
                                break;
                            }
                        }
                    }
                }
            }
            approx_dmc_and_show();

            progressBar1.Value = 100;
        }

        private void approx_dmc_and_show()
        {
            List<int> exceptionList_in = new List<int>();
            rec_in.Clear();
            //Console.WriteLine("These IN colors are recommended:");
            foreach (int i in Enumerable.Range(0, nr_colors_in))
            {
                //  Console.WriteLine("Color: " + sortedPixelColors[i] + " occurences: " + sortedOccurence[i]);

                // is already dmc
                if (all_dmc_colors.Contains(sortedPixelColors[i]))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = all_dmc_names[all_dmc_colors.IndexOf(sortedPixelColors[i])];
                    item.BackColor = all_dmc_colors[all_dmc_colors.IndexOf(sortedPixelColors[i])];
                    rec_in.Add(item);
                    //    Console.WriteLine("This is dmc color : " + all_dmc_names[all_dmc_colors.IndexOf(sortedPixelColors[i])]);
                }
                else
                {
                    //  Console.WriteLine("This is no dmc color, we will approx it to: ");
                    int index = get_approximated_dmc_color_index(sortedPixelColors[i], exceptionList_in);
                    //Console.WriteLine(all_dmc_names[index]);
                    exceptionList_in.Add(index);
                    ListViewItem item = new ListViewItem();
                    item.Text = all_dmc_names[index];
                    item.BackColor = all_dmc_colors[index];
                    rec_in.Add(item);
                }
            }

            List<int> exceptionList_out = new List<int>();
            rec_out.Clear();
            // Console.WriteLine("These OUT colors are recommended:");
            foreach (int i in Enumerable.Range(0, nr_colors_out))
            {
                //   Console.WriteLine("Color: " + sortedPixelColors[i] + " occurences: " + sortedOccurence[i]);
                // is already dmc
                if (all_dmc_colors.Contains(sortedPixelColors[i]))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = all_dmc_names[all_dmc_colors.IndexOf(sortedPixelColors[i])];
                    item.BackColor = all_dmc_colors[all_dmc_colors.IndexOf(sortedPixelColors[i])];
                    rec_out.Add(item);
                    //    Console.WriteLine("This is dmc color : " + all_dmc_names[all_dmc_colors.IndexOf(sortedPixelColors[i])]);
                }
                else
                {
                    //     Console.WriteLine("This is no dmc color, we will approx it to: ");
                    int index = get_approximated_dmc_color_index(sortedPixelColors[i], exceptionList_out);
                    //     Console.WriteLine(all_dmc_names[index]);
                    exceptionList_out.Add(index);
                    ListViewItem item = new ListViewItem();
                    item.Text = all_dmc_names[index];
                    item.BackColor = all_dmc_colors[index];
                    rec_out.Add(item);
                }
            }

            // add to listviews
            listView6.Items.Clear();
            foreach (ListViewItem item in rec_in)
            {
                listView6.Items.Add(item);
            }
            listView7.Items.Clear();
            foreach (ListViewItem item in rec_out)
            {
                listView7.Items.Add(item);
            }
        }

        private int get_approximated_dmc_color_index(Color c, List<int> exceptionList)
        {
            /*This function calculate the closest dmc color to input color!*/
            int? best_index = null;
            int? best_approx = null;

            for (int i = 0; i < all_dmc_colors.Count; i++)
            {
                if (exceptionList.Contains(i))
                {
                    continue;
                }
                if (best_index == null)
                {
                    best_index = i;
                    best_approx = Math.Abs(all_dmc_colors[i].R - c.R) + Math.Abs(all_dmc_colors[i].G - c.G) + Math.Abs(all_dmc_colors[i].B - c.B) + Math.Abs(all_dmc_colors[i].A - c.A);
                }
                int curr_approx = Math.Abs(all_dmc_colors[i].R - c.R) + Math.Abs(all_dmc_colors[i].G - c.G) + Math.Abs(all_dmc_colors[i].B - c.B) + Math.Abs(all_dmc_colors[i].A - c.A);
                if (curr_approx < best_approx )
                {
                    best_approx = curr_approx;
                    best_index = i;
                }
            }
            return best_index ?? default(int);
        }

        private void show_recommended_colors(List<Color> clist, List<int> ilist)
        {
            for (int i = 0; i < ilist.Count; i++)
            {
                Console.WriteLine(clist[i].ToString() + " " + ilist[i].ToString());
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            // TODO add x and y

            // generate several pixelized images and show results
            if (checkBox1.Checked)
            {

            listView2.Items.Clear();
            imageList2.Images.Clear();

            }
            if (loaded_image != null)
            {
                progressBar1.Value = 5;
                // pixelize 1
                Bitmap image1 = Pixelate(loaded_image, new System.Drawing.Rectangle(0, 0, loaded_image.Width, loaded_image.Height), new Point(x,y));
                string key = listView2.Items.Count.ToString();

                imageList2.ImageSize = new Size(listview_image_size, listview_image_size);

                imageList2.Images.Add(key, image1);

                // add an item
                var listViewItem = listView2.Items.Add("");
                // and tell the item which image to use
                listViewItem.ImageKey = key;

                progressBar1.Value = 25;
                // palett color 1 
                Bitmap image2 = loaded_image;

                key = listView2.Items.Count.ToString();

                // get collected colors:
                List<Color> in_color = new List<Color>();
                foreach (Control c in flowLayoutPanel1.Controls)
                {
                    ColorBox color_box = (ColorBox)c;
                    in_color.Add(color_box.BackColor);
                }

                List<Color> out_color = new List<Color>();
                foreach (Control c in flowLayoutPanel2.Controls)
                {
                    ColorBox color_box = (ColorBox)c;
                    out_color.Add(color_box.BackColor);
                }

                // todo fix with colors here!
                Color[] colors = in_color.ToArray();
                Bitmap newImage1 = ConvertToColors(image2, colors);
                ColorPalette pal = newImage1.Palette;
                for (int i = 0; i < out_color.Count; i++)
                {
                    pal.Entries[i] = out_color[i];
                }
                newImage1.Palette = pal;

                imageList2.ImageSize = new Size(listview_image_size, listview_image_size);
                imageList2.Images.Add(key, newImage1);

                // add an item
                listViewItem = listView2.Items.Add(" ");
                // and tell the item which image to use
                listViewItem.ImageKey = key;

                progressBar1.Value = 50;
                // combine first two 1
                Bitmap image3 = image1;
                key = listView2.Items.Count.ToString();

                colors = in_color.ToArray();

                Bitmap newImage = ConvertToColors(image3, colors);
                pal = newImage.Palette;
                for (int i = 0; i < out_color.Count; i++)
                {
                    pal.Entries[i] = out_color[i];
                }
                newImage.Palette = pal;

                imageList2.ImageSize = new Size(listview_image_size, listview_image_size);
                imageList2.Images.Add(key, newImage);

                // add an item
                listViewItem = listView2.Items.Add(" ");
                // and tell the item which image to use
                listViewItem.ImageKey = key;


                progressBar1.Value = 75;

                // combine first two 1
                Bitmap image4 = Pixelate((Bitmap)newImage1.Clone(), new System.Drawing.Rectangle(0, 0, loaded_image.Width, loaded_image.Height), new Point(x, y));

                key = listView2.Items.Count.ToString();

                imageList2.ImageSize = new Size(listview_image_size, listview_image_size);

                imageList2.Images.Add(key, image4);

                // add an item
                listViewItem = listView2.Items.Add(" ");
                // and tell the item which image to use
                listViewItem.ImageKey = key;


                progressBar1.Value = 100;
            }

        }

        public static Bitmap PaintOn32bpp(Image image, Color? transparencyFillColor)
        {
            Bitmap bp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(bp))
            {
                if (transparencyFillColor.HasValue)
                    using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(255, transparencyFillColor.Value)))
                        gr.FillRectangle(myBrush, new System.Drawing.Rectangle(0, 0, image.Width, image.Height));
                gr.DrawImage(image, new System.Drawing.Rectangle(0, 0, bp.Width, bp.Height));
            }
            return bp;
        }
        public static Byte[] GetImageData(Bitmap sourceImage, out Int32 stride)
        {
            BitmapData sourceData = sourceImage.LockBits(new System.Drawing.Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            stride = sourceData.Stride;
            Byte[] data = new Byte[stride * sourceImage.Height];
            Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
            sourceImage.UnlockBits(sourceData);
            return data;
        }
        public static Byte[] Convert32BitTo8Bit(Byte[] imageData, Int32 width, Int32 height, Color[] palette, ref Int32 stride)
        {
            if (stride < width * 4)
                throw new ArgumentException("Stride is smaller than one pixel line!", "stride");
            Byte[] newImageData = new Byte[width * height];
            for (Int32 y = 0; y < height; y++)
            {
                Int32 inputOffs = y * stride;
                Int32 outputOffs = y * width;
                for (Int32 x = 0; x < width; x++)
                {
                    // 32bppArgb: Order of the bytes is Alpha, Red, Green, Blue, but
                    // since this is actually in the full 4-byte value read from the offset,
                    // and this value is considered little-endian, they are actually in the
                    // order BGRA. Since we're converting to a palette we ignore the alpha
                    // one and just give RGB.
                    Color c = Color.FromArgb(imageData[inputOffs + 2], imageData[inputOffs + 1], imageData[inputOffs]);
                    // Match to palette index
                    newImageData[outputOffs] = (Byte)GetClosestPaletteIndexMatch(c, palette);
                    inputOffs += 4;
                    outputOffs++;
                }
            }
            stride = width;
            return newImageData;
        }

        public static Int32 GetClosestPaletteIndexMatch(Color col, Color[] colorPalette)
        {
            Int32 colorMatch = 0;
            Int32 leastDistance = Int32.MaxValue;
            Int32 red = col.R;
            Int32 green = col.G;
            Int32 blue = col.B;
            for (Int32 i = 0; i < colorPalette.Length; i++)
            {
                Color paletteColor = colorPalette[i];
                Int32 redDistance = paletteColor.R - red;
                Int32 greenDistance = paletteColor.G - green;
                Int32 blueDistance = paletteColor.B - blue;
                Int32 distance = (redDistance * redDistance) + (greenDistance * greenDistance) + (blueDistance * blueDistance);
                if (distance >= leastDistance)
                    continue;
                colorMatch = i;
                leastDistance = distance;
                if (distance == 0)
                    return i;
            }
            return colorMatch;
        }

        public static Bitmap ConvertToColors(Bitmap image, Color[] colors)
        {
            Int32 width = image.Width;
            Int32 height = image.Height;
            Int32 stride;
            Byte[] hiColData;
            // use "using" to properly dispose of temporary image object.
            using (Bitmap hiColImage = PaintOn32bpp(image, colors[0]))
                hiColData = GetImageData(hiColImage, out stride);
            Byte[] eightBitData = Convert32BitTo8Bit(hiColData, width, height, colors, ref stride);
            return BuildImage(eightBitData, width, height, stride, PixelFormat.Format8bppIndexed, colors, Color.Black);
        }

        public static Bitmap BuildImage(Byte[] sourceData, Int32 width, Int32 height, Int32 stride, PixelFormat pixelFormat, Color[] palette, Color? defaultColor)
        {
            Bitmap newImage = new Bitmap(width, height, pixelFormat);
            BitmapData targetData = newImage.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, newImage.PixelFormat);
            Int32 newDataWidth = ((Image.GetPixelFormatSize(pixelFormat) * width) + 7) / 8;
            // Compensate for possible negative stride on BMP format.
            Boolean isFlipped = stride < 0;
            stride = Math.Abs(stride);
            // Cache these to avoid unnecessary getter calls.
            Int32 targetStride = targetData.Stride;
            Int64 scan0 = targetData.Scan0.ToInt64();
            for (Int32 y = 0; y < height; y++)
                Marshal.Copy(sourceData, y * stride, new IntPtr(scan0 + y * targetStride), newDataWidth);
            newImage.UnlockBits(targetData);
            // Fix negative stride on BMP format.
            if (isFlipped)
                newImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            // For indexed images, set the palette.
            if ((pixelFormat & PixelFormat.Indexed) != 0 && palette != null)
            {
                ColorPalette pal = newImage.Palette;
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                {
                    if (i < palette.Length)
                        pal.Entries[i] = palette[i];
                    else if (defaultColor.HasValue)
                        pal.Entries[i] = defaultColor.Value;
                    else
                        break;
                }
                newImage.Palette = pal;
            }
            return newImage;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private Bitmap Pixelate(Bitmap image, System.Drawing.Rectangle rectangle, Point pixelateSize)
        {
            to_cells = new List<Cell>();
            Bitmap pixelated = new System.Drawing.Bitmap(image.Width, image.Height);

            int j = 1;

            // make an exact copy of the bitmap provided
            using (Graphics graphics = System.Drawing.Graphics.FromImage(pixelated))
                graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            // look at every pixel in the rectangle while making sure we're within the image bounds
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx += pixelateSize.X)
            {

                int i = 1;
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy += pixelateSize.Y)
                {
                    Int32 offsetX = pixelateSize.X / 2;
                    Int32 offsetY = pixelateSize.Y / 2;


                    // make sure that the offset is within the boundry of the image
                    while (xx + offsetX >= image.Width) offsetX--;
                    while (yy + offsetY >= image.Height) offsetY--;

                    // get the pixel color in the center of the soon to be pixelated area
                    Color pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);

                    // Console.WriteLine(i.ToString() + " " + j.ToString());
                    to_cells.Add(new Cell(i, j, pixel, null));

                    i++;

                    // for each pixel in the pixelate size, set it to the center color
                    for (Int32 x = xx; x < xx + pixelateSize.X && x < image.Width; x++)
                        for (Int32 y = yy; y < yy + pixelateSize.Y && y < image.Height; y++)
                            pixelated.SetPixel(x, y, pixel);
                }
                j++;
            }

            return pixelated;
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

     

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception)
            {
                // will pass this for now
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // open dialog
            // load our selected image
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string pattern_path = openFileDialog1.FileName;

                gridControl1.InitializeFromXml(pattern_path);

                // open xml or excel document

                // visualize on gridControl1
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // resize gridcontrol cells
            gridControl1.ResetColWidthEntries();
            gridControl1.ResetRowHeightEntries();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // reset gridcontrol
            gridControl1.ClearCells(GridRangeInfo.Table(), true);
        }

        private void FillCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void integerTextBox1_TextChanged(object sender, EventArgs e)
        {
            fill_tresh = int.Parse(integerTextBox1.Text);
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            // TODO clear when there is only one cell selected
            gridControl1.ClearCells(gridControl1.Selections.Ranges, true);
            
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            add_border_all();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            add_border(get_selected_cells());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int i = int.Parse(integerTextBox2.Text);
            cell_size = new Size(i, i);
            gridControl1.SetColWidth(0, gridControl1.ColCount, i);
            gridControl1.SetRowHeight(0, gridControl1.RowCount, i);
        }

        private void integerTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // clear grid
            gridControl1.ClearCells(gridControl1.Selections.Ranges, true);

            // set created image to our cells
            do_changes(to_cells);

            // jump to other tab
            tabControl1.SelectedIndex = 0;


        }

        private void integerTextBox3_TextChanged(object sender, EventArgs e)
        {
            nr_colors_in = int.Parse(integerTextBox3.Text);

            // set size of listbox1 and update
            flowLayoutPanel1.Controls.Clear();

            foreach (int i in Enumerable.Range(0, nr_colors_in))
            {
                flowLayoutPanel1.Controls.Add(new ColorBox());
            }
            approx_dmc_and_show();
        }

        private void integerTextBox4_TextChanged(object sender, EventArgs e)
        {
            nr_colors_out = int.Parse(integerTextBox4.Text);

            // set size of listbox2 and update
            flowLayoutPanel2.Controls.Clear();

            foreach (int i in Enumerable.Range(0, nr_colors_out))
            {
                flowLayoutPanel2.Controls.Add(new ColorBox());
            }
            approx_dmc_and_show();
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void listView6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

            flowLayoutPanel1.Controls.Clear();
            // in recommended color use 
            foreach (ListViewItem item in rec_in)
            {
                ColorBox tmpBox = new ColorBox();
                tmpBox.BackColor = item.BackColor;
                flowLayoutPanel1.Controls.Add(tmpBox);
            }
        }   

        private void button14_Click(object sender, EventArgs e)
        {

            flowLayoutPanel2.Controls.Clear();
            // out recommended color use
            foreach (ListViewItem item in rec_out)
            {
                ColorBox tmpBox = new ColorBox();
                tmpBox.BackColor = item.BackColor;
                flowLayoutPanel2.Controls.Add(tmpBox);
            }
        }

        private void groupBox15_Enter(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void integerTextBox5_TextChanged(object sender, EventArgs e)
        {
            listview_image_size = int.Parse(integerTextBox5.Text);
        }

        private void integerTextBox6_TextChanged(object sender, EventArgs e)
        {
            x = int.Parse(integerTextBox6.Text);
        }

        private void integerTextBox7_TextChanged(object sender, EventArgs e)
        {
            y = int.Parse(integerTextBox7.Text);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void KnittingTab_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox16_Enter(object sender, EventArgs e)
        {

        }

        private void listView7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void listView6_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox10_Enter(object sender, EventArgs e)
        {

        }

        private void listView5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            // open flowchart file
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string loaded_diagram_path = openFileDialog1.FileName;

                try { 
                diagram1.LoadBinary(loaded_diagram_path);
                this.diagram1.Refresh();
                } catch (Exception)
                {
                    // do nothing here, file is not of correct format!
                }
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            // save flowchart to file

            if (this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            
            {
                // save as changeable
                string FileName = this.saveFileDialog1.FileName;
                this.diagram1.SaveBinary(FileName);

                // save for printing
                Bitmap img = (Bitmap)diagram1.ExportDiagramAsImage(true);
                img.Save("Test.png");

            }

        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CrochetTab_Click(object sender, EventArgs e)
        {

        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox13_Enter(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox12_Enter(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            // Merge Cells
            List<Point> selected = get_selected_cells();
            //Console.WriteLine(selected[0].X +" "+ selected[0].Y + " " + selected[selected.Count - 1].X + " " + selected[selected.Count - 1].Y);
            gridControl1.CoveredRanges.Add(GridRangeInfo.Cells(selected[0].Y, selected[0].X , selected[selected.Count-1].Y, selected[selected.Count-1].X));
        }

        private void init_flowchart()
        {
            //Enable scroll bars
            diagram1.HScroll = true;
            diagram1.VScroll = true;

        }

        private void diagram1_Click(object sender, EventArgs e)
        {
            /**/
         
            //Create a rectangular node
            Syncfusion.Windows.Forms.Diagram.Rectangle rectangle
                = new Syncfusion.Windows.Forms.Diagram.Rectangle(120, 120, 100, 70);

            //Style the rectangular node
            rectangle.FillStyle.Type = FillStyleType.LinearGradient;
            rectangle.FillStyle.Color = Color.FromArgb(128, 0, 0);
            rectangle.FillStyle.ForeColor = Color.FromArgb(225, 0, 0);

            rectangle.ShadowStyle.Visible = true;

            //Border style
            rectangle.LineStyle.LineColor = Color.RosyBrown;
            rectangle.LineStyle.LineWidth = 2.0f;
            rectangle.LineStyle.LineJoin = LineJoin.Miter;

            //Add a label to the rectangular node
            Syncfusion.Windows.Forms.Diagram.Label label
               = new Syncfusion.Windows.Forms.Diagram.Label();
            label.Text = "Hello!";
            label.FontStyle.Family = "Arial";
            label.FontColorStyle.Color = Color.White;
            rectangle.Labels.Add(label);

            //Add the rectangular node to the model
            diagram1.Model.AppendChild(rectangle);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // clear flowchart diagram
            diagram1.Model.Clear();
        }
    }

}