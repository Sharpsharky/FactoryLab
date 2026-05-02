using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryLab.Core.Data
{
    [CreateAssetMenu(fileName = "NewElement", menuName = "FactoryLab/Element Definition")]
    public class ElementDefinitionSO : ScriptableObject
    {
        [SerializeField] private string _id;
        public string elementName;

        public string Id => _id;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
        }
        public CategorySO category;
        public Vector3 size = Vector3.one;
        public Color color = Color.white;
        public PrimitiveType primitiveShape = PrimitiveType.Cube;
        public List<PortDefinition> ports = new ();

        public IEnumerable<PortDefinition> GetInputPorts() => 
            ports.Where(port => port.portType == PortType.Input);

        public IEnumerable<PortDefinition> GetOutputPorts() => 
            ports.Where(port => port.portType == PortType.Output);
    }
}
