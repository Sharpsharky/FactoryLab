using UnityEngine;
using FactoryLab.Core.Data;

namespace FactoryLab.App.Views
{
    public class PortView : MonoBehaviour
    {
        public string PortName { get; private set; }
        public PortType Direction { get; private set; }
        public PlacedElementView Owner { get; private set; }

        private static readonly Color OutputColor  = new Color(0.2f, 0.85f, 0.2f);
        private static readonly Color InputColor   = new Color(0.85f, 0.2f, 0.2f);
        private static readonly Color SelectedColor = Color.yellow;

        private Renderer _renderer;

        private void Awake() => _renderer = GetComponent<Renderer>();

        public void Initialize(PortDefinition port, PlacedElementView owner)
        {
            PortName  = port.portName;
            Direction = port.portType;
            Owner     = owner;
            SetSelected(false);
        }

        public void SetOwner(PlacedElementView owner) => Owner = owner;

        public void SetSelected(bool selected) =>
            _renderer.material.color = selected ? SelectedColor
                : Direction == PortType.Output ? OutputColor : InputColor;
    }
}
