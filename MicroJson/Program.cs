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

  public Dyn() { }
  public Dyn(string str) { this.str = str; }
  public Dyn(double num) { this.num = num; }
  public Dyn(bool @bool) { this.@bool = @bool; }
  public Dyn(List<Dyn> arr) { this.arr = arr; }
  public Dyn(Dictionary<string, Dyn> obj) { this.obj = obj; }

  public override bool Equals(object? obj)
  {
    if (obj == null || GetType() != obj.GetType()) return false;
    var other = (Dyn)obj;
    if (str != null) return str == other.str;
    if (num != null) return num == other.num;
    if (@bool != null) return @bool == other.@bool;
    if (arr != null) return arr == other.arr;
    if (this.obj != null) return this.obj == other.obj;
    return false;
  }
}

public class Ctx {
  public string str;
  public int idx;
  public char? Peek() => idx < str.Length ? str[idx] : null;
  public char? Next() => idx < str.Length ? str[idx++] : null;
}

public static class Mjson {
  public static Dyn Parse(string str) => Parse(new Ctx() { str = str });

  public static Dyn Parse(Ctx ctx)
  {
    var value = ParseValue(ctx);
    // complain if theres garbage after the final valid element
    if (ctx.idx != ctx.str.Length) throw new Exception("Expected end of string! Error tbd");
    return value;
  }

  static Dyn ParseValue(Ctx ctx)
  {
    WhiteSpace(ctx);
    var value = ctx.Next() switch
    {
      '"' => ParseString(ctx),
      '{' => ParseObject(ctx),
      //case '[': return ParseArray(ctx);
      _ => throw new Exception("Can't parse. TBD error message here")
    };
    WhiteSpace(ctx);
    return value;
  }

  static void WhiteSpace(Ctx ctx)
  {
    while (true) {
      var ch = ctx.Peek();
      if (ch == ' ' || ch == '\n' || ch == 'r' || ch == '\t')
      {
        ctx.Next();
        continue;
      }
      break;
    }
  }

  static Dyn ParseObject(Ctx ctx)
  {
    var obj = new Dyn() { obj = new Dictionary<string, Dyn>() };
    while(true)
    {
      WhiteSpace(ctx);
      switch (ctx.Next())
      {
        case '"':
          var key = ParseString(ctx); // GC variant of this that's not wrapped in Dyn?
          WhiteSpace(ctx);
          if (ctx.Next() != ':') throw new Exception("Expected :");
          var value = ParseValue(ctx);
          obj.obj[key.str!] = value;
          break;
        case '}':
          return obj;
      }
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
        case '\\':
          var chEscaped = ctx.Next();
          sb.Append(chEscaped switch
          {
            '"' => '"',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            // TODO \u + 4 hex digits
            _ => throw new Exception("Invalid escaped character: " + chEscaped)
          });
          break;
        default: sb.Append(ch); break;
      }
    }

    throw new Exception("String never ended!");
  }
}