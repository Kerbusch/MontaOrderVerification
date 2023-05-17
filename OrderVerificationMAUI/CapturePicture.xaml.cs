using OpenCvSharp;
using System.Diagnostics;
using RabbitMQ;

namespace OrderVerificationMAUI;

public partial class CapturePicture : ContentPage
{
    string sku_number;
    string path;
    string picture_path;
    int picture = 0;
    int total_pictures = 50;
    int last_number;
    Mat last_picture;
    private DataSetImageSender _setToRabbitMq;

    // Constuctor
    public CapturePicture(string new_sku_number)
    {
        _setToRabbitMq = new DataSetImageSender("20.13.19.141", "python_test_user", "jedis");

        InitializeComponent();
        if (new_sku_number == "") {
            Sku_label.Text = "No sku number provided";
            sku_number = "No_sku_number_provided";
        }
        else {
            sku_number = new_sku_number;
        }

        path = getPath();
        picture_path = path + "last_picture.jpg";
        picture_counter.Text = (total_pictures - picture).ToString();

        last_number = getPreviousNumbers(sku_number);
        Sku_label.Text = sku_number + " (" + (last_number + 1) + ") ";
    }

    // Returns the base path of the repo directory
    private string getPath()
    {
        string path = Path.GetDirectoryName(AppContext.BaseDirectory);
        string[] paths = path.Split('\\');
        path = "";
        for (int i = 0; i < paths.Length; i++) {
            path += paths[i] + "\\";
            if (paths[i] == "MontaOrderVerification") {
                break;
            }
        }
        return path;
    }

    // Checks what the highes nummer 
    private int previousNumber()
    {
        string[] all_files = Directory.GetFiles(path + picture_path + sku_number + "\\", "*.jpg");
        List<int> numbers = new List<int>();

        foreach (string fileName in all_files) {
            string tmp = fileName.Replace(path + picture_path + sku_number + "\\" + sku_number + " (", "");
            tmp = tmp.Replace(").jpg", "");
            int i = int.Parse(tmp);
            numbers.Add(i);
        }

        if (numbers.Count > 0) {
            return numbers.Max();
        }
        return 0;
    }

    // Makes the next picture and displays it on the screen
    private async void clickedNextPicture(object sender, EventArgs e)
    {
        if (picture != 0) {
            AutoLabeler.createLabel(picture_path, (sku_number + " (" + (picture + last_number).ToString() + ").jpg"), sku_number);
            while (!sendPicture(OpenCvSharp.Cv2.ImRead(picture_path))) {
                bool answer = await DisplayAlert("Connection error", "Can't connect to the server", "Close", "Retry");
                if (answer) {
                    await Navigation.PushAsync(new MainPage());
                    return;
                }
            }
        }

        if (picture >= (total_pictures - 1))
        {
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

        last_picture.ImWrite(picture_path);
        last_image.Source = picture_path;

        picture++;
        picture_counter.Text = (total_pictures - picture).ToString();
        Sku_label.Text = sku_number + " (" + (picture + last_number + 1) + ") ";
    }

    // Replaces the last made picture with a new picture and displays the new picture on the screen 
    private async void clickedRetakeLastPicture(object sender, EventArgs e)
    {
        if (picture == 0) { 
            await DisplayAlert("Camera error", "Fail to make picture", "Close");
        }

        last_picture = CameraModule.takePicture();
        if (last_picture == null) { return; }

        last_picture.ImWrite(path + "last_picture.jpg");
        last_image.Source = path + "last_picture.jpg";
    }

    // Sends picture with rabbitmq to the server, returns false if failed
    private bool sendPicture(OpenCvSharp.Mat picture)
    {
        _setToRabbitMq.sendToDataSetImageToServer((long)Convert.ToDouble(sku_number), "Oat", picture);
        return true;
    }

    // gets the last used number of the given sku from rabbitmq
    private int getPreviousNumbers(string sku_number)
    {
        return 1;
    }
}