using System;
using UnityEngine;
using Zenject;
using FactoryLab.App.Views;

namespace FactoryLab.App.Controllers
{
    public class DragDropController : ITickable, IInitializable, IDisposable
    {
        private const int   MouseButtonLeft   = 0;
        private const int   MouseButtonRight  = 1;
        private const float RaycastDistance   = 100f;
        private const float DragHoverHeight   = 0.05f;

        private readonly Camera          _camera;
        private readonly TableController _table;
        private readonly Bounds          _tableBounds;

        private PlacedElementView _dragging;
        private Vector3           _dragOffset;
        private readonly float             _tableY;

        public bool IsDragging => _dragging != null;

        [Inject]
        public DragDropController(Camera camera, TableController table, Bounds tableBounds)
        {
            _camera      = camera;
            _table       = table;
            _tableBounds = tableBounds;
            _tableY      = tableBounds.max.y;
        }

        public void Initialize() => _table.OnElementSpawned += BeginDrag;
        public void Dispose()    => _table.OnElementSpawned -= BeginDrag;

        public void Tick()
        {
            if (IsDragging)
            {
                UpdateDraggedPosition();
                if (Input.GetMouseButtonUp(MouseButtonLeft)) Drop();
            }
            else
            {
                CheckPickup();
                CheckRightClick();
            }
        }

        private void BeginDrag(PlacedElementView view)
        {
            _dragging    = view;
            _dragOffset  = Vector3.zero;
        }
        
        private void CheckPickup()
        {
            if (!Input.GetMouseButtonDown(MouseButtonLeft)) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, RaycastDistance)) return;
            if (hit.collider.GetComponent<PortView>() != null) return;

            var view = hit.collider.GetComponentInParent<PlacedElementView>();
            if (view == null) return;

            var tablePos = HitTable();
            _dragOffset  = tablePos.HasValue
                ? new Vector3(
                    view.transform.position.x - tablePos.Value.x,
                    0f,
                    view.transform.position.z - tablePos.Value.z)
                : Vector3.zero;

            _dragging = view;
        }

        private void CheckRightClick()
        {
            if (!Input.GetMouseButtonDown(MouseButtonRight)) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, RaycastDistance)) return;
            if (hit.collider.GetComponent<PortView>() != null) return;

            var view = hit.collider.GetComponentInParent<PlacedElementView>();
            if (view != null) _table.RequestContextMenu(view.Data, Input.mousePosition);
        }

        private void UpdateDraggedPosition()
        {
            var pos = HitTable();
            if (pos.HasValue)
            {
                var clamped = ClampToTable(pos.Value + new Vector3(_dragOffset.x, 0f, _dragOffset.z));
                _dragging.transform.position = new Vector3(clamped.x, pos.Value.y + DragHoverHeight, clamped.z);
            }
        }

        private void Drop()
        {
            var pos = HitTable();
            if (pos.HasValue)
            {
                var final = ClampToTable(pos.Value + new Vector3(_dragOffset.x, 0f, _dragOffset.z));
                final.y = pos.Value.y;

                _dragging.Data.Position      = final;
                _dragging.transform.position = final;
            }
            _dragging   = null;
            _dragOffset = Vector3.zero;
        }

        private Vector3? HitTable()
        {
            var ray   = _camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.up * _tableY);
            if (!plane.Raycast(ray, out float dist)) return null;
            return ray.GetPoint(dist);
        }

        private Vector3 ClampToTable(Vector3 pos)
        {
            var half = _dragging != null
                ? _dragging.Data.Definition.size * 0.5f
                : Vector3.zero;

            pos.x = Mathf.Clamp(pos.x, _tableBounds.min.x + half.x, _tableBounds.max.x - half.x);
            pos.z = Mathf.Clamp(pos.z, _tableBounds.min.z + half.z, _tableBounds.max.z - half.z);
            return pos;
        }
    }
}
