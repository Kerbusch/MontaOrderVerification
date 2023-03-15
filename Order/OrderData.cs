using System.Text.Json;

namespace OrderLogic; 

//Class for storing the order list.
public class OrderData
{
    private uint _id_counter = 0; //counter for making new orders
    private List<Order> _orders; //list of orders
    private string _file_path; //path to the json file for saving
    
    //load from file as constructor
    public OrderData(string file_path) {
        _file_path = file_path;
        loadFromFile(file_path);
        setIdCounter();
    }

    //import the order list from a file
    public void loadFromFile(string file_path) {
        //get string from file

        string json_string = "";
        try {
            json_string = File.ReadAllText(file_path);
        }
        catch {
            Console.WriteLine("ReadAllText could not read file");
        }

        //convert string to data using json serialize
        try {
            _orders = JsonSerializer.Deserialize<List<Order>>(json_string);
        }
        catch {
            Console.WriteLine("JsonSerializer could not deserialize the json string");
            _orders = new List<Order>();
        }

        //todo: set _id_counter;
    }

    //save the local order list to a file
    public void saveToFile(string file_path) {
        var json_serializer_options = new JsonSerializerOptions();
        json_serializer_options.WriteIndented = true;
        //create json string
        string json_string = JsonSerializer.Serialize(_orders, json_serializer_options);

        //write file
        File.WriteAllText(file_path, json_string);
    }

    //set te if counter to the max value from the input file
    private void setIdCounter() {
        foreach (Order order in _orders) {
            if (order.id > _id_counter) {
                _id_counter = order.id;
            }
        }

        _id_counter++;
    }

    //get a reference of the local orders list
    public ref List<Order> getOrders() {
        return ref _orders;
    }
    
    //add order to the local list, this runs the order constructor with the _id_counter
    public void addOrder(long[] skus_, long[] expected_skus_) {
        _orders.Add(new Order(_id_counter++, skus_, expected_skus_));
    }

    //remove the order from the local list
    public bool removeOrder(Order order) {
     return _orders.Remove(order);
    }

    //find a order with the given id, the first item (bool) is to check if it's found
    public (bool, Order) findOrder(int id) {
     //go trough all orders and return true + the order if its found
     foreach (Order order in _orders) {
         if (order.id == id) {
             return (true, order);
         }
     }
     
     //if there is nog order found it returns a false and a new empty order
     return (false, new Order());
    }

    //replace order used for editing a order
    public void replaceOrder(Order order_to_replace, Order replacement_order) {
     int index = _orders.FindIndex(s => s == order_to_replace);
     if (index != -1) {
         _orders[index] = replacement_order;
     }
    }

    //replace a order based on id of the order its replacing
    public void replaceOrder(int id, Order replacement_order) {
     int index = _orders.FindIndex(s => s.id == id);
     if (index != -1) {
         _orders[index] = replacement_order;
     }
    }
}