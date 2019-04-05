using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]
namespace XrayCS
{
    class ComponentMap
    {
        private Dictionary<Type, int> _map = new Dictionary<Type, int>();
        private uint _maximumSize;
        private int _size;

        public uint MaximumSize { get => _maximumSize; }
        public int Size { get => _size; private set => _size = value; }

        public ComponentMap(uint maximumSize = 255)
        {
            _maximumSize = maximumSize;
        }

        public int Register<Component>() where Component : XrayCS.Component
        {
            if(Size < MaximumSize)
            {
                if(_map.ContainsKey(typeof(Component)))     // _map should not contain the key already
                {
                    throw new ArgumentException("Component " + typeof(Component).ToString() + " already exists in this ComponentMap.");
                }
                else
                {
                    _map.Add(typeof(Component), Size);
                    return Size++;
                }
            }
            else
            {
                throw new Exception("Maximum size for this ComponentMap exceeded: Size = " 
                                    + Size.ToString() + ", MaxSize = " + MaximumSize.ToString());
            }
        }

        public bool Contains<Component>() where Component : XrayCS.Component
        {
            return _map.ContainsKey(typeof(Component));
        }

        public int Lookup<Component>(bool throwOnError = true) where Component : XrayCS.Component
        {
            if(Contains<Component>())
            {
                return _map.GetValueOrDefault(typeof(Component));
            }
            else if(throwOnError)
            {
                throw new ArgumentException("Component " + typeof(Component).ToString() + " not located in map.");
            }
            else
            {
                return -1;
            }
        }

        public int Lookup(Type component, bool throwOnError = true)
        {
            if (! (component.IsSubclassOf(typeof(XrayCS.Component)) || component.Equals(typeof(XrayCS.Component))) )
            {
                throw new ArgumentException("Type " + component.ToString() + " is not derived from XrayCS.Component");
            }
            if (_map.ContainsKey(component))
            {
                return _map.GetValueOrDefault(component);
            }
            else if(throwOnError)
            {
                throw new ArgumentException("Component " + component.ToString() + " not located in map.");
            }
            else
            {
                return -1;
            }
        }
    }
}
