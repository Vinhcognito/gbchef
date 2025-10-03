using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gbchef
{
    public class SpriteSheetService
    {
        private readonly Dictionary<string, CroppedBitmap> _spriteCache;

        public SpriteSheetService(string imgFileName, string jsonFileName)
        {
            var imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imgFileName);
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", jsonFileName);

            var spriteSheet = new BitmapImage(new Uri(imgPath));
            spriteSheet.CacheOption = BitmapCacheOption.OnLoad;
            spriteSheet.Freeze();

            string spriteJson = File.ReadAllText(jsonPath);
            var root = JsonSerializer.Deserialize<SpriteJsonRoot>(spriteJson)!;

            _spriteCache = [];
            foreach (var sprite in root.Sprites)
            {
                var cropped = new CroppedBitmap(
                    spriteSheet,
                    new Int32Rect(sprite.X, sprite.Y, sprite.Width, sprite.Height)
                );
                cropped.Freeze();
                _spriteCache[sprite.FileName] = cropped;
            }
        }

        public CroppedBitmap GetSprite(string spriteName)
        {
            if (!_spriteCache.TryGetValue(spriteName, out var sprite))
                throw new ArgumentException($"Sprite not found: {spriteName}");

            return sprite;
        }
    }

    public class SpriteJsonRoot
    {
        [JsonPropertyName("sprites")]
        public required List<Sprite> Sprites { get; set; }
    }

    public class Sprite
    {
        [JsonPropertyName("fileName")]
        public required string FileName { get; set; }
        [JsonPropertyName("x")]
        public required int X { get; set; }
        [JsonPropertyName("y")]
        public required int Y { get; set; }
        [JsonPropertyName("width")]
        public required int Width { get; set; }
        [JsonPropertyName("height")]
        public required int Height { get; set; }
    }
}
