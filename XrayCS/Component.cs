﻿namespace XrayCS
{
    // This is probably the most documented 3-lines of code I've made in my life.

    /// <summary>
    /// Component: This is the base class for all components. See documentation for instructions on how to
    /// extend this class.
    /// </summary>
    /// <example>
    /// We'll implement the derived class HealthComponent.
    /// <code>
    /// class HealthComponent : Component
    /// {
    ///     public int Health;
    ///     
    ///     public HealthComponent(int health)
    ///     {
    ///         Health = health;
    ///     }
    ///     
    ///     public HealthComponent() : HealthComponent(0) { }
    ///     
    ///     protected override Component _Clone() { return this.Clone(); }
    ///     public new HealthComponent Clone() { return new HealthComponent(this.Health); }
    /// }
    /// </code>
    /// There's some important things to note about this code.
    /// <list type="number">
    /// <item>
    /// <code>_Clone()</code> is a <code>protected override</code>, and its implementation makes HealthComponent
    /// non-abstract. Its return type is component because it must implement the signature of XrayCS.Component.
    /// </item>
    /// <item>
    /// <code>Clone()</code> is a <code>public new</code>, and it actually returns the new object. The
    /// <code>new</code> keyword causes it to hide the XrayCS.Component.Clone() method.
    /// </item>
    /// <item>
    /// All derived components must implement a constructor without arguments, which is used as the
    /// fallback constructor when adding components to an entity.
    /// </item>
    /// </list>
    /// </example>
    public abstract class Component
    {
        /// <summary>
        /// _Clone(): This method is overridden to facilitate cloning of components.
        /// </summary>
        /// <remarks>
        /// This is usually accomplished by calling the <ref>Clone()</ref> in the override implementation
        /// and rewriting the Clone() method to return a new component of the correct type.
        /// </remarks>
        /// <returns>A new component object</returns>
        protected abstract Component _Clone();

        /// <summary>
        /// This method is also modified to facilitate cloning of components.
        /// </summary>
        /// <remarks>
        /// This method gets hidden when its derived, but it is used when <code>Clone()</code>-ing
        /// arrays of components.
        /// </remarks>
        /// <returns></returns>
        public Component Clone() { return this._Clone(); }
    }
}
