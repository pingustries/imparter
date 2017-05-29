namespace Imparter.Cmd
{
    internal class TestCommand : IMessage
    {
        public string Input { get; }

        public TestCommand(string input)
        {
            Input = input;
        }
    }
}