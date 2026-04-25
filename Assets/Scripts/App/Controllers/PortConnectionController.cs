using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.App.Views;

namespace FactoryLab.App.Controllers
{
    public class PortConnectionController : ITickable
    {
        private readonly Camera _camera;
        private readonly TableController _table;
        private readonly DragDropController _dragDrop;

        private PortView _pendingOutput;

        [Inject]
        public PortConnectionController(Camera camera, TableController table, DragDropController dragDrop)
        {
            _camera  = camera;
            _table   = table;
            _dragDrop = dragDrop;
        }

        public void Tick()
        {
            if (_dragDrop.IsDragging) return;
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f)) return;

            var port = hit.collider.GetComponent<PortView>();

            if (port == null)
            {
                CancelPending();
                return;
            }

            HandlePortClick(port);
        }

        private void HandlePortClick(PortView port)
        {
            if (port.Direction == PortType.Output)
            {
                CancelPending();
                _pendingOutput = port;
                port.SetSelected(true);
            }
            else if (_pendingOutput != null)
            {
                TryConnect(_pendingOutput, port);
                CancelPending();
            }
        }

        private void TryConnect(PortView output, PortView input)
        {
            if (output.Owner == input.Owner) return;

            var state = _table.LayoutState;
            if (state.IsPortConnected(output.Owner.Data.Id, output.PortName)) return;
            if (state.IsPortConnected(input.Owner.Data.Id,  input.PortName))  return;

            _table.AddConnection(
                output.Owner.Data.Id, output.PortName,
                input.Owner.Data.Id,  input.PortName);
        }

        private void CancelPending()
        {
            _pendingOutput?.SetSelected(false);
            _pendingOutput = null;
        }
    }
}
