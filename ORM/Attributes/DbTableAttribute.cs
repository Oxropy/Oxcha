using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute :Attribute
    {
        public string Name;
    }
}
