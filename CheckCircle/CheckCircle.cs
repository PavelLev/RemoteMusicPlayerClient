using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RemoteMusicPlayerClient.CustomFrameworkElements
{
    public class CheckCircle : CheckBox
    {
        public static readonly DependencyProperty OnBrushProperty;
        public static readonly DependencyProperty OffBrushProperty;

        static CheckCircle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckCircle),
                new FrameworkPropertyMetadata(typeof(CheckCircle)));

            OnBrushProperty = DependencyProperty.Register("OnBrush", typeof(Color), typeof(CheckCircle),
                new PropertyMetadata(Color.FromRgb(255, 100, 0)));
            OffBrushProperty = DependencyProperty.Register("OffBrush", typeof(Color), typeof(CheckCircle),
                new PropertyMetadata(Color.FromRgb(145, 145, 145)));
        }

        public CheckCircle()
        {
            Loaded += CheckCircleLoaded;
            var dictionary = new ResourceDictionary
            {
                Source = new Uri("/CheckCircle;component/CheckCircleStyle.xaml", UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(dictionary);
            Style = (Style)Resources[typeof(CheckCircle)];
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if (Template.FindName("CurrentBrushGradientStop", this) is GradientStop gradientStop)
            {
                gradientStop.Color = OnBrush;
            }
            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            if (Template.FindName("CurrentBrushGradientStop", this) is GradientStop gradientStop)
            {
                gradientStop.Color = OffBrush;
            }
            base.OnUnchecked(e);
        }

        private void CheckCircleLoaded(object sender, RoutedEventArgs e)
        {
            if (Template.FindName("CurrentBrushGradientStop", this) is GradientStop gradientStop)
            {
                gradientStop.Color = IsChecked == true ? OnBrush : OffBrush;
            }
        }


        public Color OnBrush
        {
            get => (Color)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }


        public Color OffBrush
        {
            get => (Color)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }
    }
}
