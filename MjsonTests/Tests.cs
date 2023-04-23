namespace MjsonTests {
  public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void Strings() {
      Assert.That(PicoJson.Parse("\"Foo\"").AsString(), Is.EqualTo("Foo"));
      Assert.That(PicoJson.Parse("   \"Foo\"   ").AsString(), Is.EqualTo("Foo"));
      Assert.That(PicoJson.Parse("\"\\\"\"").AsString(), Is.EqualTo("\""));
    }

    [Test]
    public void Numbers() {
      void AssertNumber(string json, double expected) {
        Assert.That(PicoJson.Parse(json).AsNumber(), Is.EqualTo(expected));
      }

      AssertNumber("1", 1);
      AssertNumber("1234567890", 1234567890);
      AssertNumber("1234567890.75", 1234567890.75);
      AssertNumber(".75", .75);
      AssertNumber("0.75", 0.75);
      AssertNumber("-0.75", -0.75);
      AssertNumber("0", 0);
      AssertNumber("-0", -0);
      AssertNumber("-.0", -.0);

      // Assert.Throws<Exception>(() => Mjson.Parse("0."));
      // todo bad inputs
      // todo exponents
    }

    [Test]
    public void Objects() {
      void AssertObject(string json, Dyn expected) {
        var obj = PicoJson.Parse(json);
        Assert.That(obj, Is.EqualTo(expected), $"{obj} did not equal {expected}");
      }

      var empty = new Dyn(new Dictionary<string, Dyn>());
      AssertObject("{}", empty);
      AssertObject("{ }", empty);
      AssertObject(" { }", empty);
      AssertObject(" { } ", empty);
      AssertObject("     {     }     ", empty);

      var objFooBar = new Dyn(new Dictionary<string, Dyn>() { { "foo", new Dyn("bar") } });
      AssertObject(@"{""foo"":""bar""}", objFooBar);
      AssertObject(@" { ""foo"" : ""bar"" } ", objFooBar);

      var objFooBarBaz = new Dyn(new Dictionary<string, Dyn>() { { "foo", new Dyn("bar") }, { "baz", new Dyn(1234) } });
      AssertObject(@"{""foo"":""bar"",""baz"":1234}", objFooBarBaz);
      AssertObject(@" { ""foo"" : ""bar"" , ""baz"" : 1234 } ", objFooBarBaz);

      var objNested = new Dyn(new Dictionary<string, Dyn>() { { "foo", new Dyn(new Dictionary<string, Dyn>() { { "baz", new Dyn(1234) } }) } });
      AssertObject(@"{""foo"":{""baz"":1234}}", objNested);
    }

    [Test]
    public void Arrays() {
      void AssertArray(string json, Dyn expected) {
        var obj = PicoJson.Parse(json);
        Assert.That(obj, Is.EqualTo(expected), $"{obj} did not equal {expected}");
      }
      var empty = new Dyn(new List<Dyn>());
      AssertArray("[]", empty);
      AssertArray("[ ]", empty);
      AssertArray(" [ ] ", empty);
      AssertArray("     [     ]     ", empty);
      var arrFooBar = new Dyn(new List<Dyn>() { new Dyn("foo"), new Dyn("bar") });
      AssertArray(@"[""foo"",""bar""]", arrFooBar);
      AssertArray(@" [ ""foo"" , ""bar"" ] ", arrFooBar);
      var arrFooBarBaz = new Dyn(new List<Dyn>() { new Dyn("foo"), new Dyn("bar"), new Dyn(1234) });
      AssertArray(@"[""foo"",""bar"",1234]", arrFooBarBaz);
      AssertArray(@" [ ""foo"" , ""bar"" , 1234 ] ", arrFooBarBaz);
      var arrNested = new Dyn(new List<Dyn>() { new Dyn(new List<Dyn>() { new Dyn("foo"), new Dyn("bar") }) });
      AssertArray(@"[[""foo"",""bar""]]", arrNested);
    }
  }
}