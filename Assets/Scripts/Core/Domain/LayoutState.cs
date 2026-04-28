using System.Collections.Generic;
using System.Linq;

namespace FactoryLab.Core.Domain
{
    public class LayoutState
    {
        private readonly List<PlacedElement>  _elements    = new();
        private readonly List<ConnectionData> _connections = new();

        public IReadOnlyList<PlacedElement>  Elements    => _elements;
        public IReadOnlyList<ConnectionData> Connections => _connections;

        public void AddElement(PlacedElement element) => _elements.Add(element);

        public bool RemoveElement(string id)
        {
            var element = GetElement(id);
            if (element == null) return false;
            _connections.RemoveAll(c => c.FromElementId == id || c.ToElementId == id);
            return _elements.Remove(element);
        }

        public void AddConnection(ConnectionData connection) => _connections.Add(connection);

        public bool RemoveConnection(string id)
        {
            var connection = _connections.FirstOrDefault(c => c.Id == id);
            return connection != null && _connections.Remove(connection);
        }

        public PlacedElement GetElement(string id) =>
            _elements.FirstOrDefault(e => e.Id == id);

        public IEnumerable<ConnectionData> GetConnectionsFrom(string elementId) =>
            _connections.Where(c => c.FromElementId == elementId);

        public IEnumerable<ConnectionData> GetConnectionsTo(string elementId) =>
            _connections.Where(c => c.ToElementId == elementId);

        public bool HasConnectionBetween(string fromId, string toId) =>
            _connections.Any(c => c.FromElementId == fromId && c.ToElementId == toId);

        public void Clear()
        {
            _elements.Clear();
            _connections.Clear();
        }
    }
}
