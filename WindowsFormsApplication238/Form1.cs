using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WindowsFormsApplication238
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var fileDialog = new OpenFileDialog {Filter = "Image file|*.jpg;*.png;*.bmp"};
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                var image = (Bitmap)Bitmap.FromFile(fileDialog.FileName);
                EdgeDetection(image, 15);
                BackgroundImage = image;
                Height = image.Height;
                Width = image.Width;
            }
        }

        /*Полякова Полина 3к 91 гр. Лабораторная 1. 18.	Выделение контуров изображения.*/
        public static void EdgeDetection(Bitmap image, float limit)
        {
            Bitmap cloneImage = (Bitmap)image.Clone();

            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSource = cloneImage.LockBits(new Rectangle(0, 0, cloneImage.Width, cloneImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int step = bmData.Stride;

            unsafe
            {
                byte* pointer = (byte*)(void*)bmData.Scan0;
                byte* pSource = (byte*)(void*)bmSource.Scan0;

                int offset = step - image.Width * 3;
                int imageWidth = image.Width - 1;
                int imageHeight = image.Height - 1;

                for (int y = 0; y < imageHeight; ++y)
                {
                    for (int x = 0; x < imageWidth; ++x)
                    {
                        //  | p0 |  p1  |
                        //  |    |  p2  |
                        var p0 = ToGray(pSource);
                        var p1 = ToGray(pSource + 3);
                        var p2 = ToGray(pSource + 3 + step);

                        if (Math.Abs(p1 - p2) + Math.Abs(p1 - p0) > limit)
                            pointer[0] = pointer[1] = pointer[2] = 255;
                        else
                            pointer[0] = pointer[1] = pointer[2] = 0;

                        pointer += 3;
                        pSource += 3;
                    }
                    pointer += offset;
                    pSource += offset;
                }
            }

            image.UnlockBits(bmData);
            cloneImage.UnlockBits(bmSource);
        }

        static unsafe float ToGray(byte* bgr)
        {
            return bgr[2] * 0.3f + bgr[1] * 0.59f + bgr[0] * 0.11f;
        }
    }
}
