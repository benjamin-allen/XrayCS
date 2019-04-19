using System;

namespace XrayCS
{
    /// <summary>
    /// This is the base class for all Events. See documentation for instructions on how to extend
    /// this class.
    /// </summary>
    /// <remarks>We will implement the derived class DamageEvent.
    /// <code>
    /// class DamageEvent : Event
    /// {
    ///     private int _damage;
    ///     public int Damage { get => _damage; set => _damage = value; }
    ///     
    ///     public DamageEvent(int damage = 0) : 
    ///         base(null, new Type[] { typeof(HealthComponent) }, null)
    ///     {
    ///         Damage = damage;
    ///     }
    ///     
    ///     public override void CallOnMatch(Entity entity)
    ///     {
    ///         HealthComponent hc = entity.Get&lt;HealthComponent&gt;();
    ///         hc.Health -= Damage;
    ///     }
    /// }
    /// </code>
    /// <para>A real version of this event would likely implement multiple constructors, or call 
    /// the base constructor with the entity responsible for generating the DamageEvent. Access 
    /// of fields is not restricted; the only reserved words for events are _source, _matches,
    /// _excludes, and priority, and their associated properties.</para>
    /// <para>The <see cref="CallOnMatch(Entity)"/> method must be overridden by all derived
    /// events. Provided certain conditions are met, it is safe to assume that the entity passed
    /// through the arguments owns all components in the <see cref="Matches"/> list, and that it
    /// does not have any components in the <see cref="Excludes"/> list. This is guaranteed by the
    /// <see cref="DispatchToEntity(Entity)"/> method, which is called by the 
    /// <see cref="Publisher"/> object. These methods are public, however, and their descriptions
    /// outline what, if any, edge conditions they can manage.</para>
    /// </remarks>
    public abstract class Event
    {
        private Entity _source;
        private Type[] _matches;
        private Type[] _excludes;
        private int priority;

        /// <summary>
        /// The <see cref="Entity"/> responsible for generating this event.
        /// </summary>
        public Entity Source { get => _source; private set => _source = value; }

        /// <summary>
        /// The list of types an entity must have to be processed by this event.
        /// </summary>
        public Type[] Matches { get => _matches; protected set => _matches = value; }

        /// <summary>
        /// The list of types an entity must lack to be processed by this event.
        /// </summary>
        public Type[] Excludes { get => _excludes; protected set => _excludes = value; }

        /// <summary>
        /// Determines the position of the event in a <see cref="Publisher"/> event queue.
        /// Highest values are processed last, and events with the same priority are processed
        /// last-in-first-out.
        /// </summary>
        public int Priority { get => priority; set => priority = value; }

        /// <summary>
        /// Constructs a new event.
        /// </summary>
        /// <param name="source">The entity responsible for creating this event. </param>
        /// <param name="matches">An entity processed by this event should have all of these
        /// components.</param>
        /// <param name="excludes">An entity processed by this event should have none of these
        /// components.</param>
        /// <remarks>
        /// The <paramref name="source"/> entity is not used in any library code. It's provided as
        /// a convenience for developers in case an event depends on some interplay between an
        /// event creator and the entity being called on it. This field is most useful when
        /// events are directly dispatched instead of processed with the <see cref="Publisher"/>.
        /// </remarks>
        public Event(Entity source = null, Type[] matches = null, Type[] excludes = null)
        {
            Priority = int.MaxValue;
            Source = source;
            if (matches == null)
            {
                Matches = new Type[0];
            }
            else
            {
                Matches = matches;
            }
            if (excludes == null)
            {
                Excludes = new Type[0];
            }
            else
            {
                Excludes = excludes;
            }
        }

        /// <summary>
        /// Implementation of events occurs by overwriting this method.
        /// </summary>
        /// <param name="entity">The entity this event is occuring to. Provided so that events can
        /// easily modify entity data.</param>
        public abstract void CallOnMatch(Entity entity);

        /// <summary>
        /// Calls <see cref="CallOnMatch(Entity)"/> on the <paramref name="entity"/> if it has the
        /// necessary component configuration. This is the preferred way to send an event to an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void DispatchToEntity(Entity entity)
        {
            if (entity.HasExcluding(Matches, Excludes))
            {
                CallOnMatch(entity);
            }
        }
    }
}
