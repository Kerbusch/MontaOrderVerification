namespace OrderLogic; 

public class Order {
	// order class (using class because struct are pass by value and not reference) containing the current sku's in the order, the expected sku's
	public uint id { get; set; }
	public long[] skus { get; set; } //skus currently in the order
	public long[] expected_skus { get; init; }
	public bool has_been_checked { get; set; }
	public bool is_complete { get; set; }
	public DateTime created_on { get; init; }
	
	public struct OrderFeedback {
		public long[] missing_skus { get; set; }
		public long[] excess_skus { get; set; }
	}

	public Order() {
		id = 0;
		skus = new long[] { };
		expected_skus = new long[] { };
		has_been_checked = false;
		is_complete = false;
		created_on = DateTime.Now;
	}

	//constructor that takes: id, skus, expected_skus and creates a order with this
	public Order(uint id, long[] skus, long[] expected_skus) {
		this.id = id;
		this.skus = skus;
		this.expected_skus = expected_skus;
		this.has_been_checked = false;
		this.is_complete = false;
		this.created_on = DateTime.Now;
	}

	//Check if the order is complete based on the skus and the expected skus
	public OrderFeedback? checkComplete() {
		//walk trough the expected_skus array and remove sku if it is present.
		long[] expected_skus = this.expected_skus;
		List<long> excess_skus = new List<long>();
		
		foreach (var sku in skus) {
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
			is_complete = true;
			has_been_checked = true;
			return null;
		}

		//else return false and the order is not complete
		is_complete = false;
		has_been_checked = true;
		return new Order.OrderFeedback {
			missing_skus = expected_skus,
			excess_skus = excess_skus.ToArray(),
		};
	}

	public string getStatus(Order.OrderFeedback? order_feedback = null) {
		//generate a string that describes the order status.
		if (expected_skus == null || expected_skus.Length == 0) {
			return "Order: " + id + ", has no expected sku's. This could be an error.";
		}
		
		if (has_been_checked) {
			if (is_complete) {
				return "Order: " + id + ", is complete!";
			}
			//else
			
			//generate order incomplete string
			String line = "Order: " + id + ", is incomplete.";

			if (order_feedback.HasValue) {
				if (order_feedback.Value.missing_skus.Length != 0) {
					line += " the missing skus are: " + string.Join(", ", order_feedback.Value.missing_skus) + ".";
				}
				
				if (order_feedback.Value.excess_skus.Length != 0) {
					line += " the excess skus are: " + string.Join(", ", order_feedback.Value.excess_skus) + ".";
				}
			}

			return line;
		}
		//else
		
		//order has not been check so checking:
		var feedback = checkComplete();
		return getStatus(feedback);
	}
	
	public bool Equals ( Order obj )
	{
		return this.id == obj.id &&
		       this.skus.SequenceEqual(obj.skus) &&
		       this.expected_skus.SequenceEqual(obj.expected_skus) &&
		       this.has_been_checked == obj.has_been_checked &&
		       this.is_complete == obj.is_complete &&
		       this.created_on == obj.created_on;
	}

	public static bool operator ==(Order lhs, Order rhs) =>
		lhs.Equals(rhs);

	public static bool operator !=(Order lhs, Order rhs) =>
		!lhs.Equals(rhs);

	public override bool Equals(object? obj) {
		// return base.Equals(obj);
		return Equals(obj as Order);
	}
}