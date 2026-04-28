using System.Text;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Validation;

namespace FactoryLab.App.Controllers
{
    public class DebugSpawner : MonoBehaviour
    {
        private ElementLibrarySO   _library;
        private TableController    _table;
        private DragDropController _dragDrop;
        private ValidationService  _validation;
        private Camera             _camera;

        [Inject]
        public void Construct(ElementLibrarySO library, TableController table,
                              DragDropController dragDrop, ValidationService validation,
                              Camera camera)
        {
            _library    = library;
            _table      = table;
            _dragDrop   = dragDrop;
            _validation = validation;
            _camera     = camera;
        }

        private void Update()
        {
            for (int i = 0; i < _library.elements.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    Spawn(i);
            }

            if (Input.GetKeyDown(KeyCode.V))
                RunValidation();
        }

        private void Spawn(int index)
        {
            var pos = _camera.transform.position + _camera.transform.forward * 5f;
            pos.y = 0f;

            var view = _table.SpawnAndGet(_library.elements[index], pos);
            _dragDrop.BeginDrag(view);
        }

        private void RunValidation()
        {
            var result = _validation.Validate(_table.LayoutState);
            var sb = new StringBuilder();

            sb.AppendLine(result.IsValid
                ? "ALID LAYOUT"
                : $"issues: {result.Issues.Count}");

            foreach (var issue in result.Issues)
            {
                var prefix = issue.Type == ValidationIssueType.Error ? "Error" : "Warn ";
                sb.AppendLine($"  {prefix} {issue.Description}");
            }

            if (result.IsValid)
                Debug.Log(sb.ToString());
            else if (result.HasErrors)
                Debug.LogError(sb.ToString());
            else
                Debug.LogWarning(sb.ToString());
        }
    }
}
