using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Piko
{
    public partial class Form1 : Form
    {
        private string moveDirectory = "";

        public Form1() => Initialize();

        private void Initialize()
        {
            InitializeComponent();
            pictureViewer1.PicturesUpdated += PictureViewer1_PicturesUpdated;
            pictureViewer1.CurrentPictureChanged += PictureViewer1_CurrentPictureChanged;
            timer4.Start();
            try
            {
                pictureViewer1.SetImage(Environment.GetCommandLineArgs()[1], false);
            }
            catch (Exception)
            {
            }
        }

        private void PictureViewer1_CurrentPictureChanged(PictureViewer sender, EventArgs e)
        {
            Text = sender.currentPicture;
            textBox1.Focus();
        }

        private void PictureViewer1_PicturesUpdated(PictureViewer sender, EventArgs e)
        {
           
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureViewer1.OpenImage();
            dToolStripMenuItem = pictureViewer1.pictureItems;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    timer6.Start();
                    break;
                case Keys.Right:
                    timer5.Start();
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            timer5.Stop();
            timer6.Stop();
        }

        private void pictureViewer1_Resize(object sender, EventArgs e)
        {
            pictureViewer1.NextPicture();
            pictureViewer1.PreviousPicture();
          
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
                panel3.Left = Width / 2;
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            timer1.Start();
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Stop();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            var multiplier = 1.1;

            Image MyImage = pictureViewer1.Image;

            Bitmap MyBitMap = new Bitmap(MyImage, Convert.ToInt32(MyImage.Width * multiplier),
                Convert.ToInt32(MyImage.Height * multiplier));

            Graphics Graphic = Graphics.FromImage(MyBitMap);

            Graphic.InterpolationMode = InterpolationMode.High;

            pictureViewer1.Image = MyBitMap;
        }

        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            timer2.Start();
        }

        private void button10_MouseUp(object sender, MouseEventArgs e)
        {
            timer2.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            var multiplier = 1.1;

            Image MyImage = pictureViewer1.Image;

            Bitmap MyBitMap = new Bitmap(MyImage, Convert.ToInt32(MyImage.Width / multiplier),
                Convert.ToInt32(MyImage.Height / multiplier));

            Graphics Graphic = Graphics.FromImage(MyBitMap);

            Graphic.InterpolationMode = InterpolationMode.High;

            pictureViewer1.Image = MyBitMap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                pictureViewer1.ResetZoom();
                textBox1.Focus();
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                pictureViewer1.PreviousPicture();
                textBox1.Focus();
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                pictureViewer1.NextPicture();
                textBox1.Focus();
            }
            catch { }
        }

        private void pictureViewer1_SizeChanged(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureViewer1.RotatePicture();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var s = new SetSpeed();
            if (s.ShowDialog(button7) != DialogResult.OK || pictureViewer1.currentPicture == "") return;
            timer3.Interval = (int)(1000 * s.Speed);
            timer3.Start();
            button7.Hide();
            button8.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            button7.Show();
            button8.Hide();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            pictureViewer1.NextPicture();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            panel3.Left = (panel2.ClientSize.Width - panel3.Width) / 2;
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            pictureViewer1.NextPicture();
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            pictureViewer1.PreviousPicture();
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            try
            {
                MoveFileCopy();
            }
            catch { MessageBox.Show("Error"); }
        }

        private void MoveFileCopy()
        {
            if (moveDirectory != "") goto CheckExists;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) { moveDirectory = folderBrowserDialog1.SelectedPath; }
            else { return; }

            CheckExists:
            var newFile = moveDirectory + @"\" + new FileInfo(Text).Name;
            if (!File.Exists(newFile)) goto StartSave;

            var message = MessageBox.Show(this, "File exists in the destination already", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (message) { case DialogResult.Yes: pictureViewer1.Image.Save(newFile); break; default: return; }

            StartSave:
            pictureViewer1.Image.Save(newFile);
        }

        private void changeDestinationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) moveDirectory = folderBrowserDialog1.SelectedPath;
        }

        private void pictureViewer1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        

        private void button10_Click_1(object sender, EventArgs e)
        {
            
        }

        private void pictureViewer1_PicturesUpdated_1(PictureViewer sender, EventArgs e)
        {
            try
            {
                dToolStripMenuItem.DropDownItems.Clear();
                dToolStripMenuItem.DropDownItems.AddRange(pictureViewer1.pictureItems.DropDownItems);
            }
            catch (Exception)
            {
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureViewer1.RotatePicture();
        }

        
    }
}