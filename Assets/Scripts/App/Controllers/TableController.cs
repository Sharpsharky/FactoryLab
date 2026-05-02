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
        private readonly ElementViewFactory  _factory;
        private readonly ConnectionView.Pool _connectionPool;
        private readonly float               _tableY;

        private readonly Dictionary<string, PlacedElementView> _elementViews    = new();
        private readonly Dictionary<string, ConnectionView>    _connectionViews = new();

        public event Action<PlacedElementView>      OnElementSpawned;
        public event Action<PlacedElement, Vector2> OnContextMenuRequested;
        public event Action                         OnLayoutChanged;

        public LayoutState LayoutState { get; }

        [Inject]
        public TableController(LayoutState layoutState, ElementViewFactory factory,
                               ConnectionView.Pool connectionPool, Bounds tableBounds)
        {
            LayoutState     = layoutState;
            _factory        = factory;
            _connectionPool = connectionPool;
            _tableY         = tableBounds.max.y;
        }

        public void SpawnElement(ElementDefinitionSO definition)
        {
            var element = new PlacedElement(definition, Vector3.zero);
            LayoutState.AddElement(element);

            var view = _factory.CreateElementView(element);
            _elementViews[element.Id] = view;

            OnElementSpawned?.Invoke(view);
        }

        public void RemoveElement(string elementId)
        {
            var orphanIds = LayoutState
                .GetConnectionsFrom(elementId)
                .Concat(LayoutState.GetConnectionsTo(elementId))
                .Select(c => c.Id)
                .ToList();

            foreach (var id in orphanIds)
                DestroyConnectionView(id);

            LayoutState.RemoveElement(elementId);

            if (_elementViews.TryGetValue(elementId, out var view))
            {
                UnityEngine.Object.Destroy(view.gameObject);
                _elementViews.Remove(elementId);
            }

            OnLayoutChanged?.Invoke();
        }

        public void AddConnection(string fromElementId, string toElementId, PortView fromPort, PortView toPort)
        {
            var connection = new ConnectionData(fromElementId, toElementId);
            LayoutState.AddConnection(connection);

            var connView = _connectionPool.Spawn(connection.Id, fromPort, toPort);
            connView.name = $"Connection_{connection.Id}";
            _connectionViews[connection.Id] = connView;

            OnLayoutChanged?.Invoke();
        }

        public void RemoveConnection(string connectionId)
        {
            LayoutState.RemoveConnection(connectionId);
            DestroyConnectionView(connectionId);
            OnLayoutChanged?.Invoke();
        }

        public void RequestContextMenu(PlacedElement element, Vector2 screenPos) =>
            OnContextMenuRequested?.Invoke(element, screenPos);

        public void ApplyHighlights(IReadOnlyDictionary<string, HighlightState> highlights)
        {
            foreach (var (id, view) in _elementViews)
            {
                var state = highlights.GetValueOrDefault(id, HighlightState.Default);
                view.SetHighlight(state);
            }
        }

        public void ClearHighlights()
        {
            foreach (var view in _elementViews.Values)
                view.SetHighlight(HighlightState.Default);
        }

        public void LoadLayout(LayoutSaveData data, ElementLibrarySO library)
        {
            ClearAll();

            foreach (var eData in data.Elements)
            {
                var def = library.GetById(eData.DefinitionId);
                if (def == null)
                {
                    Debug.LogWarning($"[Load] Definition not found: {eData.DefinitionId}");
                    continue;
                }

                var pos     = new Vector3(eData.X, _tableY, eData.Z);
                var element = new PlacedElement(eData.Id, def, pos);
                LayoutState.AddElement(element);
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
                LayoutState.AddConnection(connection);

                var connView = _connectionPool.Spawn(cData.Id, fromPort, toPort);
                _connectionViews[cData.Id] = connView;
            }

            OnLayoutChanged?.Invoke();
        }

        private void ClearAll()
        {
            foreach (var view in _connectionViews.Values)
                _connectionPool.Despawn(view);
            _connectionViews.Clear();

            foreach (var view in _elementViews.Values)
                UnityEngine.Object.Destroy(view.gameObject);
            _elementViews.Clear();

            LayoutState.Clear();
        }
        
        private void DestroyConnectionView(string connectionId)
        {
            if (_connectionViews.TryGetValue(connectionId, out var view))
            {
                _connectionPool.Despawn(view);
                _connectionViews.Remove(connectionId);
            }
        }
    }
}
