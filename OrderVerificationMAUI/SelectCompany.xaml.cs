using System.Diagnostics;
using System;

namespace OrderVerificationMAUI;

public partial class SelectCompany : ContentPage
{
    string sku_number;
	public SelectCompany(string sku)
	{
        InitializeComponent();
        sku_number = sku;
        drop_down_menu.ItemsSource = getVendors();
    }

    private List<string> getVendors()
    {
        string path = "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\OrderVerificationMAUI\\Vendors.txt";
        string text = File.ReadAllText(path);
        var vec = new List<string>();
        foreach (string item in text.Split("\n")) {
            if (item.Length > 0) {
                vec.Add(item);
            }
        }
        vec.Add("Add new vendor");
        return vec;
    }

    private async void clickedContinue(object sender, EventArgs e)
    {
        if (drop_down_menu.SelectedItem.ToString() == "Add new vendor") {
            await Navigation.PushAsync(new AddVendor(sku_number));
        }
        else if(drop_down_menu.SelectedItem.ToString() != string.Empty) {
            await Navigation.PushAsync(new CapturePicture(sku_number, drop_down_menu.SelectedItem.ToString()));
        }
    }
}