namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PropertyFixtureReflect : FixtureReflectAbstract
    {

        private readonly Dictionary<string, Tuple<object[], PropertyInfo>> propertiesObjects;
        private readonly Dictionary<string, Tuple<IFixture, PropertyInfo>> propertiesFixture;
        private readonly Dictionary<string, PropertyInfo> properties;

        public PropertyFixtureReflect(Type type) : base(type)
        {
            this.properties = new Dictionary<string, PropertyInfo>();
            this.propertiesFixture = new Dictionary<string, Tuple<IFixture, PropertyInfo>>();
            this.propertiesObjects = new Dictionary<string, Tuple<object[], PropertyInfo>>();
        }

        public override void Add(string key, IFixture fixture, MemberInfo member) => this.propertiesFixture.Add(key, new Tuple<IFixture, PropertyInfo>(fixture, (PropertyInfo)member));

        public override void Add(string key, object[] values, MemberInfo member) => this.propertiesObjects.Add(key, new Tuple<object[], PropertyInfo>(values, (PropertyInfo)member));

        public override void Add(string key, MemberInfo member) => this.properties.Add(key, (PropertyInfo)member);

        /// <summary>
        /// Method implement from the class FixtureReflectAbstract.
        /// This method calls the method ParseDictionaryAndInvokeAction, from the same parent class, to parse each dictionary and give the the PropertyInfo a value.
        /// </summary>
        /// <param name="obj">Object created with the same type as TargetType</param>
        public override void SetValues(object obj)
        {
            ParseDictionaryAndInvokeAction(
                this.properties,
                (info) =>
                    {
                        var value = GetValue(info, obj, () => GetRandomValue(info.PropertyType));
                        info.SetValue(obj, value);
                    }
            );
            ParseDictionaryAndInvokeAction(
                this.propertiesFixture,
                (tuple) =>
                {
                    var value = GetValue(tuple.Item2, obj, () => tuple.Item1.New());
                    tuple.Item2.SetValue(obj, value);
                }
            );
            ParseDictionaryAndInvokeAction(
                this.propertiesObjects,
                (tuple) =>
                {
                    var value = GetValue(tuple.Item2, obj, () => tuple.Item1[this.random.Next(0, tuple.Item1.Length)]);
                    tuple.Item2.SetValue(obj, value);
                }
            );
        }
    }
}
