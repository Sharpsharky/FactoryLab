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
        private readonly LayoutState _layoutState;
        private readonly ElementViewFactory _factory;

        private readonly Dictionary<string, PlacedElementView> _elementViews = new();
        private readonly Dictionary<string, ConnectionView>    _connectionViews = new();

        public event Action<PlacedElement> OnContextMenuRequested;

        public LayoutState LayoutState => _layoutState;

        [Inject]
        public TableController(LayoutState layoutState, ElementViewFactory factory)
        {
            _layoutState = layoutState;
            _factory     = factory;
        }

        public void SpawnElement(ElementDefinitionSO definition, Vector3 position)
        {
            SpawnAndGet(definition, position);
        }

        public PlacedElementView SpawnAndGet(ElementDefinitionSO definition, Vector3 position)
        {
            var element = new PlacedElement(definition, position);
            _layoutState.AddElement(element);

            var view = _factory.CreateElementView(element);
            _elementViews[element.Id] = view;
            return view;
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
        }

        public void RemoveConnection(string connectionId)
        {
            _layoutState.RemoveConnection(connectionId);
            DestroyConnectionView(connectionId);
        }

        public void RequestContextMenu(PlacedElement element) =>
            OnContextMenuRequested?.Invoke(element);

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
