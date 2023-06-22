// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;

namespace MudExtensions
{
    public partial class RuleView<T> : MudComponentBase
    {
        [Parameter] public MudTreeViewItemM3<FilterRule<T>> Leaf { get; set; } = new();

        [CascadingParameter(Name = "Root")] public MudFilter<T> Root { get; set; }

        private Condition? ParentCondition
        {
            get
            {
                if (Leaf.Parent != null)
                {
                    return Leaf.Parent.Value.Condition;
                }
                else
                {
                    return Condition.AND;
                }
            }
        }

        private object _valueObject;
        private string _valueString;
        private double? _valueNumber;
        private Enum _valueEnum = null;
        private bool? _valueBool;
        private DateTime? _valueDate;
        private TimeSpan? _valueTime;
        private bool _isMultSelect;


        private string Operator
        {
            get => Leaf.Value.Operator;
            set
            {
                if (value != null)
                {
                    if (value.Equals("is one of") || value.Equals("is not one of"))
                    {
                        _isMultSelect = true;
                    }
                    else if (!string.IsNullOrEmpty(Operator))
                    {
                        if (Leaf.Value.Operator.Equals("is one of") || Leaf.Value.Operator.Equals("is not one of"))
                        {
                            ClearValueStates();
                            Leaf.Value.Value = null;
                        }

                        MultiSelectValues = new HashSet<string>();
                        _isMultSelect = false;
                    }
                }
                Leaf.Value.Operator = value;
            }
        }

        private IEnumerable<string> MultiSelectValues { get; set; } = new HashSet<string>();

        protected bool HasChildren
        {
            get
            {
                var retVal = Leaf.Items.Any();
                return retVal;
            }
        }

        protected override Task OnParametersSetAsync()
        {
            if (Leaf.Value == null)
            {
                return base.OnParametersSetAsync();
            }

            if (Leaf.Value.FieldType.IsString)
            {
                if (Leaf.Value.Value is JsonElement element)
                {
                    _valueString = element.GetString();
                }
                else
                {
                    _valueString = Leaf.Value.Value?.ToString();
                }
            }
            else if (Leaf.Value.FieldType.IsNumber)
            {
                if (Leaf.Value.Value is JsonElement element)
                {
                    _valueNumber = element.GetDouble();
                }
                else
                {
                    _valueNumber = Leaf.Value.Value == null ? null : Convert.ToDouble(Leaf.Value.Value);
                }
            }
            else if (Leaf.Value.FieldType.IsEnum)
            {
                if (Leaf.Value.Value == null)
                {
                    _valueEnum = null;
                }
                else
                {
                    if (Leaf.Value.Value.GetType().IsEnum)
                    {
                        _isMultSelect = false;
                        _valueEnum = (Enum)Leaf.Value.Value;
                    }
                    else if (Leaf.Value.Value is string strVal)
                    {
                        _isMultSelect = true;
                        if (!string.IsNullOrWhiteSpace(strVal))
                        {
                            MultiSelectValues = strVal.Split(',').Select(v => v.Trim()).ToList();
                        }
                    }
                    else if (Leaf.Value.Value.GetType() == typeof(object))
                    {
                        _isMultSelect = true;
                        MultiSelectValues = (IEnumerable<string>)Leaf.Value.Value;
                    }
                    else
                    {
                        if (Leaf.Value.Operator.Equals("is one of") || Leaf.Value.Operator.Equals("is not one of"))
                        {
                            _isMultSelect = true;
                            if (Leaf.Value.Value is JsonElement element)
                            {
                                MultiSelectValues = element.ToString().Split(',').Select(v => v.Trim()).ToList();
                            }
                            else
                            {
                                MultiSelectValues = (IEnumerable<string>)Leaf.Value.Value;
                            }
                        }
                        else
                        {
                            var t = Leaf.Value.FieldType;
                            var tt = t.InnerType;
                            var v = (Enum)Enum.ToObject(tt, ((JsonElement)Leaf.Value.Value).GetInt32());
                            _valueEnum = v;
                        }
                    }
                }
            }
            else if (Leaf.Value.FieldType.IsBoolean)
            {
                if (Leaf.Value.Value is JsonElement element)
                {
                    _valueBool = element.GetBoolean();
                }
                else
                {
                    _valueBool = Leaf.Value.Value == null ? null : Convert.ToBoolean(Leaf.Value.Value);
                }
            }
            else if (Leaf.Value.FieldType.IsDateTime)
            {
                if (Leaf.Value.Value is JsonElement element)
                {
                    var dateTime = element.GetDateTime();
                    _valueDate = Leaf.Value.Value == null ? null : dateTime;
                    _valueTime = Leaf.Value.Value == null ? null : dateTime.TimeOfDay;
                }
                else
                {
                    var dateTime = Convert.ToDateTime(Leaf.Value.Value);
                    _valueDate = Leaf.Value.Value == null ? null : dateTime;
                    _valueTime = Leaf.Value.Value == null ? null : dateTime.TimeOfDay;
                }
            }
            return base.OnParametersSetAsync();
        }

