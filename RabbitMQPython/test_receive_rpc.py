import time
import cv2
import receive_rpc
import numpy as np


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
    show_image = True
    if show_image:
        cv2.imshow('received image', image)

        cv2.waitKey(0)
        cv2.destroyAllWindows()
    else:
        print("sleeping to simulate a workload")
        time.sleep(2)

    lst = [8719992763139, 8719992763917]
    return lst


# class test function
if __name__ == '__main__':
    receiver = receive_rpc.RabbitMQReceiver(
        "20.13.19.141",
        "python_test_user",
        "jedis",
        manualGetSkusFromImage)
    receiver.run()
