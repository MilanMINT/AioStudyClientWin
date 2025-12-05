using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AioStudy.UI.WpfServices
{
    public static class ProgressBarSmoothBehavior
    {
        public static readonly DependencyProperty SmoothValueProperty =
            DependencyProperty.RegisterAttached(
                "SmoothValue",
                typeof(double),
                typeof(ProgressBarSmoothBehavior),
                new PropertyMetadata(0.0, OnSmoothValueChanged));

        public static double GetSmoothValue(DependencyObject obj) =>
            (double)obj.GetValue(SmoothValueProperty);

        public static void SetSmoothValue(DependencyObject obj, double value) =>
            obj.SetValue(SmoothValueProperty, value);

        private static void OnSmoothValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ProgressBar progressBar)
            {
                var animation = new DoubleAnimation
                {
                    To = (double)e.NewValue,
                    Duration = TimeSpan.FromMilliseconds(1000),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
            }
        }
    }
}
