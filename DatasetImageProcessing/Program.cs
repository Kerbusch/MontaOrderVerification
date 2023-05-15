using DatasetImageProcessing;
using OpenCvSharp;
using RabbitMQ;

class Run
{
    static async Task Main() {
        DatasetImages dataset_images = new DatasetImages();
        
        using RabbitMQ.DataSetImageReceiver image_receiver = new DataSetImageReceiver(
            "20.13.19.141",
            "python_test_user",
            "jedis",
            dataset_images.saveImageToDatasetAndChangeColor
        );
        
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        
        return;
        
        Mat image = Cv2.ImRead("C:\\Users\\Daan\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\20230515_123935.jpg");
        // Mat image1 = Cv2.ImRead("C:\\Users\\Daan\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\8719992763511.jpg");
        long sku_number = 8719992763139;
        // long sku_number1 = 8719992763511;

        var x = new DatasetImages();
        x.saveImageToDatasetAndChangeColor(sku_number, "oot", image);
        // x.saveImageToDatasetAndChangeColor(sku_number1, "oot", image1);
        return;

        long[] skus = { 8719992763634, 8719992763573 };
        foreach (long sku in skus)
        {
            string path = "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\Trainedmodel\\DatasetOOT\\Training\\" + sku;
            string new_path = "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\Trainedmodel\\DatasetOOT\\new_dataset\\" + sku;

            string[] all_files = Directory.GetFiles(path, "*.jpg");
            int index = 0;

            foreach (string fileName in all_files)
            {
                var dataset_images0 = new DatasetImages();

                Mat picture = Cv2.ImRead(fileName);
                dataset_images0.saveImageToDatasetAndChangeColor(picture, new_path, sku, index++);
            }
        }
    }
}

