using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bCommon.Validation
{
    public class ValidationErrors<TKey, TValue>
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
}
