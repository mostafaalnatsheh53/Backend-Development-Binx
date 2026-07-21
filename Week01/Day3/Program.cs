
/*class Program
{
    struct Person
    {
        public string name;
        public int age;
        public int birthMonth;
    }
    static void Main(string[] args)
    {
        Person person;
        person.name = "aba";
        person.age = 23;
        person.birthMonth = 5;
        Console.WriteLine($"{person.name} - {person.age}");
        Console.WriteLine($"{person.name} - {person.age}");
        string newName = ReturnPerson(out int newAge);
        Console.WriteLine($"{newName} - {newAge}");

    }
    static string ReturnPerson(out int age)
    {
        Console.WriteLine("Enter your name :");
        string name = Console.ReadLine();

        Console.WriteLine("Enter your age :");
        age = Convert.ToInt32(Console.ReadLine());
        return name;
    }
}
*/

/*class Car
{
    public string Brand="";
    public int Year;
    
   
    static void Main(string[] args)
    {
            Car car1=new Car();
            car1.Brand="BMW";
            car1.Year=2025;
            Console.WriteLine(car1.Brand);
            Console.WriteLine( car1.Year);





    }
}*/
/*using System.Data;

class Student
{
    public string Name = "";
    public int Age = 0;
    public string University = "";
}

public record Book(string Title, string Author, int Pages);

struct Point
{
    public int X;
}

class Program
{
    static void Main(string[] args)
    {
        Book book = new Book("Clean Code", "Robert C. Martin", 464);

        Console.WriteLine(book.Title);
        Console.WriteLine(book.Author);
        Console.WriteLine(book.Pages);

        Console.WriteLine("--------------");

        Point p1 = new Point();

        p1.X = 10;

        Point p2 = p1;

        p2.X = 50;

        Console.WriteLine(p1.X);
        Console.WriteLine(p2.X);
    }
}*/


/*public record CreateUserRequest(string Email, string DisplayName);
class Program
{

    static void Main(string[] args)
    {
       CreateUserRequest request = new CreateUserRequest("mostafa@gmail.com", "Mostafa");

        Console.WriteLine(request.Email);
        Console.WriteLine(request.DisplayName);
    }
}*/
/*public class UserAccount
{
    private string _email = "";
    public string Email
    {
        get { return _email; }

    }
    public void ChangeEmail(string newEmail)
    {
        if (newEmail.Contains("@"))
        {
            _email = newEmail;
        }

    }

}*/

/*class Program
{
    class User
{
    public string Name="";
    public void Login()
    {
                Console.WriteLine("User logged in");

    }
}
class Admin : User
{
    public void DeleteUser()
    {
        Console.WriteLine("User deleted");
    }
    
}
interface Iprinter
    {
        void Print();
    }
    class Report : Iprinter
    {
        public void Print()
        {
            Console.WriteLine("Printing report...");
        }
    }

    static void Main(string[] args)
    {
        Admin admin = new Admin();

admin.Name = "Mostafa";

admin.Login();

admin.DeleteUser();

Report report = new Report();

report.Print();
     
    }
}*/
/*class program
{
    interface IAnimal
{
    void Speak();
}
class Dog : IAnimal
    {
        public void Speak()
        {
                    Console.WriteLine("Woof");

        } 
        
    }

    class Cat  : IAnimal
    {
        public void Speak()
        {
                    Console.WriteLine("Meow");

        } 
        
    }
    void  Main(string[] args)
    {

        static void MakeSound(IAnimal animal)
    {
        animal.Speak();
    }

    static void Main(string[] args)
    {
        Dog dog = new Dog();
        Cat cat = new Cat();

        MakeSound(dog);
        MakeSound(cat);
    }
       
    }
}*/
class Program
{
    class Book : IPrintable
    {
        private string _title;
        private bool _isBorrowed;

        public string Title
        {
            get { return _title; }
        }

        public bool IsBorrowed
        {
            get { return _isBorrowed; }
        }

        public Book(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

            _title = title;
            _isBorrowed = false;
        }

        public void Borrow()
        {
            _isBorrowed = true;
        }

        public void Print()
        {
            Console.WriteLine($"Book: {Title}");
        }
    }

    class Member : IPrintable
    {
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public Member(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            _name = name;
        }

        public void Print()
        {
            Console.WriteLine($"Member: {Name}");
        }
    }

    public record BorrowRequest(string MemberName, string BookTitle);

    interface IPrintable
    {
        void Print();
    }

    static void PrintInfo(IPrintable item)
    {
        item.Print();
    }

    static void Main(string[] args)
    {
        Book book = new Book("Clean Code");
        Member member = new Member("Mostafa");

        BorrowRequest request = new BorrowRequest("Mostafa", "Clean Code");

        PrintInfo(book);
        PrintInfo(member);

        book.Borrow();

        Console.WriteLine(book.IsBorrowed);
    }
}
