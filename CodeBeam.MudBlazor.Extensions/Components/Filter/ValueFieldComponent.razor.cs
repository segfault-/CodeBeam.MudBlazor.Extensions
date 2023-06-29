﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System;

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
                if (AtomicPredicate is not null)
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
                if(FieldType is not null && FieldType.InnerType is not null && value is not null)
                {
                    ValueObject = Enum.Parse(FieldType.InnerType, value);
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


        public override async Task SetParametersAsync(ParameterView parameters)
        {
            Console.WriteLine("--> ValueFieldComponent<T>:SetParametersAsync()");
            await base.SetParametersAsync(parameters);

            if (parameters.TryGetValue<AtomicPredicate<T>>("AtomicPredicate", out var atomicPredicate))
            {
                AtomicPredicate = atomicPredicate;
                Console.WriteLine($"SomeParameter: {AtomicPredicate}");

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
                        ValueNumber = Convert.ToDouble(ValueObject);
                    }

                    if (FieldType.IsEnum)
                    {
                        ValueEnum = ValueObject.ToString();
                    }

                    if (FieldType.IsBoolean)
                    {
                        ValueBool = (bool)ValueObject;
                    }

                    if (FieldType.IsDateTime)
                    {
                        if (ValueObject is DateTime dateTime)
                        {
                            ValueDate = dateTime;
                            ValueTime = dateTime.TimeOfDay;
                        }
                    }

                    if (FieldType.IsGuid)
                    {
                        ValueGuid = (Guid)ValueObject;
                    }
                }
            }

            if (parameters.TryGetValue<EventCallback>("ValueFieldChanged", out var valueFieldChanged))
            {
                ValueFieldChanged = valueFieldChanged;
                Console.WriteLine($"SomeParameter: {ValueFieldChanged}");
            }

        }

        protected override void OnParametersSet()
        {
            if (AtomicPredicate is not null)
            {

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
                        ValueNumber = Convert.ToDouble(ValueObject);
                    }

                    if (FieldType.IsEnum)
                    {
                        ValueEnum = ValueObject.ToString();
                    }

                    if (FieldType.IsBoolean)
                    {
                        ValueBool = (bool)ValueObject;
                    }

                    if (FieldType.IsDateTime)
                    {
                        if (ValueObject is DateTime dateTime)
                        {
                            ValueDate = dateTime;
                            ValueTime = dateTime.TimeOfDay;
                        }
                    }

                    if (FieldType.IsGuid)
                    {
                        ValueGuid = (Guid)ValueObject;
                    }
                }
            }
            base.OnParametersSet();
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
    }
}
