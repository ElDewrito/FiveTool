using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FiveTool
{
    /// <summary>
    /// Holds configuration data.
    /// </summary>
    /// <remarks>
    /// This is currently not thread-safe for simplicity reasons.
    /// </remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        private static Config _currentConfig = new Config();

        /// <summary>
        /// The recommended default config path for FiveTool.
        /// </summary>
        public const string DefaultPath = "FiveTool.json";

        /// <summary>
        /// Gets or sets the path to the game's root directory. Can be <c>null</c> or empty.
        /// </summary>
        [JsonProperty("gameRoot")]
        public string GameRoot { get; set; }

        /// <summary>
        /// Gets or sets the currently-loaded global configuration.
        /// </summary>
        public static Config Current
        {
            get { return _currentConfig; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _currentConfig = value;
            }
        }

        public static Config Load(string path)
        {
            using (var file = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<Config>(new JsonTextReader(file));
            }
        }

        public void Save(string path)
        {
            using (var file = File.CreateText(path))
            {
                var serializer = new JsonSerializer { Formatting = Formatting.Indented };
                serializer.Serialize(file, this);
            }
        }
    }
}
