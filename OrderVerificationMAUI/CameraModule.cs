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

        public static void makeBackgroundTransparent(string pictureFilePath) {
                   

            // load as color image BGR
            OpenCvSharp.Mat input = OpenCvSharp.Cv2.ImRead(pictureFilePath);

            
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
            OpenCvSharp.Cv2.ImWrite(pictureFilePath, input_bgra);
        }


        public static void getContours(/*string pictureFilePath*/) 
        {
           

        }
    }
}

