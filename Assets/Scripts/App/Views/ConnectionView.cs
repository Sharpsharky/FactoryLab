using UnityEngine;
using Zenject;

namespace FactoryLab.App.Views
{
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionView : MonoBehaviour
    {
        public string ConnectionId { get; private set; }

        private LineRenderer _line;
        private PortView     _from;
        private PortView     _to;

        private void Awake() => _line = GetComponent<LineRenderer>();

        public void Initialize(string connectionId, PortView from, PortView to)
        {
            ConnectionId = connectionId;
            _from        = from;
            _to          = to;
        }

        private void Update()
        {
            if (_from == null || _to == null) return;
            _line.SetPosition(0, _from.transform.position);
            _line.SetPosition(1, _to.transform.position);
        }

        public class Pool : MonoMemoryPool<string, PortView, PortView, ConnectionView>
        {
            protected override void Reinitialize(string connectionId, PortView from, PortView to, ConnectionView view)
                => view.Initialize(connectionId, from, to);
        }
    }
}
