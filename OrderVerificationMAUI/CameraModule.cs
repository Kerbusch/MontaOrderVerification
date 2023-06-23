using OpenCvSharp;
using System.Diagnostics;

namespace OrderVerificationMAUI
{
    public class CameraModule
    {
        // Take picture and return the mat
        public static Mat takePicture()
        {
            String livestream_window = "Photo Window (Press any key to take a picture)";

            Cv2.NamedWindow(livestream_window, WindowFlags.Normal);
            Cv2.SetWindowProperty(livestream_window, WindowPropertyFlags.Fullscreen, 1);

            Mat frame = new Mat();
            VideoCapture video_capture = null;
            
            try {
                video_capture = new VideoCapture(0);
                video_capture.Set(VideoCaptureProperties.FrameWidth, 1280);
                video_capture.Set(VideoCaptureProperties.FrameHeight, 720);
            }
            catch (OpenCvSharpException e) {
                Debug.WriteLine(e.Message);
                return frame;
            }

            while (Cv2.WaitKey(1) == -1) {
                if (video_capture.Read(frame)) {
                    Cv2.ImShow(livestream_window, frame);
                }
            }

            video_capture.Dispose();
            Cv2.DestroyAllWindows();

            return frame;
        }
    }
}

