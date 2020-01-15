using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stringy.Tests
{
    public class Person
    {
        public Person()
        {
        }

        public Person(string firstname, string lastname, int age)
        { 
            Age = age;
            Firstname = firstname;
            Lastname = lastname;
        }

        public int Age { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }
    }

    public class UseCaseTests
    {
        [Fact]
        public void TestListFilter()
        {
            const string filterStr = "person.Age < 18 && person.Firstname.StartsWith('A')";
            var people = new List<Person>
            {
                new Person("Alex", "Jones", 13),
                new Person("Graham", "Olson", 58),
                new Person("Jessica", "Parker", 22),
                new Person("Alice", "Petraeus", 16)
            };

            var engine = new Stringy();
            var filteredPeople =
                people.Where(p => engine.Set("person", p).EvaluateExpression<bool>(filterStr)).ToList();

            Assert.Collection(filteredPeople, p => Assert.True(p.Firstname == "Alex" && p.Age == 13),
                                              p => Assert.True(p.Firstname == "Alice" && p.Age == 16));
        }
    }
}
