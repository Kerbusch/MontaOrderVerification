namespace OrderVerificationMAUI;

public partial class MainPage : ContentPage
{
	bool input = false;

	public MainPage()
	{
		InitializeComponent();
	}

    /*private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
		if (SkuEntry.Text != "" && !input)
        {
			SkuEntry.Text = SkuEntry.Text.Replace("Sku invoeren", "");
			input = true;
        }
		/*else if (SkuEntry.Text == "")
		{
			SkuEntry.Text = "Sku invoeren";
			input = false;
        }
        SemanticScreenReader.Announce(SkuEntry.Text);
    }*/

    private void Clicked_1(object sender, EventArgs e)
    {
        SkuEntry.Text += "1";
    }
    private void Clicked_2(object sender, EventArgs e)
    {
        SkuEntry.Text += "2";
    }
    private void Clicked_3(object sender, EventArgs e)
    {
        SkuEntry.Text += "3";
    }
    private void Clicked_4(object sender, EventArgs e)
    {
        SkuEntry.Text += "4";
    }
    private void Clicked_5(object sender, EventArgs e)
    {
        SkuEntry.Text += "5";
    }
    private void Clicked_6(object sender, EventArgs e)
    {
        SkuEntry.Text += "6";
    }
    private void Clicked_7(object sender, EventArgs e)
    {
        SkuEntry.Text += "7";
    }
    private void Clicked_8(object sender, EventArgs e)
    {
        SkuEntry.Text += "8";
    }
    private void Clicked_9(object sender, EventArgs e)
    {
        SkuEntry.Text += "9";
    }
    private void Clicked_0(object sender, EventArgs e)
    {
        SkuEntry.Text += "0";
    }
    private void Clicked_delete(object sender, EventArgs e)
    {
        SkuEntry.Text = "";
    }
    private void Clicked_back(object sender, EventArgs e)
    {
        if (SkuEntry.Text != "")
        {
            SkuEntry.Text = SkuEntry.Text.Remove(SkuEntry.Text.Length - 1);
        }
    }

    private void Clicked_enter(object sender, EventArgs e)
    {
        SkuEntry.Text = "To next page";
    }
}

