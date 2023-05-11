using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace DatasetImageProcessing
{
    public class YoloLabel {
        // dictionary for converting skus to yolo indexes
        private Dictionary<long, int> yolo_indexes = new Dictionary<long, int>() {
            {8719992763139, 0},
            {8719992763351, 1},
            {8719992763511, 2},
            {8719992763542, 3}
        };
	
        // boundary box struct
        private struct BoundaryBox {
            public float x_center { get; set; }
            public float y_center { get; set; }
            public float width { get; set; }
            public float height { get; set; }
        }

        //get the yolo index from the sku using the dictionary above
        private int getYoloIndexFromSku(long sku) {
            return yolo_indexes[sku];
        }

        //convert the xml boundary box to the yolo format
        private BoundaryBox convertToYoloBoundaryBox(int width, int height, int xmin, int ymin, int xmax, int ymax) {
            float x_center = ((xmax + xmin) / (float)2) / (float)width;
            float y_center = ((ymax + ymin) / (float)2) / (float)height;
            float bbox_width = (xmax - xmin) / (float)width;
            float bbox_height = (ymax - ymin) / (float)height;

            return new BoundaryBox{
                x_center = x_center,
                y_center = y_center,
                width = bbox_width,
                height = bbox_height
            };
        }

        //return a yolo format string from the yolo index and boundary box
        private string getBoundaryBoxString(int yolo_index, BoundaryBox boundary_box) {
            return yolo_index + " " + boundary_box.x_center + " " + boundary_box.y_center + " " + boundary_box.width + " " + boundary_box.height;
        }

        public string getBoundaryBoxStringFromXmlFormat(long sku ,int width, int height, int xmin, int ymin, int xmax, int ymax) {
            return getBoundaryBoxString(
                getYoloIndexFromSku(sku),
                convertToYoloBoundaryBox(
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
    
    internal class ImageLabeling
    {
        // Constructor
        public ImageLabeling() { }

        // 
        public int[] getBoundingBox(Mat input)
        {
            return new int[] { 0, 0, 0 ,0 };
        }

        // 
        public void createLabel(int[] bounding_box, Mat picture, string sku, string path)
        {
        }
    }
}
