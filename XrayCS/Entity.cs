using System;
using System.Collections.Generic;

namespace XrayCS
{
    /// <summary>
    /// Manages a collection of components and provides abstractions for their use
    /// </summary>
    /// <remarks>
    /// <para>Compared to the disaster that is <see cref="Component"/>, this class is relatively
    /// easy to describe. There is a maximum number of components that entities can create,
    /// this is primarily done to control the size of the component array.</para>
    /// <para>Additionally, note that all generic types passed to methods of this class 
    /// must implement an argument-less constructor and must derive from <see cref="Component"/></para>
    /// </remarks>
    /// \todo Implement entity loading and cloning.
    public class Entity
    {
        private Component[] _data;
        private ComponentMap _map;
        private int _numRegisteredComponents;
        private int _numCurrentComponents;
        private int _maxComponents;

        /// <summary>
        /// The number of components registered in the <see cref="ComponentMap"/> of this entity.
        /// </summary>
        public int NumRegisteredComponents { get => _numRegisteredComponents; private set => _numRegisteredComponents = value; }

        /// <summary>
        /// <see cref="NumComponents"/> cannot equal or exceed this value.
        /// </summary>
        public int MaxComponents { get => _maxComponents; private set => _maxComponents = value; }

        /// <summary>
        /// The current number of components constructed in this entity.
        /// </summary>
        /// <remarks>This may be less than <see cref="NumRegisteredComponents"/>, but never more.</remarks>
        public int NumComponents { get => _numCurrentComponents; private set => _numCurrentComponents = value; }

        /// <summary>
        /// Constructs a new <see cref="Entity"/>.
        /// </summary>
        /// <param name="maxComponents">The number of components the entity is allowed to own</param>
        public Entity(uint maxComponents = 50)
        {
            _map = new ComponentMap(maxComponents);
            _data = new Component[_map.MaximumSize];
            NumRegisteredComponents = 0;
            NumComponents = 0;
            MaxComponents = (int)maxComponents;
        }

        /// <summary>
        /// Create a new component of the specified type, and allocate space for it if necessary
        /// </summary>
        /// <param name="componentType">A type object containing the type of the component. It
        /// must be a derived type.</param>
        /// <param name="c">If this value is non-null, the new component will be constructed by
        /// <see cref="Component.Clone()"/></param>
        /// <returns>A reference to the new component.</returns>
        /// <exception cref="ArgumentException"> If the component map is full</exception>
        /// <exception cref="ArgumentException"> If a component of type 
        /// <paramref name="componentType"/> already exists in this entity.</exception>
        /// \todo: I'm very unahppy with the cyclomatic complexity of this function. There has to
        /// be a better way to develop it.
        public Component Add(Type componentType, Component c = null)
        {
            bool mapIsFull = NumRegisteredComponents == MaxComponents;
            bool mapContainsComponent = _map.Contains(componentType);
            object component = Activator.CreateInstance(componentType);
            if (mapContainsComponent)
            {
                component = _data[_map.Lookup(componentType)];
            }   // We can now guarantee that (component = null) or (component = ref)
            if (mapContainsComponent && component == null)
            {   // Add the component back to the data and increment NumComponents
                if (c == null)
                {
                    c = Activator.CreateInstance(componentType) as Component;
                }
                int index = _map.Lookup(componentType);
                _data[index] = c.Clone();
                NumComponents += 1;
                return _data[index];
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
                    c = Activator.CreateInstance(componentType) as Component;
                }
                int index = _map.Register(componentType);
                _data[index] = c.Clone();
                NumComponents += 1;
                NumRegisteredComponents += 1;
                return _data[index];
            }
        }

        /// <summary>
        /// Create a new component of type <typeparamref name="Component"/>, and allocate space
        /// for it if necessary.
        /// </summary>
        /// <typeparam name="Component">The type of component to add.</typeparam>
        /// <param name="c">If this value is non-null, the new component will be constructed by
        /// <see cref="Component.Clone()"/></param>
        /// <returns>A reference to the new component.</returns>
        /// <exception cref="ArgumentException"> If the component map is full</exception>
        /// <exception cref="ArgumentException"> If a component of type <typeparamref name="Component"/>
        /// already exists in this entity.</exception>
        public Component Add<Component>(XrayCS.Component c = null)
            where Component : XrayCS.Component
        {
            return Add(typeof(Component), c) as Component;
        }

