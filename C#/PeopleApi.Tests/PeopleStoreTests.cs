using PeopleApi.Services;
using NUnit.Framework;
using System.Linq;

[TestFixture]
public class PeopleStoreTests
{
    [Test]
    public void GetAll_ReturnsAllPeople()
    {
        // Arrange
        var store = new PeopleStore();
        store.Add("Alice", 30);
        store.Add("Bob", 25);

        // Act
        var people = store.GetAll().ToList();

        // Assert
        Assert.That(people.Count, Is.EqualTo(2));
        Assert.IsTrue(people.Any(p => p.Name == "Alice" && p.Age == 30));
        Assert.IsTrue(people.Any(p => p.Name == "Bob" && p.Age == 25));
    }

    [Test]
    public void GetById_ReturnsCorrectPerson()
    {
        // Arrange
        var store = new PeopleStore();
        var person = store.Add("Charlie", 40);

        // Act
        var result = store.GetById(person.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo("Charlie"));
        Assert.That(result.Age, Is.EqualTo(40));
    }

    [Test]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var store = new PeopleStore();

        // Act
        var result = store.GetById(999);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void Add_AddsPersonCorrectly()
    {
        // Arrange
        var store = new PeopleStore();

        // Act
        var person = store.Add("Dana", 22);

        // Assert
        Assert.IsNotNull(person);
        Assert.That(person.Name, Is.EqualTo("Dana"));
        Assert.That(person.Age, Is.EqualTo(22));
        Assert.IsTrue(store.GetAll().Any(p => p.Id == person.Id));
    }

    [Test]
    public void Clear_RemovesAllPeople()
    {
        // Arrange
        var store = new PeopleStore();
        store.Add("Eve", 28);

        // Act
        store.Clear();

        // Assert
        Assert.IsEmpty(store.GetAll());
    }

    [Test]
    public void Search_ReturnsMatchingPeople()
    {
        // Arrange
        var store = new PeopleStore();
        store.Add("Frank", 35);
        store.Add("Francine", 29);
        store.Add("George", 40);

        // Act
        var results = store.Search("Fran").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.IsTrue(results.All(p => p.Name.Contains("Fran")));
    }

    [Test]
    public void Search_ReturnsEmpty_WhenNoMatch()
    {
        // Arrange
        var store = new PeopleStore();
        store.Add("Helen", 31);

        // Act
        var results = store.Search("Zoe");

        // Assert
        Assert.IsEmpty(results);
    }
}
