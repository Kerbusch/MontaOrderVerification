using System.Text.Json;
using OpenCvSharp;
using RabbitMQ.Client;

namespace RabbitMQ; 

//Class for sending images to the dataset using a simple RabbitMQ queue
public class DataSetImageSender : IDisposable {
	//RabbitMQ exchange name and routing key. If this changes the code also had to be changed.
	//In a future version this this should be in a configuration or definition file.
	private const string _exchange_name = "ai_server";
	private const string _routing_key = "dataset_image";
	private const string _queue_name = "dataset_images";

	private readonly IConnection _connection;
	private readonly IModel _channel;

	//Struct for json serializing
	private struct _SendStruct {
		public long sku { get; set; }
		public string vendor { get; set; }
		public Byte[] image_data { get; set; }
	}

	//Constructor that creates the connection to the RabbitMQ server
	public DataSetImageSender(string hostname, string username, string password) {
		
		var factory = new ConnectionFactory {
			HostName = hostname,
			UserName = username,
			Password = password
		};
		
		//create connection and channel
		_connection = factory.CreateConnection(); //todo: add try expect
		_channel = _connection.CreateModel();

		//declare exchange, queue and bind
		_channel.QueueDeclare(
			queue: _queue_name,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);
		
		_channel.QueueBind(
			queue: _queue_name,
			exchange: _exchange_name,
			routingKey: _routing_key
		);
		
		_channel.ExchangeDeclare(
			exchange:_exchange_name,
			type: ExchangeType.Direct,
			durable: true,
			autoDelete: false,
			arguments: null
		);
	}
	
	//Constructor that creates the connection to the RabbitMQ server using the project settings
	public DataSetImageSender() : 
		this(
			Settings.rabbitmq_hostname,
			Settings.rabbitmq_username, 
			Settings.rabbitmq_password
		) 
	{ }

	// Function for sending the dataset images to the queue.
	public void sendToDataSetImageToServer(long sku, string vendor, Mat image) {
		//create basis properties
		IBasicProperties properties = _channel.CreateBasicProperties();
		
		//set properties
		properties.Persistent = true;

		//create json
		var json_struct = new _SendStruct {
			sku = sku,
			vendor = vendor,
			image_data = image.ToBytes()
		};
		var json_string_bytes = JsonSerializer.SerializeToUtf8Bytes(json_struct);

		//publish request to queue
		_channel.BasicPublish(
			exchange: _exchange_name,
			routingKey: _routing_key,
			basicProperties: properties,
			body: json_string_bytes
		);
	}

	//When this class is deleted this function is called
	public void Dispose() {
		_connection.Close();
	}
}