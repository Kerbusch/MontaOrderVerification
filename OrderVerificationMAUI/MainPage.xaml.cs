namespace OrderVerificationMAUI;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}


    private async void PhotoButton_Clicked(object sender, EventArgs e)
    {
        SaveCaptureOpenCV captureOpenCV = new SaveCaptureOpenCV();
        SaveCaptureOpenCV.Save();
		await Navigation.PushAsync(new NewPage1());
    }
}

