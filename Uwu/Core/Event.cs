using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{
    /// <summary>
    /// Base class for Events.   Subclass to provide event constructors by name.
    /// </summary>
    public class Event
    {
        string name;
        object data;
        public Event(string name, object data)
        {
            this.name = name;
            this.data = data;
        }

        public T Data<T>() where T : class
        {
            return data as T;
        }

        public string Name { get { return name; } }

    }
}
