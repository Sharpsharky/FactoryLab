using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using FactoryLab.Core.Validation;
using ILayoutController = FactoryLab.Core.Interfaces.ILayoutController;

namespace FactoryLab.Ui.Views
{
    public class ValidationPanelView : MonoBehaviour
    {
        private const string MessageValid = "Layout is valid";

        private static readonly Color ErrorColor   = new (1f, 0.35f, 0.35f);
        private static readonly Color WarningColor = new (1f, 0.85f, 0.35f);

        [SerializeField] private TMP_Text  _summaryText;
        [SerializeField] private Transform _issueContainer;
        [SerializeField] private TMP_Text  _issuePrefab;

        private ILayoutController       _layoutController;
        private readonly List<TMP_Text> _issueTexts = new();

        [Inject]
        public void Construct(ILayoutController layoutController)
        {
            _layoutController = layoutController;
        }

        private void Start()
        {
            _layoutController.OnValidationCompleted += Show;
        }

        private void OnDestroy()
        {
            _layoutController.OnValidationCompleted -= Show;
        }

        private void Show(ValidationResult result)
        {
            foreach (var t in _issueTexts)
                Destroy(t.gameObject);
            _issueTexts.Clear();

            _summaryText.text = result.IsValid
                ? MessageValid
                : $"{result.Issues.Count} issue(s) found";

            foreach (var issue in result.Issues)
            {
                var label = Instantiate(_issuePrefab, _issueContainer);
                label.text  = $"[{issue.Type}] {issue.Description}";
                label.color = issue.Type == ValidationIssueType.Error
                    ? ErrorColor
                    : WarningColor;
                _issueTexts.Add(label);
            }
        }
    }
}
