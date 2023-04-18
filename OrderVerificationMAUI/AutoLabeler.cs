using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrderVerificationMAUI
{
    // Class to label picture
    public class AutoLabeler
    {
        // Constructor
        public AutoLabeler() { }

        // Loop through all the images and labels them
        public static void labelPictures(List<List<string>> files, string sku_nummer)
        {
            foreach (List<string> file in files)
            {
                createLabel(file, sku_nummer);
            }
        }

        // creates a label for the given image
        public static void createLabel (List<string> file, string sku_nummer)
        {
            if (!File.Exists(file[1].Replace("jpg", "xml")))
            {
                OpenCvSharp.Mat image = CameraModule.makeBackgroundTransparent(OpenCvSharp.Cv2.ImRead(file[1]));
                int[][] max_min = getMaxMin(image);
                using (XmlWriter writer = XmlWriter.Create(file[1].Replace("jpg", "xml")))
                {
                    writer.WriteStartDocument();    
                    writer.WriteStartElement("annotation");
                    writer.WriteElementString("folder", sku_nummer);
                        writer.WriteElementString("filename", file[0]);
                        writer.WriteElementString("path", file[1]);
                        writer.WriteStartElement("source");
                            writer.WriteElementString("database", "Unknown");
                        writer.WriteEndElement();
                        writer.WriteStartElement("size");
                            writer.WriteElementString("width", $"{image.Width}");
                            writer.WriteElementString("height", $"{image.Height}");
                            writer.WriteElementString("depth", $"{image.Depth}");
                        writer.WriteEndElement();
                        writer.WriteElementString("segmented", "0");
                        writer.WriteStartElement("object");
                            writer.WriteElementString("name", sku_nummer);
                            writer.WriteElementString("pose", "Unspecified");
                            writer.WriteElementString("truncated", "0");
                            writer.WriteElementString("difficult", "0");
                            writer.WriteStartElement("bndbox");
                                writer.WriteElementString("xmin", $"{max_min[0][1]}");
                                writer.WriteElementString("ymin", $"{max_min[1][1]}");
                                writer.WriteElementString("xmax", $"{max_min[0][0]}");
                                writer.WriteElementString("ymax", $"{max_min[1][0]}");
                            writer.WriteEndElement();
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                }
            }
        }

        // Gets the max and min values of the non black pixels
        public static int[][] getMaxMin(OpenCvSharp.Mat image)
        {
            int[] x_values = { 0, image.Width }; // maxX, minX
            int[] y_values = { 0, image.Height }; // maxY, minY
            int[][] rtn = { x_values, y_values };

            for (int y = 0; y < image.Rows; ++y)
            {
                for (int x = 0; x < image.Cols; ++x)
                {
                    if (image.At<Vec4b>(y, x)[3] != 0)
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

            return rtn;
        }
    }   
} 
