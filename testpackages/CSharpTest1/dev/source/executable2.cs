namespace SimpleAssembly
{
    public class Program2
    {
        public static int Main(params string[] args)
        {
            Test.Foo foo = new Test.Foo();
            foo.Print();

            return 0;
        }
    }
}