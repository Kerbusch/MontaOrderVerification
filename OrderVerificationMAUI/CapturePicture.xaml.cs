using System.Diagnostics;

namespace OrderVerificationMAUI;

public partial class CapturePicture : ContentPage
{
    string vendor;
    string sku_number;
    string path;
    string picture_path = "Trainedmodel\\DatasetOOT\\Training\\";
    string last_path = "";
    int picture = 0;
    int total_pictures = 1;
    int last_number;

    // Constuctor
    public CapturePicture(string new_sku_number, string new_vendor)
	{
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

        path = getPath();
        picture_counter.Text = (total_pictures - picture).ToString();
    
        if (Directory.Exists(path + picture_path + sku_number))
        {
            last_number = previousNumbers();
            Sku_label.Text = vendor + ": " + sku_number + " (" + (last_number + 1) + ") ";
        }
        else
        {
            Directory.CreateDirectory(path + picture_path + sku_number);
            Sku_label.Text = vendor + ": " + sku_number + " (1) ";
        }
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

    // Checks what the highes nummer 
    private int previousNumbers()
    {
        string[] all_files = Directory.GetFiles(path + picture_path + sku_number + "\\", "*.jpg");
        List<int> numbers = new List<int>();

        foreach (string fileName in all_files)
        {
            string tmp = fileName.Replace(path + picture_path + sku_number + "\\" + sku_number + " (", "");
            tmp = tmp.Replace(").jpg", "");
            int i = int.Parse(tmp);
            numbers.Add(i);
        }

        if (numbers.Count > 0)
        {
            return numbers.Max();
        }
        return 0;
    }

    // Makes the next picture and displays it on the screen
    private async void clickedNextPicture(object sender, EventArgs e)
    {
        if (last_path != "") { 
            List<string> tmp = new List<string>() { (sku_number + " (" + (picture + last_number).ToString() + ").jpg"), last_path };
            AutoLabeler.createLabel(tmp, sku_number);
        }

        if (total_pictures >= -picture) {
            if (button_next_picture.Text == "Take next picture") {
                button_next_picture.Text = "Next sku";
            }
            else {
                await Navigation.PushAsync(new MainPage());
                return;
            }
        }

        last_path = path + picture_path + sku_number + "\\" + sku_number + " (" + (picture + last_number + 1).ToString() + ").jpg";

        CameraModule cameraModule = new CameraModule();
        CameraModule.takePicture(last_path);

        last_image.Source = last_path;

        picture++;
        picture_counter.Text = (total_pictures - picture).ToString();
        Sku_label.Text = vendor + ": " + sku_number + " (" + (picture + last_number + 1) + ") ";
    }

    // Replaces the last made picture with a new picture and displays the new picture on the screen 
    private void clickedRetakeLastPicture(object sender, EventArgs e)
    {
        if (picture == 0) { return; }

        CameraModule cameraModule = new CameraModule();
        CameraModule.takePicture(last_path);

        last_image.Source = last_path;
    }
}