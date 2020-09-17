using System.Collections.Generic;

namespace Cli.Code.Runtime.Model
{
    public class MessageCollection
    {
        public List<Message> Messages { get; set; }
        public List<CommandInfo> CommandHistory { get; set; }

        public MessageCollection()
        {
            Messages = new List<Message>();
            CommandHistory = new List<CommandInfo>();
        }
    }
}