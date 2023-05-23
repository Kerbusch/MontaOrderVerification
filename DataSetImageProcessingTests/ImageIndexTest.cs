namespace DataSetImageProcessingTests;

public class TestImageIndex {
	private string _dataset_path = "../../../DatasetTestDir/";
	private DatasetImages _dataset_images;
	private ImageIndex _image_index;

	public void recreateDatasetTestDir() {
		//recreate DatasetTestDir
		Directory.Delete(_dataset_path, true);
		Directory.CreateDirectory(_dataset_path);
	}
	
	[SetUp]
	public void Setup() {
		_dataset_images = new DatasetImages(_dataset_path);
		_image_index = _dataset_images.getNewImageIndex();
	}
	
	[Test] //test constructor
	public void constructorNoPathTest() {
		//no path
		Assert.Throws<ArgumentException>(
			delegate { new ImageIndex(null); });
	}
	
	[Test] //test constructor
	public void constructorWrongFolderPathTest() {
		//wrong path to non existent folder
		Assert.Throws<ArgumentException>(
			delegate { new ImageIndex(_dataset_path + "folder\\"); });
	}
	
	[Test] //test constructor
	public void constructorWrongFolderPathToFileTest() {
		//wrong path to file
		Assert.Throws<ArgumentException>(
			delegate { new ImageIndex(_dataset_path + "item.txt"); });
	}
	
	[Test] //test constructor
	public void constructorRightPathTest() {
		//right path
		Assert.DoesNotThrow(
			delegate { new ImageIndex(_dataset_path); });
	}

	[Test]
	public void getSkuIndexTest() {
		//run code
		int sku_index = _image_index.getSkuIndex(8719992763139, "Oot");
		
		//check output
		Assert.That(sku_index, Is.Zero);
		
		//check if file create
		Assert.That(File.Exists(path: _dataset_path + "Oot" + "\\" + _image_index.getSkuIndexesFileName()), Is.True);

		recreateDatasetTestDir();
	}
	
	[Test]
	public void getSkuIndexAndIncrementTest() {
		//run code
		int sku_index = _image_index.getSkuIndexAndIncrement(8719992763139, "Oot");
		
		//check first output
		Assert.That(sku_index, Is.Zero);
		
		//check if incremented
		int sku_index_updated = _image_index.getSkuIndex(8719992763139, "Oot");
		Assert.That(sku_index_updated, Is.EqualTo(1));

		recreateDatasetTestDir();
	}
}
