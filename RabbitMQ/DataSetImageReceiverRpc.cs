using Emgu.CV;
using Emgu.CV.CvEnum;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitmqTestRecive; 

public class DataSetImageReceiverRpc {
	private const string _queue_name = "dataset_images";

	private ConnectionFactory _connectionFactory;
	private readonly IConnection _connection;
	private readonly IModel _channel;
    
	private EventingBasicConsumer _consumer;

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

		_consumer.Received += (sender, ea) =>
		{
			handleMessage(sender, ea);
		};
        
		_channel.BasicConsume(
			queue: _queue_name,
			autoAck: false,
			consumer: _consumer);
	}

	private void handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
		var body = basic_deliver_event_args.Body.ToArray();

		var imageMat = new Mat();
        
		CvInvoke.Imdecode(body, ImreadModes.AnyColor, imageMat);

		CvInvoke.Imwrite("../../../gotten_image.png", imageMat);
        
		Console.WriteLine("writen image to gotten_image.png");
        
		_channel.BasicAck(deliveryTag: basic_deliver_event_args.DeliveryTag, multiple: false);
	}
}