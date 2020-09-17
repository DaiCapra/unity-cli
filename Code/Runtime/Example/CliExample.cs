using Cli.Code.Runtime.Controller;
using Cli.Code.Runtime.Model;
using Cli.Code.Runtime.View;
using UnityEngine;

namespace Cli.Code.Runtime.Example
{
    public class CliExample : MonoBehaviour
    {
        [SerializeField] private CliView cliView;

        private CliService _cliService;

        public void Awake()
        {
            _cliService = new CliService();
            cliView.Init(_cliService);
            var args = new[] {"bar1", "bar2", "bar3", "bar4"};
            var c1 = "foo1";
            var c2 = "foo2";
            var c3 = "foo3";

            _cliService.RegisterCommand(new CommandInfo {Name = c1, Action = Foo, Args = args});
            _cliService.RegisterCommand(new CommandInfo {Name = c2, Action = Foo});
            _cliService.RegisterCommand(new CommandInfo {Name = c3, Action = Foo, Args = args});
        }

        private void Foo(string[] args)
        {
        }
    }
}