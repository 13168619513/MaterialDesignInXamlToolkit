﻿using MaterialDesignColors.Recommended;
using MaterialDesignThemes.Wpf;
using ShowMeTheXAML;
using System.Windows;

namespace MaterialDesignColors.WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //This is an alterate way to initialize MaterialDesignInXAML if you don't use the MaterialDesignResourceDictionary in App.xaml
            //this.WithMaterialDesign(MaterialDesignTheme.Light, CyanSwatch.Cyan500, LimeSwatch.LimeA700);

            
            //Illustration of setting culture info fully in WPF:
            /*             
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            */
            base.OnStartup(e);
        }
    }
}
