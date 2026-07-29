// *Day 1 — Generics & Advanced Collections — 8 hours

using System;
// * 1.2 Writing Generic Methods and Classes
/*class Program
{
    public class Box<T>
    {
        
        public T Value { get; set; }
    }
    static void Main(string[] args)
    {
        Box<int> numberOfbox = new();
        numberOfbox.Value = 10;
        Box<string> nameOfbox = new();
        nameOfbox.Value = "Mostafa";
        Console.WriteLine(numberOfbox.Value);
        Console.WriteLine(nameOfbox.Value);
    }*/
// *Exercise 
/*
    public class Repository<T> where T : class
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    }
    public class User
{
    public string Name { get; set; } = "";
}
    static void Main(string[] args)
    {
        Repository<User> userRepository =new();
        userRepository.Add(new User { Name = "Mostafa" });
        userRepository.Add(new User { Name = "Ali" });
        userRepository.Add(new User { Name = "Ahmad" });
        IReadOnlyList<User> users =userRepository.GetAll();
        foreach (User user in users)
        {
            Console.WriteLine(user.Name);
        }
    }

}*/
// *1.3 Generic Constraints

/*class Program
{
    public class Box<T>
    {
        
        public T Value { get; set; }
    }
    static void Main(string[] args)
    {
        Box<int> numberOfbox = new();
        numberOfbox.Value = 10;
        Box<string> nameOfbox = new();
        nameOfbox.Value = "Mostafa";
        Console.WriteLine(numberOfbox.Value);
        Console.WriteLine(nameOfbox.Value);
    }*/
// *Exercise 
using System.Reflection;

/*public class Program
{
    // *where T :class
    class Storage<T> where T : class
    {
        private readonly List<T> _items = new();
        public void Add(T item) => _items.Add(item);
        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    }
    public class Student
    {
        public string Name { get; set; } = "";
    }
    static void Main(string[] args)
    {
        Storage<Student> Students = new();
        Students.Add(new Student { Name = "Mostafa" });
        Students.Add(new Student { Name = "Ali" });
        Students.Add(new Student { Name = "Ahmad" });
       IReadOnlyList<Student> Studen=Students.GetAll();

        foreach (Student user in Studen)
        {
            Console.WriteLine(user.Name);
        }

    }
}*/
// */where T : IComparable

/*public class Program
{

    class Comparer<T> where T : IComparable
    {
        public bool IsGreater(T first, T second)
        {
            return first.CompareTo(second) > 0;
        }
    }
    static void Main(string[] args)
    {
        Comparer<int> c = new();
        Console.WriteLine(c.IsGreater(20, 10));
        Console.WriteLine(c.IsGreater(5, 30));




    }
}*/
// */where T : new()
/*
public class Program
{
    class Factory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }
    class Car
    {
        public string Model { get; set; } = "BMW";
    }


    static void Main(string[] args)
    {
        Factory<Car> factory = new();

        Car car = factory.Create();

        Console.WriteLine(car.Model);



    }

}*/

// */1.4 Choosing Between Collection Interfaces
/*class Program
{


    static void Main(string[] args)
    {

      
  //1.IEnumerable < T >
        // ! use just for read you cant names.Add("Omar") ❌
/*IEnumerable<string> names = new List<string>
{
  "Mostafa",
"Ali",
"Ahmad"
};
 foreach (string name in names)
 {
     Console.WriteLine(name);
 }


}*/
// */2.IReadOnlyList<T>
// ! use for read by index know the number of items but cant modify ❌
/*IReadOnlyList<string> names = new List<string>
    {
    "Mostafa",
    "Ali",
    "Ahmad"

    };
    foreach (string name in names)
 {
     Console.WriteLine(name);
 }*/
// */3.IList<T>
//! for every thing
/*IList<string> names = new List<string>();
names.Add("Mostafa");
names.Remove("Mostafa");
names[0] = "Ali";
Console.WriteLine(names.Count);
Console.WriteLine(names[0]);
}




}
*/
// *Hands-On Lab: Build a Generic Repository

    class Programs
{
    class Repository<T> where T : class // ! T must be a class (reference type).
    {
        public T? Find(Predicate<T> predicate)
        {
            return _items.Find(predicate);
        }
        private readonly List<T> _items = new();
        public void Add(T item)
        {
            _items.Add(item);
        }
        //public List<T> GetAll(){    return _items; }
        public IReadOnlyList<T> GetAll()
        {
            return _items.AsReadOnly();
        }


    }
    class Student
    {
        public string Name { get; set; } = "";
    }

    class Book
    {
        public string Title { get; set; } = "";
    }



    static void Main(string[] args)
    {
        Repository<Student> studentRepository = new();
        Repository<Book> bookRepository = new();
        studentRepository.Add(new Student { Name = "Mostafa" });
        studentRepository.Add(new Student { Name = "Ali" });
        bookRepository.Add(new Book { Title = "Clean Code" });
        bookRepository.Add(new Book { Title = "C# in Depth" });
        Student? student = studentRepository.Find(s => s.Name == "Ali");

        if (student != null)
        {
            Console.WriteLine(student.Name);
        }
    }

}


