using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

namespace NuPlot.Demo
{
    /// <summary>
    /// Window demonstrating the use of the NuPlot component.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DynamicTimeScaleProperty =
            DependencyProperty.Register("DynamicTimeScale", typeof(int), typeof(MainWindow), new PropertyMetadata(49));

        private Random _random = new Random();
        private List<int> _live = new List<int>();
        private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();

            DependencyPropertyDescriptor.FromProperty(DynamicTimeScaleProperty, typeof(MainWindow)).AddValueChanged(this, (s, e) =>
            {
                Trace.WriteLine(string.Format("DynamicTimeScale changed to {0}.", DynamicTimeScale));
                OnPropertyChanged("DynamicTimeScaleData");
            });

            Loaded += (s, e) =>
                {
                    _timer = new Timer(new TimerCallback(_ =>
                    {
                        _live.Add(_live.Count);

                        //Dispatcher.BeginInvoke(new Action(() =>
                        //{
                            var handler = PropertyChanged;
                            if (handler != null)
                            {
                                handler(this, new PropertyChangedEventArgs("Live"));
                            }
                        //}));

                    }), null, 1000, 1000);
                };

            Unloaded += (s, e) =>
                {
                    _timer.Dispose();
                };
        }

        public IList<int> Live
        {
            get { return _live; }
        }

        public int DynamicTimeScale
        {
            get { return (int)GetValue(DynamicTimeScaleProperty); }
            set { SetValue(DynamicTimeScaleProperty, value); }
        }

        public IList<StockPrice> StockPrices
        {
            get
            {
                var prices = new List<StockPrice>();
                using (var reader = new StreamReader(App.GetResourceStream(new Uri("StockPrices.txt", UriKind.Relative)).Stream))
                {
                    for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        if (line.StartsWith("#")) continue;

                        var items = line.Split(',');
                        int year = int.Parse(items[0].Substring(0, 4));
                        int month = int.Parse(items[0].Substring(4, 2));
                        int day = int.Parse(items[0].Substring(6, 2));
                        double open = double.Parse(items[2], CultureInfo.InvariantCulture);
                        prices.Add(new StockPrice { Date = new DateTime(year, month, day), Price = open });
                    }
                }
                return prices;
            }
        }

        public StockPrice[] DynamicTimeScaleData
        {
            get
            {
                var n = 1000;
                var prices = new StockPrice[n];
                var time = new DateTime(1800, 1, 1);
                var delta = TimeSpan.FromMilliseconds(ScaleToMilliseconds(DynamicTimeScale));
                for (int i = 0; i < n; i++)
                {
                    prices[i] = new StockPrice { Date = time, Price = i };
                    time = time + delta;
                }
                return prices;
            }
        }

        private double ScaleToMilliseconds(int scale)
        {
            const int x0 = 0;
            const int x1 = 100;
            double x = (scale - x0) / (double)(x1 - x0); // [0, 1]

            var logY0 = Math.Log(1);
            var logY1 = Math.Log(365.0 * 24 * 3600 * 1000); // 1 year in milliseconds
            double logY = logY0 + x * (logY1 - logY0);
            return Math.Exp(logY);
        }

        public Point[] SincPoints
        {
            get
            {
                // sample adapted from NPlot demo PlotSincFunction.
                int N = 1000;
                double[] a = new double[N];
                double[] b = new double[N];
                double mult = 0.00001f;
                for (int i = 0; i < N; ++i)
                {
                    a[i] = ((double)_random.Next(1000) / 5000.0f - 0.1f) * mult;
                    if (i == N / 2)
                    {
                        b[i] = 1.0f * mult;
                    }
                    else
                    {
                        var x = -12.5 + i * 25.0 / (N - 1);
                        b[i] = Math.Sin(x) / x;
                        b[i] *= mult;
                    }
                    a[i] += b[i];
                }

                Point[] p = new Point[N];
                for (int i = 0; i < N; i++)
                {
                    p[i] = new Point(-500 + 1000 * (double)i / N, a[i]);
                }
                return p;
            }
        }

        public Point[] Sine
        {
            get
            {
                int N = 1000;
                Point[] p = new Point[N];
                for (int i = 0; i < N; i++)
                {
                    p[i].X = -Math.PI + i * (2 * Math.PI) / (N - 1);
                    p[i].Y = Math.Sin(p[i].X);
                }
                return p;
            }
        }

        public Point[] Cosine
        {
            get
            {
                int N = 1000;
                Point[] p = new Point[N];
                for (int i = 0; i < N; i++)
                {
                    p[i].X = -Math.PI + i * (2 * Math.PI) / (N - 1);
                    p[i].Y = Math.Cos(p[i].X);
                }
                return p;
            }
        }

        public Point[] Tangent
        {
            get
            {
                int N = 1000;
                Point[] p = new Point[N];
                for (int i = 0; i < N; i++)
                {
                    p[i].X = -Math.PI + i * (2 * Math.PI) / (N - 1);
                    p[i].Y = Math.Tan(p[i].X);
                }
                return p;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
