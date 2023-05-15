using DatasetImageProcessing;
using OpenCvSharp;
using System.Drawing;

class Run
{
    static async Task Main()
    {
        long[] skus = { 8719992763634, 8719992763573 };
        foreach (long sku in skus)
        {
            string path = "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\Trainedmodel\\DatasetOOT\\Training\\" + sku;
            string new_path = "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\Trainedmodel\\DatasetOOT\\new_dataset\\" + sku;

            string[] all_files = Directory.GetFiles(path, "*.jpg");
            int index = 0;

            foreach (string fileName in all_files)
            {
                var dataset_images = new DatasetImages();

                Mat picture = Cv2.ImRead(fileName);
                dataset_images.saveImageToDatasetAndChangeColor(picture, new_path, sku, index++);
            }
        }
    }
}

