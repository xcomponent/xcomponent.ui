
namespace XComponent.Common.UI.Grid.ViewModel
{
    // Corresponds to Syncfusion's FilterType enum..
    public enum GridFilterType
    {
        LessThan,
        LessThanOrEqual,
        Equals,
        NotEquals,
        GreaterThanOrEqual,
        GreaterThan,
        StartsWith,
        EndsWith,
        Contains,
        Undefined,
        Between,
    }

    // Corresponds to Syncfusion's PredicateType enum..
    public enum GridPredicateType
    {
        And,
        Or,
    }

    // Corresponds to Syncfusion's FilterBehavior enum..
    public enum GridFilterBehavior
    {
        StronglyTyped,
        StringTyped,
    }
}
