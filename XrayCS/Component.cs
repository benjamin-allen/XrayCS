namespace XrayCS
{
    public abstract class Component
    {
        protected abstract Component _Clone();
        public Component Clone() { return this._Clone(); }
    }
}
