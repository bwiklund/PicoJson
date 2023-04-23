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


      var something = new Dictionary<string, Dyn>() { { "foo", new Dyn("bar") } };
      AssertObject(@"{""foo"":""bar""}", something);
      AssertObject(@" { ""foo"" : ""bar"" } ", something);
    }
  }
}