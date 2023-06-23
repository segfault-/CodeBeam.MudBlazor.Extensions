﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class AtomicPredicateComponent<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T> Filter { get; set; }
        [Parameter] public AtomicPredicate<T> AtomicPredicate { get; set; }

        public Task OnPropertySelectChangedAsync()
        {
            return Task.CompletedTask;
        }

        protected string ClassName => new CssBuilder("mud-atomic-predicate")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();
    }
}
