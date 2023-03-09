namespace OrderLogic; 

public class OrderStatus {
	public bool checkComplete(ref Order order) {
		//walk trough the expected_skus array and remove sku if it is present.
		int[] expected_skus = order.expected_skus;
		List<int> excess_skus = new List<int>();
		
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
			return true;
		}

		//else return false and the order is not complete
		order.is_complete = false;
		order.has_been_checked = true;
		return false;
	}

	public string getOrderStatus(Order order) {
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

	public void printOrderInConsole(Order order) {
		//get the string from getOrderStatus and print it to the console
		Console.WriteLine(getOrderStatus(order));
	}
}