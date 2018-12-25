using System;

namespace Common.Storage
{
    public class IdentifiableEntity : Entity
    {
        public string Id { get; protected set; }

        protected IdentifiableEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}