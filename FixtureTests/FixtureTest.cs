namespace FixtureTests
{
    using Fixtr;
    using FixtureTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FixtureTest
    {
        [TestMethod]
        public void CreateStudentUsingNew()
        {
            //IFixture fixture = new FixtureReflect();
            var school = (School)new FixtureReflect(typeof(School))
                        .New();
            Assert.IsNotNull(school);
            Assert.IsNotNull(school.Name);
            Assert.IsNotNull(school.something);
        }
    }
}
