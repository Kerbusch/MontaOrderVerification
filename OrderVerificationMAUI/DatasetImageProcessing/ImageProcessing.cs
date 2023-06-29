using OpenCvSharp;
using System.Drawing;

namespace DatasetImageProcessing
{
    // Class to process images by changing the background to a color or to alpha (transparent)
    public class ImageProcessing
    {
        // Function to change background color to the given RGB color
        public Mat changeColor(Mat org_image, Mat alpha_image, System.Drawing.Color color)
        {
            Mat new_mat = alpha_image.Clone();
            for (int y = 0; y < alpha_image.Rows; ++y)
            {
                for (int x = 0; x < alpha_image.Cols; ++x)
                {
                    if (alpha_image.At<Vec4b>(y, x)[3] == 0)
                    {
                        new_mat.At<Vec4b>(y, x)[0] = color.B;
                        new_mat.At<Vec4b>(y, x)[1] = color.G;
                        new_mat.At<Vec4b>(y, x)[2] = color.R;
                    }
                    else
                    {
                        new_mat.At<Vec4b>(y, x) = org_image.At<Vec4b>(y, x);
                    }
                }
            }

            return new_mat;
        }

        // Function to change background pixels to alpha (transparent) and thus return a image with a transparent background
        public Mat getAlphaMat(Mat image)
        {
            Mat input = new();
            OpenCvSharp.Size s = new OpenCvSharp.Size(9, 9);
            Cv2.GaussianBlur(image, input, s, 0);
            //Cv2.CvtColor(input, input, ColorConversionCodes.BGR2BGRA);
            //Mat[] channels = Cv2.Split(input);
            //Cv2.Merge(channels, input);

            for (int y = 0; y < input.Rows; ++y)
            {
                for (int x = 0; x < input.Cols; ++x)
                {
                    if (input.At<Vec4b>(y, x)[0] <= 50 && input.At<Vec4b>(y, x)[1] <= 50 && input.At<Vec4b>(y, x)[2] <= 50)
                    {
                        input.At<Vec4b>(y, x)[3] = 0;
                    }
                }
            }

            return input;
        }

    }
}
