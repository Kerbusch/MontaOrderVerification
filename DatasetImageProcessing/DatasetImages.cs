using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace DatasetImageProcessing
{
    internal class DatasetImages
    {
        // Constructor
        public DatasetImages() { }

        // 
        public void saveImageToDatasetAndChangeColor(Mat picture, string path, long sku, int index)
        {
            var image_processing = new ImageProcessing();
            var image_labeling = new ImageLabeling();

            Mat alpha = image_processing.getAlphaMat(picture);
            int[] bounding_box = image_labeling.getBoundingBox(alpha);

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
                image_labeling.createLabel(bounding_box, new_mat, sku, path, file_name);
            }
        }

        // 
        public string getSkuFolderPath(long sku, string vendor)
        {
            return "C:\\Users\\jarno\\OneDrive\\Documenten\\MontaOrderVerification\\DatasetImageProcessing\\test_pictures\\";
        }
    }
}
