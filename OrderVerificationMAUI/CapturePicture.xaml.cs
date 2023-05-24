using OpenCvSharp;
using System.Diagnostics;
using RabbitMQ;

namespace OrderVerificationMAUI;

public partial class CapturePicture : ContentPage
{
    string vendor;
    string sku_number;
    string last_picture_path;
    int picture = 0;
    int total_pictures = 3;
    Mat last_picture;
    private DataSetImageSender _send_to_RabbitMQ;
    private SkuIndexRequest _get_sku_index;

    // Constuctor
    public CapturePicture(string new_sku_number, string new_vendor)
	{
        _send_to_RabbitMQ = new DataSetImageSender("20.13.19.141", "python_test_user", "jedis");
        _get_sku_index = new SkuIndexRequest("20.13.19.141", "python_test_user", "jedis");
        InitializeComponent();

        if (new_vendor == "") {
            new_vendor = "No_vendor_provided";
        }
        vendor = new_vendor;

        if (new_sku_number == "") {
            Sku_label.Text = vendor + ": No sku number provided";
            sku_number = "No_sku_number_provided";
        }
        else {
            sku_number = new_sku_number;
        }

        picture_counter.Text = (total_pictures - picture).ToString();
        Sku_label.Text = sku_number + " (" + (getPreviousSkuIndex() + 1) + ") ";
        last_picture_path = getPath();
        last_image.Source = last_picture_path + "\\missing_first_picture.jpg";
        Directory.CreateDirectory(last_picture_path + "\\trash");
    }

    // Destructor
    ~CapturePicture()
    {
        Directory.Delete(last_picture_path + "\\trash", true);
    }

    // Returns the base path of the repo directory
    private string getPath()
    {
        string path = Path.GetDirectoryName(AppContext.BaseDirectory);
        string[] paths = path.Split('\\');
        path = "";
        for (int i = 0; i < paths.Length; i++) {
            path += paths[i] + "\\";
            if (paths[i] == "OrderVerificationMAUI") {
                break;
            }
        }
        return path;
    }

    // Makes the next picture and displays it on the screen
    private async void clickedNextPicture(object sender, EventArgs e)
    {
        if (picture != 0) {
            while (!sendPicture(last_picture)) {
                bool answer = await DisplayAlert("Connection error", "Can't connect to the server", "Close", "Retry");
                if (answer) {
                    await Navigation.PushAsync(new MainPage());
                    return;
                }
            }
        }

        if (picture >= (total_pictures - 1)) { 
            if (button_next_picture.Text == "Take next picture") {
                button_next_picture.Text = "Finish this sku";
            }
            else {
                await Navigation.PushAsync(new MainPage());
                return;
            }
        }

        last_picture = CameraModule.takePicture();
        if (last_picture == null) {
            await DisplayAlert("Camera error", "Fail to make picture", "Close");
        }

        last_picture.SaveImage(last_picture_path + "\\trash\\last_picture_" + picture + ".jpg");
        last_image.Source = last_picture_path + "\\trash\\last_picture_" + picture + ".jpg";

        picture++;
        picture_counter.Text = (total_pictures - picture).ToString();
        Sku_label.Text = vendor + ": " + sku_number + " (" + (getPreviousSkuIndex() + 2) + ") ";
    }

    // Replaces the last made picture with a new picture and displays the new picture on the screen 
    private async void clickedRetakeLastPicture(object sender, EventArgs e)
    {
        if (picture == 0) { 
            await DisplayAlert("Camera error", "Fail to make picture", "Close");
        }

        last_picture = CameraModule.takePicture();
        if (last_picture == null) { return; }

        last_picture.ImWrite(last_picture_path);
        last_image.Source = last_picture_path;
    }

    // Sends picture with rabbitmq to the server, returns false if failed
    private bool sendPicture(OpenCvSharp.Mat picture)
    {
        _send_to_RabbitMQ.sendToDataSetImageToServer((long)Convert.ToDouble(sku_number), vendor, picture);
        return true; //hardcoded for now. Maby add later if server doesn't respond, return false
    }

    // gets the last used number of the given sku from rabbitmq
    private int getPreviousSkuIndex()
    {
        return _get_sku_index.getSkuIndex((long)Convert.ToDouble(sku_number), vendor).Result;
    }
}