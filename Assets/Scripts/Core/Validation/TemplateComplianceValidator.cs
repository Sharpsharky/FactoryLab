using System.Linq;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;

namespace FactoryLab.Core.Validation
{
    public class TemplateComplianceValidator : ILayoutValidator
    {
        private readonly LayoutTemplateSO _template;

        public TemplateComplianceValidator(LayoutTemplateSO template)
        {
            _template = template;
        }

        public ValidationResult Validate(LayoutState state)
        {
            var result = new ValidationResult();

            foreach (var required in _template.requiredConnections)
            {
                bool found = state.Connections.Any(c =>
                {
                    var from = state.GetElement(c.FromElementId);
                    var to   = state.GetElement(c.ToElementId);
                    return from?.Definition == required.fromElement &&
                           to?.Definition   == required.toElement;
                });

                if (!found)
                    result.AddError(
                        $"Missing connection: {required.fromElement.elementName} -> {required.toElement.elementName}");
            }

            return result;
        }
    }
}
