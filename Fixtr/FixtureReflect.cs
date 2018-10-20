namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class FixtureReflect : IFixture
    {
        public Type TargetType
        {
            get; private set;
        }
        private readonly ConstructorInfo constructorInfo;
        private readonly ParameterInfo[] parameterInfos;
        private readonly FixtureReflectAbstract fields;
        private readonly FixtureReflectAbstract properties;
        
        public FixtureReflect(Type type)
        {
            this.TargetType = type;
            this.constructorInfo = this.TargetType.GetConstructors().GetValue(0) as ConstructorInfo;
            if (this.constructorInfo != null)
                this.parameterInfos = this.constructorInfo.GetParameters();
            this.fields = new FieldFixtureReflect(this.TargetType);
            this.properties = new PropertyFixtureReflect(this.TargetType);
        }
        
        public object New()
        {
            var obj = GetObject();
            this.fields.SetValues(obj);
            this.properties.SetValues(obj);
            return obj;
        }

        /// <summary>
        /// Creates object from with the same type as TargetType.
        /// Checks if the object as a contructor and if it doesn't it uses the default constructor.
        /// </summary>
        /// <returns>object from with the same type as TargetType</returns>
        private object GetObject()
        {
            if (this.constructorInfo == null)
            {
                return Activator.CreateInstance(this.TargetType);
            }
            var parameters = new List<object>();
            foreach (var parameterInfo in this.parameterInfos)
            {
                //works for now change later. Perguntar ao engenheiro
                parameters.Add(this.fields.GetRandomValue(parameterInfo.ParameterType));
            }
            return this.constructorInfo.Invoke(parameters.ToArray());
        }

        /// <summary>
        /// Creates an array of objects with the same type as TargetType using the method New
        /// </summary>
        /// <param name="size">size of the array</param>
        /// <returns>array of objects</returns>
        public object[] Fill(int size)
        {
            var objects = new object[size];
            for (var i = 0 ; i < size ; i++)
                objects[i] = New();
            return objects;
        }

        IFixture Member(string name)
        {
            var field = this.TargetType.GetField(name);
            var propertyInfo = this.TargetType.GetProperty(name);
            if (field != null)
                this.fields.Add(name, this.TargetType);
            else if (propertyInfo != null)
                this.properties.Add(name, propertyInfo);
            else
                throw new ArgumentException("No field or property exists with the given name");
            return this;
        }

        public IFixture Member(string name, params object[] parameters)
        {
            var fieldInfo = this.TargetType.GetField(name);
            var propertyInfo = this.TargetType.GetProperty(name);
            if (fieldInfo != null)
                this.fields.Add(name, parameters, fieldInfo);
            else if (propertyInfo != null)
                this.properties.Add(name, parameters, propertyInfo);
            else
                throw new ArgumentException("No field or property exists with the given name");
            return this;
        }

        public IFixture Member(string name, IFixture fixt)
        {
            var fieldInfo = this.TargetType.GetField(name);
            var propertyInfo = this.TargetType.GetProperty(name);
            if (fieldInfo != null)
                this.fields.Add(name, fixt, fieldInfo);
            else if (propertyInfo != null)
                this.properties.Add(name, fixt, this.TargetType.GetProperty(name));
            else
                throw new ArgumentException("No field or property exists with the given name");
            return this;
        }
    }
}
