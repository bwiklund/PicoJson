public class Ctx {
  public string str;
  public int idx;

  public char? Peek() => idx < str.Length ? str[idx] : null;

  public char? Next() => idx < str.Length ? str[idx++] : null;

  public void Expect(char ch) {
    if (!Accept(ch)) {
      throw new Exception($"Expected '{ch}' at position {idx}, got '{Peek()}' instead");
    }
  }

  public void Expect(string expectStr) {
    if (!Accept(expectStr)) {
      throw new Exception($"Expected '{expectStr}' at position {idx}, got '{Peek()}' instead");
    }
  }

  public bool Accept(char ch) {
    if (Peek() != ch) {
      return false;
    } else {
      idx += 1;
      return true;
    }
  }

  public bool Accept(string acceptStr) {
    if (idx + acceptStr.Length > str.Length) return false;

    for (var i = 0; i < acceptStr.Length; i++) {
      if (str[idx + i] != acceptStr[i]) return false;
    }

    idx += acceptStr.Length;
    return true;
  }
}
