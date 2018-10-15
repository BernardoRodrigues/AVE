namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public class PropertyFixtureReflect : FixtureReflectAbstract
    {

        private readonly Dictionary<string, Tuple<object, PropertyInfo>> propertiesObjects;
        private readonly Dictionary<string, Tuple<IFixture, PropertyInfo>> propertiesFixture;
        private readonly Dictionary<string, PropertyInfo> properties;

        public PropertyFixtureReflect(Type type) : base(type)
        {
            this.properties = new Dictionary<string, PropertyInfo>();
            this.propertiesFixture = new Dictionary<string, Tuple<IFixture, PropertyInfo>>();
            this.propertiesObjects = new Dictionary<string, Tuple<object, PropertyInfo>>();
        }

    }
}
