using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public partial class PropertySelectComponent<T> : MudComponentBase, IDisposable
    {
        // The property type from the last update
        private Type? _previousPropertyType;

        // The currently active AtomicPredicate
        private AtomicPredicate<T>? _currentAtomicPredicate;

        // The currently selected property
        protected Property<T>? SelectedProperty { get; set; }

        [Parameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback PropertySelectChanged { get; set; }
        [Parameter] public EventCallback PropertySelectTypeChanged { get; set; }

        protected string ClassName => new CssBuilder("mud-property-select")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            // Unsubscribe from old predicate if necessary
            if (_currentAtomicPredicate != AtomicPredicate && _currentAtomicPredicate != null)
            {
                _currentAtomicPredicate.PropertyChanged -= HandleAtomicPredicatePropertyChanged;
            }

            // Update the current predicate and subscribe to it
            _currentAtomicPredicate = AtomicPredicate;

            if (_currentAtomicPredicate != null)
            {
                _currentAtomicPredicate.PropertyChanged += HandleAtomicPredicatePropertyChanged;
            }

            await base.SetParametersAsync(parameters);

            // Update the selected property and remember its type
            if (AtomicPredicate != null)
            {
                SelectedProperty = Filter?.Properties?.SingleOrDefault(p => GetPropertyName(p.PropertyExpression) == AtomicPredicate.Member);
                _previousPropertyType = TypeIdentifier.GetPropertyTypeFromExpression(SelectedProperty?.PropertyExpression);
            }
        }

        // This method is called when any property of the AtomicPredicate changes
        private void HandleAtomicPredicatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Add specific handlers for property changes if necessary
            // e.g., update the UI when the Member property changes
        }

        // This method is called when the selected property changes
        protected async Task HandleSelectedPropertyChangeAsync()
        {
            Type? newPropertyType = TypeIdentifier.GetPropertyTypeFromExpression(SelectedProperty?.PropertyExpression);

            if (AtomicPredicate != null)
            {
                AtomicPredicate.Member = GetPropertyName(SelectedProperty?.PropertyExpression);
            }

            // If the type has changed, trigger the PropertySelectTypeChanged event
            if (_previousPropertyType != newPropertyType)
            {
                await PropertySelectTypeChanged.InvokeAsync();
                _previousPropertyType = newPropertyType;
            }
            else
            {
                await PropertySelectChanged.InvokeAsync();
            }
        }

        protected Func<Property<T>, string, bool> SearchFunc =>
            (property, value) => property.ComputedTitle?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false;

        public static string? GetPropertyName(Expression<Func<T, object>>? expression)
        {
            if (expression is null)
            {
                return null;
            }

            return ExpressionGenerator.GetFullPropertyName(expression);
        }

        public void Dispose()
        {
            if (_currentAtomicPredicate != null)
            {
                _currentAtomicPredicate.PropertyChanged -= HandleAtomicPredicatePropertyChanged;
            }
        }
    }
}
