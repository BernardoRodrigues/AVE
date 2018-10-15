namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;


    public class FieldFixtureReflect : FixtureReflectAbstract
    {

        private readonly Dictionary<string, Tuple<object, FieldInfo>> fieldsObjects;
        private readonly Dictionary<string, Tuple<IFixture, FieldInfo>> fieldsFixture;
        private readonly Dictionary<string, FieldInfo> fields;

        public FieldFixtureReflect(Type type) : base(type)
        {
            this.fields = new Dictionary<string, FieldInfo>();
            this.fieldsFixture = new Dictionary<string, Tuple<IFixture, FieldInfo>>();
            this.fieldsObjects = new Dictionary<string, Tuple<object, FieldInfo>>();
        }
    }
}
