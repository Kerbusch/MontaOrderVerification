using System.Drawing;
using OpenCvSharp;
using Point = OpenCvSharp.Point;

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

		//compare images by making a mask and checking the coverage
		Mat compare_result = new Mat();

		Cv2.Compare(result, red_image, compare_result, CmpType.EQ);

		int amount_of_pixel = compare_result.Width * compare_result.Height;
		int amount_of_white_pixel = 0;
		
		for (int row = 0; row < compare_result.Rows; row++) {
			for (int col = 0; col < compare_result.Cols; col++) {
				compare_result.At<Vec3b>(row, col);
				if (compare_result.At<Vec3b>(row, col) == new Vec3b(255, 255, 255)) {
					amount_of_white_pixel++;
				}
			}
		}

		Assert.That(amount_of_white_pixel, Is.EqualTo(amount_of_pixel));
	}
	
	[Test]
	public void getAlphaMatTest() {
		Mat image = Cv2.ImRead(_test_images_path + "8719992763139.jpg");
		Mat alpha_image = Cv2.ImRead(_test_images_path + "8719992763139_alpha.png");
		
		
		//run code
		var result = new ImageProcessing().getAlphaMat(image);

		//compare images by making a mask and checking the coverage
		Mat compare_result = new Mat();

		Cv2.Compare(result, alpha_image, compare_result, CmpType.EQ);

		int amount_of_pixel = compare_result.Width * compare_result.Height;
		int amount_of_white_pixel = 0;
		
		for (int row = 0; row < compare_result.Rows; row++) {
			for (int col = 0; col < compare_result.Cols; col++) {
				compare_result.At<Vec3b>(row, col);
				if (compare_result.At<Vec3b>(row, col) == new Vec3b(255, 255, 255)) {
					amount_of_white_pixel++;
				}
			}
		}

		Assert.That(amount_of_white_pixel, Is.EqualTo(amount_of_pixel));
	}
}