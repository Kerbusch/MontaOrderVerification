using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace DatasetImageProcessing
{
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
