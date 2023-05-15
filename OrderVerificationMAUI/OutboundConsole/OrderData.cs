using System.Runtime.InteropServices;
using System.Text.Json;

namespace OrderLogic; 

//Class for storing the order list.
public class OrderData: IDisposable {
    private uint _id_counter = 0; //counter for making new orders
    private List<Order> _orders = new List<Order>(); //list of orders
    private string _file_path; //path to the json file for saving

    //load from file as constructor
    public OrderData(string file_path) {
        _file_path = file_path;
        loadFromFile(file_path);
        setIdCounter();
    }

    //import the order list from a file
    public void loadFromFile(string file_path = "") {
        //get string from file
        if (file_path == "") {
            file_path = _file_path;
        }

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
        catch (ArgumentNullException) {
            Console.WriteLine("JsonSerializer.Deserialize returned ArgumentNullException");
            _orders = new List<Order>();
        }
        catch (JsonException) {
            Console.WriteLine("JsonSerializer.Deserialize returned JsonException");
            _orders = new List<Order>();
        }
        catch (NotSupportedException) {
            Console.WriteLine("JsonSerializer.Deserialize returned NotSupportedException");
            _orders = new List<Order>();
        }
    }

    //save the local order list to a file
    public void saveToFile(string file_path = "") {
        if (file_path == "") {
            file_path = _file_path;
        }
        
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
    public ref List<Order> getAllOrders() {
        return ref _orders;
    }

    //return order based on id. This can be used to find order
    public bool tryGetOrder(uint id, out Order result) {
        for (int i = 0; i < _orders.Count; i++) {
            if (_orders[i].id == id) {
                result = _orders[i];
                return true;
            }
        }

        result = new Order();
        return false;
    }

    //set the sku array in the order skus with the given id
    public void setSkusInOrder(uint id, long[] skus) {
        for (int i = 0; i < _orders.Count; i++) {
            if (_orders[i].id == id) {
                _orders[i].skus = skus;
                _orders[i].has_been_checked = false;
                _orders[i].is_complete = false;
            }
        }
    }
    
    //add order to the local list, this runs the order constructor with the _id_counter, inputs: skus and expected_skus
    public void addOrder(long[] skus_, long[] expected_skus_) {
        _orders.Add(new Order(_id_counter++, skus_, expected_skus_));
    }

    //add order to the local list, this runs the order constructor with the _id_counter, inputs: expected_skus
    public void addOrder(long[] expected_skus_) {
        _orders.Add(new Order(_id_counter++, new long[]{} , expected_skus_));
    }

    //remove the order from the local list
    public bool removeOrder(Order order) {
        return _orders.Remove(order);
    }

    public void Dispose() {
        saveToFile();
        Console.WriteLine("saved to file");
    }
}