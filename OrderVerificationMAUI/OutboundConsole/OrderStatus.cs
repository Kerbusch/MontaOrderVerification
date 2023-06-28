namespace OrderLogic; 

public class OrderStatus {
	//check if the given order is complete based on the skus and the expected skus of the order
	public static Order.OrderFeedback? checkComplete(ref Order order) {
		return order.checkComplete();
	}

	//return a string feedback based on the bool in a order.
	public static string getOrderStatus(Order order, Order.OrderFeedback? order_feedback = null) {
		return order.getStatus(order_feedback);
	}

	//print the order feedback in the console. Uses: getOrderStatus();
	public static void printOrderInConsole(Order order) {
		//get the string from getOrderStatus and print it to the console
		Console.WriteLine(getOrderStatus(order));
	}
}