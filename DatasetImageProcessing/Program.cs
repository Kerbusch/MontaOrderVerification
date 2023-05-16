using DatasetImageProcessing;
using OpenCvSharp;

class Run
{
    static async Task Main()
    {
        var image = new ImageProcessing();
        Mat picture = Cv2.ImRead("C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\DatasetImageProcessing\\test.jpg");
        int[] color = {255, 0, 0}; // {210, 184, 87}
        Mat alpha = image.getAlphaMat(picture);
        Mat new_picture = image.changeColor(alpha, color);
        Cv2.ImWrite("C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\DatasetImageProcessing\\new_test.jpg", new_picture);
    }
}

