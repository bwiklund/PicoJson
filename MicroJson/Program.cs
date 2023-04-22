// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

public class Dyn {
  public string? str;
  public double? num;

  public string? AsString() => str;
  public double? AsDouble() => num;
}
public static class Mjson {
  public static Dyn Parse(string str, int idx = 0)
  {
    return new Dyn() { str = "Not foo" };
  }
}