using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace DatasetImageProcessing
{
    // Class to process images by changing the background to a color or to alpha (transparent)
    internal class ImageProcessing
    {
        // Constructor
        public ImageProcessing() { }

        // Function to change background color to the given RGB color
        public Mat changeColor(Mat image, int[] RGB)
        {
            Mat new_mat = image;
            for (int y = 0; y < image.Rows; ++y) {
                for (int x = 0; x < image.Cols; ++x) {
                    //Console.WriteLine(y + ", " + x);
                    if (image.At<Vec4b>(y, x)[3] == 0) {
                        new_mat.At<Vec4b>(y, x)[0] = (byte)RGB[2];
                        new_mat.At<Vec4b>(y, x)[1] = (byte)RGB[1];
                        new_mat.At<Vec4b>(y, x)[2] = (byte)RGB[0];
                        new_mat.At<Vec4b>(y, x)[3] = 0;
                    }
                    else {
                        new_mat.At<Vec4b>(y, x) = image.At<Vec4b>(y, x);
                    }
                }
            }

            return new_mat;
        }

        // Function to change background pixels to alpha (transparent)
        public Mat getAlphaMat(Mat image)
        {
            Mat input = new();
            OpenCvSharp.Size s = new OpenCvSharp.Size(3, 3);
            Cv2.GaussianBlur(image, input, s, 0);
            Cv2.CvtColor(input, input, ColorConversionCodes.BGR2BGRA);
            Mat[] channels = Cv2.Split(input);
            Cv2.Merge(channels, input);

            for (int y = 0; y < input.Rows; ++y) {
                for (int x = 0; x < input.Cols; ++x) {
                    if (input.At<Vec4b>(y, x)[0] <= 100 && input.At<Vec4b>(y, x)[1] <= 100 && input.At<Vec4b>(y, x)[2] <= 100) {
                        input.At<Vec4b>(y, x)[3] = 0;
                    }
                }
            }

            return input;
        }

    }
}
