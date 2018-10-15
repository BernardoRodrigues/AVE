namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class FixtureReflect : IFixture
    {
        private readonly Dictionary<string, FieldInfo> fields;
        private readonly Dictionary<string, PropertyInfo> properties;
        private readonly Random random;
        public Type TargetType
        {
            get; private set;
        }
        private readonly ConstructorInfo constructorInfo;
        private readonly ParameterInfo[] parameterInfos;
        private readonly Dictionary<string, Tuple<object, FieldInfo>> fieldsObjects;
        private readonly Dictionary<string, Tuple<object, PropertyInfo>> propertiesObjects;
        private readonly Dictionary<string, Tuple<IFixture, FieldInfo>> fieldsFixture;
        private readonly Dictionary<string, Tuple<IFixture, PropertyInfo>> propertiesFixture;

        public FixtureReflect(Type type)
        {
            this.TargetType = type;
            this.fields = new Dictionary<string, FieldInfo>();
            this.properties = new Dictionary<string, PropertyInfo>();
            this.fieldsObjects = new Dictionary<string, Tuple<object, FieldInfo>>();
            this.propertiesObjects = new Dictionary<string, Tuple<object, PropertyInfo>>();
            this.fieldsFixture = new Dictionary<string, Tuple<IFixture, FieldInfo>>();
            this.propertiesFixture = new Dictionary<string, Tuple<IFixture, PropertyInfo>>();
            this.random = new Random();
            this.constructorInfo = this.TargetType.GetConstructors().GetValue(0) as ConstructorInfo;
            if (this.constructorInfo != null)
                this.parameterInfos = this.constructorInfo.GetParameters();
        }

        public object New()
        {
            var obj = GetObject();
            if (this.fields.Count != 0)
            {
                var keys = this.fields.Keys;
                foreach (var key in keys)
                {
                    var field = this.fields[key];
                    field.SetValue(obj, GetRandomValue(field.FieldType));
                }
            }
            if (this.properties.Count != 0)
            {
                var keys = this.properties.Keys;
                foreach (var key in keys)
                {
                    var property = this.properties[key];
                    property.SetValue(obj, GetRandomValue(property.PropertyType));
                }
            }
            return obj;
        }

        private object GetObject()
        {
            if (this.constructorInfo == null)
            {
                return Activator.CreateInstance(this.TargetType);
            }
            var parameters = new List<object>();
            foreach (var parameterInfo in this.parameterInfos)
            {
                parameters.Add(GetRandomValue(parameterInfo.ParameterType));
            }
            return this.constructorInfo.Invoke(parameters.ToArray());
        }

        private object GetRandomValue(Type parameterType)
        {
            if (parameterType.IsValueType && parameterType.IsPrimitive)
                return Activator.CreateInstance(parameterType);
            if (parameterType.Equals(typeof(string)))
                return RandomString();
            if (parameterType.IsArray)
                return new FixtureReflect(parameterType.GetElementType()).Fill(this.random.Next(1, 20));
            else
                return new FixtureReflect(parameterType).New();
        }

        private string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 50)
              .Select(s => s[this.random.Next(s.Length)]).ToArray());
        }

        public object[] Fill(int size)
        {
            var objects = new object[size];
            for (var i = 0 ; i < size ; i++)
                objects[i] = New();
            return objects;
        }

        IFixture Member(string name)
        {
            var fieldInfo = this.TargetType.GetField(name);
            var propertyInfo = this.TargetType.GetProperty(name);
            if (fieldInfo != null)
                this.fields.Add(name, fieldInfo);
            else if (propertyInfo != null)
                this.properties.Add(name, propertyInfo);
            else
                throw new ArgumentException("No field or property exists with the given name");
            return this;
        }

        public IFixture Member(string name, params object[] parameters)
        {
            var fieldInfo = this.TargetType.GetField(name);
            if (fieldInfo != null)
                this.fieldsObjects.Add(name, new Tuple<object, FieldInfo>(parameters[this.random.Next() % parameters.Length], fieldInfo));
            else
                this.propertiesObjects.Add(name, new Tuple<object, PropertyInfo>(parameters[this.random.Next() % parameters.Length], this.TargetType.GetProperty(name)));
            return this;
        }

        public IFixture Member(string name, IFixture fixt)
        {
            var fieldInfo = this.TargetType.GetField(name);
            if (fieldInfo != null)
                this.fieldsObjects.Add(name, new Tuple<object, FieldInfo>(fixt, fieldInfo));
            else
                this.propertiesObjects.Add(name, new Tuple<object, PropertyInfo>(fixt, this.TargetType.GetProperty(name)));
            return this;
        }
    }
}
