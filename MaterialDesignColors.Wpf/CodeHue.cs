﻿using System;
using System.Windows.Media;

namespace MaterialDesignColors
{
    public class CodeHue
    {
        public string Name { get; }
        public string Interval { get; }
        public Color Color { get; }

        public CodeHue(string name, string interval, string color)
        {
            Name = name;
            Interval = interval;
            Color = (Color) ColorConverter.ConvertFromString(color);
        }

        public Color ShiftLightnesss(int amount = 1)
        {
            var lab = Rgb2Lab(Color);
            var shifted = new Lab(lab.L - LAB_CONSTANTS.Kn * amount, lab.A, lab.B);
            return Lab2Rgb(shifted);
        }

        public Color Darken(int amount = 1)
        {
            return ShiftLightnesss(amount);
        }

        public Color Lighten(int amount = 1)
        {
            return ShiftLightnesss(-amount);
        }

        public Lab Rgb2Lab(Color c)
        {
            var xyz = Rgb2Xyz(c);
            return new Lab(116.0 * xyz.Y - 16.0, 500.0 * (xyz.X - xyz.Y), 200.0 * (xyz.Y - xyz.Z));
        }

        public Xyz Rgb2Xyz(Color c)
        {
            var r = Rgb_Xyz(c.R);
            var g = Rgb_Xyz(c.G);
            var b = Rgb_Xyz(c.B);
            var x = Xyz_Lab((0.4124564 * r + 0.3575761 * g + 0.1804375 * b) / LAB_CONSTANTS.Xn);
            var y = Xyz_Lab((0.2126729 * r + 0.7151522 * g + 0.0721750 * b) / LAB_CONSTANTS.Yn);
            var z = Xyz_Lab((0.0193339 * r + 0.1191920 * g + 0.9503041 * b) / LAB_CONSTANTS.Zn);

            return new Xyz(x, y, z);
        }

        private double Rgb_Xyz(double r)
        {
            if ((r /= 255.0) <= 0.04045)
                return r / 12.92;
            else
                return Math.Pow((r + 0.055) / 1.055, 2.4);
        }

        private double Xyz_Lab(double t)
        {
            if (t > LAB_CONSTANTS.t3)
                return Math.Pow(t, 1.0 / 3.0);
            else
                return t / LAB_CONSTANTS.t2 + LAB_CONSTANTS.t0;
        }

        private double Lab_Xyz(double t)
        {
            if (t > LAB_CONSTANTS.t1)
                return t * t * t;
            else
                return LAB_CONSTANTS.t2 * (t - LAB_CONSTANTS.t0);
        }

        private double Xyz_Rgb(double r)
        {
            if (r <= 0.00304)
                return 255.0 * (12.92 * r);
            else
                return 255.0 * (1.055 * Math.Pow(r, 1.0 / 2.4) - 0.055);
        }

        public Xyz Lab2Xyz(Lab lab)
        {
            var y = (lab.L + 16.0) / 116.0;
            var x = double.IsNaN(lab.A) ? y : y + lab.A / 500.0;
            var z = double.IsNaN(lab.B) ? y : y - lab.B / 200.0;

            y = LAB_CONSTANTS.Yn * Lab_Xyz(y);
            x = LAB_CONSTANTS.Xn * Lab_Xyz(x);
            z = LAB_CONSTANTS.Zn * Lab_Xyz(z);

            return new Xyz(x, y, z);
        }

        public Color Lab2Rgb(Lab lab)
        {
            byte clip(double d)
            {
                if (d < 0) return 0;
                if (d > 255) return 255;
                return (byte)Math.Round(d);
            }
            var xyz = Lab2Xyz(lab);

            var r = Xyz_Rgb(3.2404542 * xyz.X - 1.5371385 * xyz.Y - 0.4985314 * xyz.Z);
            var g = Xyz_Rgb(-0.9692660 * xyz.X + 1.8760108 * xyz.Y + 0.0415560 * xyz.Z);
            var b = Xyz_Rgb(0.0556434 * xyz.X - 0.2040259 * xyz.Y + 1.0572252 * xyz.Z);

            return Color.FromRgb(clip(r), clip(g), clip(b));
        }

        public static Color ContrastingForeGroundColor(Color color)
        {
            double rgb_srgb(double c)
            {
                c = c / 255.0;
                return (c <= 0.03928)
                    ? c = c / 12.92
                    : c = Math.Pow((c + 0.055) / 1.055, 2.4);
            }
            var r = rgb_srgb(color.R);
            var g = rgb_srgb(color.G);
            var b = rgb_srgb(color.B);

            var luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            return luminance > 0.179 ? Colors.Black : Colors.White;
        }

        public class Xyz
        {
            public double X { get; }
            public double Y { get; }
            public double Z { get; }

            public Xyz(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public class Lab
        {
            public double L { get; }
            public double A { get; }
            public double B { get; }

            public Lab(double l, double a, double b)
            {
                L = l;
                A = a;
                B = b;
            }
        }

        private class LAB_CONSTANTS
        {
            public const double Kn = 18;

            public const double Xn = 0.95047;
            public const double Yn = 1;
            public const double Zn = 1.08883;

            public const double t0 = 0.137931034;
            public const double t1 = 0.206896552;
            public const double t2 = 0.12841855;
            public const double t3 = 0.008856452;
        }
    }
}
