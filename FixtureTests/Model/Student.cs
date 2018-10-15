namespace FixtureTests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Student
    {

        public int Nr
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public School School
        {
            get; set;
        }
        public Address Addr
        {
            get; set;
        }
        public string naturality;

    }
}
