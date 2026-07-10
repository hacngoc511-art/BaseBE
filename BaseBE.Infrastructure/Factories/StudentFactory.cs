using BaseBE.Domain.Entities;

namespace BaseBE.Infrastructure.Factories;

public class StudentFactory
{
    public Student Create(string fullName, int age, string email)
    {
        return new Student
        {
            FullName = fullName,
            Age = age,
            Email = email
        };
    }
}
