from ultralytics import YOLO
import cv2

# Load a model
model = YOLO('runs/detect/oot_model/weights/best.pt')  # load a custom model

# Predict with the model
results = model.predict('datasets/DatasetOOT/Test/640x640_test_grijs2.png')
res_plotted = results[0].plot()

cv2.imshow("result", res_plotted)
cv2.imwrite("out.jpg", res_plotted)

cv2.waitKey(0)
