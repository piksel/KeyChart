using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace KeyboardCheatSheet.Models
{
    class KeyboardInfo
    {
        [JsonPropertyName("keyboard_name")]
        public string KeyboardName { get; set; }
        public string Url { get; set; }
        public string Maintainer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<string, KeyLayout> Layouts { get; set; }
    }

    public class KeyLayout
    {
        public Key[] Layout { get; set; }
    }

    public class Key
    {
        public int X { get; set; }
        public int Y { get; set; }

        [DefaultValue(1)]
        public int W { get; set; } = 1;
    }

}
