using System.Runtime.Intrinsics.X86;
int x = 15;
int y = x;
y = 40;
Console.WriteLine(x);
Console.WriteLine(y);
bool a = true;
bool b = a;
b = false;
Console.WriteLine(a);
Student s1 = new Student();
s1.Name = "Sara";
Student s2 = s1;
s2.Name = "Lina";

Console.WriteLine(s1.Name);
Console.WriteLine(s2.Name);

Car c1 = new Car();
c1.brand = "BMW";

Car c2 = new Car();
c2.brand = "Audi";

Console.WriteLine(c1.brand);
Console.WriteLine(c2.brand);

// 2.2 Variables, Type Inference, and Naming

var num = 3;
num = 4;

Console.WriteLine(num);

// 2.3 Control Flow

int age = 20;

if (age >= 18)
{
    Console.WriteLine("Adult");
}

int age2 = 15;

if (age2 >= 18)
{
    Console.WriteLine("Adult");
}
else
{
    Console.WriteLine("Not Adult");
}

int marks = 85;

if (marks >= 90)
{
    Console.WriteLine("Excellent");
}
else if (marks >= 70)
{
    Console.WriteLine("Good");
}
else if (marks >= 50)
{
    Console.WriteLine("Pass");
}
else
{
    Console.WriteLine("Fail");
}

// Switch Expression

int score = 85;

string grade = score switch
{
    >= 90 => "Excellent",
    >= 70 => "Proficient",
    >= 50 => "Developing",
    _ => "Below Standard"
};

Console.WriteLine(grade);

// for

for (int i = 1; i <= 5; i++)
{
    Console.WriteLine(i);
}

// foreach

string[] names =
{
    "Sara",
    "Ali",
    "Lina"
};

foreach (string studentName in names)
{
    Console.WriteLine(studentName);
}

// while

int ii = 1;

while (ii <= 5)
{
    Console.WriteLine(ii);
    ii++;
}

// 2.4 Nullable Reference Types

string? userName = null;

if (userName != null)
{
    Console.WriteLine(userName.Length);
}
else
{
    Console.WriteLine("No name entered.");
}

//Hands-On Lab: Types & Control Flow Practice
//1. Write a console program with at least 3 value-type and 3 reference-type variables, printing each one's type using"GetType()

int age3 = 25;
double salary = 555.9;
bool isStudent = true;


string username = "Mostafa";
int[] numbers = { 1, 2, 3, 4, 5 };
Student student = new Student();


Console.WriteLine(age3.GetType());
Console.WriteLine(salary.GetType());
Console.WriteLine(isStudent.GetType());

Console.WriteLine(username.GetType());
Console.WriteLine(numbers.GetType());
Console.WriteLine(student.GetType());
//2. Write a method that demonstrates the value-vs-reference copy behavior, printing before and after a mutation.


 ValueTypeExample();
 ReferenceTypeExample();

static void ValueTypeExample()
{
    int xx=10;
    int yy=xx;

    Console.WriteLine($"Before:x={xx},y={yy}");
    yy=50;
        Console.WriteLine($"after:x={xx},y={yy}");
}
static void ReferenceTypeExample()
{
     Student ss1=new Student();
     ss1.Name="mostafa";
     Student ss2 = ss1;
    Console.WriteLine($"Before:ss1={ss1.Name},ss2={ss2.Name}");
    ss2.Name="Lina";
    Console.WriteLine($"after:ss1={ss1.Name},ss2={ss2.Name}");
}
//3. Write a grade-classifier method using a switch expression, covering at least 4 score ranges.
Console.WriteLine(GetGrade(95));
Console.WriteLine(GetGrade(75));
Console.WriteLine(GetGrade(55));
Console.WriteLine(GetGrade(30));
static string GetGrade(int score)
{
    return score switch
    {
        >= 90 => "Excellent",
        >= 70 => "Good",
        >= 50 => "Pass",
        _ => "Fail"
    };
}
//4. Write a small program that reads user input and handles a possibly-null value safely, with nullable reference types enabled
Console.Write("Enter your name: ");

string? name = Console.ReadLine();

if (name != null)
{
    Console.WriteLine($"Welcome, {name}!");
}
else
{
    Console.WriteLine("No name was entered.");
}

class Student
{
    public string Name = "";
}

class Car
{
    public string brand = "";
}
