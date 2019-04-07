using System.Collections.Generic;
using System;

namespace XrayCS
{
    internal class EventComparer : IComparer<Event>
    {
        public int Compare(Event x, Event y)
        {
            return y.Priority - x.Priority; // backwards so that the last element is the highest priority
        }
    }

    class Publisher
    {
        private HashSet<Entity> _entities;
        private List<Event> _eventQueue;
        private int _numEntities;
        private readonly EventComparer _comparer = new EventComparer();
        
        private HashSet<Entity> Entities { get => _entities; set => _entities = value; }
        private List<Event> EventQueue { get => _eventQueue; set => _eventQueue = value; }

        public int NumEntities { get => _numEntities; private set => _numEntities = value; }

        internal EventComparer Comparer => _comparer;

        public Publisher()
        {
            Entities = new HashSet<Entity>();
            NumEntities = 0;
            EventQueue = new List<Event>();
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

        public void AddEvent(Event @event)
        {
            EventQueue.Add(@event);
            EventQueue.Sort(Comparer);
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

        public void ProcessTopEvent()
        {
            Event @event = EventQueue[EventQueue.Count - 1];
            EventQueue.RemoveAt(EventQueue.Count - 1);
            ProcessEvent(@event);
        }

        public void ProcessQueue()
        {
            while(EventQueue.Count > 0)
            {
                ProcessTopEvent();
            }
        }
    }
}
