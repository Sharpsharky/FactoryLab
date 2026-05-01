using UnityEngine;
using UnityEngine.UI;
using Zenject;
using FactoryLab.Core.Domain;
using ILayoutController = FactoryLab.Core.Interfaces.ILayoutController;

namespace FactoryLab.Ui.Views
{
    public class ContextMenuView : MonoBehaviour
    {
        [SerializeField] private RectTransform        _panel;
        [SerializeField] private Button               _editButton;
        [SerializeField] private Button               _deleteButton;
        [SerializeField] private ElementInfoPanelView _infoPanel;

        [SerializeField] private float _yOffset = 1.5f;

        private ILayoutController _layoutController;
        private Camera            _camera;
        private PlacedElement     _target;

        [Inject]
        public void Construct(ILayoutController layoutController, Camera camera)
        {
            _layoutController = layoutController;
            _camera           = camera;
        }

        private void Start()
        {
            _layoutController.OnContextMenuRequested += Show;
            _editButton.onClick.AddListener(OnEdit);
            _deleteButton.onClick.AddListener(OnDelete);
            _panel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_panel.gameObject.activeSelf) return;
            if (!Input.GetMouseButtonDown(0)) return;

            if (!RectTransformUtility.RectangleContainsScreenPoint(_panel, Input.mousePosition, _camera))
                Hide();
        }

        private void OnDestroy()
        {
            _layoutController.OnContextMenuRequested -= Show;
        }

        private void Show(PlacedElement element, Vector2 _)
        {
            _target = element;

            _panel.position = element.Position + Vector3.up * _yOffset;
            _panel.rotation = _camera.transform.rotation;

            _panel.gameObject.SetActive(true);
        }

        private void Hide()
        {
            _panel.gameObject.SetActive(false);
            _target = null;
        }

        private void OnEdit()
        {
            if (_target != null)
                _infoPanel.Show(_target);
            Hide();
        }

        private void OnDelete()
        {
            if (_target != null)
                _layoutController.RemoveElement(_target.Id);
            Hide();
        }
    }
}
