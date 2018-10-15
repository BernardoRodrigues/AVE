namespace Fixtr
{
    using System;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ValidationAttribute : Attribute
    {

        public string Validation
        {
            get; private set;
        }

        public ValidationAttribute(string validation)
        {
            this.Validation = validation;
        }

    }
}
