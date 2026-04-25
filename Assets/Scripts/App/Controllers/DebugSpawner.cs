using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;

namespace FactoryLab.App.Controllers
{
    public class DebugSpawner : MonoBehaviour
    {
        private ElementLibrarySO  _library;
        private TableController   _table;
        private DragDropController _dragDrop;
        private Camera            _camera;

        [Inject]
        public void Construct(ElementLibrarySO library, TableController table,
                              DragDropController dragDrop, Camera camera)
        {
            _library  = library;
            _table    = table;
            _dragDrop = dragDrop;
            _camera   = camera;
        }

        private void Update()
        {
            for (int i = 0; i < _library.elements.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    Spawn(i);
            }
        }

        private void Spawn(int index)
        {
            var pos = _camera.transform.position + _camera.transform.forward * 5f;
            pos.y = 0f;

            var view = _table.SpawnAndGet(_library.elements[index], pos);
            _dragDrop.BeginDrag(view);
        }
    }
}
