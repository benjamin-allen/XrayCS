using Newtonsoft.Json;

namespace XrayCS
{
    // This is probably the most documented 3-lines of code I've made in my life.

    /// <summary>
    /// This is the base class for all components. See documentation for instructions on how to
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
    /// <see cref="_Clone()"/> is a protected override, and its implementation makes HealthComponent
    /// non-abstract. Its return type is Component because it must implement the signature of XrayCS.Component.
    /// </item>
    /// <item>
    /// <see cref="Clone()"/> is a public new, and it actually returns the new object. The
    /// new keyword causes it to hide the XrayCS.Component.Clone() method.
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
        /// This method is overridden to facilitate cloning of components.
        /// </summary>
        /// <remarks>
        /// This is usually accomplished by calling the Clone() in the override implementation
        /// and rewriting the Clone() method to return a new component of the correct type.
        /// </remarks>
        /// <returns>A new component object</returns>
        protected abstract Component _Clone();

        /// <summary>
        /// This method is also modified to facilitate cloning of components.
        /// </summary>
        /// <remarks>
        /// This method gets hidden when its derived, but it is used when `Clone()`-ing
        /// arrays of components.
        /// </remarks>
        /// <returns>A new component object</returns>
        public Component Clone() { return this._Clone(); }

        /// <summary>
        /// Populates an object's data from a JSON string. This method will overwrite existing
        /// data in the component.
        /// </summary>
        /// <param name="json">The JSON data to </param>
        /// <remarks>This method is virtual in case derived classes wish to impose restrictions
        /// on what can be loaded from JSON.</remarks>
        public virtual void LoadJson(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }
}
