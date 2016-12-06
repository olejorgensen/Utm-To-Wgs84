using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Globalization;
using System.Diagnostics;

namespace ProjNetUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private static CultureInfo EnglishCulture = new CultureInfo("en-US");
        private CoordinateTransformationFactory
            cstFactory = new CoordinateTransformationFactory();

        private bool IsBusy { get; set; }
        private List<int> zones;
        public Window1()
        {
            InitializeComponent();

            zones = new List<int>();
            for (int i = 1; i < 61; i++)
            {
                zones.Add(i);
            }
            zone.ItemsSource = zones;
            zone.SelectedIndex = 31;
        }

        private void LatLonTextChanged()
        {
            if (!IsInitialized || IsBusy) return;

            try
            {
                var output = TransformWgs84ToUtm32(
                    new[]
                    {
                        double.Parse(longitudeTextBox.Text, EnglishCulture),
                        double.Parse(latitudeTextBox.Text, EnglishCulture)
                    }
                );

                IsBusy = true;

                easting.Text = output[1].ToString("0.00", EnglishCulture);
                northing.Text = output[0].ToString("0.00", EnglishCulture);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void isNorth_Checked(object sender, RoutedEventArgs e)
        {
            LatLonTextChanged();
        }

        private void LatLonTextChanged(object sender, SelectionChangedEventArgs e)
        {
            LatLonTextChanged();
        }

        private void LatLonTextChanged(object sender, TextChangedEventArgs e)
        {
            LatLonTextChanged();
        }
        
        private void Utm32TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsInitialized || IsBusy) return;

            try
            {
                var output = TransformUtm32ToWgs84(
                    new[]
                    {
                        double.Parse(easting.Text, EnglishCulture),
                        double.Parse(northing.Text, EnglishCulture)
                    }
                );

                IsBusy = true;

                longitudeTextBox.Text = output[1].ToString(EnglishCulture);
                latitudeTextBox.Text = output[0].ToString(EnglishCulture);


            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private double[] TransformUtm32ToWgs84(double[] points)
        {
            ICoordinateTransformation utm32ToWgs84 = cstFactory.CreateFromCoordinateSystems(
                ProjectedCoordinateSystem.WGS84_UTM((int)zone.SelectedItem, isNorth.IsChecked == true),
                GeographicCoordinateSystem.WGS84
            );

            return utm32ToWgs84.MathTransform.Transform(points);
        }

        private double[] TransformWgs84ToUtm32(double[] points)
        {
            ICoordinateTransformation wgs84ToUtm32 = cstFactory.CreateFromCoordinateSystems(
               GeographicCoordinateSystem.WGS84,
               ProjectedCoordinateSystem.WGS84_UTM((int)zone.SelectedItem, isNorth.IsChecked == true)
           );

            return wgs84ToUtm32.MathTransform.Transform(points);
        }

    }
}
