namespace XComponent.Common.UI.Behaviors
{
    public class ValueChangedParameters
    {
        public ValueChangedParameters(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public double OldValue { get; private set; }

        public double NewValue { get; private set; }
    }
}
