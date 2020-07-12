namespace Vysn.Commons {
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Snowflake {
        private readonly ulong _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Snowflake(ulong value) {
            _value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator ulong(in Snowflake value) {
            return value._value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Snowflake(in ulong value) {
            return new Snowflake(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool operator ==(Snowflake snowflake, Snowflake other) {
            return snowflake._value == other._value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool operator !=(Snowflake snowflake, Snowflake other) {
            return !(snowflake == other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool operator ==(Snowflake snowflake, ulong other) {
            return snowflake._value == other;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool operator !=(Snowflake snowflake, ulong other) {
            return snowflake._value != other;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="snowflake"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out Snowflake snowflake) {
            if (!ulong.TryParse(value, out var result)) {
                snowflake = default;
                return false;
            }

            snowflake = new Snowflake(result);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Snowflake other) {
            return _value == other._value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return obj switch {
                ulong val           => val == _value,
                Snowflake snowflake => Equals(snowflake)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return _value.GetHashCode();
        }
    }
}