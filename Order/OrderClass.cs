namespace OrderLogic; 

public class Order {
	// order class (using class because struct are pass by value and not reference) containing the current sku's in the order, the expected sku's
	public uint id { get; set; }
	public long[] skus { get; set; } //skus currently in the order
	public long[] expected_skus { get; init; }
	public bool has_been_checked { get; set; }
	public bool is_complete { get; set; }
	public DateTime created_on { get; init; }

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