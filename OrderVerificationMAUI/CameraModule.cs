using Emgu.CV;

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
            String win1 = "Photo Window (Press any key to take a picture)";
            CvInvoke.NamedWindow(win1, Emgu.CV.CvEnum.WindowFlags.Normal);
            CvInvoke.SetWindowProperty(win1, Emgu.CV.CvEnum.WindowPropertyFlags.FullScreen, 1);

            Mat frame = new Mat();
            VideoCapture capture = new VideoCapture();

            while (CvInvoke.WaitKey(1) == -1)
            {
                capture.Read(frame);
                CvInvoke.Imshow(win1, frame);
            }

            frame.Save(pictureFilePath);

            frame.Dispose();
            capture.Dispose();

            CvInvoke.DestroyWindow(win1);
        }
    }
}

