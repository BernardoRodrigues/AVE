namespace Fixtr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class FixtureReflectAbstract
    {
        private readonly Random random;
        protected Type TargetType
        {
            get; private set;
        }

        public FixtureReflectAbstract(Type type)
        {
            this.random = new Random();
            this.TargetType = type;
        }

        protected object GetValue(MemberInfo info, Type type, object obj)
        {
            var value = GetRandomValue(type);
            if (info.GetCustomAttribute(typeof(ValidationAttribute)) is ValidationAttribute attribute)
            {

                var methodInfo = this.TargetType.GetMethod(attribute.Validation);

                if (methodInfo != null)
                {
                    var result = (bool)methodInfo.Invoke(obj, new object[] { value });
                    while (!result)
                    {
                        value = GetRandomValue(type);
                        result = (bool)methodInfo.Invoke(obj, new object[] { value });
                    }
                }
                return value;
            }
            return value;
        }

        protected string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 50)
              .Select(s => s[this.random.Next(s.Length)]).ToArray());
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

        public void Set(Dictionary<string, MemberInfo> members, object obj)
        {
            foreach (var key in members.Keys)
            {
                var member = members[key];
                var value = GetValue(member, GetMemberType(member), obj);
            }
        }

        private Type GetMemberType(MemberInfo member) => member.MemberType == MemberTypes.Property ? (member as PropertyInfo).PropertyType : (member as FieldInfo).FieldType;
    }
}
