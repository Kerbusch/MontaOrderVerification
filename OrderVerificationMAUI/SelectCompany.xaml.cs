using System;

namespace OrderVerificationMAUI;

public partial class SelectCompany : ContentPage
{
    string sku_number;
	public SelectCompany(string sku)
	{
        InitializeComponent();

        sku_number = sku;

        var vec = new List<string>();
        vec.Add("appel");
        vec.Add("peer");
        drop_down_menu.ItemsSource = vec;
    }

    private async void clickedMakePictures(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CapturePicture(sku_number));
    }
}