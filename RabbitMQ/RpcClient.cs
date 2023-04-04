using System.Collections.Concurrent;
using System.Text.Json;
using Emgu.CV;
using Emgu.CV.Structure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQRpcClient {
	
}

public class RpcClient: IDisposable {
	//private
	private const string _queue_name = "rpc_queue";

	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _reply_queue_name;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<List<long>>> _callback_mapper = new();
	private EventingBasicConsumer _consumer;

	//public
	
	//constructor
	public RpcClient(string hostname, string username, string password) {
		var factory = new ConnectionFactory {
			HostName = hostname,
			UserName = username,
			Password = password
		};

		//create connection and channel
		_connection = factory.CreateConnection(); //todo: add try expect
		_channel = _connection.CreateModel();
		
		//declare the queue
		_reply_queue_name = _channel.QueueDeclare().QueueName;
		
		//setup consumer
		_consumer = new EventingBasicConsumer(_channel);
		
		//setup callback
		_consumer.Received += (model, ea) => {
			//check if correlation id is correct
			if (!_callback_mapper.TryRemove(ea.BasicProperties.CorrelationId, out var task_completion_source)) {
				return;
			}
			
			//get body
			var body = ea.Body.ToArray();
			
			//get json from body
			List<long>? skus = new List<long>();
			
			if (ea.BasicProperties.ContentType == "application/json") {
				skus = JsonSerializer.Deserialize<List<long>>(body);
				if (skus == null) {
					Console.WriteLine("skus == null");
					return;
				}
				
			}

			task_completion_source.TrySetResult(skus);
		};

		_channel.BasicConsume(
			consumer: _consumer,
			queue: _reply_queue_name,
			autoAck: true
		);
	}

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
		var task_completion_source = new TaskCompletionSource<List<long>>();
		var tcs = TaskCreationOptions.RunContinuationsAsynchronously;
		
		//add tcs to callback mapper
		_callback_mapper.TryAdd(correlation_id, task_completion_source);
		
		//public request to queue
		_channel.BasicPublish(
			exchange: string.Empty,
			routingKey: _queue_name,
			basicProperties: properties,
			body: image_data
		);

		cancellation_token.Register(() => _callback_mapper.TryRemove(correlation_id, out _));
		return task_completion_source.Task;
	}

	public void Dispose() {
		_connection.Close(); //this function hangs the application
	}
}