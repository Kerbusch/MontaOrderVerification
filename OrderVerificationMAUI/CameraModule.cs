using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using OpenCvSharp;
using OpenCvSharp.Internal;
using OpenCvSharp.Internal.Vectors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OpenCvSharp.LineIterator;

namespace OrderVerificationMAUI
{
    public class CameraModule
    {
        // Constructor
        public CameraModule() {   }

        // Take picture and save it at the given path
        public static void takePicture(string pictureFilePath)
        {
            String liveStreamWindow = "Photo Window (Press any key to take a picture)";
            //Create a window using the specified name
            CvInvoke.NamedWindow(liveStreamWindow, Emgu.CV.CvEnum.WindowFlags.Normal);
            //set window to fullscreen
            CvInvoke.SetWindowProperty(liveStreamWindow, Emgu.CV.CvEnum.WindowPropertyFlags.FullScreen, 1);
            //Frame
            Emgu.CV.Mat frame = new Emgu.CV.Mat();
            //Videocapture
            Emgu.CV.VideoCapture capture = new Emgu.CV.VideoCapture();

            //capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
            //capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);

            capture.Read(frame);
            //Set camera capture width
            //capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
            //Set camera capture height
            //capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);
            //loop for live view of camera. When a key is pressed loop will stop
            while (CvInvoke.WaitKey(1) == -1)
            {
                capture.Read(frame);
                CvInvoke.Imshow(liveStreamWindow, frame);
            }

            frame.Save(pictureFilePath);

            frame.Dispose();
            capture.Dispose();
            CvInvoke.DestroyWindow(liveStreamWindow);
        }

        public static void getContours(/*string pictureFilePath*/) 
        {

            //gausion filter voor mask. - de huidige image. 


            //load image
            string pictureFilePath = "C:/Users/jojoo/Source/Repos/Kerbusch/MontaOrderVerification/OrderVerificationMAUI/Oot.jpg";
            //Image<Gray, Byte> img1 = new Image<Gray, Byte>(pictureFilePath);

            //String Window = "Photo Window (Press any key to take a picture)";
            //Create a window using the specified name
            //CvInvoke.NamedWindow(Window, Emgu.CV.CvEnum.WindowFlags.Normal);
            //set window to fullscreen
            //CvInvoke.SetWindowProperty(Window, Emgu.CV.CvEnum.WindowPropertyFlags.FullScreen, 0);
            //CvInvoke.Imshow(liveStreamWindow, img1);
            //CvInvoke.Imshow(Window, img1);

            //extract contours

            //string pictureFilePath2 = "C:/Users/jojoo/Source/Repos/Kerbusch/MontaOrderVerification/OrderVerificationMAUI/contours.jpg";
            //Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            //Mat hier = new Mat();
            //CvInvoke.FindContours(img1, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            //CvInvoke.DrawContours(receivedImage, contours, 0, new MCvScalar(255, 0, 0), 2);
            //Image<Gray, Byte> receivedImage = new Image<Gray, byte>(pictureFilePath2);
            //String countourWindow = "contour Window (Press any key to take a picture)";
            //Create a window using the specified name
            //CvInvoke.NamedWindow(countourWindow, Emgu.CV.CvEnum.WindowFlags.Normal);
            //set window to fullscreen
            //CvInvoke.SetWindowProperty(countourWindow, Emgu.CV.CvEnum.WindowPropertyFlags.FullScreen, 0);

            // load as color image BGR

            OpenCvSharp.Mat input = OpenCvSharp.Cv2.ImRead(pictureFilePath);
            //OpenCvSharp.Cv2.ImShow("testwindow", input);
            OpenCvSharp.Mat input_bgra = new();
            OpenCvSharp.Cv2.CvtColor(input, input_bgra, ColorConversionCodes.BGR2BGRA);
            //# First create the image with alpha channel
            //cv::cvtColor(rgb_data, rgba, cv::COLOR_RGB2RGBA);

            //# Split the image for access to alpha channel
            //out OpenCvSharp.Mat[] channels = new OpenCvSharp.Mat[4];
            /*OpenCvSharp.Mat b = new OpenCvSharp.Mat();
            OpenCvSharp.Mat g = new OpenCvSharp.Mat();
            OpenCvSharp.Mat r = new OpenCvSharp.Mat();
            OpenCvSharp.Mat a = new OpenCvSharp.Mat();*/
            OpenCvSharp.Mat[] channels = OpenCvSharp.Cv2.Split(input_bgra);

            //# Assign the mask to the last channel of the image
            //channels[3] = alpha_data;

            OpenCvSharp.Cv2.Merge(channels, input_bgra);
            //# Finally concat channels for rgba image
            //cv::merge(channels, 4, input_bgra);

            for (int y = 0; y < input_bgra.Rows; ++y)
            {
                for (int x = 0; x < input_bgra.Cols; ++x)
                {
                    Vec4b pixel = input_bgra.At<Vec4b>(y,x);
                    // if pixel is dark
                    if (pixel[0] <= 100 && pixel[1] <= 100 && pixel[2] <= 100)
                    {
                        // set alpha to zero:
                        input_bgra.At<Vec4b>(y, x)[3] = 0;
                        //pixel[0] = 0;
                        //pixel[1] = 0;
                        //pixel[2] = 0;
                    }
                }
            }
            OpenCvSharp.Cv2.ImWrite("C:/Users/jojoo/Source/Repos/Kerbusch/MontaOrderVerification/OrderVerificationMAUI/transparent.png", input_bgra);
            /*cv::Mat input = cv::imread("C:/StackOverflow/Input/transparentWhite.png");

            cv::Mat input_bgra;
            cv::cvtColor(input, input_bgra, CV_BGR2BGRA);

            // find all white pixel and set alpha value to zero:
            for (int y = 0; y < input_bgra.rows; ++y)
                for (int x = 0; x < input_bgra.cols; ++x)
                {
                    cv::Vec4b & pixel = input_bgra.at<cv::Vec4b>(y, x);
                    // if pixel is white
                    if (pixel[0] == 255 && pixel[1] == 255 && pixel[2] == 255)
                    {
                        // set alpha to zero:
                        pixel[3] = 0;
                    }
                }

            // save as .png file (which supports alpha channels/transparency)
            cv::imwrite("C:/StackOverflow/Output/transparentWhite.png", input_bgra);*/


            //show img
            //CvInvoke.Imshow(Window, img3);


        }
    }
}

