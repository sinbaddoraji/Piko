using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Piko
{
    public partial class Form1 : Form
    {
        private Timer centralizePanel = new Timer();
        private Timer zoomTimer = new Timer();
        private string moveDirectory = "";
        private bool zoomOut = true;

        public Form1()
        {
            InitializeComponent();
            pictureViewer1.CurrentPictureChanged += delegate (PictureViewer sender, EventArgs e)
            {
                Text = sender.currentPicture;
                textBox1.Focus();
            };
            centralizePanel.Tick += delegate {
                panel3.Left = (panel2.ClientSize.Width - panel3.Width) / 2; 
            };
            zoomTimer.Tick += Zoom;

            centralizePanel.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) pictureViewer1.SetImage(args[1], false);
        }
        #region Zoom

        private void Button1_MouseDown(object sender, MouseEventArgs e) => StartZoom(false);

        private void Button10_MouseDown(object sender, MouseEventArgs e) => StartZoom(true);

        private void StopZoom(object sender, MouseEventArgs e) => zoomTimer.Stop();

        private void StartZoom(bool z)
        {
            zoomOut = z;
            zoomTimer.Start();
        }

        private void Zoom(object sender, EventArgs e)
        {
            var multiplier = zoomOut ? 0.9 : 1.1;

            Image MyImage = pictureViewer1.Image;

            Bitmap MyBitMap = new Bitmap(MyImage, Convert.ToInt32(MyImage.Width * multiplier),
                Convert.ToInt32(MyImage.Height * multiplier));

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

        #endregion Zoom

        #region Slide Show

        private void PlayButtonSwitch(bool fromPlay)
        {
            if (fromPlay)
            {
                button7.Hide();
                button8.Show();
            }
            else
            {
                button7.Show();
                button8.Hide();
            }
        }

        private void PlaySlideShow(object sender, EventArgs e)
        {
            var s = new SetSpeed();
            if (s.ShowDialog(button7) != DialogResult.OK || pictureViewer1.currentPicture == "") return;
            playSpeed.Interval = s.Speed;
            PlayButtonSwitch(true);
            playSpeed.Start();
        }

        private void PauseSlideShow(object sender, EventArgs e)
        {
            playSpeed.Stop();
            PlayButtonSwitch(false);
        }

        #endregion Slide Show

        #region Image Manipulation

        private void NextPicture(object sender, EventArgs e) => pictureViewer1.NextPicture();

        private void PreviousPicture(object sender, EventArgs e) => pictureViewer1.PreviousPicture();

        private void RotatePicture(object sender, EventArgs e) => pictureViewer1.RotatePicture();

        private void RotateImage(object sender, EventArgs e) => pictureViewer1.RotatePicture();

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureViewer1.OpenImage();
            dToolStripMenuItem = pictureViewer1.pictureItems;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Down:
                    gotoNextImage.Start();
                    break;

                case Keys.Right:
                case Keys.Up:
                    gotoPreviousImage.Start();
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (gotoPreviousImage.Enabled) gotoPreviousImage.Stop();
            if (gotoNextImage.Enabled) gotoNextImage.Stop();
        }

        private void PictureViewer1_Resize(object sender, EventArgs e)
        {
            pictureViewer1.NextPicture();
            pictureViewer1.PreviousPicture();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureViewer1.PreviousPicture();
            textBox1.Focus();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            pictureViewer1.NextPicture();
            textBox1.Focus();
        }

        #endregion Image Manipulation

        #region Extra Functionality

        private void Form1_Resize(object sender, EventArgs e) => panel3.Left = Width / 2;

        private void MoveImage(object sender, EventArgs e) => MoveFileCopy();

        private void MoveFileCopy()
        {
            try
            {
                if (moveDirectory != "") goto CheckExists;

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) moveDirectory = folderBrowserDialog1.SelectedPath;
                else return;

                CheckExists:
                var newFile = moveDirectory + @"\" + new FileInfo(Text).Name;
                if (!File.Exists(newFile)) goto StartSave;

                var message = MessageBox.Show(this, "File exists in the destination already", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (message) { case DialogResult.Yes: pictureViewer1.Image.Save(newFile); break; default: return; }

                StartSave:
                pictureViewer1.Image.Save(newFile);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void ChangeDestinationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                moveDirectory = folderBrowserDialog1.SelectedPath;
        }

        private void DoPrintImage(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() != DialogResult.OK) return;

            printDocument1.PrinterSettings = printDialog1.PrinterSettings;
            printDocument1.DocumentName = Text;
            printDocument1.Print();
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //Adjust the size of the image to the page to print the full image without loosing any part of it
            e.Graphics.DrawImage(Image.FromFile(pictureViewer1.currentPicture), e.MarginBounds);
        }

        #endregion Extra Functionality

        
    }
}