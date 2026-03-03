namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

public class F1Team
{
    public F1Team(string name, string firstDriver, string secondDriver)
    {
        Name = name;
        FirstDriver = firstDriver;
        SecondDriver = secondDriver;
    }

    public string Name { get; set; }
    public string FirstDriver { get; set; }
    public string SecondDriver { get; set; }
}