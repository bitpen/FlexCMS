using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bCommon.Validation
{
    /// <summary>
    /// Abstract base for a business layer model that implements validatable features
    /// </summary>
    public abstract class AbstractValidatableBLM<TKey, TValue>
    {
        public class ValidationErrors
        {
            private List<KeyValuePair<TKey, TValue>> errors;
            private int count;

            public ValidationErrors()
            {
                errors = new List<KeyValuePair<TKey, TValue>>();
                count = 0;
            }

            public void Add(TKey key, TValue value)
            {
                errors.Add(new KeyValuePair<TKey, TValue>(key, value));
                Count();
            }

            public Boolean Any()
            {
                return count > 0;
            }

            public int Count()
            {
                count = errors.Count;
                return count;
            }

            public Lookup<TKey, TValue> ToLookup()
            {
                return (Lookup<TKey, TValue>)errors.ToLookup(i => i.Key, i => i.Value);
            }
        }

        public enum ValidatableFields
        {

        }

        public ValidationErrors CreateValidationErrorsCollection()
        {
            return new ValidationErrors();
        }
    }
}