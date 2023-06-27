using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.Text.Json;

namespace MudExtensions
{
#nullable enable
    public partial class ValueFieldComponent<T> : MudComponentBase
    {
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback ValueFieldChanged { get; set; }

        protected FieldType? FieldType;

        private object? _valueObject;
        protected object? ValueObject
        {
            get => _valueObject;
            set
            {
                _valueObject = value;
                if(AtomicPredicate is not null)
                {
                    AtomicPredicate.Value = value;
                }             
            }
        }

        private string? _valueString;
        protected string? ValueString
        {
            get => _valueString;
            set
            {
                _valueString = value;
                ValueObject = value;
            }
        }

        private double? _valueNumber;
        protected double? ValueNumber
        {
            get => _valueNumber;
            set
            {
                _valueNumber = value;
                ValueObject = value;
            }
        }

        private Enum? _valueEnum;
        protected Enum? ValueEnum
        {
            get => _valueEnum;
            set
            {
                _valueEnum = value;
                ValueObject = value;
            }
        }

        private bool? _valueBool;
        protected bool? ValueBool
        {
            get => _valueBool;
            set
            {
                _valueBool = value;
                ValueObject = value;
            }
        }

        private DateTime? _valueDateTime;
        protected DateTime? ValueDate
        {
            get => _valueDateTime;
            set
            {
                _valueDateTime = value;
                ValueObject = value;
            }
        }

        private TimeSpan? _valueTime;
        protected TimeSpan? ValueTime
        {
            get => _valueTime;
            set
            {
                _valueTime = value;
                ValueObject = value;
            }
        }

        private Guid? _valueGuid;
        protected Guid? ValueGuid
        {
            get => _valueGuid;
            set
            {
                _valueGuid = value;
                ValueObject = value;
            }
        }

        private bool _isMultiSelect;
        protected bool IsMultiSelect
        {
            get => _isMultiSelect;
            set
            {
                _isMultiSelect = value;
            }
        }

        protected string ClassName => new CssBuilder("mud-value-field")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();


        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(AtomicPredicate is not null)
            {


                // member seralized is the correct string representation.  deseralization cant set a string type to a get only property.
                // ?? hmm..... custom deserilzer?



                FieldType = FieldType.Identify(AtomicPredicate.MemberType);
                ValueObject = AtomicPredicate.Value;
                if (ValueObject is not null && FieldType is not null)
                {
                    if (FieldType.IsString)
                    {
                        ValueString = (string)ValueObject;
                    }

                    if (FieldType.IsNumber)
                    {
                        ValueNumber = (double)ValueObject;
                    }

                    if(FieldType.IsEnum)
                    {
                        ValueEnum = (Enum)ValueObject;
                    }

                    if (FieldType.IsBoolean)
                    {
                        ValueBool = (bool)ValueObject;
                    }

                    if(FieldType.IsDateTime)
                    {
                        ValueDate = (DateTime)ValueObject;
                        ValueTime = (TimeSpan)ValueObject;
                    }

                    if(FieldType.IsGuid)
                    {
                        ValueGuid = (Guid)ValueObject;
                    }
                }
            }         
        }

        protected async Task OnValueFieldChangedAsync()
        {
            await ValueFieldChanged.InvokeAsync();
        }

    }
}
