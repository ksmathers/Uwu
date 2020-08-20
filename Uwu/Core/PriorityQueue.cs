using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{
    /// <summary>
    /// A queue of objects of type T each of which is associated with a priority.   Objects can be inserted at any priority
    /// level and will be sorted into the overall list.  The lowest priority item can be removed from the list, or alternatively
    /// only removed if the priority is below a specified level.
    /// 
    /// Using Runtime.Milliseconds for the priority can be used for realtime future event scheduling provided that the queue
    /// is checked for expiring events on a periodic basis.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> where T : class
    {
        List<int> priorities;
        List<T> items;
        public PriorityQueue()
        {
            priorities = new List<int>();
            items = new List<T>();
        }

        /// <summary>
        /// Inserts an item into the queue
        /// </summary>
        /// <param name="prio">lowest first</param>
        /// <param name="item"></param>
        public void Insert(int prio, T item)
        {
            lock (this) {
                int i;
                for (i = 0; i < priorities.Count; i++) {
                    if (priorities[i] > prio) {
                        break;
                    }
                }
                if (i < priorities.Count) {
                    priorities.Insert(i, prio);
                    items.Insert(i, item);
                } else {
                    priorities.Add(prio);
                    items.Add(item);
                }
            }
        }

        /// <summary>
        /// Returns lowest priority item in the queue, which is the first one that could be returned.
        /// </summary>
        public int MinPriority {
            get {
                int prio = -1;
                lock (this) {
                    if (priorities.Count > 0) prio = priorities[0];
                }
                return prio;
            }
        }

        /// <summary>
        /// removes an item from the queue if its priority is below the given level
        /// </summary>
        /// <param name="prio">the priority limit</param>
        /// <returns>item or null</returns>
        public T RemoveIfLessThan(int prio)
        {
            T item;
            lock (this) {
                if (MinPriority >= prio) return null;
                item = Remove();
            }
            return item;
        }

        /// <summary>
        /// removes an item from the queue
        /// </summary>
        /// <returns>item or null</returns>
        public T Remove()
        {
            T item;
            lock (this) {
                if (priorities.Count == 0) return null;
                item = this.items[0];
                this.items.RemoveAt(0);
                this.priorities.RemoveAt(0);
            }
            return item;
        }
    }
}
