using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Piko
{
    internal class PictureViewer : PictureBox
    {
        public delegate void PictureUpdate(PictureViewer sender, EventArgs e);

        public event PictureUpdate PicturesUpdated;

        public delegate void PictureChanged(PictureViewer sender, EventArgs e);

        public event PictureChanged CurrentPictureChanged;

        private List<string> pictures;
        public string currentPicture;

        Image innerImage;

        private string Current_Picture
        {
            set
            {
                currentPicture = value;
                try
                {
                    CurrentPictureChanged(this, new EventArgs());
                }
                catch (Exception)
                {
                }
            }
        }

        private readonly OpenFileDialog openFile;

        private int zoom = 100;

        public ToolStripMenuItem pictureItems;

        public PictureViewer()
        {
            SizeMode = PictureBoxSizeMode.CenterImage;
            openFile = new OpenFileDialog();
            this.SizeChanged += PictureViewer_SizeChanged;
        }

        private void PictureViewer_SizeChanged(object sender, EventArgs e)
        {
            if(Image != null)RefreshPicture();
        }

        private void ListImages()
        {
            var items = new ToolStripMenuItem();
            foreach (var item in pictures)
            {
                var a = new ToolStripMenuItem
                {
                    Name = item
                };
                a.Text = new FileInfo(a.Name).Name;
                a.Click += delegate
                {
                    SetImage(a.Name, false);
                };
                items.DropDownItems.Add(a);
            }
            pictureItems = items;
        }

        public void RefreshPicture()
        {
            //SetImage(currentPicture,false);
            ScaleImage(false);
        }

        public void RotatePicture()
        {
            var I = Image;
            I.RotateFlip(RotateFlipType.Rotate90FlipNone);
            Image = new Bitmap(I);
            //ScaleImage(false);
        }

        public void NextPicture()
        {
            //Get index of current picture and increase value by one
            //set current image to the image of this new index value
            var currentPictureIndex = pictures.IndexOf(currentPicture);
            try
            {
                SetImage(pictures[currentPictureIndex + 1], true);
            }
            catch (Exception)
            {
                SetImage(pictures[0], true);
            }
        }

        public void PreviousPicture()
        {
            //Get index of current picture and reduce value by one
            //set current image to the image of this new index value
            var currentPictureIndex = pictures.IndexOf(currentPicture);
            try
            {
                SetImage(pictures[currentPictureIndex - 1], true);
            }
            catch (Exception)
            {
                SetImage(pictures[pictures.Count - 1], true);
            }
        }

        public void SetImage(string imageToBeDisplayed, bool same)
        {
            zoom = 100;
            //set selected image as image to be displayed
            innerImage = Image.FromFile(imageToBeDisplayed, true);
            
            Current_Picture = imageToBeDisplayed;
            ScaleImage(false);
            Timer addPics = new Timer() { Interval = 1};
            addPics.Tick += delegate {
                var imageParent = new DirectoryInfo(imageToBeDisplayed).Parent;

                if (imageParent != null) pictures = Directory.GetFiles(imageParent.FullName).Where(IsImage).ToList();
                //ScaleImage(false);
                ListImages();
                PicturesUpdated?.Invoke(this, new EventArgs());
                addPics.Stop();
            };
            if (same == false)
            {
                addPics.Start();
            }

            
        }

        private static bool IsImage(string image)
        {
            try
            {
                Image.FromFile(image);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void OpenImage()
        {
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                SetImage(openFile.FileName, false);
            }
        }

        private void Zoom(int zoomFactor)
        {
            zoom = zoomFactor;
            ScaleImage(false);
        }

        public void ZoomUp()
        {
            Zoom(zoom + 50);
        }

        public void ZoomDown()
        {
            Zoom(zoom - 50);
        }

        public void ResetZoom()
        {
            Zoom(100);
        }

        private void ScaleImage(bool rotate)
        {
            Image currentImage;
            int imageWidth = -4;
            int imageHeight = 0;
            try
            {
                currentImage = rotate ? Image : innerImage;
                imageWidth = currentImage.Width;
                imageHeight = currentImage.Height;
            } catch { }

            if (imageWidth == -4) return;

            var image = rotate ? Image : innerImage;


            Image thumbnail =
                new Bitmap(Width, Height); // changed parm names
            Graphics graphic = Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)Width / (double)imageWidth;
            double ratioY = (double)Height / (double)imageHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;
            

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(imageHeight * ratio);
            int newWidth = Convert.ToInt32(imageWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((Width - (imageWidth * ratio)) / 2);
            int posY = Convert.ToInt32((Height - (imageHeight * ratio)) / 2);

            graphic.Clear(BackColor); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                             100L);
            


            //imageWidth *= zoom / 100;
            //imageHeight *= zoom / 100;
            

            Image = thumbnail;
        }
    }
}