﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
#nullable enable
    public partial class AtomicPredicateComponent<T> : MudComponentBase
    {
        [Parameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public CompoundPredicate<T>? Parent { get; set; }
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator LogicalOperator { get; set; }
        [Parameter] public uint Depth { get; set; }
        [Parameter] public EventCallback AtomicPredicateComponentChanged { get; set; }
        protected string ClassName => new CssBuilder("mud-atomic-predicate")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected async Task OnPropertySelectChangedAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:OnPropertySelectChangedAsync()");
            AtomicPredicate?.ClearOperatorAndValues();
            await AtomicPredicateComponentChanged.InvokeAsync();

        }

        protected async Task OnValueFieldChangedAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:OnValueFieldChangedAsync()");
            await AtomicPredicateComponentChanged.InvokeAsync();
        }

        protected async Task OnOperatorSelectChangedAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:OnOperatorSelectChangedAsync()");
            await AtomicPredicateComponentChanged.InvokeAsync();
        }

        protected async Task RemovePredicateUnitAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:RemovePredicateUnitAsync()");
            AtomicPredicate?.Remove();
            await AtomicPredicateComponentChanged.InvokeAsync();
        }
    }
}
