using System.Diagnostics;
using OpenCvSharp;
using System.Drawing;

namespace DatasetImageProcessing
{
    //DatasetImages class for going through the whole process for processing a dataset image. See: saveImageToDatasetAndChangeColor
    public class DatasetImages{
        private string _dataset_path; // "/dataset_folder/" not "/dataset_folder/vendor/sku"
        private ImageProcessing _image_processing = new ImageProcessing();
        public ImageIndex _image_index { get; }
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
            Color.FromArgb(255, 210, 184, 87),
        };

        // Constructor that sets the _dataset_path and throws exceptions accordingly
        public DatasetImages(string dataset_path) {
            _dataset_path = dataset_path;
            if (_dataset_path == null) {
                throw new ArgumentException("no dataset path");
            }

            if (!(_dataset_path != null && Directory.Exists(_dataset_path))) {
                Directory.CreateDirectory(_dataset_path);
                Debug.WriteLine("DatasetImages constructor detected that there is no dataset folder on the given path. Creating this directory.");
            }

            //set _image_index
            _image_index = new ImageIndex(_dataset_path);
        }
        
        // create a new ImageIndex using the _dataset_path member
        public ImageIndex getNewImageIndex() {
            return new ImageIndex(_dataset_path);
        }

        //Run the whole process for 1 dataset image. This wil generate a bounding box and multiple images with different backgrounds.
        //These will be saves to the dataset folder in the vendor and sku folders and with a index file. These folders and index file
        //will be generated if they do not exist
        public void saveImageToDatasetAndChangeColor(long sku, string vendor, Mat image) {
            Debug.WriteLine("Dataset image processing: starting image with sku number: {0} with vendor: {1}", sku, vendor);
            
            //create path with vendor
            string path = Path.Combine(_dataset_path, vendor + "/");
                 
            //check if vendor path exist
            if (!Directory.Exists(path: path)) {
                Directory.CreateDirectory(path: path);
            }
            
            //create path with sku
            path = Path.Combine(_dataset_path, vendor + "/", sku + "/");
                
            //check if sku path exist otherwise create this directory
            if (!Directory.Exists(path: path)) {
                Directory.CreateDirectory(path: path);
            }

            //get the sku image index
            int index = _image_index.getSkuIndexAndIncrement(sku, vendor);
            
            //check if sku is in yolo indexes dictionary
            var yolo_label = new YoloLabel();
            if (!yolo_label.checkSkuIndexKnown(sku)) {
                Debug.WriteLine("Sku: {0}, with vendor: {1}, is not known in the yolo indexes dictionary. Saving the original image without processing and bounding box.", sku, vendor);
                
                //save original image
                Cv2.ImWrite(Path.Combine(path, index + ".png"), image);

                return;
            }

            //get alpha image of input
            Mat alpha_image = _image_processing.getAlphaMat(image);
            
            //get the bounding box
            int[] bounding_box = ImageLabeling.getBoundingBox(alpha_image);

            //save original image
            var original_image_filename = index + "_original.png";
            Cv2.ImWrite(Path.Combine(path, original_image_filename), image);
            ImageLabeling.createLabelFile(bounding_box, image, sku, path, original_image_filename);

            //for each color that we want to change the background to
            foreach (Color color in _colors) {
                //create filename for new image
                string image_filename = + index + "_" + color.Name + ".png";
                
                //create new image with the changed background
                Mat new_image = _image_processing.changeColor(image, alpha_image, color);

                //write the new image to a file
                Cv2.ImWrite(Path.Combine(path, image_filename), new_image);
                
                //write the bounding box file
                ImageLabeling.createLabelFile(bounding_box, new_image, sku, path, image_filename);
            }
            
            Debug.WriteLine("Dataset image processing: Done with sku number: {0} with vendor: {1}", sku, vendor);
        }
    }
}
