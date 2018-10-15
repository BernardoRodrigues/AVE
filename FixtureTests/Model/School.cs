namespace FixtureTests.Model
{
    public class School
    {

        public string Name
        {
            get; set;
        }

        public int something;

        public School(string name, int something)
        {
            this.Name = name;
            this.something = something;
        }

    }
}
