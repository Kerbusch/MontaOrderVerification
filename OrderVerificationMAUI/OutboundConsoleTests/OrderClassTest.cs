namespace OutboundConsoleTests;

public class OrderClassTest {
    //check if order is equals without created_on
    private bool checkEqualsWithoutCreatedOn(Order order0, Order order1) {
        return order0.id == order1.id &&
               order0.skus.SequenceEqual(order1.skus) &&
               order0.expected_skus.SequenceEqual(order1.expected_skus) &&
               order0.has_been_checked == order1.has_been_checked &&
               order0.is_complete == order1.is_complete;
    }
    
    [SetUp]
    public void Setup() {
    }

    [Test] 
    public void equalsOperatorTest() {
        var date_time = DateTime.Now;
        
        var order_base = new Order {
            id = 0,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 2},
            has_been_checked = false,
            is_complete = false,
            created_on = date_time
        };
        
        var order_same = new Order {
            id = 0,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 2},
            has_been_checked = false,
            is_complete = false,
            created_on = date_time
        };
        
        var order_diff_id = new Order {
            id = 2,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 2},
            has_been_checked = false,
            is_complete = false,
            created_on = date_time
        };
        
        var order_diff_skus = new Order {
            id = 2,
            skus = new long[] {5, 2, 6},
            expected_skus = new long[] {1, 2},
            has_been_checked = false,
            is_complete = false,
            created_on = date_time
        };
        
        var order_diff_expected_skus = new Order {
            id = 2,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 3, 2},
            has_been_checked = false,
            is_complete = false,
            created_on = date_time
        };
        
        var order_diff_has_been_checked = new Order {
            id = 2,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 2},
            has_been_checked = true,
            is_complete = false,
            created_on = date_time
        };
        
        var order_diff_is_complete = new Order {
            id = 2,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 2},
            has_been_checked = false,
            is_complete = true,
            created_on = date_time
        };
        
        var order_diff_created_on = new Order {
            id = 2,
            skus = new long[] {1, 2},
            expected_skus = new long[] {1, 2},
            has_been_checked = false,
            is_complete = false,
            created_on = DateTime.Now
        };
        
        Assert.Multiple((() =>
        {
            Assert.That(order_base, Is.EqualTo(order_same));
            Assert.That(order_base == order_same, Is.True);
            Assert.That(order_base, Is.Not.EqualTo(order_diff_id));
            Assert.That(order_base, Is.Not.EqualTo(order_diff_skus));
            Assert.That(order_base, Is.Not.EqualTo(order_diff_expected_skus));
            Assert.That(order_base, Is.Not.EqualTo(order_diff_has_been_checked));
            Assert.That(order_base, Is.Not.EqualTo(order_diff_is_complete));
            Assert.That(order_base, Is.Not.EqualTo(order_diff_created_on));
        }));
    }

    [Test]
    public void constructorTest() {
        Order order0 = new Order(0, new long[] {1, 2}, new long[] {3, 4});
        Order order1 = new Order(0, new long[] {1, 2}, new long[] {3, 4});
        
        Assert.Multiple((() =>
        {
            Assert.That(order0, Is.Not.EqualTo(order1));
            Assert.That(checkEqualsWithoutCreatedOn(order0, order1), Is.True);
        }));
    }
    
    [Test]
    public void checkCompleteTest() {
        //order 0
        Order order_0 = new Order(0, new long[] {1, 2}, new long[] {3, 4});

        var expected_result_0 = new Order.OrderFeedback {
            missing_skus = new long[] { 3, 4 },
            excess_skus = new long[] { 1, 2 }
        };
        
        var result_0 = order_0.checkComplete();
        
        //order 1
        Order order_1 = new Order(0, new long[] {2, 3}, new long[] {3, 4});
        
        var expected_result_1 = new Order.OrderFeedback {
            missing_skus = new long[] { 4 },
            excess_skus = new long[] { 2 }
        };
        
        var result_1 = order_1.checkComplete();
        
        //order 2
        Order order_2 = new Order(0, new long[] {}, new long[] {3, 4});
        
        var expected_result_2 = new Order.OrderFeedback {
            missing_skus = new long[] { 3, 4 },
            excess_skus = new long[] {}
        };

        var result_2 = order_2.checkComplete();
        
        //order 3
        Order order_3 = new Order(0, new long[] {3, 4}, new long[] {3, 4});

        var result_3 = order_3.checkComplete();

        Assert.Multiple( (() =>
        {
            //order 0
            Assert.That(result_0, Is.Not.Null);
            Assert.That(result_0?.missing_skus, Is.EqualTo(expected_result_0.missing_skus));
            Assert.That(result_0?.excess_skus, Is.EqualTo(expected_result_0.excess_skus));
            
            //order 1
            Assert.That(result_1, Is.Not.Null);
            Assert.That(result_1?.missing_skus, Is.EqualTo(expected_result_1.missing_skus));
            Assert.That(result_1?.excess_skus, Is.EqualTo(expected_result_1.excess_skus));
            
            //order 2
            Assert.That(result_2, Is.Not.Null);
            Assert.That(result_2?.missing_skus, Is.EqualTo(expected_result_2.missing_skus));
            Assert.That(result_2?.excess_skus, Is.EqualTo(expected_result_2.excess_skus));
            
            //order 3
            Assert.That(result_3, Is.Null);
        }));
    }
    
    [Test]
    public void getStatusTest() {
        //order 0
        Order order_0 = new Order(0, new long[] {1, 2}, new long[] {3, 4});

        var feedback_0 = order_0.checkComplete();

        var result_0 = order_0.getStatus(feedback_0);
        
        //order 1
        Order order_1 = new Order(0, new long[] {2, 3}, new long[] {3, 4});

        var feedback_1 = order_1.checkComplete();

        var result_1 = order_1.getStatus(feedback_1);
        
        //order 2
        Order order_2 = new Order(0, new long[] {}, new long[] {3, 4});
        
        var feedback_2 = order_2.checkComplete();

        var result_2 = order_2.getStatus(feedback_2);
        
        //order 3
        Order order_3 = new Order(0, new long[] {3, 4}, new long[] {3, 4});
        
        var feedback_3 = order_3.checkComplete();

        var result_3 = order_3.getStatus(feedback_3);
        
        //order 4
        Order order_4 = new Order(0, new long[] {3, 4}, new long[] {});
        
        var feedback_4 = order_4.checkComplete();

        var result_4 = order_4.getStatus(feedback_4);
        
        Assert.Multiple((() =>
        {
            //order 0
            Assert.That(result_0, Is.EqualTo("Order: 0, is incomplete. the missing skus are: 3, 4. the excess skus are: 1, 2."));
            
            //order 1
            Assert.That(result_1, Is.EqualTo("Order: 0, is incomplete. the missing skus are: 4. the excess skus are: 2."));
            
            //order 2
            Assert.That(result_2, Is.EqualTo("Order: 0, is incomplete. the missing skus are: 3, 4."));
            
            //order 3
            Assert.That(result_3, Is.EqualTo("Order: 0, is complete!"));
            
            //order 4
            Assert.That(result_4, Is.EqualTo("Order: 0, has no expected sku's. This could be an error."));
        }));
    }
}