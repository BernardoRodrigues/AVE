namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;


    public class FieldFixtureReflect : FixtureReflectAbstract
    {

        private readonly Dictionary<string, Tuple<object[], FieldInfo>> fieldsObjects;
        private readonly Dictionary<string, Tuple<IFixture, FieldInfo>> fieldsFixture;
        private readonly Dictionary<string, FieldInfo> fields;

        public FieldFixtureReflect(Type type) : base(type)
        {
            this.fields = new Dictionary<string, FieldInfo>();
            this.fieldsFixture = new Dictionary<string, Tuple<IFixture, FieldInfo>>();
            this.fieldsObjects = new Dictionary<string, Tuple<object[], FieldInfo>>();
        }

        public override void Add(string key, IFixture fixture, MemberInfo member) => this.fieldsFixture.Add(key, new Tuple<IFixture, FieldInfo>(fixture, (FieldInfo)member));

        public override void Add(string key, object[] values, MemberInfo member) => this.fieldsObjects.Add(key, new Tuple<object[], FieldInfo>(values, (FieldInfo)member));

        public override void Add(string key, MemberInfo member) => this.fields.Add(key, (FieldInfo)member);

        /// <summary>
        /// Method implement from the class FixtureReflectAbstract.
        /// This method calls the method ParseDictionaryAndInvokeAction, from the same parent class, to parse each dictionary and give the the FieldInfo a value.
        /// </summary>
        /// <param name="obj">Object created with the same type as TargetType</param>
        public override void SetValues(object obj)
        {
            ParseDictionaryAndInvokeAction(
                this.fields,
                (info) =>
                {
                    var value = GetValue(info, obj, () => GetRandomValue(info.FieldType));
                    info.SetValue(obj, value);
                }
            );
            ParseDictionaryAndInvokeAction(
                this.fieldsFixture,
                (tuple) =>
                {
                    var value = GetValue(tuple.Item2, obj, () => tuple.Item1.New());
                    tuple.Item2.SetValue(obj, value);
                }
            );
            ParseDictionaryAndInvokeAction(
                this.fieldsObjects,
                (tuple) =>
                {
                    var value = GetValue(tuple.Item2, obj, () => tuple.Item1[this.random.Next(0, tuple.Item1.Length)]);
                    tuple.Item2.SetValue(obj, value);
                }
            );
        }
    }
}
