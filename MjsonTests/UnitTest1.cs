namespace MjsonTests {
  public class Tests {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
      Assert.That(Mjson.Parse("\"Foo\"").AsString(), Is.EqualTo("Foo"));
    }
  }
}