using OpenCvSharp;
using System.Diagnostics;

namespace OrderVerificationMAUI
{
    public class CameraModule
    {
        // Constructor
        public CameraModule() {   }


        // Take picture and return the mat
        public static Mat takePicture()
        {
            String liveStreamWindow = "Photo Window (Press any key to take a picture)";


            // for later, if we want to implement small live feed window over application instead of full screen window.
            /*OpenCvSharp.Cv2.NamedWindow(liveStreamWindow, WindowFlags.AutoSize);

            OpenCvSharp.Size size = new OpenCvSharp.Size(640, 480);
            Cv2.ResizeWindow(liveStreamWindow, size);
            Cv2.MoveWindow(liveStreamWindow, 250, 220);
            Cv2.SetWindowProperty(liveStreamWindow, WindowPropertyFlags.Topmost, 1);*/

            Cv2.NamedWindow(liveStreamWindow, WindowFlags.Normal);
            Cv2.SetWindowProperty(liveStreamWindow, WindowPropertyFlags.Fullscreen, 1);

            Mat frame = new Mat();

            VideoCapture videoCapture = null;

            try
            {
                videoCapture = new VideoCapture(0);
            }
            catch (OpenCvSharpException e)
            {
                Debug.WriteLine(e.Message);
                return frame;
            }

            while (Cv2.WaitKey(1) == -1)
            {
                if (videoCapture.Read(frame))
                {
                    Cv2.ImShow(liveStreamWindow, frame);
                }
            }

            videoCapture.Dispose();
            Cv2.DestroyAllWindows();

            return frame;

        }

        // makes all the pixels around the object transparent
        public static Mat makeBackgroundTransparent(Mat original) {
            Mat input = new();
            OpenCvSharp.Size s = new OpenCvSharp.Size(3, 3);
            Cv2.GaussianBlur(original, input, s, 0);

            Mat input_bgra = new();
            Cv2.CvtColor(input, input_bgra, ColorConversionCodes.BGR2BGRA);
            
            Mat[] channels = Cv2.Split(input_bgra);


            Cv2.Merge(channels, input_bgra);

            for (int y = 0; y < input_bgra.Rows; ++y)
            {
                for (int x = 0; x < input_bgra.Cols; ++x)
                {
                    Vec4b pixel = input_bgra.At<Vec4b>(y, x);
                    if (pixel[0] <= 100 && pixel[1] <= 100 && pixel[2] <= 100)
                    {
                        input_bgra.At<Vec4b>(y, x)[3] = 0;
                    }
                }
            }

            return input_bgra;
        }
    }
}

