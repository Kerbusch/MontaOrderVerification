namespace OrderLogic; 

public class Order {
	// order class (using class because struct are pass by value and not reference) containing the current sku's in the order, the expected sku's
	public uint id { get; set; }
	public long[] skus { get; set; } //skus currently in the order
	public long[] expected_skus { get; init; }
	public bool has_been_checked { get; set; }
	public bool is_complete { get; set; }
	public DateTime created_on { get; init; }
	
	public struct RedundantAndMissingSkus {
		public List<long> redundant_skus { get; init; }
		public List<long> missing_skus { get; init; }
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
	public RedundantAndMissingSkus checkComplete() {
		List<long> redundant_skus = new List<long>();
		List<long> missing_skus = expected_skus.ToList();
		
		//loop through all skus
		foreach (long sku in skus) {
			if (expected_skus.Contains(sku)) {
				//remove sku from missing array
				missing_skus.Remove(sku);
			}
			else {
				//add skus to redundant array
				redundant_skus.Add(sku);
			}
		}
		
		//redundant_skus now contains a list of skus that are in the order but are not meant te be there.
		//missing_skus now contains a list of skus that are not in the order but are expected to be there.
		
		//check if the redundant_skus and missing_skus array length is zero. if so the order is complete
		if (redundant_skus.Count == 0 && missing_skus.Count == 0) {
			is_complete = true;
		}
		else {
			is_complete = false;
		}
		
		//set the has been checked flag
		has_been_checked = true;

		//return the redundant and missing skus
		return new RedundantAndMissingSkus {
			redundant_skus = redundant_skus,
			missing_skus = missing_skus
		};
	}

	public string getStatus(RedundantAndMissingSkus? redundant_and_missing_skus = null) {
		//generate a string that describes the order status.
		if (expected_skus == null || expected_skus.Length == 0) {
			return "The order has no expected sku's. This could be an error.";
		}
		
		if (has_been_checked) {
			if (is_complete) {
				return "The order is complete!";
			}
			//else
			//TODO: add order feedback using redundant_and_missing_skus member
			return "The order is incomplete";
		}
		//else
		return "The order has not been checked. Run checkComplete first!";
	}
	
	public static bool operator ==(Order lhs, Order rhs) => 
		lhs.id == rhs.id &&
		lhs.skus == rhs.skus &&
		lhs.expected_skus == rhs.expected_skus &&
		lhs.has_been_checked == rhs.has_been_checked &&
		lhs.is_complete == rhs.is_complete &&
		lhs.created_on == rhs.created_on;
	
	public static bool operator !=(Order lhs, Order rhs) =>
		!(lhs == rhs);
}