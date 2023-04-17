using RabbitMQRpcClient;
using Emgu.CV;

class Run {
	private 
	static async Task Main() {
		using RpcClient client_v2 = new RpcClient("localhost", "python_test_user", "jedis");
		
		Mat vectron_image  = CvInvoke.Imread("../../../vectron.png");
		Mat sea_image  = CvInvoke.Imread("../../../sea.jpg");

		var skus_task = client_v2.getSkusFromServerWithImageAsync(vectron_image);
		var skus2_task = client_v2.getSkusFromServerWithImageAsync(sea_image);

		List<long> skus = await skus_task;
		foreach (long sku in skus) {
			Console.WriteLine("sku vectron :{0}", sku);
		}

		
		List<long> skus2 = await skus2_task;
		foreach (long sku in skus) {
			Console.WriteLine("sku sea :{0}", sku);
		}
		
		Console.WriteLine("async done");
	}
}


