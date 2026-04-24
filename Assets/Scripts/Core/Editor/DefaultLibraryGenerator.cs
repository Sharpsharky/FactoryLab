using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FactoryLab.Core.Data;

namespace FactoryLab.Core.Editor
{
    public static class DefaultLibraryGenerator
    {
        private const string DataPath = "Assets/Data";
        private const string ElementsPath = "Assets/Data/Elements";

        [MenuItem("FactoryLab/Generate Default Library")]
        public static void Generate()
        {
            EnsureDirectories();

            var zbiornikA    = CreateZbiornikSurowcaA();
            var zbiornikB    = CreateZbiornikSurowcaB();
            var filtr        = CreateFiltrWstepny();
            var mieszalnik   = CreateMieszalnik();
            var podgrzewacz  = CreatePodgrzewacz();
            var pakowarka    = CreatePakowarka();

            CreateLibrary(new List<ElementDefinitionSO>
            {
                zbiornikA, zbiornikB, filtr, mieszalnik, podgrzewacz, pakowarka
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[FactoryLab] Default library generated at Assets/Data/");
        }

        private static void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder(ElementsPath))
                AssetDatabase.CreateFolder(DataPath, "Elements");
        }

        private static ElementDefinitionSO CreateZbiornikSurowcaA()
        {
            var so = GetOrCreate<ElementDefinitionSO>(ElementsPath, "ZbiornikSurowcaA");
            so.elementName   = "Zbiornik surowca A";
            so.category      = "Źródło";
            so.size          = new Vector3(1f, 1.5f, 1f);
            so.color         = new Color(0.2f, 0.45f, 0.9f);
            so.primitiveShape = PrimitiveType.Cylinder;
            so.ports = new List<PortDefinition>
            {
                Output("Wyjście", "Mieszalnik", "Filtr wstępny")
            };
            MarkDirty(so);
            return so;
        }

        private static ElementDefinitionSO CreateZbiornikSurowcaB()
        {
            var so = GetOrCreate<ElementDefinitionSO>(ElementsPath, "ZbiornikSurowcaB");
            so.elementName    = "Zbiornik surowca B";
            so.category       = "Źródło";
            so.size           = new Vector3(1f, 1.5f, 1f);
            so.color          = new Color(0.2f, 0.8f, 0.9f);
            so.primitiveShape = PrimitiveType.Cylinder;
            so.ports = new List<PortDefinition>
            {
                Output("Wyjście", "Mieszalnik")
            };
            MarkDirty(so);
            return so;
        }

        private static ElementDefinitionSO CreateFiltrWstepny()
        {
            var so = GetOrCreate<ElementDefinitionSO>(ElementsPath, "FiltrWstepny");
            so.elementName    = "Filtr wstępny";
            so.category       = "Przetwarzanie";
            so.size           = new Vector3(1f, 1f, 1f);
            so.color          = new Color(0.85f, 0.85f, 0.3f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                Input("Wejście",  "Zbiornik surowca A"),
                Output("Wyjście", "Mieszalnik")
            };
            MarkDirty(so);
            return so;
        }

        private static ElementDefinitionSO CreateMieszalnik()
        {
            var so = GetOrCreate<ElementDefinitionSO>(ElementsPath, "Mieszalnik");
            so.elementName    = "Mieszalnik";
            so.category       = "Przetwarzanie";
            so.size           = new Vector3(1.5f, 1f, 1.5f);
            so.color          = new Color(0.85f, 0.5f, 0.2f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                Input("Wejście 1", "Zbiornik surowca A", "Zbiornik surowca B", "Filtr wstępny"),
                Input("Wejście 2", "Zbiornik surowca A", "Zbiornik surowca B", "Filtr wstępny"),
                Output("Wyjście",  "Podgrzewacz")
            };
            MarkDirty(so);
            return so;
        }

        private static ElementDefinitionSO CreatePodgrzewacz()
        {
            var so = GetOrCreate<ElementDefinitionSO>(ElementsPath, "Podgrzewacz");
            so.elementName    = "Podgrzewacz";
            so.category       = "Przetwarzanie";
            so.size           = new Vector3(1f, 1.2f, 1f);
            so.color          = new Color(0.9f, 0.3f, 0.2f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                Input("Wejście",  "Mieszalnik"),
                Output("Wyjście", "Pakowarka")
            };
            MarkDirty(so);
            return so;
        }

        private static ElementDefinitionSO CreatePakowarka()
        {
            var so = GetOrCreate<ElementDefinitionSO>(ElementsPath, "Pakowarka");
            so.elementName    = "Pakowarka";
            so.category       = "Końcowy";
            so.size           = new Vector3(1.5f, 1f, 1f);
            so.color          = new Color(0.45f, 0.9f, 0.4f);
            so.primitiveShape = PrimitiveType.Cube;
            so.ports = new List<PortDefinition>
            {
                Input("Wejście", "Podgrzewacz")
            };
            MarkDirty(so);
            return so;
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

        private static PortDefinition Input(string name, params string[] compatible) =>
            new PortDefinition { portName = name, portType = PortType.Input, compatibleWith = new List<string>(compatible) };

        private static PortDefinition Output(string name, params string[] compatible) =>
            new PortDefinition { portName = name, portType = PortType.Output, compatibleWith = new List<string>(compatible) };
    }
}
