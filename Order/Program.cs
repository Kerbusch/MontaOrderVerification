//item 1 = 8719992763917
//item 2 = 8719992763139

using OrderLogic;


public class run {
    public static void Main() {
        Console.WriteLine("path:");
        Console.WriteLine(Path.GetFullPath("../../../file.json"));
        
        //setup ordeData
        // var order_data = new OrderData(@"C:\Users\daank\RiderProjects\ConsoleAppTest\JsonTest\file.json");
        var order_data = new OrderData("../../../file.json");

        //check the orders in order_data
        order_data.getOrders() = OrderStatus.checkComplete(order_data.getOrders().ToArray()).ToList();

        //save order to file
        // order_data.saveToFile(@"C:\Users\daank\RiderProjects\ConsoleAppTest\JsonTest\file.json");
        order_data.saveToFile("../../../file.json");
    }
}