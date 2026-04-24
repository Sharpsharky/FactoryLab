using System;
using System.Collections.Generic;

namespace FactoryLab.Core.Data
{
    [Serializable]
    public class PortDefinition
    {
        public string portName;
        public PortType portType;
        public List<string> compatibleWith = new ();
    }
}
