// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

public class Dyn {
  public string? str;
  public double? num;
  public bool? @bool;
  public List<Dyn>? arr;
  public Dictionary<string, Dyn>? obj;

  public string? AsString() => str;
  public double? AsDouble() => num;
  public bool? AsBool() => @bool;
  public List<Dyn>? AsArray() => arr;
  public Dictionary<string, Dyn>? AsObject() => obj;
}

public class Ctx {
  public string str;
  public int idx;
  public char? Next() => str[idx++];
}

public static class Mjson {
  public static Dyn Parse(string str) => Parse(new Ctx() { str = str });

  public static Dyn Parse(Ctx ctx)
  {
    return ParseValue(ctx);
  }

  static Dyn ParseValue(Ctx ctx)
  {
    switch (ctx.Next())
    {
      case '"': return ParseString(ctx);
      //case '[': return ParseArray(ctx);
      //case '{': return ParseObject(ctx);
      default: throw new Exception("Can't parse. TBD error message here");
    }
  }

  static Dyn ParseString(Ctx ctx)
  {
    var sb = new StringBuilder();

    while (ctx.Next() is char ch)
    {
      switch (ch)
      {
        case '"': return new Dyn() { str = sb.ToString() };
        case '\\': sb.Append(ctx.Next()); break;
        default: sb.Append(ch); break;
      }
    }

    throw new Exception("String never ended!");
  }
}