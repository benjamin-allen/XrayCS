﻿using System;

namespace XrayCS
{
    public abstract class Event
    {
        private Entity _source;
        private Type[] _matches;
        private Type[] _excludes;

        public Entity Source { get => _source; private set => _source = value; }
        public Type[] Matches { get => _matches; private set => _matches = value; }
        public Type[] Excludes { get => _excludes; private set => _excludes = value; }

        public Event(Entity source = null, Type[] matches = null, Type[] excludes = null)
        {
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
