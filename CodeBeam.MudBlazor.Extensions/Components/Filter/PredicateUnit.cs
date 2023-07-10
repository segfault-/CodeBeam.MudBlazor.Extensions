using MudExtensions.Components.Filter;
using System.Text.Json.Serialization;

namespace MudExtensions
{
#nullable enable

    /// <summary>
    /// Base class for predicate units
    /// </summary>
    public abstract class PredicateUnit<T> : ObservableObject
    {
        protected PredicateUnit(PredicateUnit<T>? parent)
        {
            Parent = parent;
        }

        [JsonIgnore] public abstract PredicateUnit<T>? Parent { get; set; }

        public virtual bool? RemovePredicate(PredicateUnit<T> predicate) 
        { 
            return false; 
        }
    }
}