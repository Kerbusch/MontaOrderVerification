using System.Diagnostics;
using OrderLogic;

namespace OrderLogger;

public class Logger {
//private:
    private string _category = "Logger";
    private string _start_text_file = "The logger started at: " + DateTime.Now + ":";
    private string _path;

    private async Task handleErrorStringAsync(string message) {
        if (write_debug_console) {
            Debug.WriteLine(message, _category);
        }
        if (write_file) {
            writeToFileAsync(message);
        }
    }

    private async Task writeToFileAsync(string message) {
        message += "\n";
        await File.AppendAllTextAsync(_path, message);
    }
    
    
//public:
    public bool write_file = true;
    public bool write_debug_console = true;

    public Logger(string path) {
        _path = path;
        
        //write default string with date to log file
        writeToFileAsync(_start_text_file);
    }

    public void logError(string message) {
        // _ = handleErrorStringAsync(message);
        _ = Task.Run(() => handleErrorStringAsync(message));
    }

    public void logError(Exception exception) {
        _ = handleErrorStringAsync(exception.ToString());
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
        _ = handleErrorStringAsync(message);
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
        _ = handleErrorStringAsync(message);
    }
}