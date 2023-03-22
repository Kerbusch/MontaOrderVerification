using System.Numerics;

namespace OrderVerificationMAUI;

public partial class Capture_picture : ContentPage
{
    string sku_number;
    string path;
    string picture_path = "Trainedmodel\\DatasetOOT\\Training\\";
    string last_path = "";
    int picture = 0;
    int total_pictures = 5;

    // Constuctor
    public Capture_picture(string new_sku_number)
	{
		InitializeComponent();
        if (new_sku_number == "")
        {
            Sku_label.Text = "No sku number provided";
            sku_number = "No_sku_number_provided";
        }
        else
        {
            sku_number = new_sku_number;
            Sku_label.Text += sku_number;
        }

        path = getPath();
        live_image.Source = path + "picture.jpg";
        picture_counter.Text = (total_pictures - picture).ToString();
    }

    // Returns the base path of the repo directory
    private string getPath()
    {
        string path = Path.GetDirectoryName(AppContext.BaseDirectory);
        string[] paths = path.Split('\\');
        path = "";
        for (int i = 0; i < paths.Length; i++)
        {
            path += paths[i] + "\\";
            if (paths[i] == "MontaOrderVerification")
            {
                break;
            }

        }
        return path;
    }

    // Makes the next picture and displays it on the screen
    private void clickedNextPicture(object sender, EventArgs e)
    {
        if (total_pictures == picture) { return; }

        if (!Directory.Exists(path + picture_path + sku_number))
        {
            Directory.CreateDirectory(path + picture_path + sku_number);
        }

        last_path = path + picture_path + sku_number + "\\" + sku_number + " (" + (picture + 1).ToString() + ").jpg";

        CameraModule cameraModule = new CameraModule();
        CameraModule.takePicture(last_path);

        last_image.Source = last_path;

        picture++;
        picture_counter.Text = (total_pictures - picture).ToString();
    }

    // Replaces the last made picture with a new picture and displays the new picture on the screen 
    private void clickedDeleteLastPicture(object sender, EventArgs e)
    {
        if (picture == 0) { return; }

        CameraModule cameraModule = new CameraModule();
        CameraModule.takePicture(last_path);

        last_image.Source = last_path;
    }
}