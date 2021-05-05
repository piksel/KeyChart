using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using Windows.UI.Xaml;
using KeyChart;
using KeyChart.Keyboards;
using static Windows.UI.Xaml.DependencyProperty;

#nullable enable

namespace KeyChart
{
    public partial class LayerKey
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key", typeof(Key), typeof(LayerKey), new PropertyMetadata(default(Key)));

        public Key? Key
        {
            get => (Key) GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public LayerKey()
        {
            InitializeComponent();

            //RegisterPropertyChangedCallback(KeyProperty, UpdateTemplateProps);
            // RegisterPropertyChangedCallback(KeyMapProperty, UpdateTemplateProps);

            // Translation += new Vector3(0, 0, 3);
        }

    }
}