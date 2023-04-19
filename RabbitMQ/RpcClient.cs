using System.Collections.Concurrent;
using System.Text.Json;
using Emgu.CV;
using Emgu.CV.Structure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RpcClient;

//Class for sending images to the AI server using RabbitMQ with a RPC service
public class SkuFromImageRpc: IDisposable {
	//RabbitMQ exchange name and routing key. If this changes the code also had to be changed.
	//In a future version this this should be in a configuration or definition file.
	private const string _exchange_name = "ai_server";
	private const string _routing_key = "sku_image";
	private const string _queue_name = "rpc_queue_sku_image";

	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _reply_queue_name;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<List<long>>> _callback_mapper = new();
	private EventingBasicConsumer _consumer;

	//Constructor that creates the connection to the RabbitMQ server and starts the consumer
	public SkuFromImageRpc(string hostname, string username, string password) {
		var factory = new ConnectionFactory {
			HostName = hostname,
			UserName = username,
			Password = password
		};

		//create connection and channel and starts the consumer
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

		//declare the queue
		_reply_queue_name = _channel.QueueDeclare().QueueName;
		
		//setup consumer
		_consumer = new EventingBasicConsumer(_channel);
		
		//setup callback
		_consumer.Received += (model, ea) => {
			handleMessage(model, ea);
		};

		// add consumer
		_channel.BasicConsume(
			consumer: _consumer,
			queue: _reply_queue_name,
			autoAck: true
		);
	}

	// Handler for the incoming messages. This checks the correlation id, converts the body to the output long.
	private void handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
		//check if correlation id is correct
		if (!_callback_mapper.TryRemove(basic_deliver_event_args.BasicProperties.CorrelationId, out var task_completion_source)) {
			return;
		}
			
		//get body
		var body = basic_deliver_event_args.Body.ToArray();

		//create var for json output
		List<long>? skus = new List<long>();
			
		//check if ContentType = application/json
		if (basic_deliver_event_args.BasicProperties.ContentType == "application/json") {
			//get json from body
			skus = JsonSerializer.Deserialize<List<long>>(body);
				
			//check if return was null
			if (skus == null) {
				Console.WriteLine("skus == null");
				return;
			}
		}

		task_completion_source.TrySetResult(skus);
	}

	// Function for getting sku's from in input image using a rabbitmq RPC service
	public Task<List<long>> getSkusFromServerWithImageAsync(Mat image, CancellationToken cancellation_token = default) {
		//create basis properties
		IBasicProperties properties = _channel.CreateBasicProperties();
		
		//set properties
		var correlation_id = Guid.NewGuid().ToString();
		properties.CorrelationId = correlation_id;
		properties.ReplyTo = _reply_queue_name;
		properties.ContentType = "image/jpg";

		//convert image to byte arrey
		var image_bgr = image.ToImage<Bgr, Byte>();
		byte[] image_data = image_bgr.ToJpegData();
		
		//create task completion source
		var task_completion_source = new TaskCompletionSource<List<long>>(TaskCreationOptions.RunContinuationsAsynchronously);

		//add task completion source to callback mapper
		_callback_mapper.TryAdd(correlation_id, task_completion_source);
		
		//public request to exchange
		_channel.BasicPublish(
			exchange: _exchange_name,
			routingKey: _routing_key,
			basicProperties: properties,
			body: image_data
		);

		cancellation_token.Register(() => _callback_mapper.TryRemove(correlation_id, out _));
		return task_completion_source.Task;
	}

	//When this class is deleted this function is called
	public void Dispose() {
		_connection.Close();
		// To make the close function not hang the TaskCompletionSource is created with:
		// TaskCreationOptions.RunContinuationsAsynchronously
		// This is a workaround but should be fine
	}
}

//Class for sending images to the dataset using a simple RabbitMQ queue
public class DataSendImageToServer {
	//RabbitMQ exchange name and routing key. If this changes the code also had to be changed.
	//In a future version this this should be in a configuration or definition file.
	private const string _exchange_name = "ai_server";
	private const string _routing_key = "dataset_image";
	private const string _queue_name = "dataset_images";

	private readonly IConnection _connection;
	private readonly IModel _channel;

