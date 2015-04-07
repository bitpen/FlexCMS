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
        public enum ValidatableFields
        {

        }

        public ValidationErrors<TKey, TValue> CreateValidationErrorsCollection()
        {
            return new ValidationErrors<TKey, TValue>();
        }
    }
}