using OpenCvSharp;
using System.Drawing;

namespace DatasetImageProcessing
{
    // Class to process images by changing the background to a color or to alpha (transparent)
    public class ImageProcessing
    {
        // Function to change background color to the given RGB color
        public Mat changeColor(Mat org_image, Mat alpha_image, Color color)
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
        
        // binary image with white lines will get filled with white pixels
        private static Mat fillOutlines(Mat input) 
        {
            for (int y = 0; y < input.Rows; y++)
            {
                int begin_x = 0;
                for (int x = 0; x < input.Cols; x++)
                {
                    if (input.At<byte>(y, x) == 0)
                    {
                        begin_x = x;
                    }
                    else
                    {
                        break;
                    }
                }
                int end_x = input.Cols;
                if (begin_x != end_x)
                {
                    for (int x = end_x; x > 0; x--)
                    {
                        if (input.At<byte>(y, x) == 0)
                        {
                            end_x = x;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (begin_x < end_x)
                {
                    for (int x = begin_x; x < end_x; x++)
                    {
                        input.At<byte>(y, x) = 255;
                    }
                }
            }
            return input;        
        }
        
        //original image will get a transparant background (alpha to 0). 
        public static Mat makeBackgroundTransparent(Mat original)
        {
            if (original.Empty()) throw new ArgumentNullException(paramName: nameof(original), message: "makeBackgroundTransparent Mat original is empty");

            Mat _original = new();
            Cv2.CvtColor(original, _original, ColorConversionCodes.BGR2GRAY);
            Mat alpha = _original.Clone();
            //gaussian blur corresponding to image size.
            Mat _image_gaussian = new();
            int _gaussian_filter_size = 68;
            var _gaussian_size = decimal.ToInt32(original.Width / _gaussian_filter_size);
            //check if the size is uneven
            if(_gaussian_size%2 == 0)
            {
                _gaussian_size -= 1;
            }
            Size s = new(_gaussian_size, _gaussian_size);
            Cv2.GaussianBlur(alpha, _image_gaussian, s, 0);

            //increase contrast and brightness for better canny edge detection
            var _contrast = 1.5; //Contrast control
            var _brightness = 1;//  Brightness control
            Mat _image_brighter = new();
            Cv2.ConvertScaleAbs(_image_gaussian, _image_brighter, _contrast, _brightness);

            //apply canny edge detection
            Mat _image_canny = new();
            int _canny_first_iteration = 70;
            int _canny_second_iteration = 50;
            Cv2.Canny(_image_brighter, _image_canny, _canny_first_iteration, _canny_second_iteration, apertureSize:3,L2gradient:false);

            //dilate the image so the edges are thicker and connect better
            Mat _image_dilation = new();
            Cv2.Dilate(_image_canny,_image_dilation, element:new Mat());

            //fill the outlines of the canny edges
            Mat _image_filled_lines = fillOutlines( _image_dilation );

            //get contours of image
            var contoursExternalForeground = Cv2.FindContoursAsArray(_image_filled_lines, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            
            //draw contours on the 
            alpha.DrawContours(contoursExternalForeground, -1, new Scalar(255), thickness: -1);

            //merge the Foreground mask with the original image channels so that the background is transparent
            Mat dst = new();            
            Mat[] bgr = Cv2.Split(original);
            var bgra = new[] { bgr[0], bgr[1], bgr[2], alpha };
            Cv2.Merge(bgra, dst);

            //return the final image with transparent background
            return dst;
        }

    }
}
