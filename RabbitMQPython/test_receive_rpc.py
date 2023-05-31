import time
import cv2
import receive_rpc
import numpy as np
from ultralytics import YOLO
import cv2
import matplotlib.pyplot as plt
import numpy as np
import os


class ObjectDetectionModel:
    def __init__(self):
        self.model = self.loadObjectDetectionModel()
        self.names = self.model.names
        print("ObjectDetectionModel loaded")

    def loadObjectDetectionModel(self):
        try:
            print("Loading model")
            return YOLO(
                '../ObjectDetection/MontaOrderVerification/Best_oot_model/weights/Best_oot_model.pt')  # load the custom model
        except:
            print("could not load model")
            exit(200)


    def getSkusFromImage(self, image: np.ndarray) -> list[int]:
        print("Getting skus from image")
        results = self.model.predict(source=image, conf=0.45)

        skus = []
        for result in results:
            for c in result.boxes.cls:
                if self.names[int(c)] == "keto granola":
                    skus.append(8719992763139)
                elif self.names[int(c)] == "pumpkin spice":
                    skus.append(8719992763351)
                elif self.names[int(c)] == "koffie keto":
                    skus.append(8719992763917)
                elif self.names[int(c)] == "Chia Spice":
                    skus.append(8719992763078)
                else:
                    skus.append(self.names[int(c)])
        print(skus)
        return skus


# class test function
if __name__ == '__main__':
    print("Running test")
    image = cv2.imread('../Datasets/Dataset_OOT_V3_640/Test/test (5).jpg')

    ObjectDetector = ObjectDetectionModel()

    receiver = receive_rpc.RabbitMQReceiver(
        "20.13.19.141",
        "python_test_user",
        "jedis",
        ObjectDetector.getSkusFromImage)
    receiver.run()
