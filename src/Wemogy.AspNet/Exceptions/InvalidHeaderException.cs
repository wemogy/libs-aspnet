using System;

namespace Wemogy.AspNet.Exceptions
{
    public class InvalidHeaderException : Exception
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public InvalidHeaderException(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
