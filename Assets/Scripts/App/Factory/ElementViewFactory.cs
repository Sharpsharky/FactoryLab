using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;
using FactoryLab.App.Views;

namespace FactoryLab.App.Factory
{
    public class ElementViewFactory
    {
        private const float PortSphereScale = 0.2f;
        private const float PortSideOffset  = 0.35f;

        public PlacedElementView CreateElementView(PlacedElement element)
        {
            var def  = element.Definition;
            var root = new GameObject(def.elementName);
            root.transform.position = element.Position;

            CreateBody(def, root.transform);

            var view = root.AddComponent<PlacedElementView>();

            var portViews = CreatePorts(def, root.transform, view);
            view.Initialize(element, portViews);

            return view;
        }

        private static void CreateBody(ElementDefinitionSO def, Transform parent)
        {
            var body = GameObject.CreatePrimitive(def.primitiveShape);
            body.name = "Body";
            body.transform.SetParent(parent);
            body.transform.localScale    = def.size;
            body.transform.localPosition = new Vector3(0f, def.size.y * 0.5f, 0f);
            body.GetComponent<Renderer>().material.color = def.color;

            // Ports handle their own colliders — body collider drives element dragging
            var col = body.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }

        private static List<PortView> CreatePorts(ElementDefinitionSO def, Transform parent, PlacedElementView owner)
        {
            var portViews = new List<PortView>();
            var inputs    = def.ports.Where(p => p.portType == PortType.Input).ToList();
            var outputs   = def.ports.Where(p => p.portType == PortType.Output).ToList();

            PlacePorts(inputs,  PortType.Input,  def.size, parent, owner, portViews);
            PlacePorts(outputs, PortType.Output, def.size, parent, owner, portViews);

            return portViews;
        }

        private static void PlacePorts(List<PortDefinition> ports, PortType side, Vector3 elementSize,
            Transform parent, PlacedElementView owner, List<PortView> portViews)
        {
            if (ports.Count == 0) return;

            float xSide = side == PortType.Output
                ?  elementSize.x * 0.5f + PortSideOffset
                : -elementSize.x * 0.5f - PortSideOffset;

            float yPos = elementSize.y * 0.5f;

            for (int i = 0; i < ports.Count; i++)
            {
                float zPos = ports.Count == 1
                    ? 0f
                    : Mathf.Lerp(-elementSize.z * 0.3f, elementSize.z * 0.3f, (float)i / (ports.Count - 1));

                var portGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                portGO.name = ports[i].portName;
                portGO.transform.SetParent(parent);
                portGO.transform.localPosition = new Vector3(xSide, yPos, zPos);
                portGO.transform.localScale    = Vector3.one * PortSphereScale;

                var portView = portGO.AddComponent<PortView>();
                portView.Initialize(ports[i], owner);
                portViews.Add(portView);
            }
        }
    }
}
