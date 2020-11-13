using System;

namespace Personal_Keyboard_Mapper.Lib.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException()
        {
            
        }

        public ConfigurationException(string msg) : base(msg)
        { 
        }

        public ConfigurationException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}