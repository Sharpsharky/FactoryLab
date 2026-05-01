using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using FactoryLab.Core;
using FactoryLab.Core.Validation;
using FactoryLab.Core.Interfaces;
using ILayoutController = FactoryLab.Core.Interfaces.ILayoutController;

namespace FactoryLab.Ui.Views
{
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private Button       _validateButton;
        [SerializeField] private TMP_Dropdown _modeDropdown;
        [SerializeField] private Button       _saveButton;
        [SerializeField] private Button       _loadButton;

        private ILayoutController _layoutController;
        private ISaveLoadService  _saveLoad;

        [Inject]
        public void Construct(ILayoutController layoutController, ISaveLoadService saveLoad)
        {
            _layoutController = layoutController;
            _saveLoad         = saveLoad;
        }

        private void Start()
        {
            _validateButton.onClick.AddListener(() => _layoutController.ValidateLayout());
            _modeDropdown.onValueChanged.AddListener(OnModeChanged);
            _saveButton.onClick.AddListener(() => _saveLoad.Save());
            _loadButton.onClick.AddListener(() => _saveLoad.Load());
            _layoutController.OnValidationCompleted += OnValidationCompleted;

            RefreshMode();
        }

        private void OnDestroy()
        {
            _layoutController.OnValidationCompleted -= OnValidationCompleted;
        }

        private void OnModeChanged(int index)
        {
            _layoutController.EvaluationMode = (EvaluationMode)index;
            RefreshMode();
        }

        private void OnValidationCompleted(ValidationResult result) { }

        private void RefreshMode()
        {
            _modeDropdown.value = (int)_layoutController.EvaluationMode;
            _validateButton.gameObject.SetActive(_layoutController.EvaluationMode == EvaluationMode.Test);
        }
    }
}
