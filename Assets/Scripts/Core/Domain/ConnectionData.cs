using System;

namespace FactoryLab.Core.Domain
{
    public class ConnectionData
    {
        public string Id            { get; }
        public string FromElementId { get; }
        public string ToElementId   { get; }

        public ConnectionData(string fromElementId, string toElementId)
            : this(Guid.NewGuid().ToString(), fromElementId, toElementId) { }

        public ConnectionData(string id, string fromElementId, string toElementId)
        {
            Id            = id;
            FromElementId = fromElementId;
            ToElementId   = toElementId;
        }
    }
}
