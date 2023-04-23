﻿// See https://aka.ms/new-console-template for more information
using System.Text;

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

  public override bool Equals(object? obj) {
    if (obj == null || GetType() != obj.GetType()) return false;
    var other = (Dyn)obj;
    if (str != null) return str == other.str;
    if (num != null) return num == other.num;
    if (@bool != null) return @bool == other.@bool;
    if (arr != null) return arr.SequenceEqual(other.arr!);
    if (this.obj != null) return this.obj.SequenceEqual(other.obj!);
    return false;
  }

  public override int GetHashCode() {
    if (str != null) return str.GetHashCode();
    if (num != null) return num.GetHashCode();
    if (@bool != null) return @bool.GetHashCode();
    if (arr != null) return arr.GetHashCode();
    if (obj != null) return obj.GetHashCode();
    return 0;
  }

  public override string ToString() {
    if (str != null) return str;
    if (num != null) return num.ToString();
    if (@bool != null) return @bool.ToString();
    if (arr != null) return arr.ToString();
    if (obj != null) {
      var sb = new StringBuilder();
      sb.Append("{");
      var first = true;
      foreach (var kv in obj) {
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