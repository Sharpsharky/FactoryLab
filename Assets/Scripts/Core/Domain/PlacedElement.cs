using System;
using UnityEngine;
using FactoryLab.Core.Data;

namespace FactoryLab.Core.Domain
{
    public class PlacedElement
    {
        public string Id { get; }
        public ElementDefinitionSO Definition { get; }
        public Vector3 Position { get; set; }

        public PlacedElement(ElementDefinitionSO definition, Vector3 position)
            : this(Guid.NewGuid().ToString(), definition, position) { }

        public PlacedElement(string id, ElementDefinitionSO definition, Vector3 position)
        {
            Id = id;
            Definition = definition;
            Position = position;
        }
    }
}
