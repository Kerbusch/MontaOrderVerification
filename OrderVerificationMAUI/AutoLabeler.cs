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
        public static void labelPictures(string[][] files, string sku_nummer)
        {
            foreach (string[] file in files)
            {
                CameraModule.makeBackgroundTransparent(file[1]);
                createLabel(file, sku_nummer);
            }
        }

        // creates a label for the given image
        public static void createLabel (string[] file, string sku_nummer)
        {
            if (File.Exists(file[1].Replace("png", "xml")))
            {
                OpenCvSharp.Mat image = OpenCvSharp.Cv2.ImRead(file[1]);
                int[] max_min = getMaxMin(image);
                using (XmlWriter writer = XmlWriter.Create(file[1].Replace("png", "xml")))
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
                                writer.WriteElementString("xmin", $"{max_min[0]}");
                                writer.WriteElementString("ymin", $"{max_min[1]}");
                                writer.WriteElementString("xmax", $"{max_min[2]}");
                                writer.WriteElementString("ymax", $"{max_min[3]}");
                            writer.WriteEndElement();
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                }
            }
        }

        // Gets the max and min values of the non black pixels
        public static int[] getMaxMin(OpenCvSharp.Mat image)
        {
            int[] rtn = { 0, 0, image.Width, image.Height }; // maxX, maxY, minX, minY

            for (int y = 0; y < image.Rows; ++y)
            {
                for (int x = 0; x < image.Cols; ++x)
                {
                    Vec4b pixel = image.At<Vec4b>(y, x);
                    if (pixel[0] < 255 && pixel[1] < 255 && pixel[2] < 255)
                    {
                        if (x > rtn[0]) // maxX
                        {
                            rtn[0] = x;
                        }

                        if (y > rtn[1]) // maxY
                        {
                            rtn[1] = y;
                        }

                        if (x < rtn[2]) // minx
                        {
                            rtn[2] = x;
                        }

                        if (y < rtn[3]) // minY
                        {
                            rtn[3] = y;
                        }
                    }
                }
            }

            return rtn;
        }
    }   
} 