        /// <summary>
        /// Deletes the component from the entity by nullifying it.
        /// </summary>
        /// <typeparam name="Component">The component to delete.</typeparam>
        /// <param name="throwOnError">If true, failure to find the component will result
        /// in an exception.</param>
        /// <returns>True if the component was successfully removed. False if it was not found
        /// or was already deleted.</returns>
        /// <exception cref="ArgumentException">If the <typeparamref name="Component"/> cannot be found.</exception>
        public bool Remove<Component>(bool throwOnError = true) where Component : XrayCS.Component
        {
            int index = _map.Lookup<Component>(throwOnError);
            if(index > -1 && _data[index] != null)
            {
                _data[index] = null;
                NumComponents -= 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a reference to a component.
        /// </summary>
        /// <typeparam name="Component">The component to return</typeparam>
        /// <param name="throwOnError">If true, failure to find the component will result
        /// in an exception.</param>
        /// <returns>The component, or null if it is not found.</returns>
        /// <exception cref="ArgumentException">If the <typeparamref name="Component"/> cannot be found.</exception>
        public Component Get<Component>(bool throwOnError = true) where Component : XrayCS.Component
        {
            int index = _map.Lookup<Component>(throwOnError);
            if(index > -1)
            {
                return _data[index] as Component;
            }
            return null;
        }

        /// <summary>
        /// Check whether the entity owns an instance of the specified component.
        /// </summary>
        /// <typeparam name="Component">The component under test.</typeparam>
        /// <returns>True if the entity owns the component.</returns>
        public bool Has<Component>() where Component : XrayCS.Component
        {
            return Get<Component>(false) != null;
        }

        /// <summary>
        /// Given a list of <see cref="Type"/>, check whether the entity owns all types in the list.
        /// </summary>
        /// <param name="components">Th types to be checked. If it is empty the function will always return
        /// false.</param>
        /// <returns>True if every element of <paramref name="components"/> is owned by the entity.</returns>
        public bool HasAll(Type[] components)
        {
            bool hasAll = true;
            if (components.Length == 0)
            {
                hasAll = false;
            } 
            foreach (Type type in components)
            {
                int index = _map.Lookup(type, false);
                // Abuse short-circuit evaluation to avoid looking up _data[-1];
                hasAll &= (index  > -1 && _data[index] != null);
            }
            return hasAll;
        }

        /// <summary>
        /// Given a list of <see cref="Type"/>, check whether the entity owns any of the types in the list.
        /// </summary>
        /// <param name="components">The types to be checked. If it is then the function will always return
        /// false.</param>
        /// <returns>True if any element of <paramref name="components"/> is owned by the entity.</returns>
        public bool HasAny(Type[] components)
        {
            bool hasAny = false;
            foreach (Type type in components)
            {
                int index = _map.Lookup(type, false);
                hasAny |= (index > -1 && _data[index] != null);
            }
            return hasAny;
        }

        /// <summary>
        /// Check whether the entity has all of the specified match components and none of the specified
        /// exclude components.
        /// </summary>
        /// <param name="toMatch">The list of types to search for. This function returns false if any 
        /// element of <paramref name="toMatch"/> is not found.</param>
        /// <param name="toExclude">The list of types to search against. This function returns false if any
        /// element of <paramref name="toExclude"/> is found.</param>
        /// <returns>True if the entity has all components in <paramref name="toMatch"/> and none in
        /// <paramref name="toExclude"/>.</returns>
        /// <remarks>
        /// This function behaves as logically expected for empty lists. If <paramref name="toMatch"/> is
        /// empty, the function will always return false.
        /// </remarks>
        public bool HasExcluding(Type[] toMatch, Type[] toExclude)
        {
            return HasAll(toMatch) && !HasAny(toExclude);
        }

        /// <summary>
        /// Creates a duplicate entity by calling <see cref="Component.Clone"/> on each component.
        /// </summary>
        /// <returns>A reference to the new entity.</returns>
        /// <remarks>The created entity is not guaranteed to have the same component layout as the
        /// source entity. Attempting to use the same map for both entities will likely result in
        /// an error of some type.</remarks>
        public Entity Clone()
        {
            Entity entity = new Entity((uint)MaxComponents);
            foreach (var key in _map.AllKeys())
            {
                int index = _map.Lookup(key, false);
                if(_data[index] != null)
                {
                    entity.Add(key, _data[index]);
                }
            }
            return entity;
        }

        /// <summary>
        /// Deletes all components owned by the entity.
        /// </summary>
        /// <param name="preserveMap">If true, the map and NumRegisteredComponents will not be
        /// reset.</param>
        public void Clear(bool preserveMap = true)
        {
            NumComponents = 0;
            // since data is filled in order, we can simplify the logic by deleting only up to
            // our NumRegisteredComponents index, as that's the last possible position data can be
            if(preserveMap == false)
            {
                _map = new ComponentMap((uint)MaxComponents);
                NumRegisteredComponents = 0;
            }
            for(int i = 0; i < NumRegisteredComponents; i++)
            {
                _data[i] = null;
            }
        }
    }
}
