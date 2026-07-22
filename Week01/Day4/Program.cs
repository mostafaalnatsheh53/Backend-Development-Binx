//TODO4.1 Choosing the Right Collection
/* //*List
List<string> Students= new List<string>();
Students.Add("ali");
Students.Add("Ahmed");
Students.Add("Mostaf");
foreach (string name in Students )
{
    Console.WriteLine(name);
    
}

List<string> User=new List<string>();
User.Add("ali");
User.Add("Ahmed");
User.Add("Mostaf");
User.Contains("Mostaf");
 O(n)
*/


/*Dictionary<int,string> Students= new Dictionary<int,string>();
Students.Add(1, "Ali");
Students.Add(2, "Ahmed");
Students.Add(3, "Mostafa");*/

/*  //*HashSet
HashSet<int> ids = new HashSet<int>();
ids.Add(10);
ids.Add(20);
ids.Add(10);
foreach (int id in ids)
{
    Console.WriteLine(id);
    
}*/

/*  //*exercise
List<string> names= new List<string> ();
names.Add("mostafa");
names.Add("ali");
names.Add("wisam");
names.Add("mohammad");
names.Add("ahmad");

foreach(string name in names)
{
    Console.WriteLine(name);
}

Dictionary<int,string> students= new Dictionary<int,string>();
students.Add(1,"mostafa");
students.Add(2,"ali");
students.Add(3,"wisam");
Console.WriteLine(students[2]);
HashSet<string> fruit= new HashSet<string>();
fruit.Add("Apple");
fruit.Add("Banana");
fruit.Add("Apple"); 
fruit.Add("Orange");
foreach(string fr in fruit)
{
    Console.WriteLine(fr);
}*/


//TODO 4.2 LINQ: Method Syntax and Query Syntax
using System.Runtime.CompilerServices;
/*class Program
{
    class User
    {
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }

    }
    static void Main(string[] args)
    {
        List<User> users = new()
     {
    new User
    {
        DisplayName = "Mostafa",
        Email = "mostafa@gmail.com",
        Role = "Admin",
        IsActive = true
    },
    new User
    {
        DisplayName = "Ali",
        Email = "ali@gmail.com",
        Role = "User",
        IsActive = true
    },
    new User
    {
        DisplayName = "Ahmad",
        Email = "ahmad@gmail.com",
        Role = "Admin",
        IsActive = false
    },
    new User
    {
        DisplayName = "Mohammad",
        Email = "mohammad@gmail.com",
        Role = "Admin",
        IsActive = true
    }

     };

        //!wihtout LINQ
        /*foreach (User user in users)
        {
            if (user.IsActive)
            {
                Console.WriteLine(user.DisplayName);
            }
        
       
        /* 
        //* with use LINQ
        var activeUsers = users.Where(u => u.IsActive);
        var names=users.Select(u=>u.Name);

        var nameActive= users
        .Where(u=>u.IsActive)
        .Select(u=>u.Name)
        .OrderBy(u=>u.Name);*/
//*  LINQ Practice Example

/*var activeAdminEmails = users
.Where(u => u.IsActive && u.Role == "Admin")
.Select(u => u.Email)
.OrderBy(u => u)
.ToList();

foreach (string email in activeAdminEmails)
{
    Console.WriteLine(email);
}

}
} */

//TODO 4.3 Async/Await Fundamentals
using System;
using System.Threading.Tasks;
using System.Data.Common;

/*class Program
{
    static async Task<string> GetUserName()
    {
        await Task.Delay(1000);

        return "Mostafa";
    }

    static async Task Main(string[] args)
    {
        string name = await GetUserName();

        Console.WriteLine(name);
    }
}*/



//*TODO 4.4 Exception Handling

/*class Program
{


    async Task Main(string[] args)
    {
        try
        {
            var response = await httpClient.GetAsync(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}*/
//*TODO Hands-On Lab: LINQ Queries & an Async Method
class Program
{
    class User
    {
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }
    }
    // *Lab Task 3: Async method simulating an I/O delay
    static async Task<string> GetServerMessage()
    {
        await Task.Delay(2000);
        return "Data Loaded Successfully";

    }
    static async Task Main(string[] args)
    {
        // *Lab Task 1: Create a list of 8 User objects

        List<User> users = new()
{
    new User { DisplayName = "Mostafa", Email = "mostafa@gmail.com", Role = "Admin", IsActive = true },
    new User { DisplayName = "Ahmad", Email = "ahmad@gmail.com", Role = "User", IsActive = true },
    new User { DisplayName = "Ali", Email = "ali@gmail.com", Role = "Admin", IsActive = false },
    new User { DisplayName = "Sara", Email = "sara@gmail.com", Role = "User", IsActive = true },
    new User { DisplayName = "Omar", Email = "omar@gmail.com", Role = "Manager", IsActive = true },
    new User { DisplayName = "Lina", Email = "lina@gmail.com", Role = "User", IsActive = false },
    new User { DisplayName = "Mohammad", Email = "mohammad@gmail.com", Role = "Admin", IsActive = true },
    new User { DisplayName = "Noor", Email = "noor@gmail.com", Role = "Manager", IsActive = true }
};
        // *Lab Task 2.1: LINQ Filter - Get active users
        var activeUsers = users.Where(u => u.IsActive);
        foreach (var user in activeUsers)
        {
            Console.WriteLine(user.DisplayName);

        }
        // */Lab Task 2.2: LINQ Projection - Get user names

        var names = users.Select(u => u.DisplayName);
        foreach (var name in names)
        {
            Console.WriteLine(name);

        }
        int activeCount = users.Count(user => user.IsActive);

        Console.WriteLine($"Active Users = {activeCount}");
        string message = await GetServerMessage();

        Console.WriteLine(message);
        // * Lab Task 4: Handle invalid user input using try/catch

        try
        {
            Console.WriteLine("Enter your age");
            int age = int.Parse(Console.ReadLine()!);
            Console.WriteLine($"Age = {age}");

        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter numbers only.");
        }
    }
}













