using System.Diagnostics;
using System;

namespace OrderVerificationMAUI;

public partial class SelectCompany : ContentPage
{
    string sku_number;

    // Constructor
	public SelectCompany(string sku)
	{
        InitializeComponent();
        sku_number = sku;
        GeneralFunctions generalFunctions = new GeneralFunctions();
        drop_down_menu.ItemsSource = generalFunctions.getVendors();
    }

    // Goes to the correct next screen if vendor is selected
    private async void clickedContinue(object sender, EventArgs e)
    {
        if (drop_down_menu.SelectedItem != null) {
            if (drop_down_menu.SelectedItem.ToString() == "Add new vendor") {
                await Navigation.PushAsync(new AddVendor(sku_number));
            }
            else {
                await Navigation.PushAsync(new CapturePicture(sku_number, drop_down_menu.SelectedItem.ToString()));
            }
        }
    }
}