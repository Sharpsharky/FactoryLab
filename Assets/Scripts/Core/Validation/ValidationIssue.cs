namespace FactoryLab.Core.Validation
{
    public class ValidationIssue
    {
        public ValidationIssueType Type        { get; }
        public string              Description  { get; }
        public string              ElementId    { get; }

        public ValidationIssue(ValidationIssueType type, string description, string elementId = null)
        {
            Type        = type;
            Description = description;
            ElementId   = elementId;
        }
    }
}
