using OpenCvSharp;

namespace DataSetImageProcessingTests; 

public class DatasetImagesTest {
	private string _test_images_path = "../../../TestImages/";
	private string _dataset_path = "../../../DatasetTestDir/";
	
	[SetUp]
	public void Setup() {
		Directory.CreateDirectory(_dataset_path);
	}
	
	[Test] //test constructor
	public void constructorNoPathTest() {
		//no path
		Assert.Throws<ArgumentException>(
			delegate { new DatasetImages(null); });
	}
	
	[Test] //test constructor
	public void constructorWrongFolderPathTest() {
		//wrong path to non existent folder
		Assert.Throws<ArgumentException>(
			delegate { new DatasetImages(_dataset_path + "folder\\"); });
	}
	
	[Test] //test constructor
	public void constructorWrongFolderPathToFileTest() {
		//wrong path to file
		Assert.Throws<ArgumentException>(
			delegate { new DatasetImages(_dataset_path + "item.txt"); });
	}
	
	[Test] //test constructor
	public void constructorRightPathTest() {
		//right path
		Assert.DoesNotThrow(
			delegate { new DatasetImages(_dataset_path); });
	}

	[Test]
	public void saveImageToDatasetAndChangeColorTest() {
		//setup inputs
		long sku = 8719992763139;
		string vendor = "Oot";
		Mat image = Cv2.ImRead(_test_images_path + "8719992763139.jpg");
		
		//run code
		new DatasetImages(_dataset_path).saveImageToDatasetAndChangeColor(sku, vendor, image);
		
		//check if vendor dir was made
		Assert.That(Directory.Exists(_dataset_path + vendor + "\\"), Is.True);
		
		//check if sku dir was made
		Assert.That(Directory.Exists(_dataset_path + vendor + "\\" + sku + "\\"), Is.True);
		
		//check if 22 files
		var number_of_files = Directory.EnumerateFiles(_dataset_path + vendor + "\\" + sku + "\\").Count();
		Assert.That(number_of_files, Is.EqualTo(22));
		
		//check if sku index was increased
		var sku_index = new ImageIndex(_dataset_path).getSkuIndex(sku, vendor);
		Assert.That(sku_index, Is.EqualTo(1));
		
		//recreate DatasetTestDir
		Directory.Delete(_dataset_path, true);
		Directory.CreateDirectory(_dataset_path);
	}
	
	[Test]
	public void getSkuFolderPathTest() {
		Assert.Pass();
	}
}