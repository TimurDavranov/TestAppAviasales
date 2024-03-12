using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.Primitives
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
