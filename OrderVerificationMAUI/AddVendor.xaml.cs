namespace OrderVerificationMAUI;

public partial class AddVendor : ContentPage
{
    string sku_number;

	public AddVendor(string input_sku_number)
	{
		InitializeComponent();
        sku_number = input_sku_number;        
    }

    private async void clickedAddVendor(object sender, EventArgs e)
    {
        string path = "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\OrderVerificationMAUI\\Vendors.txt";
        string text = File.ReadAllText(path);
        text += vendor_entry.Text + "\n";
        File.WriteAllText(path, text);
        await Navigation.PushAsync(new CapturePicture(sku_number, vendor_entry.Text));
    }
}