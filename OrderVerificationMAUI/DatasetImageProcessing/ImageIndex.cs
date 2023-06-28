using System.Diagnostics;
using System.Text.Json;

namespace DatasetImageProcessing; 

public class ImageIndex {
	private string _dataset_path; // "/dataset_folder" not "/dataset_folder/vendor/sku"
	private const string _sku_indexes_filename = "sku_indexes.json";

	// Constructor that sets the _dataset_path and throws exceptions accordingly
	public ImageIndex(string dataset_path) {
		_dataset_path = dataset_path;
		if (_dataset_path == null || !(_dataset_path != null && Directory.Exists(_dataset_path))) {
			throw new ArgumentException("no dataset path");
			throw new ArgumentNullException(paramName: nameof(dataset_path), message: "Dataset path does not exist");
		}
	}

	//get the _sku_indexes_filename the sku indexes filename const
	public string getSkuIndexesFileName() {
		return _sku_indexes_filename;
	}

	//get the sku index from the file
	public int getSkuIndex(long sku, string vendor) {
		return getSkuIndex(sku, vendor, false);
	}

	//get the sku index from the file and increment the value.
	public int getSkuIndexAndIncrement(long sku, string vendor) {
		return getSkuIndex(sku, vendor, true);
	}

	//get the sku index from the file
	private int getSkuIndex(long sku, string vendor, bool increment) {
		//check if sku indexes file exist if not create
		createSkuIndexesFileIfNotExist(vendor);

		//sku indexes path
		string sku_indexes_path = Path.Combine(_dataset_path, vendor + "/" + _sku_indexes_filename);
				
		//read sku indexes file
		var sku_indexes = readJsonFile(sku_indexes_path);
				
		//check add sku if not exist
		sku_indexes.TryAdd(sku, 0);

		//create output
		int output = 0;

		//increment the file
		if (increment) {
			//set output
			output = sku_indexes[sku]++;
		}
		else {
			output = sku_indexes[sku];
		}
		
		//write sku indexes file
		writeJsonFile(sku_indexes_path, sku_indexes);
	
		//else return an index of 0
		return output;
	}
	
	//check if vendor folder exist in the dataset path	
	private bool checkVendorDirExist(string vendor) {
		return Directory.Exists(path: Path.Combine(_dataset_path, vendor + "/"));
	}
	
	//check if vendor sku exist in the dataset path	
	private bool checkSkuDirExist(long sku, string vendor) {
		return Directory.Exists(path: Path.Combine(_dataset_path, vendor + "/" + sku + "/"));
	}

	//check if vendor folder exist in the dataset path and if not create the folder
	private void createVendorDirIfNotExist(string vendor) {
		if (!checkVendorDirExist(vendor)) {
			Debug.WriteLine("Vendor directory doesn't exist, creating...");
			Directory.CreateDirectory(path: Path.Combine(_dataset_path, vendor + "/"));
		}
	}

	//check if sku folder exist in the dataset path and if not create the folder
	private void createSkuIndexesFileIfNotExist(string vendor) {
		createVendorDirIfNotExist(vendor);

		var file_path = Path.Combine(_dataset_path, vendor + "/" + _sku_indexes_filename);

		if (!File.Exists(path: file_path)) {
			var file_stream =File.Create(path: file_path);
			file_stream.Close();
		}
	}
	
	//write the sku indexes to a json file json file
	private void writeJsonFile(string file_path, Dictionary<long, int> sku_indexes) {
		//create json serializer options
		var json_serializer_options = new JsonSerializerOptions();
		json_serializer_options.WriteIndented = true;
            
		//create json string
		string json_string = JsonSerializer.Serialize(sku_indexes, json_serializer_options);
            
		//write file
		File.WriteAllText(file_path, json_string);
	}
	
	//read the sku indexes from a json file
	private Dictionary<long, int> readJsonFile(string file_path) {
        //get string from file
        string json_string = "";
        
        //check if file exist
        if (File.Exists(file_path)) {
            try {
                json_string = File.ReadAllText(file_path);
            }
            catch {
                Debug.WriteLine("Function readJsonFile in ImageIndex: File.ReadAllText could not read file");
            }
        }
        
        //else create file, return empty dictionary
        else {
            var file_stream = File.Create(file_path);
            file_stream.Close();
            return new Dictionary<long, int>();
        }
        
        //check if json string is empty and return empty dictionary if true
        if (json_string == string.Empty) {
            return new Dictionary<long, int>();
            
        }
        
        Dictionary<long, int> temp_dictionary;
        
        //convert string to data using json serialize
        try {
            temp_dictionary = JsonSerializer.Deserialize<Dictionary<long, int>>(json_string);
        }
        catch (ArgumentNullException) {
            Debug.WriteLine("JsonSerializer.Deserialize returned ArgumentNullException, returning a new directory.");
            temp_dictionary = new Dictionary<long, int>();
        }
        catch (JsonException) {
            Debug.WriteLine("JsonSerializer.Deserialize returned JsonException, returning a new directory.");
            temp_dictionary = new Dictionary<long, int>();
        }
        catch (NotSupportedException) {
            Debug.WriteLine("JsonSerializer.Deserialize returned NotSupportedException, returning a new directory.");
            temp_dictionary = new Dictionary<long, int>();
        }

        return temp_dictionary;
    }
}