﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;

namespace MaterialDesignThemes.Wpf
{
    public static class MaterialDesignAssist
    {
        public static MaterialDesignTheme WithMaterialDesign(this Application application, BaseTheme theme, MaterialDesignColor primaryColor, MaterialDesignColor secondaryColor, ThemeManager themeManager = null)
        {
            var resources = application.Resources;
            return resources.WithMaterialDesign(theme, primaryColor, secondaryColor, themeManager);
        }

        public static MaterialDesignTheme WithMaterialDesign(this Application application, IBaseTheme theme, Color primaryColor, Color secondaryColor, ThemeManager themeManager = null)
        {
            var resources = application.Resources;
            return resources.WithMaterialDesign(theme, primaryColor, secondaryColor, themeManager);
        }

        public static MaterialDesignTheme WithMaterialDesign(this ResourceDictionary resources, BaseTheme theme, MaterialDesignColor primaryColor, MaterialDesignColor secondaryColor, ThemeManager themeManager = null)
        {
            return resources.WithMaterialDesign(MaterialDesignTheme.BaseThemes[theme], SwatchHelper.Lookup[primaryColor], SwatchHelper.Lookup[secondaryColor], themeManager);
        }

        public static MaterialDesignTheme WithMaterialDesign(this ResourceDictionary resources, IBaseTheme theme, Color primaryColor, Color secondaryColor, ThemeManager themeManager = null)
        {
            return resources.WithMaterialDesign(theme, new[] { ColorPalette.CreatePrimaryPalette(primaryColor), ColorPalette.CreateSecondaryPalette(secondaryColor) }, themeManager);
        }

        public static MaterialDesignTheme WithMaterialDesign(this ResourceDictionary resources, IBaseTheme theme, IReadOnlyList<ColorPalette> palettes, ThemeManager themeManager = null)
        {
            if (themeManager == null)
            {
                themeManager = new ThemeManager(resources).AttachThemeEventsToWindow();
            }

            themeManager.ThemeChangeHandlers += OnChangeTheme;
            themeManager.PaletteChangeHandlers += OnChangePalette;
            themeManager.ColorChangeHandlers += OnChangeColor;

            //NB: When the palettes are changed it hunts through the merged dictionaries.
            //Putting this at the beginning to avoid needing to hunt through all of them.
            resources.MergedDictionaries.Add(CreateEmptyThemeDictionary());
            resources.MergedDictionaries.Add(CreateEmptyPaletteDictionary());

            resources.WithTheme(theme);
            foreach(var palette in palettes)
            {
                resources.WithPalette(palette);
            }

            return new MaterialDesignTheme(themeManager, theme, palettes);
        }

        private static void OnChangeTheme(ResourceDictionary resources, IBaseTheme theme)
        {
            resources.WithTheme(theme);
        }

        private static void OnChangePalette(ResourceDictionary resources, ColorPalette palette)
        {
            resources.WithPalette(palette);
        }

        private static void OnChangeColor(ResourceDictionary resources, ColorChange colorChange)
        {
            resources.WithColor(colorChange.Name, colorChange.Color);
        }

        private static ResourceDictionary CreateEmptyThemeDictionary()
        {
            return new ResourceDictionary {
                ["ValidationErrorColor"] = new SolidColorBrush(),
                ["MaterialDesignBackground"] = new SolidColorBrush(),
                ["MaterialDesignPaper"] = new SolidColorBrush(),
                ["MaterialDesignCardBackground"] = new SolidColorBrush(),
                ["MaterialDesignToolBarBackground"] = new SolidColorBrush(),
                ["MaterialDesignBody"] = new SolidColorBrush(),
                ["MaterialDesignBodyLight"] = new SolidColorBrush(),
                ["MaterialDesignColumnHeader"] = new SolidColorBrush(),
                ["MaterialDesignCheckBoxOff"] = new SolidColorBrush(),
                ["MaterialDesignCheckBoxDisabled"] = new SolidColorBrush(),
                ["MaterialDesignTextBoxBorder"] = new SolidColorBrush(),
                ["MaterialDesignDivider"] = new SolidColorBrush(),
                ["MaterialDesignSelection"] = new SolidColorBrush(),
                ["MaterialDesignFlatButtonClick"] = new SolidColorBrush(),
                ["MaterialDesignFlatButtonRipple"] = new SolidColorBrush(),
                ["MaterialDesignToolTipBackground"] = new SolidColorBrush(),
                ["MaterialDesignChipBackground"] = new SolidColorBrush(),
                ["MaterialDesignSnackbarBackground"] = new SolidColorBrush(),
                ["MaterialDesignSnackbarMouseOver"] = new SolidColorBrush(),
                ["MaterialDesignSnackbarRipple"] = new SolidColorBrush(),
                ["MaterialDesignTextFieldBoxBackground"] = new SolidColorBrush(),
                ["MaterialDesignTextFieldBoxHoverBackground"] = new SolidColorBrush(),
                ["MaterialDesignTextFieldBoxDisabledBackground"] = new SolidColorBrush(),
                ["MaterialDesignTextAreaBorder"] = new SolidColorBrush(),
                ["MaterialDesignTextAreaInactiveBorder"] = new SolidColorBrush()
            };
        }

        private static ResourceDictionary CreateEmptyPaletteDictionary()
        {
            return new ResourceDictionary {
                ["PrimaryHueLightBrush"] = new SolidColorBrush(),
                ["PrimaryHueLightForegroundBrush"] = new SolidColorBrush(),
                ["PrimaryHueMidBrush"] = new SolidColorBrush(),
                ["PrimaryHueMidForegroundBrush"] = new SolidColorBrush(),
                ["PrimaryHueDarkBrush"] = new SolidColorBrush(),
                ["PrimaryHueDarkForegroundBrush"] = new SolidColorBrush(),
                ["SecondaryHueLightBrush"] = new SolidColorBrush(),
                ["SecondaryHueLightForegroundBrush"] = new SolidColorBrush(),
                ["SecondaryHueMidBrush"] = new SolidColorBrush(),
                ["SecondaryHueMidForegroundBrush"] = new SolidColorBrush(),
                ["SecondaryHueDarkBrush"] = new SolidColorBrush(),
                ["SecondaryHueDarkForegroundBrush"] = new SolidColorBrush(),
                // Compatibility
                ["SecondaryAccentBrush"] = new SolidColorBrush(),
                ["SecondaryAccentForegroundBrush"] = new SolidColorBrush()
            };
        }
    }
}
