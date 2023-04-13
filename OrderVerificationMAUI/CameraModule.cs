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

        public static void makeBackgroundTransparent() {

            //todo:
            //gausion filter voor mask. - de huidige image. 


            //load image
            string pictureFilePath = "C:/Users/jojoo/Source/Repos/Kerbusch/MontaOrderVerification/OrderVerificationMAUI/oot3.jpg";
            

            // load as color image BGR
            OpenCvSharp.Mat original = OpenCvSharp.Cv2.ImRead(pictureFilePath);

            //gaussian blur
            OpenCvSharp.Mat input = new();
            OpenCvSharp.Size s = new OpenCvSharp.Size(3,3);
            OpenCvSharp.Cv2.GaussianBlur(original, input, s, 0);

            String Window = "Photo Window (Press any key to take a picture)";
            //Create a window using the specified name
            CvInvoke.NamedWindow(Window, Emgu.CV.CvEnum.WindowFlags.Normal);
            Cv2.ImShow(Window, input);


            OpenCvSharp.Mat input_bgra = new();
            OpenCvSharp.Cv2.CvtColor(input, input_bgra, ColorConversionCodes.BGR2BGRA);
            
            OpenCvSharp.Mat[] channels = OpenCvSharp.Cv2.Split(input_bgra);


            OpenCvSharp.Cv2.Merge(channels, input_bgra);
            //# Finally concat channels for rgba image
            //cv::merge(channels, 4, input_bgra);

            for (int y = 0; y < input_bgra.Rows; ++y)
            {
                for (int x = 0; x < input_bgra.Cols; ++x)
                {
                    Vec4b pixel = input_bgra.At<Vec4b>(y, x);
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
        }


        public static void getContours(string pictureFilePath) 
        {
            OpenCvSharp.Mat input = OpenCvSharp.Cv2.ImRead(pictureFilePath, OpenCvSharp.ImreadModes.Grayscale);
            

            OpenCvSharp.Point[][] contours; //vector<vector<Point>> contours;
            HierarchyIndex[] hierarchyIndexes; //vector<Vec4i> hierarchy;

            Cv2.FindContours(input, out contours, out hierarchyIndexes, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            Cv2.DrawContours(input, contours, 0, Scalar.White,2);



            String liveStreamWindow = "Photo Window (Press any key to take a picture)";
            //Create a window using the specified name
            CvInvoke.NamedWindow(liveStreamWindow, Emgu.CV.CvEnum.WindowFlags.Normal);
            Cv2.ImShow(liveStreamWindow, input);

        }


        public static void test()
        {

            string pictureFilePath = "C:/Users/jojoo/Source/Repos/Kerbusch/MontaOrderVerification/OrderVerificationMAUI/test.jpg";

            String liveStreamWindow = "Test Window (Press any key to take a picture)";

            
            OpenCvSharp.Cv2.NamedWindow(liveStreamWindow, WindowFlags.AutoSize);

            OpenCvSharp.Size size = new OpenCvSharp.Size(640, 480);
            Cv2.ResizeWindow(liveStreamWindow, size);
            Cv2.MoveWindow(liveStreamWindow, 250, 220);
            Cv2.SetWindowProperty(liveStreamWindow, WindowPropertyFlags.Topmost, 1);

            OpenCvSharp.Mat mat = new OpenCvSharp.Mat();
            OpenCvSharp.VideoCapture videoCapture = new OpenCvSharp.VideoCapture(0);
            
            

            while (Cv2.WaitKey(1) == -1) 
            {
                videoCapture.Read(mat);
                Cv2.ImShow(liveStreamWindow, mat);
            }

            mat.SaveImage(pictureFilePath);
            mat.Dispose();
            videoCapture.Dispose();
            Cv2.DestroyAllWindows();

            /*String liveStreamWindow = "Photo Window (Press any key to take a picture)";
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
            CvInvoke.DestroyWindow(liveStreamWindow);*/

        }


    }
}