	//Struct for json serializing
	private struct SendStruct {
		public long sku { get; set; }
		public string vendor { get; set; }
		public Mat image { get; set; }
	}

	//Constructor that creates the connection to the RabbitMQ server
	public DataSendImageToServer(string hostname, string username, string password) {
		
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

	// Function for sending the dataset images to the queue.
	public void sendToDataSetImageToServer(long sku, string vendor, Mat image) {
		//create basis properties
		IBasicProperties properties = _channel.CreateBasicProperties();
		
		//set properties
		properties.Persistent = true;
		
		//create json
		var json_struct = new SendStruct {
			sku = sku,
			vendor = vendor,
			image = image
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
}

//Class for getting the sku image index using RabbitMQ with a RPC service
public class DataGetSkuIndexRpc : IDisposable {
	//RabbitMQ exchange name and routing key. If this changes the code also had to be changed.
	//In a future version this this should be in a configuration or definition file.
	private const string _exchange_name = "ai_server";
	private const string _routing_key = "dataset_sku_number";
	private const string _queue_name = "sku_number";

	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _reply_queue_name;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<int>> _callback_mapper = new();
	private EventingBasicConsumer _consumer;
	
	//Struct for json serializing
	private struct SendStruct {
		public long sku { get; set; }
		public string vendor { get; set; }
	}
	
	//Constructor that creates the connection to the RabbitMQ server and starts the consumer
	public DataGetSkuIndexRpc(string hostname, string username, string password) {
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
		
		//declare the queue
		_reply_queue_name = _channel.QueueDeclare().QueueName;
		
		//setup consumer
		_consumer = new EventingBasicConsumer(_channel);
		
		//setup callback
		_consumer.Received += (model, ea) => {
			handleMessage(model, ea);
		};

		// add consumer
		_channel.BasicConsume(
			consumer: _consumer,
			queue: _reply_queue_name,
			autoAck: true
		);
	}
	
	// Handler for the incoming messages. This checks the correlation id, converts the body to the output int
	private void handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
		//check if correlation id is correct
		if (!_callback_mapper.TryRemove(basic_deliver_event_args.BasicProperties.CorrelationId, out var task_completion_source)) {
			return;
		}
			
		//get body
		var body = basic_deliver_event_args.Body.ToArray();
			
		//create var for json output
		int sku_index = 0;
			
		//check if ContentType = application/json
		if (basic_deliver_event_args.BasicProperties.ContentType == "application/json") {
			//get json from body
			sku_index = JsonSerializer.Deserialize<int>(body);
				
			//check if return was null
			if (sku_index == null) {
				Console.WriteLine("skus == null");
				return;
			}
		}

		task_completion_source.TrySetResult(sku_index);
	}

	// Function for getting sku index using a rabbitmq RPC service
	public Task<int> getSkuIndex(long sku, string vendor, CancellationToken cancellation_token = default) {
		//create basis properties
		IBasicProperties properties = _channel.CreateBasicProperties();
		
		//set properties
		var correlation_id = Guid.NewGuid().ToString();
		properties.CorrelationId = correlation_id;
		properties.ReplyTo = _reply_queue_name;
		properties.ContentType = "application/json";
		
		//create json
		var json_struct = new SendStruct {
			sku = sku,
			vendor = vendor
		};
		var json_string_bytes = JsonSerializer.SerializeToUtf8Bytes(json_struct);
		
		//create task completion source
		var task_completion_source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

		//add task completion source to callback mapper
		_callback_mapper.TryAdd(correlation_id, task_completion_source);

		//public request to exchange
		_channel.BasicPublish(
			exchange: _exchange_name,
			routingKey: _routing_key,
			basicProperties: properties,
			body: json_string_bytes
		);
		
		cancellation_token.Register(() => _callback_mapper.TryRemove(correlation_id, out _));
		return task_completion_source.Task;
	}
	
	//When this class is deleted this function is called
	public void Dispose() {
		_connection.Close();
		// To make the close function not hang the TaskCompletionSource is created with:
		// TaskCreationOptions.RunContinuationsAsynchronously
		// This is a workaround but should be fine
	}
}