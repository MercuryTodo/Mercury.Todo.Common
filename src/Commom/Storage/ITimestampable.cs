using System;

namespace Common.Storage
{
    public interface ITimestampable
    {
        DateTime StorageDate { get; }
    }
}