        protected void SetButtonText(int id)
        {
            switch (id)
            {
                case 0:
                    Leaf.Value.Condition = Condition.AND;
                    break;
                case 1:
                    Leaf.Value.Condition = Condition.OR;
                    break;
            }
        }

        protected void ClearValueStates()
        {
            _valueObject = null;
            _valueString = null;
            _valueNumber = null;
            _valueEnum = null;
            _valueBool = null;
            _valueDate = null;
            _valueTime = null;
            _isMultSelect = false;
        }

        protected void AddExpressionRule()
        {
            if (!Leaf.Items.Any())
            {
                SetLeafCondition();
            }

            Leaf.Items.Add(CloneRule());
            //Leaf.Value.Field = null;
            //Leaf.Value.Operator = null;
            Leaf.Value.Value = null;
            Leaf.Value.IsExpanded = true;
            Leaf.Value.Disabled = false;
        }

        private void SetLeafCondition()
        {
            Condition? condition = null;
            if (Leaf.Parent != null)
            {
                condition = Leaf.Parent.Value.Condition;
            }
            else if (Root != null)
            {
                //condition = Root.FilterRoot.Condition;
            }

            if (condition != null)
            {
                if (condition == Condition.AND)
                {
                    condition = Condition.OR;
                }
                else
                {
                    condition = Condition.AND;
                }
            }
            else
            {
                condition = Condition.AND;
            }

            Leaf.Value.Condition = condition;
        }

        private FilterRule<T> CloneRule()
        {
            if (MultiSelectValues.Any())
            {
                var rr = new FilterRule<T>(null)
                {
                };
                return rr;
            }

            var r = new FilterRule<T>(null)
            {
            };
            return r;
        }


        protected void RemoveExpressionRule()
        {
            Root?.RemoveRule(Leaf.Value);
            Leaf.Parent?.Items.Remove(Leaf.Value);
        }

        internal void StringValueChanged(string value)
        {
            Leaf.Value.Value = value;
            _valueString = value;
        }

        internal void NumberValueChanged(double? value)
        {
            Leaf.Value.Value = value;
            _valueNumber = value;
        }

        internal void EnumValueChanged(Enum value)
        {
            Leaf.Value.Value = value;
            _valueEnum = value;
        }

        internal void ObjectValueChanged(object value)
        {
            Leaf.Value.Value = value;
            _valueObject = value;
        }

        internal void BoolValueChanged(bool? value)
        {
            Leaf.Value.Value = value;
            _valueBool = value;
        }

        internal void DateValueChanged(DateTime? value)
        {
            _valueDate = value;

            if (value != null)
            {
                var date = value.Value.Date;

                // get the time component and add it to the date.
                if (_valueTime != null)
                {
                    date = date.Add(_valueTime.Value);
                }
                Leaf.Value.Value = date;
            }
            else
            {
                Leaf.Value.Value = value;
            }
        }

        internal void TimeValueChanged(TimeSpan? value)
        {
            _valueTime = value;

            if (_valueDate != null)
            {
                var date = _valueDate.Value.Date;

                // get the time component and add it to the date.
                if (_valueTime != null)
                {
                    date = date.Add(_valueTime.Value);
                }

                Leaf.Value.Value = date;
            }
        }
    }
}
