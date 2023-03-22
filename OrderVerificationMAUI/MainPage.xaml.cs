namespace OrderVerificationMAUI;

public partial class MainPage : ContentPage
{
	string sku_number;

    // Constructor
	public MainPage()
	{
		InitializeComponent();
	}

    // Add 1 to the text box
    private void clicked1(object sender, EventArgs e)
    {
        sku_entry.Text += "1";
    }

    // Add 2 to the text box
    private void clicked2(object sender, EventArgs e)
    {
        sku_entry.Text += "2";
    }

    // Add 3 to the text box
    private void clicked3(object sender, EventArgs e)
    {
        sku_entry.Text += "3";
    }

    // Add 4 to the text box
    private void clicked4(object sender, EventArgs e)
    {
        sku_entry.Text += "4";
    }

    // Add 5 to the text box
    private void clicked5(object sender, EventArgs e)
    {
        sku_entry.Text += "5";
    }

    // Add 6 to the text box
    private void clicked6(object sender, EventArgs e)
    {
        sku_entry.Text += "6";
    }

    // Add 7 to the text box
    private void clicked7(object sender, EventArgs e)
    {
        sku_entry.Text += "7";
    }

    // Add 8 to the text box
    private void clicked8(object sender, EventArgs e)
    {
        sku_entry.Text += "8";
    }

    // Add 9 to the text box
    private void clicked9(object sender, EventArgs e)
    {
        sku_entry.Text += "9";
    }

    // Add 0 to the text box
    private void clicked0(object sender, EventArgs e)
    {
        sku_entry.Text += "0";
    }

    // Clear the text box
    private void clickedDelete(object sender, EventArgs e)
    {
        sku_entry.Text = "";
    }

    // Delete the last char of the text box
    private void clickedBack(object sender, EventArgs e)
    {
        if (sku_entry.Text != "")
        {
            sku_entry.Text = sku_entry.Text.Remove(sku_entry.Text.Length - 1);
        }
    }

    // Save the text box input and go to the next page 
    private async void clickedEnter(object sender, EventArgs e)
    {
        sku_number = sku_entry.Text;
        await Navigation.PushAsync(new Capture_picture(sku_number));
    }
}

