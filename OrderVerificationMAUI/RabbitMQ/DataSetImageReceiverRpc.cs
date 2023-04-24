﻿using System.Text.Json;
using OpenCvSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ; 

//Class for receiving the dataset images from the queue
public class DataSetImageReceiverRpc : IDisposable {
	//RabbitMQ exchange name and routing key. If this changes the code also had to be changed.
	//In a future version this this should be in a configuration or definition file.
	private const string _queue_name = "dataset_images";

	private ConnectionFactory _connectionFactory;
	private readonly IConnection _connection;
	private readonly IModel _channel;
    
	private EventingBasicConsumer _consumer;
	
	//Struct for json deserializing
	private struct _ReceiveStruct {
		public long sku { get; set; }
		public string vendor { get; set; }
		public Byte[] image_data { get; set; }
	}

	//Constructor that creates the connection to the RabbitMQ server and starts the consumer
	public DataSetImageReceiverRpc(string hostname, string username, string password) {
		//setup connection factory
		_connectionFactory = new ConnectionFactory {
			HostName = hostname,
			UserName = username,
			Password = password
		};
		
		//create connection and channel
		_connection = _connectionFactory.CreateConnection(); //todo: add try expect
		_channel = _connection.CreateModel();

		//declare queue
		_channel.QueueDeclare(
			queue: _queue_name,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);
        
		//setup consumer;
		_consumer = new EventingBasicConsumer(_channel);
		_consumer.Received += (sender, ea) => {
			_handleMessage(sender, ea);
		};
		_channel.BasicConsume(
			queue: _queue_name,
			autoAck: false,
			consumer: _consumer);
	}

	// Handler for the incoming messages. This converts the bytes to a Mat variable, so that is can later be used.
	private void _handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
		//Get the body
		var body = basic_deliver_event_args.Body.ToArray();

		try {
			//Deserialize body to _ReceiveStruct
			_ReceiveStruct received = JsonSerializer.Deserialize<_ReceiveStruct>(body);

			Console.WriteLine("sku: {0}", received.sku);
			Console.WriteLine("vendor: {0}", received.vendor);

			//convert byte array to image
			Mat imageMat = Cv2.ImDecode(received.image_data, ImreadModes.AnyColor);

			//Temp write image file
			Cv2.ImWrite("../../../gotten_image.png", imageMat);
			Console.WriteLine("writen image to gotten_image.png");
		}
		catch (Exception exception) { 
			//Write exception
			Console.WriteLine($" [.] {exception.Message}");
		}
		finally {
			//Acknowledge delivery
			_channel.BasicAck(deliveryTag: basic_deliver_event_args.DeliveryTag, multiple: false);
		}
	}
	
	//When this class is deleted this function is called
	public void Dispose() {
		_connection.Close();
	}
}