using Microsoft.UI.Xaml;

namespace ValidationExamples
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public SampleModel SampleModel { get; set; }

        public MainWindow()
        {
            SampleModel = new SampleModel();
            this.InitializeComponent();
        }
    }
}
