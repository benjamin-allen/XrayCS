using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]
namespace XrayCS
{
    /// <summary>
    /// Used for mapping System.Type objects to indices in an array. Duplicate types are
    /// disallowed, and addition of types is permanent.
    /// </summary>
    /// <remarks>
    /// Types added to the component map must be derived from XrayCS.Component.
    /// This is enforced by the compiler.
    /// </remarks>
    internal class ComponentMap
    {
        private Dictionary<Type, int> _map = new Dictionary<Type, int>();
        private uint _maximumSize;
        private int _size;

        /// <summary>
        /// The largest number of types the <see cref="ComponentMap"/> can support.
        /// </summary>
        public uint MaximumSize { get => _maximumSize; }

        /// <summary>
        /// The current number of types registered in the <see cref="ComponentMap"/>.
        /// </summary>
        public int Size { get => _size; private set => _size = value; }

        /// <summary>
        /// Constructs a new <see cref="ComponentMap"/>.
        /// </summary>
        /// <param name="maximumSize">The maximum number of types allowed in this <see cref="ComponentMap"/>.</param>
        public ComponentMap(uint maximumSize = 255)
        {
            _maximumSize = maximumSize;
        }

        /// <summary>
        /// Adds a <typeparamref name="Component"/> to this map.
        /// </summary>
        /// <typeparam name="Component">The component to add. It must derive from 
        /// <see cref="Component"/></typeparam>
        /// <returns>The index of the newly-added component.</returns>
        /// <remarks>This function simply calls the more generic one based on Type objects instead
        /// of generics.</remarks>
        public int Register<Component>() where Component : XrayCS.Component
        {
            return Register(typeof(Component));
        }

        /// <summary>
        /// Adds a component to this map
        /// </summary>
        /// <param name="component">The type of component to add. It must derive from
        /// <see cref="XrayCS.Component"/>.</param>
        /// <returns>The index of the newly-registerd component.</returns>
        /// <exception cref="ArgumentException">If the component already exists in this map.
        /// </exception>
        /// <exception cref="OutOfMemoryException">If the map already has <see cref="MaximumSize"/>
        /// elements.</exception>
        public int Register(Type component)
        {
            if (!(component.IsSubclassOf(typeof(XrayCS.Component)) || component.Equals(typeof(XrayCS.Component))))
            {
                throw new ArgumentException("Type " + component.ToString() + " is not derived from XrayCS.Component");
            }
            if (Size < MaximumSize)
            {
                if(_map.ContainsKey(component))     // _map should not contain the key already
                {
                    throw new ArgumentException("Component " + component.ToString() + " already exists in this ComponentMap.");
                }
                else
                {
                    _map.Add(component, Size);
                    return Size++;
                }
            }
            else
            {
                throw new OutOfMemoryException("Maximum size for this ComponentMap exceeded: Size = " 
                                    + Size.ToString() + ", MaxSize = " + MaximumSize.ToString());
            }
        }

        /// <summary>
        /// Determines whether this <see cref="ComponentMap"/> contains a given
        /// <typeparamref name="Component"/>.
        /// </summary>
        /// <typeparam name="Component">The component under test.</typeparam>
        /// <returns>True if the <see cref="ComponentMap"/> contains the component.</returns>
        public bool Contains<Component>() where Component : XrayCS.Component
        {
            return Contains(typeof(Component));
        }

        /// <summary>
        /// Determines whether this <see cref="ComponentMap"/> contains a given
        /// <paramref name="component"/>
        /// </summary>
        /// <param name="component">The component under test.</param>
        /// <returns>True if the <see cref="ComponentMap"/> contains the component.</returns>
        public bool Contains(Type component)
        {
            return _map.ContainsKey(component);
        }

        /// <summary>
        /// Locates the requested component in this <see cref="ComponentMap"/>.
        /// </summary>
        /// <typeparam name="Component">The component under test.</typeparam>
        /// <param name="throwOnError">If true, failure to find the component will result in an <see cref="ArgumentException"/>.</param>
        /// <returns>The index of the component, or -1 if <paramref name="throwOnError"/> is false.</returns>
        /// <exception cref="ArgumentException">If <paramref name="throwOnError"/> is true and no component is found.</exception>
        public int Lookup<Component>(bool throwOnError = true) where Component : XrayCS.Component
        {
            if(Contains<Component>())
            {
                int index;
                _map.TryGetValue(typeof(Component), out index);
                return index;
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

        /// <summary>
        /// Locates the requested component in this <see cref="ComponentMap"/>.
        /// </summary>
        /// <param name="component">A <see cref="Type"/> containing the component under test.</param>
        /// <param name="throwOnError">If true, failure to find the component will result in an <see cref="ArgumentException"/></param>
        /// <returns>The index of the component, or -1 if <paramref name="throwOnError"/> is false.</returns>
        /// <exception cref="ArgumentException">If <paramref name="throwOnError"/> is set and no component is found.</exception>
        /// <exception cref="ArgumentException">If <paramref name="component"/> is not a strict subclass of Component.</exception>
        public int Lookup(Type component, bool throwOnError = true)
        {
            if (! (component.IsSubclassOf(typeof(XrayCS.Component)) || component.Equals(typeof(XrayCS.Component))) )
            {
                throw new ArgumentException("Type " + component.ToString() + " is not derived from XrayCS.Component");
            }
            if (_map.ContainsKey(component))
            {
                int index;
                _map.TryGetValue(component, out index);
                return index;
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

        /// <summary>
        /// Returns all keys of the map.
        /// </summary>
        internal Dictionary<Type, int>.KeyCollection AllKeys()
        {
            return _map.Keys;
        }
    }
}
