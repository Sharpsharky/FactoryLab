using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactoryLab.Core.Data;
using FactoryLab.Core.Interfaces;

namespace FactoryLab.Ui.Views
{
    public class LibraryItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button   _button;

        private ElementDefinitionSO _definition;
        private IElementSpawner     _spawner;

        public void Initialize(ElementDefinitionSO definition, IElementSpawner spawner)
        {
            _definition    = definition;
            _spawner       = spawner;
            _nameText.text = definition.elementName;
            _button.onClick.AddListener(() => _spawner.SpawnElement(_definition));
        }
    }
}
