namespace System.Diagnostics.CodeAnalysis
{
#if CoreFx

    internal class SuppressMessageAttribute : Attribute
    {
        public SuppressMessageAttribute(string a, string b) { }
    }

#endif

}