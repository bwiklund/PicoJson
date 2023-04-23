namespace MjsonTests {
  public class Tests {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Strings()
    {
      Assert.That(Mjson.Parse("\"Foo\"").AsString(), Is.EqualTo("Foo"));
      Assert.That(Mjson.Parse("\"\\\"\"").AsString(), Is.EqualTo("\""));
    }

    [Test]
    public void Numbers()
    {
      void AssertNumber(string json, double expected)
      {
        Assert.That(Mjson.Parse(json).AsNumber(), Is.EqualTo(expected));
      }

      AssertNumber("1", 1);
      AssertNumber("1234567890", 1234567890);
      AssertNumber("1234567890.75", 1234567890.75);
    }

    [Test]
    public void Objects()
    {
      void AssertObject(string json, Dictionary<string, Dyn> expected)
      {
        Assert.That(Mjson.Parse(json).obj, Is.EqualTo(expected));
      }

      var empty = new Dictionary<string, Dyn>();
      AssertObject("{}", empty);
      AssertObject("{ }", empty);
      AssertObject(" { }", empty);
      AssertObject(" { } ", empty);
      AssertObject("     {     }     ", empty);


      var objFooBar = new Dictionary<string, Dyn>() { { "foo", new Dyn("bar") } };
      AssertObject(@"{""foo"":""bar""}", objFooBar);
      AssertObject(@" { ""foo"" : ""bar"" } ", objFooBar);

      var objFooBarBaz = new Dictionary<string, Dyn>() { { "foo", new Dyn("bar") }, { "baz", new Dyn(1234) } };
      AssertObject(@"{""foo"":""bar"",""baz"":1234}", objFooBarBaz);
      AssertObject(@" { ""foo"" : ""bar"" , ""baz"" : 1234 } ", objFooBarBaz);
    }
  }
}