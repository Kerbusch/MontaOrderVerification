using System.Diagnostics;

namespace OrderVerificationMAUI;

public partial class CapturePicture : ContentPage
{
    string sku_number;
    string path;
    string picture_path = "Trainedmodel\\DatasetOOT\\Training\\";
    string last_path = "";
    int picture = 0;
    int total_pictures = 30;
    int last_number;

    // Constuctor
    public CapturePicture(string new_sku_number)
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
        }

        path = getPath();
        picture_counter.Text = (total_pictures - picture).ToString();

        if (Directory.Exists(path + picture_path + sku_number))
        {
            last_number = previousNumbers();
            Sku_label.Text = sku_number + " (" + (last_number + 1) + ") ";
        }
        else
        {
            Directory.CreateDirectory(path + picture_path + sku_number);
            Sku_label.Text = sku_number + " (1) ";
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
            Debug.WriteLine(fileName);
        }

        foreach (string fileName in all_files)
        {
            Debug.WriteLine(fileName);
            string tmp = fileName.Replace(path + picture_path + sku_number + "\\" + sku_number + " (", "");
            tmp = tmp.Replace(").jpg", "");
            Debug.WriteLine(tmp);
            int i = int.Parse(tmp);
            numbers.Add(i);
        }

        foreach (int i in numbers) 
        {
            Debug.WriteLine(i);
        }

        return numbers.Max();
    }

    // Makes the next picture and displays it on the screen
    private void clickedNextPicture(object sender, EventArgs e)
    {
        if (total_pictures == picture) { return; }

        last_path = path + picture_path + sku_number + "\\" + sku_number + " (" + (picture + last_number + 1).ToString() + ").jpg";

        CameraModule cameraModule = new CameraModule();
        CameraModule.takePicture(last_path);

        last_image.Source = last_path;

        picture++;
        picture_counter.Text = (total_pictures - picture).ToString();
        Sku_label.Text = sku_number + " (" + (picture + last_number + 1) + ") ";
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