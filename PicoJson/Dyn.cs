// See https://aka.ms/new-console-template for more information
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
  // might be better to have a real "isNull" bool to avoid doing this many checks? more state to manage tho.
  public bool IsNull() => str == null && num == null && @bool == null && arr == null && obj == null;

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

  public string ToJson() {
    var sb = new StringBuilder();
    ToJson(sb);
    return sb.ToString();
  }

  public void ToJson(StringBuilder sb) {
    if (str != null) {
      sb.Append('"');
      for(var i  = 0; i < str.Length; i++) {
        var ch = str[i];
        if (ch == '\b') sb.Append("\\b");
        else if (ch == '\f') sb.Append("\\f");
        else if (ch == '\n') sb.Append("\\n");
        else if (ch == '\r') sb.Append("\\r");
        else if (ch == '\t') sb.Append("\\t");
        // TODO \u unicode
        else sb.Append(ch);
      }
      sb.Append('"');
    } else if (num != null) {
      sb.Append(num.ToString()); // TODO this probably doesn't have the exact spec as json's number format?
    } else if (@bool != null) {
      sb.Append(@bool.Value ? "true" : "false");
    } else if (arr != null) {
      sb.Append('[');
      for (var i = 0; i < arr.Count; i++) {
        if (i > 0) sb.Append(',');
        sb.Append(arr[i].ToJson());
      }
      sb.Append(']');
      return;
    } else if (obj != null) {
      sb.Append('{');
      var first = true;
      foreach (var kv in obj) {
        if (!first) sb.Append(",");
        sb.Append("\"" + kv.Key + "\":" + kv.Value.ToJson());
        first = false;
      }
      sb.Append('}');
      return;
    } else {
      sb.Append("null");
    }
  }
}
