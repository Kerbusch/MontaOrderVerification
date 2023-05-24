using OpenCvSharp;
using System.Diagnostics;

namespace OrderVerificationMAUI
{
    public class CameraModule
    {
        // Take picture and return the mat
        public static Mat takePicture()
        {
            String liveStreamWindow = "Photo Window (Press any key to take a picture)";

            Cv2.NamedWindow(liveStreamWindow, WindowFlags.Normal);
            Cv2.SetWindowProperty(liveStreamWindow, WindowPropertyFlags.Fullscreen, 1);

            Mat frame = new Mat();
            VideoCapture videoCapture = null;
            
            try {
                videoCapture = new VideoCapture(0);
                videoCapture.Set(VideoCaptureProperties.FrameWidth, 1280);
                videoCapture.Set(VideoCaptureProperties.FrameHeight, 720);
            }
            catch (OpenCvSharpException e) {
                Debug.WriteLine(e.Message);
                return frame;
            }

            while (Cv2.WaitKey(1) == -1) {
                if (videoCapture.Read(frame)) {
                    Cv2.ImShow(liveStreamWindow, frame);
                }
            }

            videoCapture.Dispose();
            Cv2.DestroyAllWindows();

            return frame;
        }
    }
}

