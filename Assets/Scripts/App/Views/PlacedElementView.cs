using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;

namespace FactoryLab.App.Views
{
    public class PlacedElementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text  _nameText;
        [SerializeField] private Transform _titleCanvas;

        public PlacedElement Data { get; private set; }

        private List<PortView> _portViews;
        private Renderer       _bodyRenderer;
        private Color          _baseColor;
        private Camera         _camera;

        public Transform TitleCanvas => _titleCanvas;

        public void Initialize(PlacedElement data, List<PortView> portViews, Camera camera)
        {
            Data          = data;
            _portViews    = portViews;
            _camera       = camera;
            _bodyRenderer = GetComponentInChildren<Renderer>();
            _baseColor    = data.Definition.color;
            _bodyRenderer.material.color = _baseColor;

            if (_nameText != null)
                _nameText.text = data.Definition.elementName;
        }

        private void Update()
        {
            if (_titleCanvas != null && _camera != null)
                _titleCanvas.rotation = _camera.transform.rotation;
        }

        public void SetHighlight(HighlightState state)
        {
            _bodyRenderer.material.color = state switch
            {
                HighlightState.Error   => Color.red,
                HighlightState.Warning => Color.yellow,
                HighlightState.Valid   => Color.green,
                _                      => _baseColor
            };
        }

        public PortView GetOutputPort() =>
            _portViews.FirstOrDefault(p => p.Direction == PortType.Output);

        public PortView GetInputPort() =>
            _portViews.FirstOrDefault(p => p.Direction == PortType.Input);
    }
}
