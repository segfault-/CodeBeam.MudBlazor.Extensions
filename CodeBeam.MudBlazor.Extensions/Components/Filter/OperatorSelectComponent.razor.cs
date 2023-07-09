using Microsoft.AspNetCore.Components;
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

        private string? _operator;
        protected string? Operator
        {
            get => _operator;
            set
            {
                _operator = value;
                if(AtomicPredicate is not null)
                {
                    AtomicPredicate.Operator = value;
                }
            }
        }

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
                }
            }

            await base.SetParametersAsync(parameters);

            Console.WriteLine($"OperatorSelectComponent::SetParametersAsync : {Operator} --> {AtomicPredicate.Operator}");
            if(AtomicPredicate is not null)
            {
                Operator = AtomicPredicate.Operator;
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals(nameof(AtomicPredicate<T>.Member)) ?? false)
            {
                Operator = null;
                InvokeAsync(StateHasChanged);
            }
            else if (e.PropertyName?.Equals("Operator") ?? false)
            {
                //await OperatorChanged.InvokeAsync();
                //if (AtomicPredicate is not null)
                //{
                //    AtomicPredicate.Value = null;
                //}
            }
        }


        protected async Task OnOperatorSelectChangedAsync()
        {
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
