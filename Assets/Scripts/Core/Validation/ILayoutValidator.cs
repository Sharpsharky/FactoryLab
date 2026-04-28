using FactoryLab.Core.Domain;

namespace FactoryLab.Core.Validation
{
    public interface ILayoutValidator
    {
        ValidationResult Validate(LayoutState state);
    }
}
