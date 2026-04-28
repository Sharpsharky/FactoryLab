using System.Linq;
using FactoryLab.Core.Domain;

namespace FactoryLab.Core.Validation
{
    public class ConnectionCompatibilityValidator : ILayoutValidator
    {
        public ValidationResult Validate(LayoutState state)
        {
            var result = new ValidationResult();

            foreach (var connection in state.Connections)
            {
                var from = state.GetElement(connection.FromElementId);
                var to   = state.GetElement(connection.ToElementId);
                if (from == null || to == null) continue;

                bool outputOk = from.Definition.GetOutputPorts().Any(p => p.IsCompatibleWith(to.Definition));
                bool inputOk  = to.Definition.GetInputPorts().Any(p => p.IsCompatibleWith(from.Definition));

                if (!outputOk || !inputOk)
                    result.AddError(
                        $"{from.Definition.elementName} cannot connect to {to.Definition.elementName}",
                        from.Id);
            }

            return result;
        }
    }
}
