using Krypton;
using Krypton.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TileEngine.LayerMap
{
    public static class RandomLightGenerator
    {
        //random generation and placement
        private static Random rand = new Random();
        //lighting
        private static float lightFov = MathHelper.TwoPi;
        public static bool RandomLightFovOn = false;

        #region Light Count
        private static uint lightCount = 200;
        public static uint LightCount
        {
            get { return lightCount; }
            set { lightCount = value; }
        }
        #endregion

        #region Light Color
        private static Color lightColor = Color.White;
        public static Color LightColor
        {
            get { return lightColor; }
            set { lightColor = value; }
        }
        public static bool RandomLightColorOn = true;
        private static byte randomLightColorGreenScale = 255;
        private static byte randomLightColorRedScale = 255;
        private static byte randomLightColorBlueScale = 255;
        public static Color RandomLightColorScale
        {
            get { return new Color(randomLightColorRedScale, randomLightColorGreenScale, randomLightColorBlueScale); }
            set
            {
                randomLightColorRedScale = value.R;
                randomLightColorGreenScale = value.G;
                randomLightColorBlueScale = value.B;
            }
        }
        #endregion

        #region Light Intensity
        private static float lightIntensity = 1.0f;
        public static float LightIntensity
        {
            get { return lightIntensity; }
            set { value = MathHelper.Clamp(value, 0.1f, 3.0f); }
        }
        public static bool RandomLightIntensityOn = true;
        private static float randomLightIntensityMin = 0.45f;
        private static float randomLightIntensityMax = 0.85f;
        public static Vector2 RandomLightIntensityScale
        {
            get { return new Vector2(randomLightIntensityMin, randomLightIntensityMax); }
            set
            {
                randomLightIntensityMin = MathHelper.Clamp(value.X, 0.1f, value.Y);
                randomLightIntensityMax = MathHelper.Clamp(value.Y, value.X, 3.0f);
            }
        }
        #endregion


        private static uint randomLightRangeMin = 50;
        private static uint randomLightRangeMax = 200;
        public static Vector2 RandomLightRangeScale
        {
            get { return new Vector2(randomLightRangeMin, randomLightRangeMax); }
            set
            {
                randomLightRangeMin = (uint)value.X;
                randomLightRangeMax = (uint)value.Y;
            }
        }

        #region CreateLights
        public static void CreateLights(Texture2D texture, KryptonEngine krypton, CollisionLayer map)
        {
            //make random lights
            for (int i = 0; i < lightCount; i++)
            {
                if (RandomLightColorOn)
                {
                    byte r = (byte)(rand.Next(randomLightColorRedScale - 64) + 64);
                    byte g = (byte)(rand.Next(randomLightColorGreenScale - 64) + 64);
                    byte b = (byte)(rand.Next(randomLightColorBlueScale - 64) + 64);
                    lightColor = new Color(r, g, b);
                }
                if (RandomLightIntensityOn)
                {
                    lightIntensity = (float)(rand.NextDouble() * randomLightIntensityMin + randomLightIntensityMax);
                }
                Light2D light = new Light2D()
                {
                    Texture = texture,
                    Range = (float)(rand.NextDouble() * (randomLightRangeMax - randomLightRangeMin) + randomLightRangeMin),
                    Color = lightColor,
                    Intensity = lightIntensity,
                    Angle = MathHelper.TwoPi * (float)rand.NextDouble(),
                    X = (float)(rand.NextDouble() * map.WidthInPixels),
                    Y = (float)(rand.NextDouble() * map.HeightInPixels)
                };

                //here we set the light's field of view
                if (i % 2 == 0)
                {
                    if (RandomLightFovOn)
                        light.Fov = MathHelper.PiOver2 * (float)(rand.NextDouble() * 0.75 + 0.25);
                    else
                        light.Fov = lightFov;
                }

                krypton.Lights.Add(light);
            }
        }

        public static void CreateLights(Texture2D texture, KryptonEngine krypton, Point startPoint, Point endPoint)
        {
            //make random lights
            for (int i = 0; i < lightCount; i++)
            {
                if (RandomLightColorOn)
                {
                    byte r = (byte)(rand.Next(randomLightColorRedScale - 64) + 64);
                    byte g = (byte)(rand.Next(randomLightColorGreenScale - 64) + 64);
                    byte b = (byte)(rand.Next(randomLightColorBlueScale - 64) + 64);
                    lightColor = new Color(r, g, b);
                }
                if (RandomLightIntensityOn)
                {
                    lightIntensity = (float)(rand.NextDouble() * randomLightIntensityMin + randomLightIntensityMax);
                }
                Light2D light = new Light2D()
                {
                    Texture = texture,
                    Range = (float)(rand.NextDouble() * (randomLightRangeMax - randomLightRangeMin) + randomLightRangeMin),
                    Color = lightColor,
                    Intensity = lightIntensity,
                    Angle = MathHelper.TwoPi * (float)rand.NextDouble(),
                    X = (float)((rand.NextDouble() + startPoint.X) * endPoint.X),
                    Y = (float)((rand.NextDouble() + startPoint.Y) * endPoint.Y)
                };

                //here we set the light's field of view
                if (i % 2 == 0)
                {
                    if (RandomLightFovOn)
                        light.Fov = MathHelper.PiOver2 * (float)(rand.NextDouble() * 0.75 + 0.25);
                    else
                        light.Fov = lightFov;
                }
                krypton.Lights.Add(light);
            }
        }
        #endregion
    }
}
