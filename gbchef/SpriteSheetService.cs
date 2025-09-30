using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace gbchef
{
    public class SpriteSheetService
    {
        private readonly BitmapImage _spriteImg;

        private readonly Dictionary<string, Int32Rect> _spriteRects;

        public SpriteSheetService(string imgFileName, string jsonFileName)
        {
            var imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imgFileName);
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", jsonFileName);

            _spriteImg = new BitmapImage(new Uri(imgPath));
            _spriteImg.CacheOption = BitmapCacheOption.OnLoad;
            _spriteRects = LoadSpriteRectsFromJson(jsonPath);
            Debug.WriteLine(_spriteRects);
        }

        public CroppedBitmap GetSprite(string spriteName)
        {
            if (!_spriteRects.TryGetValue(spriteName, out Int32Rect rect))
            {
                throw new ArgumentException($"Sprite not found: {spriteName}");
            }

            return new CroppedBitmap(
                _spriteImg,
                rect
            );
        }

        public static Dictionary<string, Int32Rect> LoadSpriteRectsFromJson(string jsonPath)
        {
            var returnValue = new Dictionary<string, Int32Rect>();

            string spriteJson = File.ReadAllText(jsonPath);
            Dictionary<string, object> jsonValue = JsonSerializer.Deserialize<Dictionary<string, object>>(spriteJson);

            JsonElement sprites = (JsonElement)jsonValue["sprites"];

            foreach (var item in sprites.EnumerateArray())
            {
                var dict = new Dictionary<string, object>();
                foreach (var property in item.EnumerateObject())
                {
                    if (property.Name == "fileName")
                    {
                        dict[property.Name] = property.Value.GetString();
                    }
                    else
                    {
                        dict[property.Name] = property.Value.GetInt32();
                    }
                }

                returnValue.Add(
                    (string)dict["fileName"],
                    new Int32Rect(
                        (int)dict["x"],
                        (int)dict["y"],
                        (int)dict["width"],
                        (int)dict["height"]
                    )
                );
            }

            return returnValue;
        }
    
    }

}
