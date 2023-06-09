﻿// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

public static class PicoJson {
  public static Dyn Parse(string str) => Parse(new Ctx() { str = str });

  public static Dyn Parse(Ctx ctx) {
    var value = ParseValue(ctx);
    // complain if theres garbage after the final valid element
    if (ctx.idx < ctx.str.Length) throw new Exception($"Expected end of string! Found '{(ctx.Peek())}' at position {ctx.idx}");
    return value;
  }

  static Dyn ParseValue(Ctx ctx) {
    WhiteSpace(ctx);
    Dyn value;
    var ch = ctx.Peek();
    if (ch == 'n') value = ParseNull(ctx);
    else if (ch == 't' || ch == 'f') value = ParseBool(ctx);
    else if (ch == '"') value = ParseString(ctx);
    else if (ch == '{') value = ParseObject(ctx);
    else if (ch == '[') value = ParseArray(ctx);
    else if ((ch >= '0' && ch <= '9') || ch == '.' || ch == '-') value = ParseNumber(ctx);
    else throw new Exception($"Couldn't parse character '{ch}' at position {ctx.idx}");

    WhiteSpace(ctx);
    return value;
  }

  static void WhiteSpace(Ctx ctx) {
    while (true) {
      var ch = ctx.Peek();
      if (ch == ' ' || ch == '\n' || ch == '\r' || ch == '\t') {
        ctx.Next();
        continue;
      }
      break;
    }
  }

  static Dyn ParseObject(Ctx ctx) {
    ctx.Expect('{');
    var obj = new Dyn() { obj = new Dictionary<string, Dyn>() };
    while (true) {
      WhiteSpace(ctx);
      switch (ctx.Peek()) {
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
          throw new Exception($"Expected key or end of object at position {ctx.idx}, got '{ctx.Peek()}'");
      }
    }
  }

  static Dyn ParseArray(Ctx ctx) {
    ctx.Expect('[');
    var arr = new Dyn() { arr = new List<Dyn>() };

    WhiteSpace(ctx);
    if (ctx.Accept(']')) { return arr; }

    while (true) {
      arr.arr.Add(ParseValue(ctx));
      if (!ctx.Accept(',')) { break; }
    }

    ctx.Expect(']');
    return arr;
  }

  static Dyn ParseNull(Ctx ctx) {
    // we already know the first char is n
    ctx.Expect("null");
    return new Dyn() { };
  }

  static Dyn ParseBool(Ctx ctx) {
    // we already know the first char is either t or f
    if (ctx.Accept("true")) {
      return new Dyn() { @bool = true };
    }
    ctx.Expect("false");
    return new Dyn() { @bool = false };
  }

  static Dyn ParseNumber(Ctx ctx) {
    var sb = new StringBuilder(); // TODO keep me around i think

    while (ctx.Peek() is char ch) {
      if ((ch >= '0' && ch <= '9') || ch == '.' || ch == '-') // TODO exponents
      {
        sb.Append(ch);
        ctx.Next();
      } else {
        break;
      }
    }

    // TODO this doesn't handle the position/count of of - and . chars correctly, check the exact json spec

    return new Dyn(double.Parse(sb.ToString())); // thanks .net. ok but actually is this spec the same? good enough for now
  }

  // note that this returns the number itself, not the char
  static int ParseHexChar(Ctx ctx) {
    if (ctx.Next() is char ch) {
      if (ch >= '0' && ch <= '9') return ch - '0';
      if (ch >= 'A' && ch <= 'F') return ch - 'A' + 10;
      if (ch >= 'a' && ch <= 'f') return ch - 'a' + 10;
      throw new Exception($"Expected hex character at position {ctx.idx}, found '{ch}'");
    }
    throw new Exception("Expected hex char, got EOF");
  }

  static Dyn ParseString(Ctx ctx) {
    ctx.Expect('"');
    var sb = new StringBuilder(); // TODO keep me around i think

    while (ctx.Next() is char ch) {
      switch (ch) {
        case '"': return new Dyn() { str = sb.ToString() };
        case '\\':
          var chEscaped = ctx.Next();
          switch (chEscaped) {
            case '"': sb.Append('"'); break;
            case 'b': sb.Append('\b'); break;
            case 'f': sb.Append('\f'); break;
            case 'n': sb.Append('\n'); break;
            case 'r': sb.Append('\r'); break;
            case 't': sb.Append('\t'); break;
            case 'u':
              var unicodeInt = (ParseHexChar(ctx) << 12) | (ParseHexChar(ctx) << 8) | (ParseHexChar(ctx) << 4) | ParseHexChar(ctx);
              sb.Append((char)unicodeInt);
              break;

            default: throw new Exception("Invalid escaped character: " + chEscaped); // right? this isn't allowed
          }
          break;
        default: sb.Append(ch); break;
      }
    }

    throw new Exception("String never ended!");
  }
}
