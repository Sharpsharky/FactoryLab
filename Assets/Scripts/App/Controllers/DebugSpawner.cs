using System.Text;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Interfaces;
using FactoryLab.Core.Validation;

namespace FactoryLab.App.Controllers
{
    public class DebugSpawner : MonoBehaviour
    {
        private ElementLibrarySO  _library;
        private IElementSpawner   _spawner;
        private ILayoutController _layoutController;

        [Inject]
        public void Construct(ElementLibrarySO library, IElementSpawner spawner,
                              ILayoutController layoutController)
        {
            _library          = library;
            _spawner          = spawner;
            _layoutController = layoutController;
        }

        private void Update()
        {
            for (int i = 0; i < _library.elements.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    _spawner.SpawnElement(_library.elements[i]);
            }

            if (Input.GetKeyDown(KeyCode.V))
                RunValidation();
        }

        private void RunValidation()
        {
            var result = _layoutController.ValidateLayout();
            var sb = new StringBuilder();

            sb.AppendLine(result.IsValid
                ? "VALID LAYOUT"
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
