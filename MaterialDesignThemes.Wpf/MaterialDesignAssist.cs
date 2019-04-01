﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MaterialDesignThemes.Wpf
{
    public static class MaterialDesignAssist
    {
        private const string ColorUriFormat =
            @"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/{0}/MaterialDesignColor.{1}.xaml";

        private const string ThemeUriFormat =
            @"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.{0}.xaml";


        public static MaterialDesignTheme WithMaterialDesign(this Application application,
            BaseTheme theme, PrimaryColor primaryColor, AccentColor accentColor)
        {
            //NB: When the palettes are changed it hunts through the merged dictionaries.
            //Putting this at the beginning to avoid needing to hunt through all of them.
            application.Resources.MergedDictionaries.Add(CreateEmptyPaletteDictionary());
            if (FindDictionary(application.Resources.MergedDictionaries, string.Format(ThemeUriFormat, "Defaults")) == null)
            {
                application.Resources.MergedDictionaries.Add(CreateDefaultThemeDictionary());
            }
            PaletteHelper.SetLightDark(theme == BaseTheme.Dark);
            Palette palette = PaletteHelper.ReplacePalette(primaryColor.ToString(), accentColor.ToString());
            return new MaterialDesignTheme(theme, primaryColor, accentColor, palette);
        }

        //public static ResourceDictionary WithMaterialDesign(this ResourceDictionary resourceDictionary, BaseTheme theme, PrimaryColor primaryColor, AccentColor accentColor)
        //{
        //    if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
        //    resourceDictionary.MergedDictionaries.Add(CreateDefaultThemeDictionary());
        //
        //    return resourceDictionary
        //        .WithTheme(theme)
        //        .WithPrimaryColor(primaryColor)
        //        .WithAccentColor(accentColor);
        //}

        private static ResourceDictionary CreateEmptyPaletteDictionary()
        {
            return new ResourceDictionary
            {
                ["PrimaryHueLightBrush"] = new SolidColorBrush(),
                ["PrimaryHueLightForegroundBrush"] = new SolidColorBrush(),
                ["PrimaryHueMidBrush"] = new SolidColorBrush(),
                ["PrimaryHueMidForegroundBrush"] = new SolidColorBrush(),
                ["PrimaryHueDarkBrush"] = new SolidColorBrush(),
                ["PrimaryHueDarkForegroundBrush"] = new SolidColorBrush(),
                ["SecondaryAccentBrush"] = new SolidColorBrush(),
                ["SecondaryAccentForegroundBrush"] = new SolidColorBrush()
            };
        }

        public static ResourceDictionary WithTheme(this ResourceDictionary resourceDictionary, BaseTheme theme)
        {
            if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
            ResourceDictionary existing = resourceDictionary.FindThemeDictionary(theme);
            if (existing != null)
            {
                resourceDictionary.MergedDictionaries.Remove(existing);
            }
            resourceDictionary.MergedDictionaries.Add(CreateThemeDictionary(theme));
            return resourceDictionary;
        }

        public static ResourceDictionary WithPrimaryColor(this ResourceDictionary resourceDictionary,
            PrimaryColor primaryColor)
        {
            if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
            ResourceDictionary existing = resourceDictionary.FindThemeDictionary(primaryColor);
            if (existing != null)
            {
                resourceDictionary.MergedDictionaries.Remove(existing);
            }
            resourceDictionary.MergedDictionaries.Add(CreateColorThemeDictionary(primaryColor));
            return resourceDictionary;
        }

        public static ResourceDictionary WithAccentColor(this ResourceDictionary resourceDictionary,
            AccentColor accentColor)
        {
            if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
            ResourceDictionary existing = resourceDictionary.FindThemeDictionary(accentColor);
            if (existing != null)
            {
                resourceDictionary.MergedDictionaries.Remove(existing);
            }
            resourceDictionary.MergedDictionaries.Add(CreateColorThemeDictionary(accentColor));
            return resourceDictionary;
        }

        public static ResourceDictionary FindThemeDictionary(this ResourceDictionary resourceDictionary,
            BaseTheme theme)
        {
            if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
            return FindDictionary<BaseTheme>(resourceDictionary.MergedDictionaries, ThemeUriFormat);
        }

        public static ResourceDictionary FindThemeDictionary(this ResourceDictionary resourceDictionary,
            PrimaryColor primaryColor)
        {
            if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
            return FindDictionary<PrimaryColor>(resourceDictionary.MergedDictionaries,
                string.Format(ColorUriFormat, "Primary", "{0}"));
        }

        public static ResourceDictionary FindThemeDictionary(this ResourceDictionary resourceDictionary,
            AccentColor accentColor)
        {
            if (resourceDictionary == null) throw new ArgumentNullException(nameof(resourceDictionary));
            return FindDictionary<PrimaryColor>(resourceDictionary.MergedDictionaries,
                string.Format(ColorUriFormat, "Accent", "{0}"));
        }

        /// <summary>
        /// Replaces a certain entry anywhere in the source dictionary and its merged dictionaries
        /// </summary>
        /// <param name="sourceDictionary">The source dictionary to start with</param>
        /// <param name="name">The entry to replace</param>
        /// <param name="value">The new entry value</param>
        public static void ReplaceEntry(this ResourceDictionary sourceDictionary, object name, object value)
        {
            if (sourceDictionary.Contains(name))
            {
                if (sourceDictionary[name] is SolidColorBrush brush && 
                    !brush.IsFrozen && value is SolidColorBrush newBrush)
                {
                    var animation = new ColorAnimation
                    {
                        From = brush.Color,
                        To = newBrush.Color,
                        Duration = new Duration(TimeSpan.FromMilliseconds(300))
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                }
                else
                {
                    sourceDictionary[name] = value; //Set value normally
                }
            }

            foreach (var dictionary in sourceDictionary.MergedDictionaries)
            {
                ReplaceEntry(dictionary, name, value);
            }
        }

        private static ResourceDictionary CreateThemeDictionary(BaseTheme theme)
            => new ResourceDictionary { Source = GetUri(theme) };

        private static ResourceDictionary CreateColorThemeDictionary(PrimaryColor color)
            => new ResourceDictionary { Source = GetUri(color) };

        private static ResourceDictionary CreateColorThemeDictionary(AccentColor color)
            => new ResourceDictionary { Source = GetUri(color) };

        private static ResourceDictionary CreateDefaultThemeDictionary()
            => new ResourceDictionary { Source = new Uri(string.Format(ThemeUriFormat, "Defaults")) };

        private static Uri GetUri(BaseTheme theme) => new Uri(string.Format(ThemeUriFormat, theme));

        private static Uri GetUri(PrimaryColor color) => new Uri(string.Format(ColorUriFormat, "Primary", color));

        private static Uri GetUri(AccentColor color) => new Uri(string.Format(ColorUriFormat, "Accent", color));

        private static ResourceDictionary FindDictionary<TEnum>(IEnumerable<ResourceDictionary> dictionaries, string formatString) where TEnum : struct
        {
            var regex = new Regex(StringFormatHelper.GetRegexPatternImpl(formatString, string.Join("|", Enum.GetValues(typeof(TEnum)).Cast<TEnum>())));
            return dictionaries.FirstOrDefault(x => regex.IsMatch(x.Source?.OriginalString ?? ""));
        }

        private static ResourceDictionary FindDictionary(IEnumerable<ResourceDictionary> dictionaries, string formatString)
        {
            var regex = new Regex(StringFormatHelper.GetRegexPatternImpl(formatString));
            return dictionaries.FirstOrDefault(x => regex.IsMatch(x.Source?.OriginalString ?? ""));
        }

        private static class StringFormatHelper
        {
            private static readonly Regex _ParameterReplacement = new Regex(@"{\d+(?<Alignment>,-?\d*)?(?<Format>:.*?)?}", RegexOptions.Compiled);

            public static bool MatchesStringFormat(string testString, string stringFormat)
            {
                var regexPattern = GetRegexPattern(stringFormat);
                return Regex.IsMatch(testString ?? "", regexPattern);
            }

            private static string GetRegexPattern(string stringFormat)
            {
                return GetRegexPatternImpl(stringFormat);
            }

            public static string GetRegexPatternImpl(string stringFormat, string replacement = ".*")
            {
                return string.Join(replacement, _ParameterReplacement.Split(stringFormat).Select(Regex.Escape));
            }
        }
    }
}