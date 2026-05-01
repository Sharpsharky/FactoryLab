using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;

namespace FactoryLab.App.Views
{
    public class PlacedElementView : MonoBehaviour
    {
        public PlacedElement Data { get; private set; }

        public event Action<PlacedElementView> OnRightClicked;

        private List<PortView> _portViews;
        private Renderer _bodyRenderer;
        private Color _baseColor;

        public void Initialize(PlacedElement data, List<PortView> portViews)
        {
            Data        = data;
            _portViews  = portViews;
            _bodyRenderer = GetComponentInChildren<Renderer>();
            _baseColor  = data.Definition.color;
            _bodyRenderer.material.color = _baseColor;
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

        public PortView GetPortView(string portName) =>
            _portViews.FirstOrDefault(p => p.PortName == portName);

        public PortView GetOutputPort() =>
            _portViews.FirstOrDefault(p => p.Direction == PortType.Output);

        public PortView GetInputPort() =>
            _portViews.FirstOrDefault(p => p.Direction == PortType.Input);

        public IReadOnlyList<PortView> PortViews => _portViews;
    }
}
