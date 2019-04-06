using System;

namespace XrayCS
{
    public abstract class Event
    {
        private Entity _source;
        private Type[] _matches;
        private Type[] _excludes;
        private string _data;

        public Entity Source { get => _source; private set => _source = value; }
        public Type[] Matches { get => _matches; private set => _matches = value; }
        public Type[] Excludes { get => _excludes; private set => _excludes = value; }
        public string Data { get => _data; set => _data = value; }

        public Event(Entity source = null, string data = "", Type[] matches = null, Type[] excludes = null)
        {
            Source = source;
            Data = data;
            if (matches == null)
            {
                Matches = new Type[0];
            }

            if (excludes == null)
            {
                Excludes = new Type[0];
            }
        }

        public abstract void CallOnMatch(Entity entity);

        public void DispatchToEntity(Entity entity)
        {
            if (entity.HasExcluding(Matches, Excludes))
            {
                CallOnMatch(entity);
            }
        }
    }
}
