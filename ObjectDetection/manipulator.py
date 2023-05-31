import os
from PIL import Image, ExifTags, ImageEnhance
import shutil

# the input folder path is the folder where the normal images are located
input_folder_path = "Datasets/Dataset_OOT_V3_640/Training"
# the output folder path is the folder where the manipulated images will be saved
output_folder_path = "Dataset_OOT_V5_640/Training"

# the same for the validation set
# #the input folder path is the folder where the normal images are located
# input_folder_path = "Datasets/Dataset_OOT_V3_640/Validation"
# #the output folder path is the folder where the manipulated images will be saved
# output_folder_path = "Dataset_OOT_V5_640/Training/Validation"


def copy_txt_file(input_file, output_folder):
    # Create the output folder if it doesn't exist
    os.makedirs(output_folder, exist_ok=True)

    # Construct the output path for the copied file
    output_file = os.path.join(output_folder, os.path.basename(input_file))

    # Copy the file to the output folder
    shutil.copyfile(input_folder_path + "/" + input_file, output_file)


def apply_brightness_filter(image, brightness_factor):
    # Create a brightness enhancer object
    enhancer = ImageEnhance.Brightness(image)

    # Adjust the brightness level
    brightened_image = enhancer.enhance(brightness_factor)

    return brightened_image


def apply_contrast_filter(image, brightness_factor):
    # Create a brightness enhancer object
    enhancer = ImageEnhance.Contrast(image)

    # Adjust the brightness level
    contrasted_image = enhancer.enhance(brightness_factor)

    return contrasted_image


def apply_sharpness_filter(image, brightness_factor):
    # Create a brightness enhancer object
    enhancer = ImageEnhance.Sharpness(image)

    # Adjust the brightness level
    sharpened_image = enhancer.enhance(brightness_factor)

    return sharpened_image


def apply_filter_to_images(input_folder, output_folder):
    # Create the output folder if it doesn't exist
    os.makedirs(output_folder, exist_ok=True)

    # Get a list of all image files in the input folder
    image_files = [f for f in os.listdir(input_folder) if os.path.isfile(os.path.join(input_folder, f))]
    # Iterate over each image file
    iterator = 0
    while iterator < 4:
        for file_name in image_files:
            if iterator == 0:
                filter_value = 1.6
                contrast_value = 0.4
            elif iterator == 1:
                filter_value = 1.3
                contrast_value = 0.7
            elif iterator == 2:
                filter_value = 0.7
                contrast_value = 1.3
            elif iterator == 3:
                filter_value = 0.4
                contrast_value = 1.6

            if file_name.__contains__(".txt"):
                # Example usage
                if file_name.__contains__("classes"):
                    if iterator == 0:
                        copy_txt_file(file_name, output_folder)
                        os.rename(output_folder_path + "/" + file_name, output_folder_path + "/" + file_name)
                    continue
                copy_txt_file(file_name, output_folder)
                os.rename(output_folder_path + "/" + file_name,
                          output_folder_path + "/" + file_name.split(".")[0] + "_brightness" + str(iterator) + ".txt")
                copy_txt_file(file_name, output_folder)
                os.rename(output_folder_path + "/" + file_name,
                          output_folder_path + "/" + file_name.split(".")[0] + "_contrasted" + str(iterator) + ".txt")
                copy_txt_file(file_name, output_folder)
                os.rename(output_folder_path + "/" + file_name,
                          output_folder_path + "/" + file_name.split(".")[0] + "_sharpened" + str(iterator) + ".txt")
                copy_txt_file(file_name, output_folder)
                os.rename(output_folder_path + "/" + file_name,
                          output_folder_path + "/" + file_name.split(".")[0] + "_combi" + str(iterator) + ".txt")

                continue
            new_filename = file_name.split(".")[0] + "_brightness" + str(iterator) + ".jpg"
            new_filename_contrasted = file_name.split(".")[0] + "_contrasted" + str(iterator) + ".jpg"
            new_filename_sharpened = file_name.split(".")[0] + "_sharpened" + str(iterator) + ".jpg"
            new_filename_combi = file_name.split(".")[0] + "_combi" + str(iterator) + ".jpg"
            # Construct the input and output paths for the current image
            input_path = os.path.join(input_folder, file_name)
            output_path = os.path.join(output_folder, new_filename)
            output_contrast_path = os.path.join(output_folder, new_filename_contrasted)
            output_sharp_path = os.path.join(output_folder, new_filename_sharpened)
            output_combi_path = os.path.join(output_folder, new_filename_combi)

            # Open the image file
            # Save the manipulated image to the output folder
            image = Image.open(input_path)
            # Apply the desired filter or manipulation to the image
            # In this example, we convert the image to grayscale 
            exif_data = image.info.get('exif')
            # Check if the Exif data contains orientation information
            brighter_image = apply_brightness_filter(image, filter_value)
            brighter_image.save(output_path, exif=exif_data)
            contrasted_image = apply_contrast_filter(image, contrast_value)
            contrasted_image.save(output_contrast_path, exif=exif_data)
            sharp_image = apply_sharpness_filter(image, filter_value)
            sharp_image.save(output_sharp_path, exif=exif_data)
            combi_image = apply_brightness_filter(image, filter_value)
            combi_image = apply_contrast_filter(combi_image, 2.1)
            combi_image.save(output_combi_path, exif=exif_data)
        iterator += 1
    print("Image manipulation complete.")


# Example usage
apply_filter_to_images(input_folder_path, output_folder_path)
