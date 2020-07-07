namespace Vysn.Commons {
    /// <summary>
    /// </summary>
    public readonly struct Markdown {
        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SingleLineCodeblock(string content) {
			return $"`{content}`";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string CodeBlock(string content, string language = default) {
			return $"```{language}\n{content}\n```";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Italics(string content) {
			return $"*{content}*";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Bold(string content) {
			return $"**{content}**";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string BoldItalics(string content) {
			return $"***{content}***";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Underline(string content) {
			return $"__{content}__";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Strikethrough(string content) {
			return $"~~{content}~~";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string UnderlineItalics(string content) {
			return $"__*{content}*__";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string UnderlineBold(string content) {
			return $"__**{content}**__";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string UnderlineBoldItalics(string content) {
			return $"__***{content}***___";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SingleLineBlockQuote(string content) {
			return $"> {content}";
		}

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string BlockQuote(string content) {
			return $">>> {content}";
		}
	}
}