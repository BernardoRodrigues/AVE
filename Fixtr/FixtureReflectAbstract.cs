namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class FixtureReflectAbstract
    {
        protected readonly Random random;
        protected Type TargetType
        {
            get; private set;
        }

        public FixtureReflectAbstract(Type type)
        {
            this.random = new Random();
            this.TargetType = type;
        }

        /// <summary>
        /// Gets value for members of the TargetType.
        /// It checks if the member has the ValidationAttribute and if it does it tries to check if the value generated complies with the method given to the ValidationAttribute. Until this doesn't happen it keeps creating random values.
        /// If the member doesn't have the ValidationAttribute it creates a random value and returns it.
        /// </summary>
        /// <param name="info">Member to be affected by the value returned from the getter</param>
        /// <param name="obj">Object created with the same type as TargetType</param>
        /// <param name="getter">Function to fetch a value (it maybe random or not)</param>
        /// <returns></returns>
        protected object GetValue(MemberInfo info, object obj, Func<object> getter)
        {
            var value = getter.Invoke();
            if (info.GetCustomAttribute(typeof(ValidationAttribute)) is ValidationAttribute attribute)
            {
                var methodInfo = this.TargetType.GetMethod(attribute.Validation);
                if (methodInfo != null)
                {
                    var result = (bool)methodInfo.Invoke(obj, new object[] { value });
                    while (!result)
                    {
                        value = getter.Invoke();
                        result = (bool)methodInfo.Invoke(obj, new object[] { value });
                    }
                }
                return value;
            }
            return value;
        }

        /// <summary>
        /// Goes through a dictionary and calls an action for each key of the dictionary.
        /// This method is used for the sake of not having similar code
        /// </summary>
        /// <typeparam name="T">Can either be Tuple, FieldInfo or PropertyInfo</typeparam>
        /// <param name="dictionary">Collection with names of fields or properties and either Tuples, FieldInfos or PropertyInfos</param>
        /// <param name="action">Action that is called for each key of the Dictionary</param>
        protected void ParseDictionaryAndInvokeAction<T>(Dictionary<string, T> dictionary, Action<T> action)
        {
            foreach (var key in dictionary.Keys)
            {
                action.Invoke(dictionary[key]);
            }
        }

        /// <summary>
        /// Helper method for generating random strings
        /// </summary>
        /// <returns></returns>
        private string RandomString()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 50)
              .Select(s => s[this.random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Method used for getting values based on their type.
        /// If the value is a primitive type this method calls a helper method for a random primitive value generation.
        /// If the value is an array this method instantiates a new FixtureReflect with the Element Type of the array and calls the method Fill with a random value between 1 and 20 to populate the array.
        /// If the value is not either of the previous examples it means it's either a struct or an object (ReferenceType) so it creates a new FixtureReflect and with the type of the object and calls the method New to create the object.
        /// </summary>
        /// <param name="parameterType">Type of the parameter that's gonna be created</param>
        /// <returns>New value for either the constructor, Field or Property of the TargetType</returns>
        public object GetRandomValue(Type parameterType)
        {
            if (parameterType.IsValueType && parameterType.IsPrimitive)
                return this.GetPrimitiveValue(parameterType);
            if (parameterType.Equals(typeof(string)))
                return RandomString();
            if (parameterType.IsArray)
                return new FixtureReflect(parameterType.GetElementType()).Fill(this.random.Next(1, 20));
            else
                return new FixtureReflect(parameterType).New();
        }

        /// <summary>
        /// Helper method used to create primitive types
        /// </summary>
        /// <param name="type">Type of the primitive type</param>
        /// <returns>Object that is a primitive type</returns>
        private object GetPrimitiveValue(Type type)
        {
            if (type.Equals(typeof(int)) || type.Equals(typeof(long)) || type.Equals(typeof(float)) || type.Equals(typeof(decimal)))
                return this.random.Next(int.MinValue, int.MaxValue);
            if (type.Equals(typeof(uint)) || type.Equals(typeof(ulong)))
                return this.random.Next((int)uint.MinValue, int.MaxValue);
            if (type.Equals(typeof(char)))
                return this.random.Next(char.MinValue, char.MaxValue);
            if (type.Equals(typeof(double)))
                return this.random.NextDouble();
            if (type.Equals(typeof(byte)))
                return this.random.Next(byte.MinValue, byte.MaxValue);
            if (type.Equals(typeof(short)))
                return this.random.Next(short.MinValue, short.MaxValue);
            if (type.Equals(typeof(ushort)))
                return this.random.Next(ushort.MinValue, ushort.MaxValue);
            if (type.Equals(typeof(sbyte)))
                return this.random.Next(sbyte.MinValue, sbyte.MaxValue);
            if (type.Equals(typeof(bool)))
                return this.random.Next(0, 1);
            return null;
        }
        
        /// <summary>
        /// Method used to set the values of the TargetType.
        /// This method is implemented in classes that extend from this class.
        /// </summary>
        /// <param name="obj">Object previously created. Same type as TargetType</param>
        public abstract void SetValues(object obj);

        /// <summary>
        /// Method used to add members to the enumerables of child classes.
        /// </summary>
        /// <param name="key">Name of Field or Info of the Target Type</param>
        /// <param name="fixture">Fixture used to create the value</param>
        /// <param name="member">FieldInfo or PropertyInfo of the TargetType with the name -> key</param>
        public abstract void Add(string key, IFixture fixture, MemberInfo member);

        /// <summary>
        /// Method used to add members to the enumerables of child classes.
        /// </summary>
        /// <param name="key">Name of Field or Info of the Target Type</param>
        /// <param name="values">Array of values used to randomly add a value</param>
        /// <param name="member">FieldInfo or PropertyInfo of the TargetType with the name -> key</param>
        public abstract void Add(string key, object[] values, MemberInfo member);

        /// <summary>
        /// Method used to add members to the enumerables of child classes.
        /// </summary>
        /// <param name="key">Name of Field or Info of the Target Type</param>
        /// <param name="member">FieldInfo or PropertyInfo of the TargetType with the name -> key</param>
        public abstract void Add(string key, MemberInfo member);
    }
}
