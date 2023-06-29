// See https://aka.ms/new-console-template for more information

using OrderLogic;

//Class for running the class above
class RunOutboundConsole {
	static async Task Main() {
		//Add order
		// using OrderData order_data = new OrderData("../../../file.json");
		// order_data.addOrder(new long[]{3,4,5,6});
		// order_data.addOrder(new long[]{1,1,1,2});
		// return;
		
		//Run the outbound process in a single loop
		using var outbound_process = new OutboundProcess(
			"20.13.19.141",
			"python_test_user",
			"jedis"
		);
		
		// outbound_process.runProcessOnce();
		outbound_process.startLoop();
	}
}