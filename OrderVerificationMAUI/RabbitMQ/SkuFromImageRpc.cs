using System.Collections.Concurrent;
using System.Text.Json;
using OpenCvSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ;

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
			_handleMessage(model, ea);
		};

		// add consumer
		_channel.BasicConsume(
			consumer: _consumer,
			queue: _reply_queue_name,
			autoAck: true
		);
	}

	// Handler for the incoming messages. This checks the correlation id, converts the body to the output long.
	private void _handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
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

		//create task completion source
		var task_completion_source = new TaskCompletionSource<List<long>>(TaskCreationOptions.RunContinuationsAsynchronously);

		//add task completion source to callback mapper
		_callback_mapper.TryAdd(correlation_id, task_completion_source);
		
		//public request to exchange
		_channel.BasicPublish(
			exchange: _exchange_name,
			routingKey: _routing_key,
			basicProperties: properties,
			body: image.ToBytes()
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