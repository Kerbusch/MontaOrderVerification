import os
import shutil
import xml.etree.ElementTree as ET


def xml_to_yolo_bbox(bbox, w, h):
    # xmin, ymin, xmax, ymax
    x_center = ((bbox[2] + bbox[0]) / 2) / w
    y_center = ((bbox[3] + bbox[1]) / 2) / h
    width = (bbox[2] - bbox[0]) / w
    height = (bbox[3] - bbox[1]) / h
    return [x_center, y_center, width, height]


def yolo_to_xml_bbox(bbox, w, h):
    # x_center, y_center width heigth
    w_half_len = (bbox[2] * w) / 2
    h_half_len = (bbox[3] * h) / 2
    xmin = int((bbox[0] * w) - w_half_len)
    ymin = int((bbox[1] * h) - h_half_len)
    xmax = int((bbox[0] * w) + w_half_len)
    ymax = int((bbox[1] * h) + h_half_len)
    return [xmin, ymin, xmax, ymax]


# add new class labels to this list
classes = ['8719992763139', '8719992763351', '8719992763511', '8719992763542']

# input_dir = "DatasetOOT/Training/"
# output_dir = "Dataset_OOT_YOLOv8/Training/"

# input_dir = "../Datasets/DatasetOOT/Validation/"
# output_dir = "../Datasets/Dataset_OOT_YOLOv8/Validation/"

# create the labels folder (output directory)
os.makedirs(output_dir, exist_ok=True)

for root, dirs, files in os.walk(input_dir):
    for filename in files:
        if filename.endswith('.xml'):
            print("Root:", root)
            print("Filename:", filename)

            # Parse TensorFlow XML file
            xml_file_path = os.path.join(root, filename)
            tree = ET.parse(xml_file_path)
            treeroot = tree.getroot()

            result = []

            width = int(treeroot.find("size").find("width").text)
            height = int(treeroot.find("size").find("height").text)

            for obj in treeroot.findall('object'):
                label = treeroot.find("folder").text
                # check for new classes and append to list
                if label not in classes:
                    classes.append(label)
                index = classes.index(label)
                pil_bbox = [int(x.text) for x in obj.find("bndbox")]
                yolo_bbox = xml_to_yolo_bbox(pil_bbox, width, height)
                # convert data to string
                bbox_string = " ".join([str(x) for x in yolo_bbox])
                result.append(f"{index} {bbox_string}")

            if result:
                # Copy and rename the image
                image_filename = os.path.splitext(filename)[0] + '.jpg'
                image_path = os.path.join(root, image_filename)
                new_image_path = os.path.join(output_dir, image_filename)
                shutil.copy(image_path, new_image_path)

                # generate a YOLO format text file for each xml file
                with open(os.path.join(output_dir, f"{os.path.splitext(filename)[0]}.txt"), "w", encoding="utf-8") as f:
                    f.write("\n".join(result))
