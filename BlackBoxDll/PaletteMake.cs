using Flir.Atlas.Image;
using Flir.Atlas.Image.Fusion;
using Flir.Atlas.Image.Palettes;
using System;
using System.Drawing;
using System.IO;

namespace BlackBoxDll
{
    public class PaletteMake
    {
        public static string jpgname;
        public static string palname;

        public static int Paljpgmake(string fname_a, string fname_b)
        {
            if (new DriveInfo(Directory.GetCurrentDirectory().Substring(0, 2)).TotalFreeSpace < 1073741824L)
                return 2;
            try
            {
                double num1 = 1000.0;
                double num2 = -1000.0;
                PaletteMake.jpgname = fname_b;
                PaletteMake.palname = fname_a;
                Palette palette = PaletteManager.Open(PaletteMake.palname);
                ThermalImageFile thermalImageFile = new ThermalImageFile(PaletteMake.jpgname);
                thermalImageFile.Fusion.Mode = (FusionMode)thermalImageFile.Fusion.VisualOnly;
                Bitmap image = thermalImageFile.Image;
                int width = thermalImageFile.Width;
                int height = thermalImageFile.Height;
                double num3 = 0.0;
                double num4 = 0.0;
                double[] numArray = new double[width * height];
                double[,] pixelsArray = thermalImageFile.ImageProcessing.GetPixelsArray();
                for (int index1 = 0; index1 < thermalImageFile.Height; ++index1)
                {
                    string empty = string.Empty;
                    for (int index2 = 0; index2 < thermalImageFile.Width; ++index2)
                    {
                        int signal = (int)pixelsArray[index1, index2];
                        double valueFromSignal = thermalImageFile.GetValueFromSignal(signal);
                        numArray[width * index1 + index2] = valueFromSignal;
                        num3 += valueFromSignal * valueFromSignal;
                        num4 += valueFromSignal;
                        if (num1 > numArray[width * index1 + index2])
                            num1 = numArray[width * index1 + index2];
                        if (num2 < numArray[width * index1 + index2])
                            num2 = numArray[width * index1 + index2];
                    }
                }
                int num5 = width * height;
                double num6 = Math.Sqrt((num3 - num4 * num4 / (double)num5) / (double)num5);
                double num7 = num4 / (double)num5;
                double num8 = num7 - num6 * 3.0;
                double num9 = num7 + num6 * 3.0;
                thermalImageFile.Dispose();
                Bitmap bitmap = new Bitmap(width, height);
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int index = numArray[width * y + x] >= num8 ? (numArray[width * y + x] <= num9 ? (int)((numArray[width * y + x] - num8) / (num9 - num8) * (double)byte.MaxValue) : (int)byte.MaxValue) : 0;
                        Color paletteColor = palette.PaletteColors[index];
                        byte r = paletteColor.R;
                        paletteColor = palette.PaletteColors[index];
                        byte g = paletteColor.G;
                        paletteColor = palette.PaletteColors[index];
                        byte b = paletteColor.B;
                        bitmap.SetPixel(x, y, Color.FromArgb((int)r, (int)g, (int)b));
                    }
                }
                string[] strArray1 = PaletteMake.jpgname.Split('.');
                string[] strArray2 = PaletteMake.palname.Split('.')[0].Split('\\');
                string filename = strArray1[0] + "_" + strArray2[strArray2.Length - 1] + ".jpg";
                bitmap.Save(filename);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        private int pallete_num(double dt, double min, double max)
        {
            if (dt < min)
                return 0;
            return dt > max ? (int)byte.MaxValue : (int)((dt - min) / (max - min) * (double)byte.MaxValue);
        }
    }
}
