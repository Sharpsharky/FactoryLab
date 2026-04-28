using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryLab.Core.Data
{
    [CreateAssetMenu(fileName = "LayoutTemplate", menuName = "FactoryLab/Layout Template")]
    public class LayoutTemplateSO : ScriptableObject
    {
        public List<RequiredConnection> requiredConnections = new();

        [Serializable]
        public class RequiredConnection
        {
            public ElementDefinitionSO fromElement;
            public ElementDefinitionSO toElement;
        }
    }
}
