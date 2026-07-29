// *Day 2 — Advanced LINQ & Deferred Execution — 8 hours


// *2.1 Deferred vs. Immediate Execution
// *Deferred Execution
/*List<int> numbers = new() { 1, 2, 3 };

var result = numbers.Where(x => x > 1);

numbers.Add(10);

foreach (var item in result)
{
    Console.WriteLine(item);
}// the 10 is included in the result because the query is executed when we iterate over it, not when we define it.*/
//*Immediate Execution
/*ToList() ToArray() Count()
List<int> numbers = new() { 1, 2, 3 };

var result = numbers
    .Where(x => x > 1)
    .ToList();

numbers.Add(10);

foreach (var item in result)
{
    Console.WriteLine(item);
}
// the 10 is not included in the result because the query is executed immediately when we call ToList(), and the result is stored in a list.
*/
//* 2.2 Grouping and Joining Data
//GroupBy
/*
class Program
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }

    }
    static void Main(string[] args)
    {
        List<Order> orders = new()
{
    new Order { Id = 1, CustomerId = 1, Amount = 100 },
    new Order { Id = 2, CustomerId = 1, Amount = 200 },
    new Order { Id = 3, CustomerId = 2, Amount = 150 },
    new Order { Id = 4, CustomerId = 2, Amount = 300 },
    new Order { Id = 5, CustomerId = 3, Amount = 50 }
};
        var groups = orders.GroupBy(o => o.CustomerId);
        foreach (var group in groups)
        {
            Console.WriteLine($"Customer ID: {group.Key}");

            foreach (var order in group)
            {
                Console.WriteLine($"   Order {order.Id} - Amount = {order.Amount}");
            }

            Console.WriteLine();
        }



    }
}*/
/*
class Program
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

    }
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }




// *JOIN

    static void Main(string[] args)
    {
        List<Customer> customers = new() //اسم العميل مع قيمة طلبه.
{
    new Customer { Id = 1, Name = "Mostafa" },
    new Customer { Id = 2, Name = "Ali" },
    new Customer { Id = 3, Name = "Ahmad" }
};

        List<Order> orders = new()
{
    new Order { Id = 1, CustomerId = 1, Amount = 100 },
    new Order { Id = 2, CustomerId = 1, Amount = 200 },
    new Order { Id = 3, CustomerId = 2, Amount = 150 },
    new Order { Id = 4, CustomerId = 3, Amount = 300 }
};
        var result = customers.Join
        (orders, c => c.Id, o => o.CustomerId,
        (c, o) => new
        { c.Name, o.Amount });



    }

}*/
// *2.3 Flattening with SelectMany
/*
class Program
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<Order> Orders { get; set; } = new();


    }
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
    static void Main(string[] args)
    {
       List<Customer> customers = new()
{
    new Customer
    {
        Id = 1,
        Name = "Mostafa",
        Orders = new List<Order>
        {
            new Order { Id = 1, CustomerId = 1, Amount = 100 },
            new Order { Id = 2, CustomerId = 1, Amount = 200 }
        }
    },

    new Customer
    {
        Id = 2,
        Name = "Ali",
        Orders = new List<Order>
        {
            new Order { Id = 3, CustomerId = 2, Amount = 150 }
        }
    },

    new Customer
    {
        Id = 3,
        Name = "Ahmad",
        Orders = new List<Order>
        {
            new Order { Id = 4, CustomerId = 3, Amount = 300 }
        }
    }
};

var result = customers.SelectMany(c => c.Orders);
foreach (var order in result)
{
    Console.WriteLine($"Order Id = {order.Id}, Amount = {order.Amount}");
}




    }

}*/
// *2.4 Common LINQ Performance Pitfalls
/* class Program
{
    static void Main(string[] args)
    {
    List<int> numbers = new()
            {
                1,2,3,4,5,6,7,8,9,10
            };

            var result = numbers
                .ToList()
                .Where(n => n > 8);

            foreach (var number in result)
            {
                Console.WriteLine(number);
            }
        }
        // !numbers.ToList().Where(...)❌
        //numbers.Where(...).ToList()✅
        //this code is inefficient because it first creates a list of all numbers and then filters them, which can be costly in terms of memory and performance. Instead, we should filter the numbers first and then convert the result to a list if needed.
        

        List<int> numbers = new()
{
    1,2,3,4,5,6,7,8,9,10
};

        var result = numbers.Where(n => n % 2 == 0);

        Console.WriteLine(result.Count()); // *this code is inefficient because it iterates over the collection twice: once to count the even numbers and once to print them. Instead, we can use a single iteration to achieve both tasks.*/
/*var result = numbers
    .Where(n => n % 2 == 0)
    .ToList();* this is true 
foreach (var n in result)
{
    Console.WriteLine(n);
}

// the right is to use a single iteration to achieve both tasks, like this:

}
}*/

// *Hands-On Lab: Grouping, Joining & Flattening
using System.Reflection;

class Program
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<Order> Orders { get; set; } = new();
    }

    static void Main(string[] args)
    {
        List<Customer> customers = new()
        {
            new Customer
            {
                Id = 1,
                Name = "Mostafa",
                Orders = new List<Order>
                {
                    new Order { Id = 1, CustomerId = 1, Amount = 100 },
                    new Order { Id = 2, CustomerId = 1, Amount = 200 }
                }
            },

            new Customer
            {
                Id = 2,
                Name = "Ali",
                Orders = new List<Order>
                {
                    new Order { Id = 3, CustomerId = 2, Amount = 150 }
                }
            },

            new Customer
            {
                Id = 3,
                Name = "Ahmad",
                Orders = new List<Order>
                {
                    new Order { Id = 4, CustomerId = 3, Amount = 300 }
                }
            },

            new Customer
            {
                Id = 4,
                Name = "Sara",
                Orders = new List<Order>
                {
                    new Order { Id = 5, CustomerId = 4, Amount = 250 }
                }
            },

            new Customer
            {
                Id = 5,
                Name = "Lina",
                Orders = new List<Order>
                {
                    new Order { Id = 6, CustomerId = 5, Amount = 175 }
                }
            },

            new Customer
            {
                Id = 6,
                Name = "Omar"
            }
        };

        List<Order> orders = customers.SelectMany(c => c.Orders).ToList();

        Console.WriteLine("===== GroupBy =====");

        var grouped = orders
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalAmount = g.Sum(o => o.Amount)
            });

        foreach (var item in grouped)
        {
            Console.WriteLine($"CustomerId: {item.CustomerId}, Total: {item.TotalAmount}");
        }

        Console.WriteLine("\n===== Join =====");

        var joined = customers.Join(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (c, o) => new
            {
                c.Name,
                o.Amount
            });

        foreach (var item in joined)
        {
            Console.WriteLine($"{item.Name} -> {item.Amount}");
        }

        Console.WriteLine("\n===== SelectMany =====");

        var flattened = customers.SelectMany(c => c.Orders);

        foreach (var order in flattened)
        {
            Console.WriteLine($"Order Id: {order.Id}, Amount: {order.Amount}");
        }
        Console.WriteLine("\n===== Deferred Execution =====");

        var query = orders.Where(o => o.Amount >= 200);

        orders.Add(new Order
        {
            Id = 7,
            CustomerId = 6,
            Amount = 500
        });

        foreach (var order in query)
        {
            Console.WriteLine($"Order Id: {order.Id}, Amount: {order.Amount}");
        }
    }
}
