namespace Fixtr
{
    using System;
    using System.Collections.Generic;

    public class FixtureCache
    {

        private static Dictionary<string, IFixture> fixtures = new Dictionary<string, IFixture>();

        public static IFixture GetFixture(Type type)
        {
            if (fixtures.ContainsKey(type.Name))
                return fixtures[type.Name];
            var fixture = new FixtureReflect(type);
            fixtures.Add(type.Name, fixture);
            return fixture;
        }

    }
}
