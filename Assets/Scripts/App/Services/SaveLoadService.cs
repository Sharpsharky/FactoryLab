using System.IO;
using FactoryLab.App.Controllers;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;
using FactoryLab.Core.Interfaces;

namespace FactoryLab.App.Services
{
    public class SaveLoadService : ISaveLoadService
    {
        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, "layout.json");

        private readonly LayoutState      _layoutState;
        private readonly TableController  _table;
        private readonly ElementLibrarySO _library;

        [Inject]
        public SaveLoadService(LayoutState layoutState, TableController table, ElementLibrarySO library)
        {
            _layoutState = layoutState;
            _table       = table;
            _library     = library;
        }

        public void Save()
        {
            var data = new LayoutSaveData();

            foreach (var element in _layoutState.Elements)
                data.Elements.Add(new ElementSaveData
                {
                    Id           = element.Id,
                    DefinitionId = element.Definition.Id,
                    X            = element.Position.x,
                    Z            = element.Position.z
                });

            foreach (var conn in _layoutState.Connections)
                data.Connections.Add(new ConnectionSaveData
                {
                    Id            = conn.Id,
                    FromElementId = conn.FromElementId,
                    ToElementId   = conn.ToElementId
                });

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
            Debug.Log($"Saved to: {SavePath}");
        }

        public bool Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("No save file found.");
                return false;
            }

            var data = JsonUtility.FromJson<LayoutSaveData>(File.ReadAllText(SavePath));
            _table.LoadLayout(data, _library);
            return true;
        }
    }
}
