using System.Diagnostics;
using System.Runtime.CompilerServices;
using OrderLogic;

namespace OrderLogger;

public class CustomExceptionTest : Exception {
    public CustomExceptionTest() {}

    public CustomExceptionTest(string message) : base(message) {}

    public CustomExceptionTest(string message, Exception innerException) : base(message, innerException) {} 
}

public class Logger {
//private:
    private string _category = "Logger";
    private string _start_text_file = "The logger started at: " + DateTime.Now + ":\n";
    private string _path;

    private void handleErrorString(string message) {
        if (write_debug_console) {
            Debug.WriteLine(message, _category);
        }
        if (write_file) {
            writeToFile(message);
        }
    }

    private void writeToFile(string message) {
        message += "\n";
        File.AppendAllText(_path, message);
    }
    
    
//public:
    public bool write_file = true;
    public bool write_debug_console = true;

    public Logger(string path) {
        _path = path;
        writeToFile(_start_text_file);
    }

    public void logError(string message) {
        handleErrorString(message);
    }

    public void logError(Exception exception) {
        handleErrorString(exception.ToString());
    }

    public void logNewOrder(Order order) {
        //base string + created_on
        string message = "A new order has been created on " + order.created_on + ": \n";
        
        //id
        message += "\tID = " + order.id + ",\n";
        
        //skus
        message += "\tSKUS = [" + string.Join(", ", order.skus) + "],\n";
        
        //expected_skus
        message += "\tEXPECTED_SKUS = [" + string.Join(", ", order.expected_skus) + "]";

        //handle message
        handleErrorString(message);
    }

    public void logSkus(long[] skus) {
        //base string
        string message = "The order recognition ai has recognised these items:\n";

        //skus
        var last = skus.Last();
        foreach (long sku in skus) {
            if (sku == last) {
                message += "\t- " + sku;
            }
            else {
                message += "\t- " + sku + "\n";
            }
        }

        //handle message
        handleErrorString(message);
    }
}