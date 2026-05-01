using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using FactoryLab.Core.Validation;
using ILayoutController = FactoryLab.Core.Interfaces.ILayoutController;

namespace FactoryLab.Ui.Views
{
    public class ValidationPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject  _panel;
        [SerializeField] private TMP_Text    _summaryText;
        [SerializeField] private Transform   _issueContainer;
        [SerializeField] private TMP_Text    _issuePrefab;
        [SerializeField] private Button      _closeButton;

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
            _closeButton.onClick.AddListener(() => _panel.SetActive(false));
            _panel.SetActive(false);
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
                ? "Layout is valid"
                : $"{result.Issues.Count} issue(s) found";

            foreach (var issue in result.Issues)
            {
                var label = Instantiate(_issuePrefab, _issueContainer);
                label.text  = $"[{issue.Type}] {issue.Description}";
                label.color = issue.Type == ValidationIssueType.Error
                    ? new Color(1f, 0.35f, 0.35f)
                    : new Color(1f, 0.85f, 0.35f);
                _issueTexts.Add(label);
            }

            _panel.SetActive(true);
        }
    }
}
