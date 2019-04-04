using System;
using System.Collections.Generic;

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

        public int Register<Component>()
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

        public bool Contains<Component>()
        {
            return _map.ContainsKey(typeof(Component));
        }

        public int Lookup<Component>(bool throwOnError = true)
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

        public int Lookup(string qualifiedTypename, bool throwOnError = true)
        {
            Type Component = Type.GetType(qualifiedTypename);
            return Lookup<Component>();
        }
    }
}
