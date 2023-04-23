using System.Reflection;

namespace MjsonTests {
  public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void Nulls() {
      Assert.That(PicoJson.Parse("null").IsNull(), Is.EqualTo(true));
      Assert.That(PicoJson.Parse("   null   ").IsNull(), Is.EqualTo(true));
    }

    [Test]
    public void Bools() {
      Assert.That(PicoJson.Parse("true").AsBool(), Is.EqualTo(true));
      Assert.That(PicoJson.Parse("false").AsBool(), Is.EqualTo(false));
      Assert.That(PicoJson.Parse("   true   ").AsBool(), Is.EqualTo(true));
      Assert.That(PicoJson.Parse("   false   ").AsBool(), Is.EqualTo(false));
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
    public void Strings() {
      Assert.That(PicoJson.Parse("\"Foo\"").AsString(), Is.EqualTo("Foo"));
      Assert.That(PicoJson.Parse("   \"Foo\"   ").AsString(), Is.EqualTo("Foo"));
      Assert.That(PicoJson.Parse("\"\\\"\"").AsString(), Is.EqualTo("\""));

      // and escaping
      Assert.That(new Dyn() { str = "\b\f\n\r\t"}.ToJson(), Is.EqualTo("\"\\b\\f\\n\\r\\t\""));

      // TODO \u unicode!
    }

    [Test]
    public void WhiteSpace() {
      Assert.That(PicoJson.Parse("1").AsNumber(), Is.EqualTo(1));
      Assert.That(PicoJson.Parse(" 1 ").AsNumber(), Is.EqualTo(1));
      Assert.That(PicoJson.Parse(" \n\r\t1\t\r\n ").AsNumber(), Is.EqualTo(1));
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

    [Test]
    public void RealData() {
      // this assumes the input is minified
      void AssertFile(string path) {
        var jsonFile = File.ReadAllText(path).Replace("\r\n", "\n");
        var obj = PicoJson.Parse(jsonFile);
        var backToJson = obj.ToJson(); // should tostring really be the serialize method?
        Assert.That(backToJson, Is.EqualTo(jsonFile));
      }

      AssertFile("Data/usgs.json");

      // TODO also test minified variants

      // TODO also test some bad inputs
    }
  }
}