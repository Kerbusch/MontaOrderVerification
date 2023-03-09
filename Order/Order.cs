namespace OrderLogic; 

public struct Order {
	// order struct containing the current sku's in the order, the expected sku's, a is checked flag and a is complete flag.
	public int[] skus { get; set; } //skus currently in the order
	public int[] expected_skus { get; set; } = Array.Empty<int>();
	public bool has_been_checked { get; set; } = false;
	public bool is_complete { get; set; } = false;
		
	public Order(int[] SKUS) {
		skus = SKUS;
	}

	public Order(int[] SKUS, int[] EXPECTED_SKUS) {
		skus = SKUS;
		expected_skus = EXPECTED_SKUS;
	}

	// public static Order operator ==(Order lhs, Order rhs) {}

	// public static Order operator !=(Order lhs, Order rhs) {}
}