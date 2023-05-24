# requirements: opencv-python, numpy, pika
import json
import socket

import cv2
import numpy as np
import pika
import pika.exceptions


# Custom InvalidHostAddress exception for when the host address of rabbitmq is invalid
class InvalidHostAddress(Exception):
    def __init__(self, host: str, port: str = "5672"):
        self.host = host
        self.port = port
        self.message = "Exception: Cant locate rabbitmq server on host: " + self.host + " with port " + self.port + "."
        super().__init__(self.message)


# Custom InvalidCredentials exception for when the credentials of rabbitmq are invalid
class InvalidCredentials(Exception):
    def __init__(self):
        super().__init__("Exception: The rabbitmq server credentials are not valid")


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
    def __init__(self, host: str, username_rabbitmq: str, password_rabbitmq: str, input_function):
        self.queue_name = "rpc_queue_sku_image"
        self.func = input_function

        # connection credentials
        credentials = pika.PlainCredentials(
            username=username_rabbitmq,
            password=password_rabbitmq
        )

        # client properties
        client_properties = {'connection_name': 'PYTHON AI: image to skus'}

        # try to get a connection to the rabbitmq server
        try:
            self.connection = pika.BlockingConnection(
                pika.ConnectionParameters(
                    host=host,
                    credentials=credentials,
                    client_properties=client_properties
                )
            )
            self.is_connected = True

        # host address is incorrect
        except socket.gaierror:
            self.is_connected = False
            raise InvalidHostAddress(host=host)

        # credentials are incorrect
        except pika.exceptions.ProbableAuthenticationError:
            self.is_connected = False
            raise InvalidCredentials

        # get a channel from the connection
        self.channel = self.connection.channel()

        # declare the queue
        self.__declareQueue()

        # set up the consumer
        self.__setupConsumer()

    # use the given member function to get the return statement and send it back in the reply queue
    def __onRequest(self, ch, method, properties, body):
        image = decodeImageFromBytes(body)

        # get the sku list from the image using the member function
        sku_list: list = self.func(image)

        # convert the sku list to json
        json_string = json.dumps(sku_list)

        # publish the json string on the return channel
        ch.basic_publish(exchange='',
                         routing_key=properties.reply_to,
                         properties=pika.BasicProperties(
                             correlation_id=properties.correlation_id,
                             content_type="application/json"
                         ),
                         body=json_string)

        # acknowledge request
        ch.basic_ack(delivery_tag=method.delivery_tag)

    # declare the queue on the rabbitmq server
    def __declareQueue(self):
        self.channel.queue_declare(
            queue=self.queue_name,
            durable=True
        )

    # subscribe as a consumer to the queue and call the __onRequest function on callback
    def __setupConsumer(self):
        self.channel.basic_consume(queue=self.queue_name, on_message_callback=self.__onRequest, auto_ack=False)

    # start consuming from the queue. This function is blocking!
    def run(self):
        self.channel.start_consuming()

    def __del__(self):
        if self.is_connected:
            self.connection.close()
