﻿using System;
using System.Collections.Generic;

namespace XrayCS
{
    public class Entity
    {
        private Component[] _data;
        private ComponentMap _map;
        private int _numRegisteredComponents;
        private int _numCurrentComponents;
        private int _maxComponents;

        public int NumRegisteredComponents { get => _numRegisteredComponents; private set => _numRegisteredComponents = value; }
        public int MaxComponents { get => _maxComponents; private set => _maxComponents = value; }
        public int NumComponents { get => _numCurrentComponents; private set => _numCurrentComponents = value; }

        public Entity(uint maxComponents = 50)
        {
            _map = new ComponentMap(maxComponents);
            _data = new Component[_map.MaximumSize];
            NumRegisteredComponents = 0;
            NumComponents = 0;
            MaxComponents = (int)maxComponents;
        }

        public Component Add<Component>(Component c = null)
            where Component : XrayCS.Component, new()
        {
            bool mapIsFull = NumRegisteredComponents == MaxComponents;
            bool mapContainsComponent = _map.Contains<Component>();
            Component component = null;
            if (mapContainsComponent)
            {
                component = _data[_map.Lookup<Component>()] as Component;
            }   // We can now guarantee that (component = null) or (component = ref)
            if (mapContainsComponent && component == null)
            {   // Add the component back to the data and increment NumComponents
                if (c == null)
                {
                    c = new Component();
                }
                _data[_map.Lookup<Component>()] = c;
                NumComponents += 1;
                return c;
            }
            else if(mapContainsComponent && component != null)
            {
                throw new ArgumentException("Attempted to add a component already owned by the entity.");
            }
            if(mapIsFull && !mapContainsComponent)
            {
                throw new ArgumentException("Attempted to add a component, but ComponentMap already contains "
                    + MaxComponents.ToString() + " components");
            }
            else
            {
                if(c == null)
                {
                    c = new Component();
                }
                var index = _map.Register<Component>();
                _data[index] = c;
                NumComponents += 1;
                NumRegisteredComponents += 1;
                return c;
            }
        }
    }
}