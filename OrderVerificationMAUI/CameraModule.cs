using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            Mat frame = new Mat();
            //Videocapture
            VideoCapture capture = new VideoCapture();

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

        public static void getContours(string pictureFilePath) { }
    }
}

