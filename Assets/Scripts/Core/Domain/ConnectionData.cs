using System;

namespace FactoryLab.Core.Domain
{
    public class ConnectionData
    {
        public string Id { get; }
        public string FromElementId { get; }
        public string FromPortName { get; }
        public string ToElementId { get; }
        public string ToPortName { get; }

        public ConnectionData(string fromElementId, string fromPortName, string toElementId, string toPortName)
            : this(Guid.NewGuid().ToString(), fromElementId, fromPortName, toElementId, toPortName) { }

        public ConnectionData(string id, string fromElementId, string fromPortName, string toElementId, string toPortName)
        {
            Id = id;
            FromElementId = fromElementId;
            FromPortName = fromPortName;
            ToElementId = toElementId;
            ToPortName = toPortName;
        }
    }
}
