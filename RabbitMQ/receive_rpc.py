# requirements: opencv-python, numpy, pika
import json
import cv2
import numpy as np
import pika


# decode the image received from rabbitmq to a np.ndarray for opencv
def decodeImageFromBytes(image_data: bytes) -> np.ndarray:
    image_array = np.frombuffer(image_data, np.uint8)
    image = cv2.imdecode(image_array, cv2.IMREAD_COLOR)
    return image


# rabbitmq server for accepting client images and returning a list of skus.
# This class is an RPC service and is blocking!
class RabbitMQReceiver:
    # constructor that runs the connection setup to the rabbitmq server on host, with
    # input_function has to be a function that accepts a numpy ndarray (image) and returns a list of skus
    def __init__(self, host: str, username_rabbitmq: str, password_rabbitmq: str, queue_name: str, input_function):
        self.queue_name = queue_name
        self.func = input_function

        # todo: add try except for rabbitmq connection
        self.connection = pika.BlockingConnection(
            pika.ConnectionParameters(
                host=host,
                credentials=pika.PlainCredentials(
                    username=username_rabbitmq,
                    password=password_rabbitmq
                )
            )
        )

        self.channel = self.connection.channel()

        self.__declareQueue()
        self.__setupConsumer()

    # use the given member function to get the return statement and send it back in the reply queue
    def __onRequest(self, ch, method, properties, body):
        print("start request")
        image = decodeImageFromBytes(body)

        sku_list = self.func(image)

        y = json.dumps(sku_list)

        ch.basic_publish(exchange='',
                         routing_key=properties.reply_to,
                         properties=pika.BasicProperties(
                             correlation_id=properties.correlation_id,
                             content_type="application/json"
                         ),
                         body=y)

        ch.basic_ack(delivery_tag=method.delivery_tag)

        print("request done")

    # declare the queue on the rabbitmq server
    def __declareQueue(self):
        self.channel.queue_declare(queue=self.queue_name)

    # subscribe as a consumer to the queue and call the __onRequest function on callback
    def __setupConsumer(self):
        self.channel.basic_consume(queue=self.queue_name, on_message_callback=self.__onRequest, auto_ack=False)

    # start consuming from the queue. This function is blocking!
    def run(self):
        print("started consuming")
        self.channel.start_consuming()

    def __del__(self):
        self.connection.close()


# debug function that will be replaced by the sku recognition AI.
def apply_filter_over_image(image: np.ndarray) -> np.ndarray:
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    return gray


# debug function that will be replaced by the sku recognition AI.
def get_skus_from_image(image: np.ndarray) -> list[int]:
    show_image = False
    if show_image:
        cv2.imshow('received image', image)

        cv2.waitKey(0)
        cv2.destroyAllWindows()

    lst = [8719992763139, 8719992763917]
    return lst


# class test function
if __name__ == '__main__':
    receiver = RabbitMQReceiver("localhost", "python_test_user", "jedis", "rpc_queue", get_skus_from_image)
    # receiver = RabbitMQReceiver("192.168.178.33", "python_test_user", "jedis", "rpc_queue", get_skus_from_image)
    receiver.run()
