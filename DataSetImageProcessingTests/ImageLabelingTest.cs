using OpenCvSharp;

namespace DataSetImageProcessingTests;

public class TestImageLabeling {
	private string _test_images_path = "../../../TestImages/";
	
	[SetUp]
	public void Setup() {
	}

	[Test]
	public void yoloIndexesTest()
    {
        YoloLabel yolo_label = new YoloLabel();
        Assert.Multiple(() =>
        {
            Assert.That(yolo_label.checkSkuIndexKnown(8719992763139), Is.True);
            Assert.That(yolo_label.getYoloIndexFromSku(8719992763139), Is.EqualTo(0));

            Assert.That(yolo_label.checkSkuIndexKnown(8719992763351), Is.True);
            Assert.That(yolo_label.getYoloIndexFromSku(8719992763351), Is.EqualTo(1));

            Assert.That(yolo_label.checkSkuIndexKnown(8719992763511), Is.True);
            Assert.That(yolo_label.getYoloIndexFromSku(8719992763511), Is.EqualTo(2));

            Assert.That(yolo_label.checkSkuIndexKnown(8719992763634), Is.True);
            Assert.That(yolo_label.getYoloIndexFromSku(8719992763634), Is.EqualTo(3));

            Assert.That(yolo_label.checkSkuIndexKnown(8719992763573), Is.True);
            Assert.That(yolo_label.getYoloIndexFromSku(8719992763573), Is.EqualTo(4));
        });
    }

    [Test]
	public void getBoundingBoxStringFromXmlFormatTest() {
		var result = new YoloLabel().getBoundingBoxStringFromXmlFormat(
			8719992763139,
			640,
			480,
			260,
			59,
			414,
			455
		);
		Assert.That(result, Is.EqualTo("0 0.5265625 0.53541666 0.240625 0.825"));
	}
	
	[Test]
	public void getBoundingBoxTest() {
		Mat alpha_image = Cv2.ImRead(_test_images_path + "8719992763139_alpha.png");

		var result = ImageLabeling.getBoundingBox(alpha_image);
		Assert.That(result, Is.EqualTo(new int[] {256, 52, 414, 462}));
	}
	
	[Test]
	public void createLabelFileTest() {
		//load images
		Mat image = Cv2.ImRead(_test_images_path + "8719992763139.jpg");
		Mat alpha_image = Cv2.ImRead(_test_images_path + "8719992763139_alpha.png");

		//create bounding box
		var bounding_box = ImageLabeling.getBoundingBox(alpha_image);

		//create label file
		ImageLabeling.createLabelFile(
			bounding_box,
			image,
			8719992763139,
			_test_images_path,
			"8719992763139.jpg"
		);
		
		//check if file exist
		Assert.That(File.Exists(path: _test_images_path + "8719992763139.txt"), Is.True);
		
		//check file content
		var file_content = File.ReadAllText(path: _test_images_path + "8719992763139.txt");
		Assert.That(file_content, Is.EqualTo("0 0.5234375 0.53541666 0.246875 0.8541667\r\n"));
		
		//if file exist delte
		if (File.Exists(path: _test_images_path + "8719992763139.txt")) {
			File.Delete(path: _test_images_path + "8719992763139.txt");
		}
	}
}