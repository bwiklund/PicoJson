// See https://aka.ms/new-console-template for more information
public class Ctx {
  public string str;
  public int idx;

  public char? Peek() => idx < str.Length ? str[idx] : null;

  public char? Next() => idx < str.Length ? str[idx++] : null;

  public void Expect(char ch) {
    var next = Next();
    if (next != ch) throw new Exception($"Expected '{ch}' at position {idx}, got '{next}' instead");
  }

  public bool Accept(char ch) {
    if (Peek() != ch) {
      return false;
    } else {
      idx += 1;
      return true;
    }
  }
}
