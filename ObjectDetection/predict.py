# import cv2
# from ultralytics import YOLO
#
# # Load the YOLOv8 model
# model = YOLO('MontaOrderVerification/oot_model0003/weights/best.pt')  # load a custom model
#
# # Open the video file
# # video_path = "path/to/your/video/file.mp4"
# # cap = cv2.VideoCapture(video_path)
#
# cap = cv2.VideoCapture(0)

# # Loop through the video frames
# while cap.isOpened():
#     # Read a frame from the video
#     success, frame = cap.read()
#
#     if success:
#         # Run YOLOv8 inference on the frame
#         results = model(frame)
#
#         # Visualize the results on the frame
#         annotated_frame = results[0].plot()
#
#         # Display the annotated frame
#         cv2.imshow("YOLOv8 Inference", annotated_frame)
#
#         # Break the loop if 'q' is pressed
#         if cv2.waitKey(1) & 0xFF == ord("q"):
#             break
#     else:
#         # Break the loop if the end of the video is reached
#         break
#
# # Release the video capture object and close the display window
# cap.release()
# cv2.destroyAllWindows()


from ultralytics import YOLO
import cv2
import matplotlib.pyplot as plt
import numpy as np
import os
# %matplotlib inline
os.environ["KMP_DUPLICATE_LIB_OK"] = "TRUE"
# Load a model
model = YOLO('MontaOrderVerification/oot_model/weights/best.pt')  # load a custom model

folder_path = '../Datasets/Dataset_OOT_V3/Test/'
for files in os.listdir(folder_path):
    # Predict with the model
    results = model.predict(os.path.join(folder_path, files))

    res_plotted = results[0].plot()

    # Convert the image from BGR to RGB
    image = cv2.cvtColor(res_plotted, cv2.COLOR_BGR2RGB)
    plt.imshow(image)
    plt.axis('off')
    plt.show()
