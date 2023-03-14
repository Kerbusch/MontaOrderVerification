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
            string pictureFilePath = "C:\\\\Users\\jojoo\\Source\\Repos\\Kerbusch\\MontaOrderVerification\\OrderVerificationMAUI\\picture.jpg";
            String win1 = "Photo Window (Press any key to take a picture)"; //The name of the window
            CvInvoke.NamedWindow(win1, Emgu.CV.CvEnum.WindowFlags.Normal); //Create the window using the specific name
            CvInvoke.SetWindowProperty(win1, Emgu.CV.CvEnum.WindowPropertyFlags.FullScreen, 0);
            Mat frame = new Mat();
            VideoCapture capture = new VideoCapture();
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);
            while (CvInvoke.WaitKey(1) == -1)
                {
                    capture.Read(frame);
                    CvInvoke.Imshow(win1, frame);
                }
            frame.Save(pictureFilePath);
            frame.Dispose();
            capture.Dispose();
            CvInvoke.DestroyWindow(win1);
            

            /*Capture camera = new Capture(0);
            CvInvoke.Resize()
            var image = camera.QueryFrame(); //take a picture
            image.Save("C:\\\\Users\\jojoo\\Source\\Repos\\Kerbusch\\MontaOrderVerification\\OrderVerificationMAUI\\picture.jpg");
            
            <Image Source="C:\\\\Users\\jojoo\\Source\\Repos\\Kerbusch\\MontaOrderVerification\\OrderVerificationMAUI\\picture.jpg" />
             */
        }
    }
}
