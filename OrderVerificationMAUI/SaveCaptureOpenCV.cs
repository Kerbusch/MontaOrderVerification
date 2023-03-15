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
    internal class SaveCaptureOpenCV
    {
        public SaveCaptureOpenCV() { }
        public static void Save()
        {
            //filepath to where picture is saved
            string pictureFilePath = "C:\\\\Users\\jojoo\\Source\\Repos\\Kerbusch\\MontaOrderVerification\\OrderVerificationMAUI\\picture.jpg";
            //name of window
            String win1 = "Photo Window (Press any key to take a picture)";
            //Create a window using the specified name
            CvInvoke.NamedWindow(win1, Emgu.CV.CvEnum.WindowFlags.Normal); 
            //set window to fullscreen
            CvInvoke.SetWindowProperty(win1, Emgu.CV.CvEnum.WindowPropertyFlags.FullScreen, 0);
            //Frame
            Mat frame = new Mat();
            //Videocapture
            VideoCapture capture = new VideoCapture();
            //Set camera capture width
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
            //Set camera capture height
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);
            //loop for live view of camera. When a key is pressed loop will stop
            while (CvInvoke.WaitKey(1) == -1)
                {
                    capture.Read(frame);
                    CvInvoke.Imshow(win1, frame);
                }
            //The latest frame will be saved on the specified file path
            frame.Save(pictureFilePath);
            //Dispose the frame
            frame.Dispose();
            //Dispose the capture
            capture.Dispose();
            //Destroy the window
            CvInvoke.DestroyWindow(win1);
            
        }
    }
}
