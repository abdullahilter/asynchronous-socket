namespace utility
{
    /// <summary>
    /// Constant variables.
    /// </summary>
    public struct Constant
    {
        /// <summary>
        /// Maximum length of the pending connections queue.
        /// </summary>
        public const int BACKLOG = 100;

        /// <summary>
        /// Size of receive buffer.
        /// </summary>
        public const int BUFFER_SIZE = 1024;

        /// <summary>
        /// The port number for the Socket communication.
        /// </summary>
        public const int PORT = 12345;

        /// <summary>
        /// End of text.
        /// </summary>
        public static readonly string ETX = string.Concat("~", (char)3);

        /// <summary>
        /// Temp buffer.
        /// </summary>
        public static readonly byte[] BUFFER = new byte[BUFFER_SIZE];
    }
}