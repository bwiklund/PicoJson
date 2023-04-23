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
  public double? AsNumber() => num;
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
    if (arr != null) return arr.SequenceEqual(other.arr!);
    if (this.obj != null) return this.obj.SequenceEqual(other.obj!);
    return false;
  }

  public override int GetHashCode()
  {
    if (str != null) return str.GetHashCode();
    if (num != null) return num.GetHashCode();
    if (@bool != null) return @bool.GetHashCode();
    if (arr != null) return arr.GetHashCode();
    if (obj != null) return obj.GetHashCode();
    return 0;
  }

  public override string ToString()
  {
    if (str != null) return str;
    if (num != null) return num.ToString();
    if (@bool != null) return @bool.ToString();
    if (arr != null) return arr.ToString();
    if (obj != null)
    {
      var sb = new StringBuilder();
      sb.Append("{");
      var first = true;
      foreach (var kv in obj)
      {
        if (!first) sb.Append(",");
        sb.Append("\"" + kv.Key + "\":" + kv.Value);
        first = false;
      }
      sb.Append("}");
      return sb.ToString();
    }
    return "null";
  }
}

public class Ctx {
  public string str;
  public int idx;
  public char? Peek() => idx < str.Length ? str[idx] : null;
  public char? Next() => idx < str.Length ? str[idx++] : null;
  public void Expect(char ch)
  {
    var next = Next();
    if (next != ch) throw new Exception("Expected " + ch + " got " + next);
  }
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
    Dyn value;
    var ch = ctx.Peek();
    if (ch == '"') value = ParseString(ctx);
    else if (ch == '{') value = ParseObject(ctx);
    //else if (ch == '[') value = ParseArray(ctx);
    else if ((ch >= '0' && ch <= '9') || ch == '.' || ch == '-') value = ParseNumber(ctx);
    else throw new Exception("Can't parse. TBD error message here");

    WhiteSpace(ctx);
    return value;
  }

  static void WhiteSpace(Ctx ctx)
  {
    while (true)
    {
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
    ctx.Expect('{');
    var obj = new Dyn() { obj = new Dictionary<string, Dyn>() };
    while (true)
    {
      WhiteSpace(ctx);
      switch (ctx.Peek())
      {
        case '"':
          var key = ParseString(ctx); // GC variant of this that's not wrapped in Dyn?
          WhiteSpace(ctx);
          ctx.Expect(':');
          var value = ParseValue(ctx);
          obj.obj[key.str!] = value;
          break;
        case '}':
          ctx.Next();
          return obj;
        case ',':
          ctx.Next();
          break;
        default:
          throw new Exception("Expected key or end of object");
      }
    }
  }

  static Dyn ParseString(Ctx ctx)
  {
    var sb = new StringBuilder(); // TODO keep me around i think

    ctx.Expect('"');
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

  static Dyn ParseNumber(Ctx ctx)
  {
    var sb = new StringBuilder(); // TODO keep me around i think

    while (ctx.Peek() is char ch)
    {
      if ((ch >= '0' && ch <= '9') || ch == '.' || ch == '-') // TODO exponents
      {
        sb.Append(ch);
        ctx.Next();
      }
      else
      {
        break;
      }
    }

    // TODO this doesn't handle the position/count of of - and . chars correctly, check the exact json spec

    return new Dyn(double.Parse(sb.ToString())); // thanks .net. ok but actually is this spec the same? good enough for now
  }
}
