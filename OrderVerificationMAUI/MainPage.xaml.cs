namespace OrderVerificationMAUI;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}


    private async void PhotoButton_Clicked(object sender, EventArgs e)
    {
        //create capture object
        SaveCaptureOpenCV captureOpenCV = new SaveCaptureOpenCV();
        //take a photo and save
        SaveCaptureOpenCV.Save();
        //switch to the photo showing page
		await Navigation.PushAsync(new NewPage1());
    }
}

