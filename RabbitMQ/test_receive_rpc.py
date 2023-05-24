import time
import cv2
import receive_rpc
import numpy as np
from ultralytics import YOLO
import cv2
import matplotlib.pyplot as plt
import numpy as np
import os

# manual inputting the skus from the image.
def manualGetSkusFromImage(image: np.ndarray) -> list[int]:
    print("input skus for this image.")
    # show image
    cv2.imshow('received image', image)
    cv2.waitKey(1)

    # get skus from commandline
    # wait till empty enter

    output_list = []

    done = False
    while not done:
        x = input(":")
        if x == "":
            done = True
        elif x == "crash":
            exit(200)
        else:
            try:
                y = int(x)
                # check if input is smaller than c# long
                if y < 9223372036854775807:
                    output_list.append(y)
                else:
                    print("value to big for a c# long")
            except ValueError:
                print("cant convert input to int, retry")

    print("send: {}".format(output_list))
    print("\n\n\n\n\n\n")

    # close image
    cv2.destroyAllWindows()
    return output_list


# debug function that will be replaced by the sku recognition AI.
def getSkusFromImage(image: np.ndarray) -> list[int]:

    model = loadObjectDetectionModel()
    names = model.names

    results = model.predict(source=image, conf=0.45)

    skus = []
    for result in results:
        for c in result.boxes.cls:
            skus.append(names[int(c)])

    return skus

def loadObjectDetectionModel():
    model = YOLO('../ObjectDetection/MontaOrderVerification/oot_model12/weights/best.pt')  # load the custom model
    return model

# class test function
if __name__ == '__main__':
    image = cv2.imread('../Datasets/Dataset_OOT_V3_640/Test/test (5).jpg')

    receiver = receive_rpc.RabbitMQReceiver(
        "20.13.19.141",
        "python_test_user",
        "jedis",
        getSkusFromImage(image))
    receiver.run()
