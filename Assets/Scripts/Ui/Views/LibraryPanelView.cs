using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Interfaces;
using TMPro;

namespace FactoryLab.Ui.Views
{
    public class LibraryPanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown    _categoryDropdown;
        [SerializeField] private Transform       _itemContainer;
        [SerializeField] private LibraryItemView _itemPrefab;

        private IElementSpawner  _spawner;
        private ElementLibrarySO _library;

        private readonly List<LibraryItemView> _items      = new();
        private readonly List<CategorySO>      _categories = new();

        [Inject]
        public void Construct(IElementSpawner spawner, ElementLibrarySO library)
        {
            _spawner = spawner;
            _library = library;
        }

        private void Start()
        {
            _categories.AddRange(_library.GetAllCategories());
            _categoryDropdown.options = _categories
                .Select(c => new TMP_Dropdown.OptionData(c.title))
                .ToList();

            _categoryDropdown.onValueChanged.AddListener(i => RefreshItems(_categories[i]));

            if (_categories.Count > 0)
                RefreshItems(_categories[0]);
        }

        private void RefreshItems(CategorySO category)
        {
            foreach (var item in _items)
                Destroy(item.gameObject);
            _items.Clear();

            foreach (var def in _library.GetByCategory(category))
            {
                var item = Instantiate(_itemPrefab, _itemContainer);
                item.Initialize(def, _spawner);
                _items.Add(item);
            }
        }
    }
}
