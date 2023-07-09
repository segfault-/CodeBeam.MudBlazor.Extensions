﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public partial class PropertySelectComponent<T> : MudComponentBase, IDisposable
    {
        private AtomicPredicate<T>? _internalAtomicPredicate;
        private Type? _oldPropertyType;

        [Parameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback PropertySelectChanged { get; set; }
        [Parameter] public EventCallback PropertySelectTypeChanged { get; set; }

        protected Property<T>? Property { get; set; }
        protected string ClassName => new CssBuilder("mud-property-select")
            .AddClass(Class)
            .Build();
        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();


        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (_internalAtomicPredicate != AtomicPredicate)
            {
                if (_internalAtomicPredicate != null)
                {
                    _internalAtomicPredicate.PropertyChanged -= HandlePropertyChanged;
                }

                _internalAtomicPredicate = AtomicPredicate;

                if (_internalAtomicPredicate != null)
                {
                    _internalAtomicPredicate.PropertyChanged += HandlePropertyChanged;
                }
            }

            await base.SetParametersAsync(parameters);

            if (AtomicPredicate is not null)
            {
                Property = Filter?.Properties?.SingleOrDefault(p => GetPropertyName(p.PropertyExpression) == AtomicPredicate.Member);
                _oldPropertyType = TypeIdentifier.GetPropertyTypeFromExpression(Property?.PropertyExpression);
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals("Member") ?? false)
            {
            }
        }

        protected async Task OnPropertyChangedAsync()
        {
            Type? newType = TypeIdentifier.GetPropertyTypeFromExpression(Property?.PropertyExpression);

            if (AtomicPredicate is not null)
            {
                AtomicPredicate.Member = GetPropertyName(Property?.PropertyExpression);
            }

            // Trigger the PropertySelectChanged event
            await PropertySelectChanged.InvokeAsync();

            if (_oldPropertyType != newType)
            {
                Console.WriteLine($"PropertySelectComponent::OnPropertyChangedAsync : {_oldPropertyType} {newType}");
                await PropertySelectTypeChanged.InvokeAsync();
                _oldPropertyType = newType; // update old type to the new type
            }
        }

        protected Func<Property<T>, string, bool> SearchFunc => (property, value) => property.ComputedTitle?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false;


        public static string? GetPropertyName(Expression<Func<T, object>>? expression)
        {
            if (expression is null)
            {
                return null;
            }
            else if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
            {
                // If the property is nullable, expression.Body will be an UnaryExpression
                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }

            throw new ArgumentException("Invalid expression", nameof(expression));
        }

        public void Dispose()
        {
            if (_internalAtomicPredicate != null)
            {
                _internalAtomicPredicate.PropertyChanged -= HandlePropertyChanged;
            }
        }
    }
}
