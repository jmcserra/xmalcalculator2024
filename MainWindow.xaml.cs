using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlendInterfaces1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class CustomComboBox : ComboBox
    {
        static CustomComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomComboBox), new FrameworkPropertyMetadata(typeof(CustomComboBox)));
        }
    }

    public partial class AppSettings
    {
        private static AppSettings instance;
        private double windowWidth;
        private double windowHeight;

        private AppSettings() { }

        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppSettings();
                }
                return instance;
            }
        }

        public double WindowHeight
        {
            get { return windowHeight; }
            set { windowHeight = value; }
        }

        public double WindowWidth
        {
            get { return windowWidth; }
            set { windowWidth = value; }
        }
    }

    public partial class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = (double)value;
            return height * 0.6; // Adjust the multiplier to your desired font size
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MainWindow : Window
    {
        private bool _isDragging;
        private Point _startPoint;
        private Point comboBoxOriginalPosition;

        public MainWindow()
        {
            InitializeComponent();

            Enumerable.Range(0, 7).Select(i => (int)Math.Pow(10, i)).ToList().ForEach(item => numbersComboBox.Items.Add(item));
            numbersComboBox.SelectedItem = numbersComboBox.Items[numbersComboBox.Items.Count - 1];
            numbersComboBox.DropDownOpened += NumbersComboBox_DropDownOpened;


            //// Create a new ImageBrush
            //ImageBrush brush = new ImageBrush();
            //// Set the ImageSource of the ImageBrush
            //brush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BlendInterfaces1;component/images/background.jpg"));
            //// Set the ImageBrush as the background of the window
            //this.Background = brush;
            this.SourceInitialized += MainWindow_SourceInitialized;
        }

        private void NumbersComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // Store the original position of the ComboBox
                comboBoxOriginalPosition = numbersComboBox.PointToScreen(new Point(0, 0));
            }
           // HandleComboBoxPosition();
        }

        private void HandleComboBoxPosition()
        {
            Point newComboBoxPosition = numbersComboBox.PointToScreen(new Point(0, 0));

            double deltaX = newComboBoxPosition.X - comboBoxOriginalPosition.X;
            double deltaY = newComboBoxPosition.Y - comboBoxOriginalPosition.Y;

            // Update the position of the ComboBox
            Canvas.SetLeft(numbersComboBox, Canvas.GetLeft(numbersComboBox) + deltaX);
            Canvas.SetTop(numbersComboBox, Canvas.GetTop(numbersComboBox) + deltaY);

            // Update the original position for future calculations
            comboBoxOriginalPosition = newComboBoxPosition;
        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
        }

        
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(this);
                double dx = currentPoint.X - _startPoint.X;
                double dy = currentPoint.Y - _startPoint.Y;

                Left += dx;
                Top += dy;
            }
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppSettings.Instance.WindowWidth = this.Width;
            // Save settings to a file or database if needed
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AppSettings.Instance.WindowWidth = this.Width;
            // Save settings to a file or database if needed
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = AppSettings.Instance.WindowWidth;
            // Load settings from a file or database if needed
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppSettings.Instance.WindowWidth = this.Width;
            AppSettings.Instance.WindowHeight = this.Height;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private int clickCount = 0;
        private DispatcherTimer timer;

        private void ButtonOnOff_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
            if (clickCount == 1)
            {
                // Start the timer
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            else if (clickCount == 2)
            {
                // Stop the timer and shut down the application
                timer.Stop();
                Application.Current.Shutdown();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Reset the click count if the second click doesn't happen within 2 seconds
            clickCount = 0;
            timer.Stop();
        }

        private void Viewbox_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
        }

        private void MessageBox_OnlyaClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was pressed!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}