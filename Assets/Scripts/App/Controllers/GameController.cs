using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using FactoryLab.Core;
using FactoryLab.Core.Domain;
using FactoryLab.Core.Interfaces;
using FactoryLab.Core.Validation;
using FactoryLab.App;

namespace FactoryLab.App.Controllers
{
    public class GameController : ILayoutController, IInitializable, IDisposable
    {
        private readonly TableController   _table;
        private readonly ValidationService _validation;
        private readonly LayoutState       _layoutState;

        private EvaluationMode _evaluationMode;

        public EvaluationMode EvaluationMode
        {
            get => _evaluationMode;
            set => _evaluationMode = value;
        }

        public event Action<PlacedElement, Vector2> OnContextMenuRequested;
        public event Action<ValidationResult>       OnValidationCompleted;

        [Inject]
        public GameController(TableController table, ValidationService validation,
                              LayoutState layoutState, EvaluationMode evaluationMode)
        {
            _table          = table;
            _validation     = validation;
            _layoutState    = layoutState;
            _evaluationMode = evaluationMode;
        }

        public void Initialize()
        {
            _table.OnContextMenuRequested += HandleContextMenu;
            _table.OnLayoutChanged        += HandleLayoutChanged;
        }

        public void Dispose()
        {
            _table.OnContextMenuRequested -= HandleContextMenu;
            _table.OnLayoutChanged        -= HandleLayoutChanged;
        }

        public void RemoveElement(string elementId) => _table.RemoveElement(elementId);

        public ValidationResult ValidateLayout()
        {
            var result = _validation.Validate(_layoutState);
            ApplyHighlights(result);
            OnValidationCompleted?.Invoke(result);
            return result;
        }

        private void HandleContextMenu(PlacedElement element, Vector2 screenPos) =>
            OnContextMenuRequested?.Invoke(element, screenPos);

        private void HandleLayoutChanged()
        {
            if (_evaluationMode == EvaluationMode.Learning)
                ValidateLayout();
        }

        private void ApplyHighlights(ValidationResult result)
        {
            if (result.IsValid)
            {
                _table.ClearHighlights();
                return;
            }

            var highlights = new Dictionary<string, HighlightState>();
            foreach (var issue in result.Issues)
            {
                if (issue.ElementId == null) continue;

                var newState = issue.Type == ValidationIssueType.Error
                    ? HighlightState.Error
                    : HighlightState.Warning;

                if (!highlights.TryGetValue(issue.ElementId, out var current) ||
                    current != HighlightState.Error)
                    highlights[issue.ElementId] = newState;
            }

            _table.ApplyHighlights(highlights);
        }
    }
}
