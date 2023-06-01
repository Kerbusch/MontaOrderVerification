namespace OrderVerificationMAUI;

public partial class AddVendor : ContentPage
{
    string sku_number;

    // Constructor
	public AddVendor(string input_sku_number)
	{
		InitializeComponent();
        sku_number = input_sku_number;        
    }

    // Adds new vendor to the vendor list and goes to the next screen
    private async void clickedAddVendor(object sender, EventArgs e)
    {
        if (vendor_entry.Text != "") {
            GeneralFunctions generalFunctions = new GeneralFunctions();
            string path = generalFunctions.getPath("OrderVerificationMAUI") + "\\Vendors.txt";
            string text = File.ReadAllText(path);
            text += vendor_entry.Text + "\n";
            File.WriteAllText(path, text);
            await Navigation.PushAsync(new CapturePicture(sku_number, vendor_entry.Text));
        }
    }
}