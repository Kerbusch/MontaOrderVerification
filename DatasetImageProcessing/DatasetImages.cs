using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;
using System.Net;
using System.Text.Json;

namespace DatasetImageProcessing
{
    internal class DatasetImages{
        //dataset path
        private string _dataset_path; // "/dataset_folder" not "/dataset_folder/vendor/sku"
        //image processing
        private ImageProcessing _image_processing = new ImageProcessing();
        //colors
        private Color[] _colors = {
            Color.Black,
            Color.White,
            Color.Red,
            Color.Blue,
            Color.Yellow,
            Color.Green,
            Color.BlueViolet,
            Color.Pink,
            Color.Orange,
            Color.FromArgb(255, 210, 184, 87)
        };

        // Constructor
        public DatasetImages() {
            //todo: set path member
            _dataset_path = "C:\\Users\\Daan\\RiderProjects\\MontaOrderVerification\\DatasetImageProcessing\\";
            if (_dataset_path == null) {
                throw new Exception("no dataset path");
            }
        }

        // 
        public void saveImageToDatasetAndChangeColor(Mat picture, string path, long sku, int index)
        {
            var image_processing = new ImageProcessing();

            Mat alpha = image_processing.getAlphaMat(picture);
            int[] bounding_box = ImageLabeling.getBoundingBox(alpha);

            Color[] colors = {Color.Black,
                              Color.White,
                              Color.Red,
                              Color.Blue,
                              Color.Yellow,
                              Color.Green,
                              Color.BlueViolet,
                              Color.Pink,
                              Color.Orange,
                              Color.FromArgb(255, 210, 184, 87)};

            for (int i = 0; i < colors.Length; i++)
            {
                string file_name = sku + "_" + index + "_" + i;
                Mat new_mat = image_processing.changeColor(picture, alpha, colors[i]);
                Cv2.ImWrite((path + "\\" + file_name + ".jpg"), new_mat);
                ImageLabeling.createLabelFile(bounding_box, new_mat, sku, path, file_name);
            }
        }

        public void saveImageToDatasetAndChangeColor(long sku, string vendor, Mat image) {
            Console.WriteLine("start image: " + DateTime.Now);
            
            //get alpha image of input
            Mat alpha_image = _image_processing.getAlphaMat(image);
            
            //get the bounding box
            int[] bounding_box = ImageLabeling.getBoundingBox(alpha_image);
            
            //create path with vendor
            string path = _dataset_path + vendor + "\\";
                 
            //check if vendor path exist
            if (!Directory.Exists(path: path)) {
                Directory.CreateDirectory(path: path);
            }
            
            //create path with sku
            path += sku + "\\";
                
            //check if sku path exist
            if (!Directory.Exists(path: path)) {
                Directory.CreateDirectory(path: path);
            }

            //get the sku image index
            int index = getSkuIndexAndIncrement(sku, vendor, _dataset_path);

            //for each color that we want to change the background to
            foreach (Color color in _colors) {
                //create filename for new image
                string image_filename = + index + "_" + color.Name + ".png";
                
                //create new image with the changed background
                Mat new_image = _image_processing.changeColor(image, alpha_image, color);

                //write the new image to a file
                Cv2.ImWrite(path + image_filename, new_image);
                
                //write the bounding box file
                ImageLabeling.createLabelFile(bounding_box, new_image, sku, path, image_filename);
            }
            Console.WriteLine("stop time: " + DateTime.Now);
        }

        //get the sku index from the file and increment the value.
        int getSkuIndexAndIncrement(long sku, string vendor, string dataset_path) {
            //const filename
            const string sku_indexes_filename = "sku_indexes.json";
            
            //sku indexes path
            string sku_indexes_path = dataset_path + vendor + "\\" + sku_indexes_filename;
            
            //read sku indexes file
            var sku_indexes = readJsonFile(sku_indexes_path);
            
            //check if key exist if not create with value 0
            if (!sku_indexes.ContainsKey(sku)) {
                sku_indexes.Add(sku, 0);
            }
            
            //set output
            int output = sku_indexes[sku]++;

            //write sku indexes file
            writeJsonFile(sku_indexes_path, sku_indexes);

            return output;
        }

        //write the sku indexes json file from the member
        public void writeJsonFile(string file_path, Dictionary<long, int> sku_indexes) {
            //create json serializer options
            var json_serializer_options = new JsonSerializerOptions();
            json_serializer_options.WriteIndented = true;
            
            //create json string
            string json_string = JsonSerializer.Serialize(sku_indexes, json_serializer_options);
            
            //write file
            File.WriteAllText(file_path, json_string);
        }
        
        //read the sku indexes json file and set the member
        public Dictionary<long, int> readJsonFile(string file_path) {
            //get string from file
            string json_string = "";
            
            //check if file exist
            if (File.Exists(file_path)) {
                try {
                    json_string = File.ReadAllText(file_path);
                }
                catch {
                    Debug.WriteLine("ReadAllText could not read file");
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
                Debug.WriteLine("JsonSerializer.Deserialize returned ArgumentNullException");
                temp_dictionary = new Dictionary<long, int>();
            }
            catch (JsonException) {
                Debug.WriteLine("JsonSerializer.Deserialize returned JsonException");
                temp_dictionary = new Dictionary<long, int>();
            }
            catch (NotSupportedException) {
                Debug.WriteLine("JsonSerializer.Deserialize returned NotSupportedException");
                temp_dictionary = new Dictionary<long, int>();
            }

            return temp_dictionary;
        }

        // 
        public string getSkuFolderPath(long sku, string vendor)
        {
            return "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\DatasetImageProcessing\\test_pictures\\";
        }
    }
}
