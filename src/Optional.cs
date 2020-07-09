namespace Vysn.Commons {
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Optional<T> {
        /// <summary>
        /// 
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hasValue"></param>
        public Optional(T value, bool hasValue) {
            Value = value;
            HasValue = hasValue;
        }
    }
}