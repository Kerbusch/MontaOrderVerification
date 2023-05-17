using System.Drawing;
using OpenCvSharp;

namespace DataSetImageProcessingTests;

public class ImageProcessTest {
	private string _test_images_path = "../../../TestImages/";

	[SetUp]
	public void Setup() {
	}

	[Test]
	public void changeColorTest() {
		Mat image = Cv2.ImRead(_test_images_path + "8719992763139.jpg");
		
		Mat alpha_image = new ImageProcessing().getAlphaMat(image);
		
		//run code
		var result = new ImageProcessing().changeColor(image, alpha_image, Color.Red);
		
		Mat red_image = Cv2.ImRead(_test_images_path + "8719992763139_red.png");
		
		Assert.Pass(); //TODO fix this
	}
	
	[Test]
	public void getAlphaMatTest() {
		Mat image = Cv2.ImRead(_test_images_path + "8719992763139.jpg");
		Mat alpha_image = Cv2.ImRead(_test_images_path + "8719992763139_alpha.png");
		
		
		//run code
		var result = new ImageProcessing().getAlphaMat(image);

		Assert.Pass(); //TODO fix this
	}
}