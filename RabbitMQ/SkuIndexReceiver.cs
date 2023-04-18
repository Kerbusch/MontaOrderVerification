﻿using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitmqTestRecive; 

public class SkuIndexReceiver {
	private const string _queue_name = "sku_number";
	
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private EventingBasicConsumer _consumer;
	
	private struct ReceiveStruct {
		public long sku { get; set; }
		public string vendor { get; set; }
	}

	public SkuIndexReceiver(string hostname, string username, string password) {
		var factory = new ConnectionFactory {
			HostName = hostname,
			UserName = username,
			Password = password
		};
		
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
			handleMessage(model, ea);
		};
		
		// add consumer
		_channel.BasicConsume(
			consumer: _consumer,
			queue: _queue_name,
			autoAck: false
		);
	}

	private void handleMessage(object? sender, BasicDeliverEventArgs basic_deliver_event_args) {
		int response = 0;

		//get the body
		var body = basic_deliver_event_args.Body.ToArray();
		
		//set the return correlation id to the received id in the properties
		var props = basic_deliver_event_args.BasicProperties;
		var replyProps = _channel.CreateBasicProperties();
		replyProps.CorrelationId = props.CorrelationId;

		try {
			//Deserialize body to ReceiveStruct
			ReceiveStruct received = JsonSerializer.Deserialize<ReceiveStruct>(body);

			Console.WriteLine("sku: {0}", received.sku);
			Console.WriteLine("vendor: {0}", received.vendor);

			//TODO: add magic

			response = 0; // = magic
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
}