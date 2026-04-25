using System.Collections.Generic;
using UnityEngine;

namespace FactoryLab.Core.Data
{
    [CreateAssetMenu(fileName = "NewElement", menuName = "FactoryLab/Element Definition")]
    public class ElementDefinitionSO : ScriptableObject
    {
        public string elementName;
        public CategorySO category;
        public Vector3 size = Vector3.one;
        public Color color = Color.white;
        public PrimitiveType primitiveShape = PrimitiveType.Cube;
        public List<PortDefinition> ports = new ();

        public IEnumerable<PortDefinition> GetInputPorts()
        {
            foreach (var port in ports)
                if (port.portType == PortType.Input)
                    yield return port;
        }

        public IEnumerable<PortDefinition> GetOutputPorts()
        {
            foreach (var port in ports)
                if (port.portType == PortType.Output)
                    yield return port;
        }

        public PortDefinition GetPort(string portName)
        {
            foreach (var port in ports)
                if (port.portName == portName)
                    return port;
            return null;
        }
    }
}
