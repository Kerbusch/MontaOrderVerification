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
        public CameraModule() 
        { 
        }

        // Take picture and save it at the given path
        public static void takePicture(string pictureFilePath)
        {
            Mat frame = new Mat();
            VideoCapture capture = new VideoCapture(1);

            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);
            
            capture.Read(frame);

            frame.Save(pictureFilePath);

            frame.Dispose();
            capture.Dispose();
        }
    }
}

