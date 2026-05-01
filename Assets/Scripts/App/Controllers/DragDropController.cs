using System;
using UnityEngine;
using Zenject;
using FactoryLab.App.Views;

namespace FactoryLab.App.Controllers
{
    public class DragDropController : ITickable, IInitializable, IDisposable
    {
        private readonly Camera          _camera;
        private readonly TableController _table;

        private PlacedElementView _dragging;
        private float             _tableY;

        public bool IsDragging => _dragging != null;

        [Inject]
        public DragDropController(Camera camera, TableController table)
        {
            _camera = camera;
            _table  = table;
            _tableY = 0f;
        }

        public void Initialize() => _table.OnElementSpawned += BeginDrag;
        public void Dispose()    => _table.OnElementSpawned -= BeginDrag;

        public void BeginDrag(PlacedElementView view) => _dragging = view;

        public void Tick()
        {
            if (IsDragging)
            {
                UpdateDraggedPosition();
                if (Input.GetMouseButtonUp(0)) Drop();
            }
            else
            {
                CheckPickup();
                CheckRightClick();
            }
        }

        private void CheckPickup()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f)) return;
            if (hit.collider.GetComponent<PortView>() != null) return;

            var view = hit.collider.GetComponentInParent<PlacedElementView>();
            if (view != null) _dragging = view;
        }

        private void CheckRightClick()
        {
            if (!Input.GetMouseButtonDown(1)) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f)) return;
            if (hit.collider.GetComponent<PortView>() != null) return;

            var view = hit.collider.GetComponentInParent<PlacedElementView>();
            if (view != null) _table.RequestContextMenu(view.Data, Input.mousePosition);
        }

        private void UpdateDraggedPosition()
        {
            var pos = HitTable();
            if (pos.HasValue)
                _dragging.transform.position = pos.Value + Vector3.up * 0.05f;
        }

        private void Drop()
        {
            var pos = HitTable();
            if (pos.HasValue)
            {
                _dragging.Data.Position      = pos.Value;
                _dragging.transform.position = pos.Value;
            }
            _dragging = null;
        }

        private Vector3? HitTable()
        {
            var ray   = _camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.up * _tableY);
            return plane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : (Vector3?)null;
        }
    }
}
