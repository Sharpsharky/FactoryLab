using System.Collections.Generic;
using FactoryLab.Core.Domain;

namespace FactoryLab.Core.Validation
{
    public class ValidationService
    {
        private readonly List<ILayoutValidator> _validators;

        public ValidationService(List<ILayoutValidator> validators)
        {
            _validators = validators;
        }

        public ValidationResult Validate(LayoutState state)
        {
            var combined = new ValidationResult();
            foreach (var validator in _validators)
                combined.Merge(validator.Validate(state));
            return combined;
        }
    }
}
