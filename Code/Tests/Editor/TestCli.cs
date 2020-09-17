using Cli.Code.Runtime.Controller;
using Cli.Code.Runtime.Model;
using NUnit.Framework;

namespace Cli.Code.Tests.Editor
{
    public class TestCli
    {
        private bool _test;

        [Test]
        public void RegisterAndExecute()
        {
            var cli = new CliService();
            cli.RegisterCommand(new CommandInfo()
            {
                Name = "Foo",
                Action = Bar,
            });

            _test = false;
            cli.Execute("");
            cli.Execute(null);
            Assert.False(_test);
            
            cli.Execute("Foo");
            Assert.True(_test);
        }

        [Test]
        public void SuggestTokens()
        {
            var cli = new CliService();
            cli.RegisterCommand(new CommandInfo() {Name = "foo"});
            cli.RegisterCommand(new CommandInfo() {Name = "bar"});
            cli.RegisterCommand(new CommandInfo() {Name = "elephant"});

            string s1 = "e";
            Assert.True(cli.Suggest(s1).Contains("echo"));
            Assert.True(cli.Suggest(s1).Contains("elephant"));
            Assert.False(cli.Suggest(s1).Contains("foo"));

            string s2 = "foo bar e";
            Assert.True(cli.Suggest(s2).Contains("echo"));
            Assert.True(cli.Suggest(s2).Contains("elephant"));
            Assert.False(cli.Suggest(s2).Contains("foo"));
        }

        private void Bar(string[] obj)
        {
            _test = true;
        }
    }
}