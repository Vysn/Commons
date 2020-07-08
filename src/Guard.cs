using System;

namespace Vysn.Commons {
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Guard {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argument"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void NotNull(string argumentName, object argument) {
            switch (argument) {
                case string str when string.IsNullOrWhiteSpace(str):
                    throw new ArgumentNullException(argumentName, "String cannot be null/empty/whitespace.");
                case byte[] byteArray when byteArray.Length == 0:
                    throw new Exception("Array cannot be empty or have length of 0.");
                case var _ when argument == null:
                    throw new ArgumentNullException(argumentName, "Argument cannot be null.");
                case int num when num <= -1:
                    throw new ArgumentOutOfRangeException(argumentName, "Value must be higher than or equal to 0.");
            }
        }
    }
}