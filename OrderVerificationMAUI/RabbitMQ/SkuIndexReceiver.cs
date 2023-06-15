using System.Diagnostics;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ; 

//Class for receiving the sku number and vendor from the queue and returning the index
public class SkuIndexReceiver : IDisposable {
	//RabbitMQ exchange name and routing key. If this changes the code also had to be changed.
	//In a future version this this should be in a configuration or definition file.
	private const string _queue_name = "sku_number";

	private readonly IConnection _connection;
	private readonly IModel _channel;
	private EventingBasicConsumer _consumer;

	private Func<long, string, int> _function;

	//Struct for json deserializing
	private struct _ReceiveStruct {
		public long sku { get; set; }
		public string vendor { get; set; }
	}

	//Constructor that creates the connection to the RabbitMQ server and starts the consumer
	public SkuIndexReceiver(string hostname, string username, string password,Func<long, string, int> function ) {
		var factory = new ConnectionFactory {
			HostName = hostname,
			UserName = username,
			Password = password
		};
		
		//save function to member
		_function = function;

		//create connection and channel
		_connection = factory.CreateConnection(); //todo: add try expect
		_channel = _connection.CreateModel();

		//declare queue
		_channel.QueueDeclare(
			queue: _queue_name,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);

		//set Qos
		_channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

		//setup consumer
		_consumer = new EventingBasicConsumer(_channel);

		//setup callback
		_consumer.Received += (model, ea) => {
			_handleMessage(model, ea);
		};

		//add consumer
		_channel.BasicConsume(
			consumer: _consumer,
			queue: _queue_name,
			autoAck: false
		);
	}

	// Handler for the incoming messages. After the processing it publishes a message to the return queue
	private void _handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
		int response = 0;

		//get the body
		var body = basic_deliver_event_args.Body.ToArray();

		//set the return correlation id to the received id in the properties
		var props = basic_deliver_event_args.BasicProperties;
		var replyProps = _channel.CreateBasicProperties();
		replyProps.CorrelationId = props.CorrelationId;
		replyProps.ContentType = "application/json";

		try {
			//Deserialize body to _ReceiveStruct
			_ReceiveStruct received = JsonSerializer.Deserialize<_ReceiveStruct>(body);

			Debug.WriteLine("Request for sku index from sku: {0}, with vendor: {1} started.", received.sku, received.vendor);
			
			response = _function(received.sku, received.vendor);
			
			Debug.WriteLine("Request for sku index from sku: {0}, with vendor: {1} sending response: {2}", received.sku, received.vendor, response);
		}
		catch (Exception exception) {
			Console.WriteLine($" [.] {exception.Message}");
			response = 0;
		}
		finally {
			//get bytes array from response
			var json_bytes = JsonSerializer.SerializeToUtf8Bytes(response);

			//Publish response to reply queue
			_channel.BasicPublish(exchange: string.Empty,
				routingKey: props.ReplyTo,
				basicProperties: replyProps,
				body: json_bytes);
			_channel.BasicAck(deliveryTag: basic_deliver_event_args.DeliveryTag, multiple: false);
		}
	}

	//When this class is deleted this function is called
	public void Dispose() {
		_connection.Close();
	}
}