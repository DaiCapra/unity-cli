using System;

namespace Cli.Code.Runtime.Model
{
    public struct CommandInfo: IEquatable<CommandInfo>
    {
        public string Name;
        public Action<string[]> Action;
        public string[] Args;

        public bool Equals(CommandInfo other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is CommandInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}