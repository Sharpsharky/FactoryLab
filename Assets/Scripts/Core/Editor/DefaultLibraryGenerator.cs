using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FactoryLab.Core.Data;

namespace FactoryLab.Core.Editor
{
    public static class DefaultLibraryGenerator
    {
        private const string DataPath       = "Assets/Data";
        private const string CategoriesPath = "Assets/Data/Categories";
        private const string ElementsPath   = "Assets/Data/Elements";

        [MenuItem("FactoryLab/Generate Default Library")]
        public static void Generate()
        {
            EnsureDirectories();

            var source     = CreateCategory("Source",     "Source");
            var processing = CreateCategory("Processing", "Processing");
            var output     = CreateCategory("Output",     "Output");

            // Create all element assets first so ports can reference them
            var tankA    = GetOrCreate<ElementDefinitionSO>(ElementsPath, "RawMaterialTankA");
            var tankB    = GetOrCreate<ElementDefinitionSO>(ElementsPath, "RawMaterialTankB");
            var filter   = GetOrCreate<ElementDefinitionSO>(ElementsPath, "PreFilter");
            var mixer    = GetOrCreate<ElementDefinitionSO>(ElementsPath, "Mixer");
            var heater   = GetOrCreate<ElementDefinitionSO>(ElementsPath, "Heater");
            var packager = GetOrCreate<ElementDefinitionSO>(ElementsPath, "PackagingMachine");

            SetupTankA(tankA,    source,     mixer, filter);
            SetupTankB(tankB,    source,     mixer);
            SetupFilter(filter,  processing, tankA, mixer);
            SetupMixer(mixer,    processing, tankA, tankB, filter, heater);
            SetupHeater(heater,  processing, mixer, packager);
            SetupPackager(packager, output,  heater);

            CreateLibrary(new List<ElementDefinitionSO>
            {
                tankA, tankB, filter, mixer, heater, packager
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[FactoryLab] Default library generated at Assets/Data/");
        }

        private static void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder(CategoriesPath))
                AssetDatabase.CreateFolder(DataPath, "Categories");
            if (!AssetDatabase.IsValidFolder(ElementsPath))
                AssetDatabase.CreateFolder(DataPath, "Elements");
        }

        private static CategorySO CreateCategory(string assetName, string title)
        {
            var so = GetOrCreate<CategorySO>(CategoriesPath, assetName);
            so.title = title;
            MarkDirty(so);
            return so;
        }

        private static void SetupTankA(ElementDefinitionSO so, CategorySO category,
            ElementDefinitionSO mixer, ElementDefinitionSO filter)
        {
            so.elementName    = "Raw Material Tank A";
            so.category       = category;
            so.size           = new Vector3(1f, 1.5f, 1f);
            so.color          = new Color(0.2f, 0.45f, 0.9f);
            so.primitiveShape = PrimitiveType.Cylinder;
            so.ports          = new List<PortDefinition> { Out("Output", mixer, filter) };
            MarkDirty(so);
        }

        private static void SetupTankB(ElementDefinitionSO so, CategorySO category,
            ElementDefinitionSO mixer)
        {
            so.elementName    = "Raw Material Tank B";
            so.category       = category;
            so.size           = new Vector3(1f, 1.5f, 1f);
            so.color          = new Color(0.2f, 0.8f, 0.9f);
            so.primitiveShape = PrimitiveType.Cylinder;
            so.ports          = new List<PortDefinition> { Out("Output", mixer) };
            MarkDirty(so);
        }

        private static void SetupFilter(ElementDefinitionSO so, CategorySO category,
            ElementDefinitionSO tankA, ElementDefinitionSO mixer)
        {
            so.elementName    = "Pre-filter";
            so.category       = category;
            so.size           = new Vector3(1f, 1f, 1f);
            so.color          = new Color(0.85f, 0.85f, 0.3f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                In("Input",   tankA),
                Out("Output", mixer)
            };
            MarkDirty(so);
        }

        private static void SetupMixer(ElementDefinitionSO so, CategorySO category,
            ElementDefinitionSO tankA, ElementDefinitionSO tankB,
            ElementDefinitionSO filter, ElementDefinitionSO heater)
        {
            so.elementName    = "Mixer";
            so.category       = category;
            so.size           = new Vector3(1.5f, 1f, 1.5f);
            so.color          = new Color(0.85f, 0.5f, 0.2f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                In("Input 1",  tankA, tankB, filter),
                In("Input 2",  tankA, tankB, filter),
                Out("Output",  heater)
            };
            MarkDirty(so);
        }

        private static void SetupHeater(ElementDefinitionSO so, CategorySO category,
            ElementDefinitionSO mixer, ElementDefinitionSO packager)
        {
            so.elementName    = "Heater";
            so.category       = category;
            so.size           = new Vector3(1f, 1.2f, 1f);
            so.color          = new Color(0.9f, 0.3f, 0.2f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                In("Input",   mixer),
                Out("Output", packager)
            };
            MarkDirty(so);
        }

        private static void SetupPackager(ElementDefinitionSO so, CategorySO category,
            ElementDefinitionSO heater)
        {
            so.elementName    = "Packaging Machine";
            so.category       = category;
            so.size           = new Vector3(1.5f, 1f, 1f);
            so.color          = new Color(0.45f, 0.9f, 0.4f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports          = new List<PortDefinition> { In("Input", heater) };
            MarkDirty(so);
        }

        private static void CreateLibrary(List<ElementDefinitionSO> elements)
        {
            var library = GetOrCreate<ElementLibrarySO>(DataPath, "ElementLibrary");
            library.elements = elements;
            MarkDirty(library);
        }

        private static T GetOrCreate<T>(string path, string assetName) where T : ScriptableObject
        {
            var fullPath = $"{path}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(fullPath);
            if (existing != null) return existing;

            var instance = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, fullPath);
            return instance;
        }

        private static void MarkDirty(Object obj) => EditorUtility.SetDirty(obj);

        private static PortDefinition In(string name, params ElementDefinitionSO[] elements) =>
            new PortDefinition
            {
                portName           = name,
                portType           = PortType.Input,
                compatibleElements = new List<ElementDefinitionSO>(elements)
            };

        private static PortDefinition Out(string name, params ElementDefinitionSO[] elements) =>
            new PortDefinition
            {
                portName           = name,
                portType           = PortType.Output,
                compatibleElements = new List<ElementDefinitionSO>(elements)
            };
    }
}
