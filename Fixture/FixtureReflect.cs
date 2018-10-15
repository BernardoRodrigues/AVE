namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class FixtureReflect : IFixture
    {
        public Type TargetType
        {
            get; private set;
        }
        private readonly ConstructorInfo constructorInfo;
        private readonly ParameterInfo[] parameterInfos;
        
        
        public FixtureReflect(Type type)
        {
            this.TargetType = type;
            this.constructorInfo = this.TargetType.GetConstructors().GetValue(0) as ConstructorInfo;
            if (this.constructorInfo != null)
                this.parameterInfos = this.constructorInfo.GetParameters();
        }

        public object New()
        {
            var obj = GetObject();
            SetFields(obj);
            SetProperties(obj);
            return obj;
        }

        private void SetProperties(object obj)
        {
            if (this.properties.Count != 0)
            {
                var keys = this.properties.Keys;
                foreach (var key in keys)
                {
                    var property = this.properties[key];
                    if (property.GetCustomAttribute(typeof(ValidationAttribute)) is ValidationAttribute attribute)
                    {
                        var methodInfo = this.TargetType.GetMethod(attribute.Validation);
                        var value = GetRandomValue(property.PropertyType);
                        var result = (bool)methodInfo.Invoke(obj, new object[] { value });
                        while (!result)
                        {
                            value = GetRandomValue(property.PropertyType);
                            result = (bool)methodInfo.Invoke(obj, new object[] { value });
                        }
                        property.SetValue(obj, value);
                    }
                    property.SetValue(obj, GetRandomValue(property.PropertyType));
                }
            }
        }

        private void SetFields(object obj)
        {
            if (this.fields.Count != 0)
            {
                var keys = this.fields.Keys;
                foreach (var key in keys)
                {
                    var field = this.fields[key];
                    if (field.GetCustomAttribute(typeof(ValidationAttribute)) is ValidationAttribute attribute)
                    {
                        var methodInfo = this.TargetType.GetMethod(attribute.Validation);
                        var value = GetRandomValue(field.FieldType);
                        var result = (bool)methodInfo.Invoke(obj, new object[] { value });
                        while (!result)
                        {
                            value = GetRandomValue(field.FieldType);
                            result = (bool)methodInfo.Invoke(obj, new object[] { value });
                        }
                        field.SetValue(obj, value);
                    }
                }
            }
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
