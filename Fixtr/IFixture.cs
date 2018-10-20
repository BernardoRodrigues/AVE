namespace Fixtr
{
    using System;

    public interface IFixture
    {
        Type TargetType { get; }
        object New();
        object[] Fill(int size);

    }
}
