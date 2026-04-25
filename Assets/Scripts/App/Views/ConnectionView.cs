using UnityEngine;

namespace FactoryLab.App.Views
{
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionView : MonoBehaviour
    {
        public string ConnectionId { get; private set; }

        private LineRenderer _line;
        private PortView _from;
        private PortView _to;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = 2;
            _line.startWidth    = 0.05f;
            _line.endWidth      = 0.05f;
            _line.useWorldSpace = true;

            var shader = Shader.Find("Universal Render Pipeline/Unlit")
                      ?? Shader.Find("Sprites/Default");
            _line.material             = new Material(shader);
            _line.material.color       = Color.white;
        }

        public void Initialize(string connectionId, PortView from, PortView to)
        {
            ConnectionId = connectionId;
            _from = from;
            _to   = to;
        }

        private void Update()
        {
            if (_from == null || _to == null) return;
            _line.SetPosition(0, _from.transform.position);
            _line.SetPosition(1, _to.transform.position);
        }
    }
}
