namespace Vysn.Commons.WebSocket.EventArgs {
    /// <summary>
    /// 
    /// </summary>
    public readonly struct DataEventArgs {
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
            => Data.Length == 0;

        internal DataEventArgs(byte[] data) {
            Data = data;
        }
    }
}