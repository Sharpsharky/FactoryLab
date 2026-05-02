using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryLab.Core.Data
{
    [CreateAssetMenu(fileName = "ElementLibrary", menuName = "FactoryLab/Element Library")]
    public class ElementLibrarySO : ScriptableObject
    {
        public List<ElementDefinitionSO> elements = new();

        public IEnumerable<CategorySO> GetAllCategories() =>
            elements.Select(e => e.category).Where(c => c != null).Distinct();

        public IEnumerable<ElementDefinitionSO> GetByCategory(CategorySO category) =>
            elements.Where(e => e.category == category);

        public ElementDefinitionSO GetById(string id) =>
            elements.FirstOrDefault(e => e.Id == id);
    }
}
