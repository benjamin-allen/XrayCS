using System.Collections.Generic;
using System;

namespace XrayCS
{
    internal class EventComparer : IComparer<Event>
    {
        public int Compare(Event x, Event y)
        {
            return x.Priority - y.Priority;
        }
    }

    class Publisher
    {
        private HashSet<Entity> _entities;
        private SortedSet<Event> _eventQueue;
        private int _numEntities;
        
        private HashSet<Entity> Entities { get => _entities; set => _entities = value; }
        private SortedSet<Event> EventQueue { get => _eventQueue; set => _eventQueue = value; }

        public int NumEntities { get => _numEntities; private set => _numEntities = value; }

        public Publisher()
        {
            Entities = new HashSet<Entity>();
            NumEntities = 0;
            EventQueue = new SortedSet<Event>(new EventComparer());
        }

        public bool AddEntity(Entity entity)
        {
            bool added = Entities.Add(entity);
            if (added == true)
            {
                NumEntities += 1; 
            }
            return added;
        }

        public bool RemoveEntity(Entity entity)
        {
            bool removed = Entities.Remove(entity);
            if(removed == true)
            {
                NumEntities -= 1;
            }
            return removed;
        }

        public bool AddEvent(Event @event)
        {
            return EventQueue.Add(@event);
        }

        public void ProcessEvent(Event @event)
        {
            foreach (Entity entity in Entities)
            {
                if (entity != null)
                {
                    @event.DispatchToEntity(entity);
                }
            }
        }

        public void ProcessQueue()
        {
            foreach (Event @event in EventQueue)
            {
                ProcessEvent(@event);
            }
        }
    }
}
