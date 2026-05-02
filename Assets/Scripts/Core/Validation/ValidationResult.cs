using System.Collections.Generic;
using System.Linq;

namespace FactoryLab.Core.Validation
{
    public class ValidationResult
    {
        private readonly List<ValidationIssue> _issues = new();

        public IReadOnlyList<ValidationIssue> Issues      => _issues;
        public bool                           IsValid      => !HasErrors;
        public bool                           HasErrors    => _issues.Any(i => i.Type == ValidationIssueType.Error);

        public void AddError(string description, string elementId = null) =>
            _issues.Add(new ValidationIssue(ValidationIssueType.Error, description, elementId));

        public void AddWarning(string description, string elementId = null) =>
            _issues.Add(new ValidationIssue(ValidationIssueType.Warning, description, elementId));

        public void Merge(ValidationResult other) =>
            _issues.AddRange(other._issues);
    }
}
