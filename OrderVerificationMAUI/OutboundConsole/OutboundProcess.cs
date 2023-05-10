namespace OrderLogic; 

using OpenCvSharp;
using RabbitMQ;

//Whole outbound process that takes a image of the order and runs it through the ai to check if the order is complete
public class OutboundProcess : IDisposable {
	private SkuFromImageRpc _rabbitmq_sku_from_image; //rabbitmq receiver
	private VideoCapture _capture; //opencvsharp4 video capture for taking pictures
	private OrderData _order_data = new OrderData("../../../file.json"); //order data member for storing order data

	//Constructor that takes the hostname, username and password for the rabbitmq receiver.
	public OutboundProcess(string hostname, string username, string password) {
		//start rabbitmq
		_rabbitmq_sku_from_image = new SkuFromImageRpc(hostname, username, password);
		
		//start the video capture so that it's ready to take pictures
		_capture = new VideoCapture();
		_capture.Open(0); // Use 0 for the default camera or specify a different index for multiple cameras
	}

	//Get the order id from the console using Console.Readline.
	private uint getOrderIdFromConsole() {
		
		Console.WriteLine("Place the order underneath the camera and enter the order id to check the order completeness");
		Console.WriteLine("This takes a picture that will be checked by the AI");
		Console.WriteLine("Enter order id: ");

		//Get in
		uint order_id = Convert.ToUInt32(Console.ReadLine());
		
		//TODO: check order_id
		
		return order_id;
	}
	
	//Get image from opencvsharp4 using the _capture member.
	public Mat takeImage() {
		//Check if _capture is open
		if (!_capture.IsOpened())
		{
			Console.WriteLine("Failed to open camera!");
			return new Mat();
		}
		
		//Get image: Mat from the capture
		Mat image = new Mat();
		_capture.Read(image);
		
		//return the image
		return image;
	}

	//The whole process from getting the order id to checking the completeness.
	public async void runProcessOnce() {
		//get order id from console
		var order_id = getOrderIdFromConsole();
		
		//check if order is in database
		if (!_order_data.tryGetOrder(order_id, out Order order)) {
			//order doesnt exist
			Console.WriteLine("Order doesn't exists.");
		}

		//take picture
		var image = takeImage();
		
		//get skus from image
		var skus_task = _rabbitmq_sku_from_image.getSkusFromServerWithImageAsync(image);
		
		//print waiting on ai server
		Console.WriteLine("Waiting on server.....");
		
		//add skus to order
		var skus = skus_task.Result;
		Console.WriteLine("test");
		// _order_data.setSkusInOrder(order_id, skus.ToArray());
		order.skus = skus.ToArray();
		
		//check order completeness
		var check_result = order.checkComplete();
		
		//display order status
		Console.WriteLine(order.getStatus(check_result));
		Console.WriteLine("\n\n");
		
		//save order data to file after each run
		_order_data.saveToFile();
	}

	//Start a loop using the runProcessOnce function. It can accept a CancellationToken to cancel the loop.
	public void startLoop(CancellationToken token = default) {
		while (!token.IsCancellationRequested) {
			runProcessOnce();
		}
		Console.WriteLine("ended startLoop");
	}

	//When this class is deleted this function is called
	public void Dispose() {
		//close camera
		_capture.Release();
		_capture.Dispose();
		
		//call dispose on _order_data
		_order_data.Dispose();
	}
}