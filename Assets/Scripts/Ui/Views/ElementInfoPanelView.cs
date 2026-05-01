using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;

namespace FactoryLab.Ui.Views
{
    public class ElementInfoPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text   _nameText;
        [SerializeField] private TMP_Text   _categoryText;
        [SerializeField] private TMP_Text   _portsText;
        [SerializeField] private Button     _closeButton;

        private void Start()
        {
            _closeButton.onClick.AddListener(() => _panel.SetActive(false));
            _panel.SetActive(false);
        }

        public void Show(PlacedElement element)
        {
            var def = element.Definition;

            _nameText.text     = def.elementName;
            _categoryText.text = def.category != null ? def.category.title : "-";
            _portsText.text    = BuildPortsText(def);

            _panel.SetActive(true);
        }

        private static string BuildPortsText(ElementDefinitionSO def)
        {
            var sb = new StringBuilder();

            foreach (var port in def.ports)
            {
                var dir = port.portType == PortType.Output ? "OUT" : "IN";
                sb.AppendLine($"[{dir}] {port.portName}");
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "No ports";
        }
    }
}
