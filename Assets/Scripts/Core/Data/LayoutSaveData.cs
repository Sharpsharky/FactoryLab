using System;
using System.Collections.Generic;

namespace FactoryLab.Core.Data
{
    [Serializable]
    public class LayoutSaveData
    {
        public List<ElementSaveData>    Elements    = new();
        public List<ConnectionSaveData> Connections = new();
    }

    [Serializable]
    public class ElementSaveData
    {
        public string Id;
        public string DefinitionId;
        public float  X;
        public float  Z;
    }

    [Serializable]
    public class ConnectionSaveData
    {
        public string Id;
        public string FromElementId;
        public string ToElementId;
    }
}
