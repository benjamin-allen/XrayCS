using System.Collections.Generic;
using System;

namespace XrayCS
{
    /// <summary>
    /// Used to sort events in descending priority.
    /// </summary>
    /// <remarks>
    /// This is technically backwards of how a normal IComparer is implemented, but it is done
    /// this way so that the next event to be processed is at the end of the list, which should
    /// cause removals to be smoother than removing from the front of the list.
    /// </remarks>
    internal class EventComparer : IComparer<Event>
    {
        /// <summary>
        /// Implements the IComparer interface. Note that this is reversed from a standard Compare
        /// function.
        /// </summary>
        /// <param name="x">The first eventt.</param>
        /// <param name="y">The second event.</param>
        /// <returns>The relative priorities of the events.</returns>
        public int Compare(Event x, Event y)
        {
            return y.Priority - x.Priority; // backwards so that the last element is the highest priority
        }
    }

    /// <summary>
    /// A class that manages events and entities. <see cref="Publisher"/> recreates the "systems"
    /// portion of the entity-component-system paradigm by allowing events to be passed to many
    /// entities.
    /// </summary>
    public class Publisher
    {
        private HashSet<Entity> _entities;
        private List<Event> _eventQueue;
        private int _numEntities;
        private readonly EventComparer _comparer = new EventComparer();

        /// <summary>
        /// The set of entities managed by the publisher.
        /// </summary>
        /// <remarks>A HashSet is used to avoid duplication of entities.</remarks>
        private HashSet<Entity> Entities { get => _entities; set => _entities = value; }

        /// <summary>
        /// A list of the events currently awaiting processing in the publisher, sorted by their
        /// priority. Numerically lower priorities are processed first.
        /// </summary>
        private List<Event> EventQueue { get => _eventQueue; set => _eventQueue = value; }

        /// <summary>
        /// The number of entities managed by this publisher.
        /// </summary>
        public int NumEntities { get => _numEntities; private set => _numEntities = value; }

        /// <summary>
        /// Used to compare event priorities for sorting.
        /// </summary>
        internal EventComparer Comparer => _comparer;

        /// <summary>
        /// Constructs a new <see cref="Publisher"/> with no entities or events.
        /// </summary>
        public Publisher()
        {
            Entities = new HashSet<Entity>();
            NumEntities = 0;
            EventQueue = new List<Event>();
        }

        /// <summary>
        /// Adds an entity to the publisher's management.
        /// </summary>
        /// <param name="entity">The entity to be added. A ref of this entity will be kept in the
        /// <see cref="HashSet{T}"/> owned by this <see cref="Publisher"/></param>
        /// <returns>True if the entity was successfully added, false otherwise.</returns>
        public bool AddEntity(Entity entity)
        {
            bool added = Entities.Add(entity);
            if (added == true)
            {
                NumEntities += 1;
            }
            return added;
        }

        /// <summary>
        /// Removes an entity from the publisher's management.
        /// </summary>
        /// <param name="entity">A reference to the entity to remove.</param>
        /// <returns>True if the entity was successfully removed, false otherwise.</returns>
        public bool RemoveEntity(Entity entity)
        {
            bool removed = Entities.Remove(entity);
            if (removed == true)
            {
                NumEntities -= 1;
            }
            return removed;
        }

        /// <summary>
        /// Adds an event to the publisher's queue.
        /// </summary>
        /// <param name="event">A reference to the event to add.</param>
        public void AddEvent(Event @event)
        {
            EventQueue.Add(@event);
            EventQueue.Sort(Comparer);
        }

        /// <summary>
        /// Immediately sends an event to all entities managed by the publisher.
        /// </summary>
        /// <param name="event">The event to send.</param>
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

        /// <summary>
        /// Sends the highest-priority event in the queue to all entities managed by the publisher.
        /// </summary>
        public void ProcessTopEvent()
        {
            Event @event = EventQueue[EventQueue.Count - 1];
            EventQueue.RemoveAt(EventQueue.Count - 1);
            ProcessEvent(@event);
        }

        /// <summary>
        /// Sends all events in the queue to all entities managed by the publisher.
        /// </summary>
        public void ProcessQueue()
        {
            while (EventQueue.Count > 0)
            {
                ProcessTopEvent();
            }
        }
    }
}
