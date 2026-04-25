using System;
using System.Collections.Generic;

namespace FactoryLab.Core.Data
{
    [Serializable]
    public class PortDefinition
    {
        public string portName;
        public PortType portType;
        public List<ElementDefinitionSO> compatibleElements   = new();

        public bool IsCompatibleWith(ElementDefinitionSO element) =>
            compatibleElements.Contains(element);
    }
}
