using System.Linq;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.App.Views;

namespace FactoryLab.App.Controllers
{
    public class PortConnectionController : ITickable
    {
        private readonly Camera             _camera;
        private readonly TableController    _table;
        private readonly DragDropController _dragDrop;

        private PortView _pendingOutput;

        [Inject]
        public PortConnectionController(Camera camera, TableController table, DragDropController dragDrop)
        {
            _camera   = camera;
            _table    = table;
            _dragDrop = dragDrop;
        }

        public void Tick()
        {
            if (_dragDrop.IsDragging) return;
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f)) return;

            var port = hit.collider.GetComponent<PortView>();
            if (port == null) { CancelPending(); return; }

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
            else if (port.Direction == PortType.Input && _pendingOutput != null)
            {
                TryConnect(_pendingOutput, port);
                CancelPending();
            }
        }

        private void TryConnect(PortView output, PortView input)
        {
            if (output.Direction != PortType.Output) return;
            if (input.Direction  != PortType.Input)  return;
            if (output.Owner == input.Owner) return;

            var state  = _table.LayoutState;
            var fromId = output.Owner.Data.Id;
            var toId   = input.Owner.Data.Id;

            if (state.HasConnectionBetween(fromId, toId)) return;

            var outputCapacity = output.Owner.Data.Definition.GetOutputPorts().Count();
            if (state.GetConnectionsFrom(fromId).Count() >= outputCapacity) return;

            var inputCapacity = input.Owner.Data.Definition.GetInputPorts().Count();
            if (state.GetConnectionsTo(toId).Count() >= inputCapacity) return;

            _table.AddConnection(fromId, toId, output, input);
        }

        private void CancelPending()
        {
            _pendingOutput?.SetSelected(false);
            _pendingOutput = null;
        }
    }
}
