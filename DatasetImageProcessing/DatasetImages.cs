using System.Diagnostics;
using OpenCvSharp;
using System.Drawing;

namespace DatasetImageProcessing
{
    public class DatasetImages{
        //dataset path
        private string _dataset_path; // "/dataset_folder" not "/dataset_folder/vendor/sku"
        //image processing
        private ImageProcessing _image_processing = new ImageProcessing();
        //image index
        public ImageIndex _image_index { get; }
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
        public DatasetImages(string dataset_path) {
            //todo: set path member
            _dataset_path = dataset_path;
            if (_dataset_path == null) {
                throw new Exception("no dataset path");
            }
            
            //set _image_index
            _image_index = new ImageIndex(_dataset_path);
        }
        
        // create a new ImageIndex using the _dataset_path member
        public ImageIndex getNewImageIndex() {
            return new ImageIndex(_dataset_path);
        }

        public void saveImageToDatasetAndChangeColor(long sku, string vendor, Mat image) {
            Debug.WriteLine("start image: " + DateTime.Now);

            //check if sku is in yolo indexes dictionary
            var yolo_label = new YoloLabel();
            if (!yolo_label.checkSkuIndexKnown(sku)) {
                Debug.WriteLine("Sku is not known in the yolo indexes dictionary. Please add this sku");
                return;
            }

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
            int index = _image_index.getSkuIndexAndIncrement(sku, vendor);
            
            //save original image
            var original_image_filename = index + "_original.png";
            Cv2.ImWrite(path + original_image_filename, image);
            ImageLabeling.createLabelFile(bounding_box, image, sku, path, original_image_filename);

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
            Debug.WriteLine("stop time: " + DateTime.Now);
        }
        

        // 
        public string getSkuFolderPath(long sku, string vendor) {
            throw new NotImplementedException();
            return string.Empty;
        }
    }
}
