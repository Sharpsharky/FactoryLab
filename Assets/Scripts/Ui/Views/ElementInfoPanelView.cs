using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;

namespace FactoryLab.Ui.Views
{
    public class ElementInfoPanelView : MonoBehaviour
    {
        private const string LabelOutput       = "OUT";
        private const string LabelInput        = "IN";
        private const string LabelConnected    = "[connected]";
        private const string LabelNotConnected = "[no connection]";
        private const string LabelNoCategory   = "-";
        private const string LabelNoPorts      = "No ports";

        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text   _nameText;
        [SerializeField] private TMP_Text   _categoryText;
        [SerializeField] private TMP_Text   _portsText;
        [SerializeField] private Button     _closeButton;

        private LayoutState _layoutState;

        [Inject]
        public void Construct(LayoutState layoutState)
        {
            _layoutState = layoutState;
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(() => _panel.SetActive(false));
            _panel.SetActive(false);
        }

        public void Show(PlacedElement element)
        {
            _nameText.text     = element.Definition.elementName;
            _categoryText.text = element.Definition.category != null
                ? element.Definition.category.title : LabelNoCategory;
            _portsText.text    = BuildPortsText(element);

            _panel.SetActive(true);
        }

        private string BuildPortsText(PlacedElement element)
        {
            var def = element.Definition;
            var sb  = new StringBuilder();

            var outputConnections = _layoutState.GetConnectionsFrom(element.Id).Count();
            var inputConnections  = _layoutState.GetConnectionsTo(element.Id).Count();

            int outputIndex = 0;
            int inputIndex  = 0;

            foreach (var port in def.ports)
            {
                var dir       = port.portType == PortType.Output ? LabelOutput : LabelInput;
                bool connected = port.portType == PortType.Output
                    ? outputConnections > outputIndex++
                    : inputConnections  > inputIndex++;

                var status = connected ? LabelConnected : LabelNotConnected;
                sb.AppendLine($"[{dir}] {port.portName} {status}");
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : LabelNoPorts;
        }
    }
}
