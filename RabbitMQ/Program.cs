using OpenCvSharp;
using RpcClient;

class client {
	public async Task sendImage() {
		//DataSendImageToServer
		using DataSendImageToServer client = new DataSendImageToServer(
			hostname: "20.13.19.141",
			username: "python_test_user",
			password: "jedis"
		);

		Mat vectron_image2 = Cv2.ImRead("../../../vectron.png");
		
		client.sendToDataSetImageToServer(8719992763917 , "Oot", vectron_image2);
		Console.WriteLine("done sending");
	}

	public async Task skuTask() {
		//clientSkuFromImageRpcV2
		using SkuFromImageRpc clientSkuFromImageRpcV2 = new SkuFromImageRpc("20.13.19.141", "python_test_user", "jedis");
		
		Mat vectron_image = Cv2.ImRead("../../../vectron.png");
		Mat virm_image = Cv2.ImRead("../../../virm.jpg");
		Mat icng_image = Cv2.ImRead("../../../icng.jpg");

		var skus_task = clientSkuFromImageRpcV2.getSkusFromServerWithImageAsync(vectron_image);
		var skus2_task = clientSkuFromImageRpcV2.getSkusFromServerWithImageAsync(virm_image);

		List<long> skus = await skus_task;
		foreach (long sku in skus) {
			Console.WriteLine("sku vectron :{0}", sku);
		}

		List<long> skus2 = await skus2_task;
		foreach (long sku in skus2) {
			Console.WriteLine("sku vrim :{0}", sku);
		}
		
		Console.WriteLine("async 1 done");
		
		// Task.Delay(5000).GetAwaiter().GetResult();
		
		var skus3_task = clientSkuFromImageRpcV2.getSkusFromServerWithImageAsync(icng_image);
		
		List<long> skus3 = await skus3_task;
		foreach (long sku in skus3) {
			Console.WriteLine("sku icng :{0}", sku);
		}

		Console.WriteLine("async 2 done");
	}

	public async Task getIndex() {
		using DataGetSkuIndexRpc data_get_sku_index = new DataGetSkuIndexRpc("20.13.19.141", "python_test_user", "jedis");
		var x = await data_get_sku_index.getSkuIndex(8719992763917, "Oot");
		Console.WriteLine("output: {0}", x);
	}
}

class run {
	static async Task Main() {
		var xClient = new client();
		// await xClient.skuTask();
		await xClient.sendImage();
		// await xClient.getIndex();
	}
}