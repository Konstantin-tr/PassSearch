using System;

namespace PasswordQueryTool.Backend.QueueContracts
{
    /// <summary>
    /// Container for a parsed line - holding all information of one line.
    /// </summary>
    public class PasswordCombination
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}