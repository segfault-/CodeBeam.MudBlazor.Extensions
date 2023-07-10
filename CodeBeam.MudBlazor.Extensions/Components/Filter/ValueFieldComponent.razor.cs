using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.ComponentModel;

namespace MudExtensions
{
#nullable enable
    public partial class ValueFieldComponent<T> : MudComponentBase, IDisposable
    {
        private AtomicPredicate<T>? _internalAtomicPredicate;

        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback ValueFieldChanged { get; set; }

        protected FieldType? FieldType;


        private IEnumerable<string>? _multiSelectValues;
        protected IEnumerable<string>? MultiSelectValues 
        {
            get => _multiSelectValues;
            set
            {
                _multiSelectValues = value;
                if (AtomicPredicate is not null)
                {
                    AtomicPredicate.MultiSelectValues = value;
                }
            }
        }

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

        private string? _valueEnum;
        protected string? ValueEnum
        {
            get => _valueEnum;
            set
            {
                _valueEnum = value;
                if (FieldType is not null && FieldType.InnerType is not null && !string.IsNullOrWhiteSpace(value))
                {
                    if ((AtomicPredicate?.Operator ?? "").Equals("is one of") || (AtomicPredicate?.Operator ?? "").Equals("is not one of"))
                    {
                        // for "is one of / is not one of" operator, value should be a comma-separated string
                        MultiSelectValues = value.Split(',', StringSplitOptions.TrimEntries);
                    }
                    else
                    {
                        MultiSelectValues = new List<string> { value };
                        ValueObject = Enum.Parse(FieldType.InnerType, value);
                    }
                }
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
                if(ValueDate is not null && ValueTime is not null)
                {
                    ValueDate = ValueDate.Value.Date + ValueTime.Value;
                   
                }
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

        protected string ClassName => new CssBuilder("mud-value-field")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// Take control of the parameter setting process.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
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

            Console.WriteLine($"ValueFieldComponent::SetParametersAsync : AtomicPredicate.Value = {AtomicPredicate?.Value}");


            await base.SetParametersAsync(parameters);
            AssignValuesFromAtomicPredicate();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Perform updates based on changes
            // Check e.PropertyName for specific property changes if needed
            if(e.PropertyName?.Equals(nameof(AtomicPredicate<T>.Operator)) ?? false)
            {
                if ((AtomicPredicate?.Operator ?? "").Equals("is one of") || (AtomicPredicate?.Operator ?? "").Equals("is not one of"))
                {
                    // for "is one of / is not one of" operator, ValueString should be a comma-separated string
                    ValueString = string.Join(",", MultiSelectValues ?? Enumerable.Empty<string>());
                }
                else
                {
                    // for "is / is not" operator, ValueString should be updated to MultiSelectValues.FirstOrDefault
                    ValueString = MultiSelectValues?.FirstOrDefault();
                }
                AssignValuesFromAtomicPredicate();
            }

            Console.WriteLine($"ValueFieldComponent::HandlePropertyChanged : AtomicPredicate.Value = {AtomicPredicate?.Value}");

        }

        private void AssignValuesFromAtomicPredicate()
        {
            if (AtomicPredicate is null)
            {
                return;
            }

            if (AtomicPredicate.Operator is null)
            {
                ValueBool = null;
                ValueDate = null;
                ValueEnum = null;
                ValueGuid = null;
                ValueNumber = null;
                ValueObject = null;
                ValueString = null;
                ValueTime = null;
            }
            else
            {
                FieldType = FieldType.Identify(AtomicPredicate.MemberType);
                ValueObject = AtomicPredicate.Value;

                if (AtomicPredicate.Operator.Equals(FilterOperator.String.IsOneOf) || AtomicPredicate.Operator.Equals(FilterOperator.String.IsNotOneOf))
                {
                    ValueString = Convert.ToString(ValueObject);
                    MultiSelectValues = ValueString?.Split(',', StringSplitOptions.TrimEntries);
                }
                else
                {
                    if (FieldType.IsString)
                    {
                        ValueString = FieldType.ConvertToString(ValueObject);
                    }

                    if (FieldType.IsNumber)
                    {
                        ValueNumber = FieldType.ConvertToDouble(ValueObject);
                    }

                    if (FieldType.IsEnum)
                    {
                        ValueEnum = FieldType.ConvertToString(ValueObject);
                    }

                    if (FieldType.IsBoolean)
                    {
                        ValueBool = FieldType.ConvertToBoolean(ValueObject);
                    }

                    if (FieldType.IsDateTime)
                    {
                        ValueDate = FieldType.ConvertToDateTime(ValueObject);
                        if (ValueDate is not null)
                        {
                            ValueTime = ValueDate.Value.TimeOfDay;
                        }
                        else
                        {
                            ValueTime = null;
                        }
                    }

                    if (FieldType.IsGuid)
                    {
                        ValueGuid = FieldType.ConvertToGuid(ValueObject);
                    }
                }
            }
        }

        public static object? ConvertToEnum(Type enumType, object value)
        {
            if (value is null || !enumType.IsEnum)
            {
                return null;
            }

            if (Enum.IsDefined(enumType, value))
            {
                return Enum.ToObject(enumType, value);
            }

            return null;
        }

        protected async Task OnValueFieldChangedAsync()
        {
            await ValueFieldChanged.InvokeAsync();
        }

        public void Dispose()
        {
            if (_internalAtomicPredicate is not null)
            {
                _internalAtomicPredicate.PropertyChanged -= HandlePropertyChanged;
            }
        }
    }
}
