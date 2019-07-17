namespace System.Diagnostics.Contracts
{
#if NET2
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class)]

    internal class PureAttribute : Attribute
    {
        public PureAttribute() { }
    }

#endif
}