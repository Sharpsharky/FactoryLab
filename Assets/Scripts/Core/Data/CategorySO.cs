using UnityEngine;

namespace FactoryLab.Core.Data
{
    [CreateAssetMenu(fileName = "NewCategory", menuName = "FactoryLab/Category")]
    public class CategorySO : ScriptableObject
    {
        public string title;
    }
}
