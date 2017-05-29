namespace Imparter.Cmd
{
    internal class TestCommand : ICommand
    {
        public string Input { get; }

        public TestCommand(string input)
        {
            Input = input;
        }
    }
}