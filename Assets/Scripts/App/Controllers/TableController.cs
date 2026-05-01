using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;
using FactoryLab.Core.Interfaces;
using FactoryLab.App.Factory;
using FactoryLab.App.Views;

namespace FactoryLab.App.Controllers
{
    public class TableController : IElementSpawner
    {
        private readonly LayoutState         _layoutState;
        private readonly ElementViewFactory  _factory;

        private readonly Dictionary<string, PlacedElementView> _elementViews    = new();
        private readonly Dictionary<string, ConnectionView>    _connectionViews = new();

        public event Action<PlacedElementView>      OnElementSpawned;
        public event Action<PlacedElement, Vector2> OnContextMenuRequested;
        public event Action                         OnLayoutChanged;

        public LayoutState LayoutState => _layoutState;

        [Inject]
        public TableController(LayoutState layoutState, ElementViewFactory factory)
        {
            _layoutState = layoutState;
            _factory     = factory;
        }

        public void SpawnElement(ElementDefinitionSO definition)
        {
            var element = new PlacedElement(definition, Vector3.zero);
            _layoutState.AddElement(element);

            var view = _factory.CreateElementView(element);
            _elementViews[element.Id] = view;

            OnElementSpawned?.Invoke(view);
        }

        public void RemoveElement(string elementId)
        {
            var orphanIds = _layoutState
                .GetConnectionsFrom(elementId)
                .Concat(_layoutState.GetConnectionsTo(elementId))
                .Select(c => c.Id)
                .ToList();

            foreach (var id in orphanIds)
                DestroyConnectionView(id);

            _layoutState.RemoveElement(elementId);

            if (_elementViews.TryGetValue(elementId, out var view))
            {
                UnityEngine.Object.Destroy(view.gameObject);
                _elementViews.Remove(elementId);
            }

            OnLayoutChanged?.Invoke();
        }

        public void AddConnection(string fromElementId, string toElementId,
                                  PortView fromPort, PortView toPort)
        {
            var connection = new ConnectionData(fromElementId, toElementId);
            _layoutState.AddConnection(connection);

            var go       = new GameObject($"Connection_{connection.Id}");
            var connView = go.AddComponent<ConnectionView>();
            connView.Initialize(connection.Id, fromPort, toPort);
            _connectionViews[connection.Id] = connView;

            OnLayoutChanged?.Invoke();
        }

        public void RemoveConnection(string connectionId)
        {
            _layoutState.RemoveConnection(connectionId);
            DestroyConnectionView(connectionId);
            OnLayoutChanged?.Invoke();
        }

        public void RequestContextMenu(PlacedElement element, Vector2 screenPos) =>
            OnContextMenuRequested?.Invoke(element, screenPos);

        public PlacedElementView GetView(string elementId) =>
            _elementViews.GetValueOrDefault(elementId);

        public IEnumerable<PlacedElementView> GetAllViews() => _elementViews.Values;

        public void ApplyHighlights(IReadOnlyDictionary<string, HighlightState> highlights)
        {
            foreach (var (id, view) in _elementViews)
            {
                var state = highlights.TryGetValue(id, out var h) ? h : HighlightState.Default;
                view.SetHighlight(state);
            }
        }

        public void ClearHighlights()
        {
            foreach (var view in _elementViews.Values)
                view.SetHighlight(HighlightState.Default);
        }

        public void ClearAll()
        {
            foreach (var view in _connectionViews.Values)
                UnityEngine.Object.Destroy(view.gameObject);
            _connectionViews.Clear();

            foreach (var view in _elementViews.Values)
                UnityEngine.Object.Destroy(view.gameObject);
            _elementViews.Clear();

            _layoutState.Clear();
        }

        public void LoadLayout(LayoutSaveData data, ElementLibrarySO library)
        {
            ClearAll();

            foreach (var eData in data.Elements)
            {
                var def = library.GetByName(eData.DefinitionName);
                if (def == null)
                {
                    Debug.LogWarning($"[Load] Definition not found: {eData.DefinitionName}");
                    continue;
                }

                var pos     = new Vector3(eData.X, eData.Y, eData.Z);
                var element = new PlacedElement(eData.Id, def, pos);
                _layoutState.AddElement(element);
                var view = _factory.CreateElementView(element);
                _elementViews[element.Id] = view;
            }

            foreach (var cData in data.Connections)
            {
                var fromView = _elementViews.GetValueOrDefault(cData.FromElementId);
                var toView   = _elementViews.GetValueOrDefault(cData.ToElementId);
                if (fromView == null || toView == null) continue;

                var fromPort = fromView.GetOutputPort();
                var toPort   = toView.GetInputPort();
                if (fromPort == null || toPort == null) continue;

                var connection = new ConnectionData(cData.Id, cData.FromElementId, cData.ToElementId);
                _layoutState.AddConnection(connection);

                var go       = new UnityEngine.GameObject($"Connection_{cData.Id}");
                var connView = go.AddComponent<ConnectionView>();
                connView.Initialize(cData.Id, fromPort, toPort);
                _connectionViews[cData.Id] = connView;
            }

            OnLayoutChanged?.Invoke();
        }

        private void DestroyConnectionView(string connectionId)
        {
            if (_connectionViews.TryGetValue(connectionId, out var view))
            {
                UnityEngine.Object.Destroy(view.gameObject);
                _connectionViews.Remove(connectionId);
            }
        }
    }
}
