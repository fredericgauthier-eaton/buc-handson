using System;

namespace PeopleApi.Services;

public record Person(int Id, string Name, int Age);
public record PersonDto(string Name, int Age);

public class PeopleStore
{
    private List<Person> _people = new();
    public IEnumerable<Person> GetAll() => _people;
    public Person? GetById(int id) => _people.FirstOrDefault(p => p.Id == id);
    public Person Add(string name, int age)
    {
        var person = new Person(_people.Count, name, age);
        _people.Add(person);
        return person;
    }
    public void Clear() => _people.Clear();
    public IEnumerable<Person> Search(string name) => _people.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
}
