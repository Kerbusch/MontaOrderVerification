using System.Diagnostics;
using OrderLogic;

namespace OrderLogger;

// Logger: logs message to the debug console and a file if enabled.
public class Logger {
//private:
    private string _category = "Logger";
    private string _start_text_file = "The logger started at: " + DateTime.Now + ":";
    private string _path; //file path for log file
    private bool _write_file; // enable / disable write to file
    private bool _write_debug_console; // enable / disable write to debug console

    //Handle the logger message. This function print to the debug console and / or the file of enabled.
    private async Task handleErrorStringAsync(string message) {
        if (_write_debug_console) {
            Debug.WriteLine(message, _category);
        }
        if (_write_file) {
            writeToFileAsync(message);
        }
    }

    //Write message to file
    private async Task writeToFileAsync(string message) {
        message += "\n";
        await File.AppendAllTextAsync(_path, message);
    }
    
    
//public:
    // default empty constructor. when calling this constructor the file writer is not enabled.
    public Logger() {
        _write_debug_console = true;
        _write_file = false;
    }

    // constructor with the path given. this enables the _write_file boolean.
    public Logger(string path) {
        _write_debug_console = true;
        _write_file = true;
        _path = path;
        
        //write default string with date to log file
        writeToFileAsync(_start_text_file);
    }

    // Log a string message
    public void logError(string message) {
        // _ = handleErrorStringAsync(message);
        _ = Task.Run(() => handleErrorStringAsync(message));
    }

    // Log an exception
    public void logError(Exception exception) {
        _ = handleErrorStringAsync(exception.ToString());
    }

    // Log a new order
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

    // Log the detected skus
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