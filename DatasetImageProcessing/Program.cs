using DatasetImageProcessing;
using OpenCvSharp;
using RabbitMQ;

class Run
{
    static async Task Main() {
        // DatasetImages dataset_images = new DatasetImages("../../../dataset");
        DatasetImages dataset_images = new DatasetImages("C:\\Users\\daank\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\dataset\\");
        
        using RabbitMQ.DataSetImageReceiver image_receiver = new DataSetImageReceiver(
            "20.13.19.141",
            "python_test_user",
            "jedis",
            dataset_images.saveImageToDatasetAndChangeColor
        );

        using RabbitMQ.SkuIndexReceiver sku_index_receiver = new SkuIndexReceiver(
            "20.13.19.141",
            "python_test_user",
            "jedis",
            dataset_images._image_index.getSkuIndex
        );
        
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        
        return;
        
        Mat image = Cv2.ImRead("C:\\Users\\daank\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\test.png");
        // Mat image1 = Cv2.ImRead("C:\\Users\\Daan\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\8719992763511.jpg");
        long sku_number = 8719992763139;
        // long sku_number1 = 8719992763511;

        var x = new DatasetImages("C:\\Users\\daank\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\dataset\\");
        x.saveImageToDatasetAndChangeColor(sku_number, "OOT", image);
        // x.saveImageToDatasetAndChangeColor(sku_number1, "oot", image1);
        return;
    }
}

