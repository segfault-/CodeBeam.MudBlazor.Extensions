﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.ComponentModel;

namespace MudExtensions
{
#nullable enable
    public partial class OperatorSelectComponent<T> : MudComponentBase, IDisposable
    {
        private AtomicPredicate<T>? _internalAtomicPredicate;

        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback OperatorChanged { get; set; }
        [Parameter] public EventCallback OperatorTypeChanged { get; set; }

        protected string ClassName => new CssBuilder("mud-operator-select")
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
                    // Handle initial state here if needed
                }
            }

            await base.SetParametersAsync(parameters);
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Perform updates based on changes
            // Check e.PropertyName for specific property changes if needed
            Console.WriteLine($"OperatorSelectComponent {e.PropertyName} has changed");

            if(e.PropertyName?.Equals("Member") ?? false)
            {
                if (AtomicPredicate is not null)
                {
                    //if(AtomicPredicate.Operator != Operator)
                    //{
                        AtomicPredicate.Operator = null;
                    //}                    
                }
            }
        }

        protected async Task OnOperatorSelectChangedAsync()
        {
            //if (AtomicPredicate is not null)
            //{
            //    AtomicPredicate.Operator = Operator;
            //}
            await OperatorChanged.InvokeAsync();
        }

        protected Func<string, string, bool> SearchFunc => (op, value) => op.Contains(value, StringComparison.OrdinalIgnoreCase);


        public void Dispose()
        {
            if (_internalAtomicPredicate != null)
            {
                _internalAtomicPredicate.PropertyChanged -= HandlePropertyChanged;
            }
        }

    }
}
