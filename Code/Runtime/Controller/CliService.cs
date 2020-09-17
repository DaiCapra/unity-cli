using System;
using System.Collections.Generic;
using System.Linq;
using Cli.Code.Runtime.Model;
using UnityEngine;

namespace Cli.Code.Runtime.Controller
{
    public class CliService
    {
        public delegate void MessageHandler(Message message);

        public MessageHandler DelMessageAdded;
        public MessageHandler DelMessagesCleared;

        private readonly Dictionary<string, CommandInfo> _commands;
        private readonly TextService _textService;

        private MessageCollection _messageCollection;

        public CliService()
        {
            _messageCollection = new MessageCollection();

            _commands = new Dictionary<string, CommandInfo>(StringComparer.CurrentCultureIgnoreCase);
            _textService = new TextService(_commands);

            RegisterCommand(new CommandInfo
            {
                Name = "Echo",
                Action = Echo,
            });
            RegisterCommand(new CommandInfo {Name = "clear", Action = s => Clear()});
        }


        public void RegisterCommand(CommandInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                Debug.LogError("Cannot add invalid command!");
                return;
            }

            if (_commands.ContainsKey(info.Name))
            {
                Debug.LogError("Cannot add duplicate command!");
                return;
            }

            _commands[info.Name] = info;
        }

        private void Echo(string[] args)
        {
            if (!HasArguments(args))
            {
                return;
            }

            var s = ConcatenateArguments(args);
            Echo(s);
        }

        public void Echo(string text)
        {
            AddMessage(new Message
            {
                Text = text
            });
        }

        private static string ConcatenateArguments(string[] args)
        {
            var s = string.Join(" ", args);
            return s;
        }

        public bool Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var tokens = TextService.GetTokens(input);
            if (tokens == null || tokens.Length <= 0)
            {
                return false;
            }

            // Convert text into command and arguments
            string command = tokens[0];
            var t = tokens.ToList();
            t.RemoveAt(0);
            var args = t.Count > 0 ? t.ToArray() : null;
            Execute(command, args);

            return true;
        }

        private bool Execute(string command, string[] args = null)
        {
            if (!_commands.ContainsKey(command))
            {
                return false;
            }

            var userCommand = _commands[command];
            userCommand.Action?.Invoke(args);
            return true;
        }

        private static bool HasArguments(string[] args)
        {
            return args != null && args.Length >= 1;
        }

        public List<string> Suggest(string text)
        {
            return _textService.Suggest(text);
        }

        public void AddMessage(Message message)
        {
            if (message == null)
            {
                return;
            }

            _messageCollection.Messages.Add(message);
            DelMessageAdded?.Invoke(message);
        }

        public void Clear()
        {
            _messageCollection.Messages.Clear();
            DelMessagesCleared?.Invoke(null);
        }

        public List<Message> GetHistory()
        {
            return _messageCollection.Messages
                .Where(t => !t.Outbound)
                .ToList();
        }
    }
}