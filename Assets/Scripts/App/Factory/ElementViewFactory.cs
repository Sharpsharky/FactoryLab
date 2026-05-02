using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;
using FactoryLab.App.Views;

namespace FactoryLab.App.Factory
{
    public class ElementViewFactory
    {
        private const float PortSphereScale    = 0.2f;
        private const float PortSideOffset     = 0.35f;
        private const float PortZSpread        = 0.3f;
        private const float TitleCanvasYOffset = 0.4f;

        private readonly PlacedElementView _prefab;
        private readonly Camera            _camera;

        [Inject]
        public ElementViewFactory([Inject(Id = "ElementPrefab")] PlacedElementView prefab, Camera camera)
        {
            _prefab  = prefab;
            _camera  = camera;
        }

        public PlacedElementView CreateElementView(PlacedElement element)
        {
            var def  = element.Definition;
            var view = Object.Instantiate(_prefab);
            view.name                    = def.elementName;
            view.transform.position      = element.Position;

            if (view.TitleCanvas != null)
                view.TitleCanvas.localPosition = new Vector3(0f, def.size.y + TitleCanvasYOffset, 0f);

            CreateBody(def, view.transform);

            var portViews = CreatePorts(def, view.transform, view);
            view.Initialize(element, portViews, _camera);

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
                    : Mathf.Lerp(-elementSize.z * PortZSpread, elementSize.z * PortZSpread, (float)i / (ports.Count - 1));

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
