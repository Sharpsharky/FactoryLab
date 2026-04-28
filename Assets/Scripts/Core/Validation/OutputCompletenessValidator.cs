using System.Linq;
using FactoryLab.Core.Domain;

namespace FactoryLab.Core.Validation
{
    public class OutputCompletenessValidator : ILayoutValidator
    {
        public ValidationResult Validate(LayoutState state)
        {
            var result = new ValidationResult();

            foreach (var element in state.Elements)
            {
                var outputCount   = element.Definition.GetOutputPorts().Count();
                var connectedCount = state.GetConnectionsFrom(element.Id).Count();

                if (connectedCount < outputCount)
                    result.AddWarning(
                        $"{element.Definition.elementName}: {outputCount - connectedCount} output ports not connected",
                        element.Id);
            }

            return result;
        }
    }
}
