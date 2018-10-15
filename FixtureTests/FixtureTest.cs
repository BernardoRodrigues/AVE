namespace FixtureTests
{
    using Fixtr.IFixture;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FixtureTest
    {
        [TestMethod]
        public void CreateStudentUsingNew()
        {
            IFixture fixture = new FixtureReflect();

        }
    }
}
