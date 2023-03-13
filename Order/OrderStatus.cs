namespace OrderLogic; 

public class OrderStatus {
	//check if the given order is complete based on the skus and the expected skus of the order
	public static Order checkComplete(Order order) {
		//walk trough the expected_skus array and remove sku if it is present.
		long[] expected_skus = order.expected_skus;
		List<long> excess_skus = new List<long>();
		
		foreach (var sku in order.skus) {
			if (expected_skus.Contains(sku) ) {
				expected_skus = expected_skus.Where(val => val != sku).ToArray(); //remove sku from expected_skus
			}
			else {
				excess_skus.Add(sku);
			}
			
		}

		// the local variable: expected_skus now contains the missing sku's this can later be used to generate feedback
		// the local variable: excess_skus now contains the excess sku's this can later be used to generate feedback
			
		//check if the expected_skus array length is zero. if so the order is complete
		if (expected_skus.Length == 0 && excess_skus.Count == 0) {
			order.is_complete = true;
			order.has_been_checked = true;
			return order;
		}

		//else return false and the order is not complete
		order.is_complete = false;
		order.has_been_checked = true;
		return order;
	}

	//run checkComplete() on all orders in the parameter list
	public static Order[] checkComplete(Order[] orders) {
		for (int i = 0; i < orders.Length; i++) {
			orders[i] = checkComplete(orders[i]);
		}

		return orders;
	}

	//return a string feedback based on the bool in a order.
	public static string getOrderStatus(Order order) {
		//generate a string that describes the order status.
		if (order.expected_skus == null || order.expected_skus.Length == 0) {
			return "The order has no expected sku's. This could be an error.";
		}
		
		if (order.has_been_checked) {
			if (order.is_complete) {
				return "The order is complete!";
			}
			//else
			return "The order is incomplete";
		}
		//else
		return "The order has not been checked. Run checkComplete first!";
	}

	//print the order feedback in the console. Uses: getOrderStatus();
	public static void printOrderInConsole(Order order) {
		//get the string from getOrderStatus and print it to the console
		Console.WriteLine(getOrderStatus(order));
	}
}