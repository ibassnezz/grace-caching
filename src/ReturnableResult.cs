using System;

namespace GraceCaching
{
    public class ReturnableResult
    {
        public Guid Guid { get; }
        public DateTime DateTime { get; }

        public ReturnableResult(Guid guid, DateTime dateTime)
        {
            Guid = guid;
            DateTime = dateTime;
        }
    }
}
