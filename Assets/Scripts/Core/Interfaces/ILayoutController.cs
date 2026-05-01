using System;
using UnityEngine;
using FactoryLab.Core.Domain;
using FactoryLab.Core.Validation;

namespace FactoryLab.Core.Interfaces
{
    public interface ILayoutController
    {
        EvaluationMode EvaluationMode { get; set; }

        void             RemoveElement (string elementId);
        ValidationResult ValidateLayout();

        event Action<PlacedElement, Vector2> OnContextMenuRequested;
        event Action<ValidationResult>       OnValidationCompleted;
    }
}
