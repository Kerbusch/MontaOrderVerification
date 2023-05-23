using OpenCvSharp;

namespace DatasetImageProcessing
{
    // class for creating the yolo label strings
    public class YoloLabel {
        // dictionary for converting skus to yolo indexes
        private Dictionary<long, int> _yolo_indexes = new Dictionary<long, int>() {
            {8719992763139, 0},
            {8719992763351, 1},
            {8719992763511, 2},
            {8719992763634, 3},
            {8719992763573, 4}
        };

        // bounding box struct
        private struct BoundingBox {
            public float x_center { get; set; }
            public float y_center { get; set; }
            public float width { get; set; }
            public float height { get; set; }
        }

        //get the yolo index from the sku using the dictionary above
        public int getYoloIndexFromSku(long sku) {
            return _yolo_indexes[sku];
        }

        //check of the sku is know in the class dictionary
        public bool checkSkuIndexKnown(long sku) {
            return _yolo_indexes.ContainsKey(sku);
        }

        //convert the xml bounding box to the yolo format
        private BoundingBox convertToYoloBoundingBox(int width, int height, int xmin, int ymin, int xmax, int ymax) {
            float x_center = ((xmax + xmin) / (float)2) / (float)width;
            float y_center = ((ymax + ymin) / (float)2) / (float)height;
            float bbox_width = (xmax - xmin) / (float)width;
            float bbox_height = (ymax - ymin) / (float)height;

            return new BoundingBox{
                x_center = x_center,
                y_center = y_center,
                width = bbox_width,
                height = bbox_height
            };
        }

        //return a yolo format string from the yolo index and bounding box
        private string getBoundingBoxString(int yolo_index, BoundingBox bounding_box) {
            //create yolo string
            string yolo_string = yolo_index + " " + bounding_box.x_center + " " + bounding_box.y_center + " " + bounding_box.width + " " + bounding_box.height;
            
            //replace ',' with '.'
            yolo_string = yolo_string.Replace(",", ".");
            
            return yolo_string;
        }

        //return a yolo format string from the yolo index and the xml bounding box values
        public string getBoundingBoxStringFromXmlFormat(long sku ,int width, int height, int xmin, int ymin, int xmax, int ymax) {
            return getBoundingBoxString(
                getYoloIndexFromSku(sku),
                convertToYoloBoundingBox(
                    width,
                    height,
                    xmin,
                    ymin,
                    xmax, 
                    ymax
                )
            );
        }
    }
    
    // class for creating the label file
    public class ImageLabeling
    {
        // get bounding box from input alpha image
        public static int[] getBoundingBox(Mat alpha_image)
        {
            int[] x_values = { 0, alpha_image.Width }; // maxX, minX
            int[] y_values = { 0, alpha_image.Height }; // maxY, minY
            int[][] rtn = { x_values, y_values };

            for (int y = 0; y < alpha_image.Rows; ++y)
            {
                for (int x = 0; x < alpha_image.Cols; ++x)
                {
                    if (alpha_image.At<Vec4b>(y, x)[3] != 0)
                    {
                        if (x > rtn[0][0]) // maxX
                        {
                            rtn[0][0] = x;
                        }

                        if (x < rtn[0][1]) // minx
                        {
                            rtn[0][1] = x;
                        }

                        if (y > rtn[1][0]) // maxY
                        {
                            rtn[1][0] = y;
                        }

                        if (y < rtn[1][1]) // minY
                        {
                            rtn[1][1] = y;
                        }
                    }
                }
            }

            //image to xml format bounding box
            int[][] max_min = rtn;

            return new int[] { max_min[0][1], max_min[1][1], max_min[0][0] ,max_min[1][0] };
        }

        // create a label in yolo format and write this to file.
        public static void createLabelFile(int[] bounding_box, Mat image, long sku, string path, string filename_image)
        {
            // sku = 89548934
            // path = ../../
            // filename_image = 28583948923_index_color.txt
            //                  28583948923_0_0.txt

            //get yolo string
            var yolo_label = new YoloLabel();
            var yolo_string = yolo_label.getBoundingBoxStringFromXmlFormat(
                sku,
                image.Width,
                image.Height,
                bounding_box[0],
                bounding_box[1],
                bounding_box[2],
                bounding_box[3]
            );

            //get filename + path for label
            string label_filename = "";
            if (filename_image.Contains(".png")) {
                label_filename = filename_image.Replace("png", "txt");
            }
            else if (filename_image.Contains(".jpg")) {
                label_filename = filename_image.Replace("jpg", "txt");
            }
            else {
                label_filename = filename_image + ".txt";
            }
            
            //bounding box path
            string bounding_box_path = Path.Combine(path, label_filename);
             
            //write label
            using (StreamWriter output_file = new StreamWriter(bounding_box_path)) {
                output_file.WriteLine(yolo_string);
            }
        }
    }
}
