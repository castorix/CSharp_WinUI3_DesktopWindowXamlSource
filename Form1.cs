using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.XamlTypeInfo;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

using WinRT;

namespace CSharp_WinUI3_DesktopWindowXamlSource
{
    public partial class Form1 : Form
    {
        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public const int WM_DPICHANGED = 0x02E0;

        public Microsoft.UI.Xaml.Hosting.DesktopWindowXamlSource? m_dwxs = null;
        private SUBCLASSPROC SubClassDelegate;

        public int m_nXPos = 382, m_nYPos = 20, m_nWidth = 400, m_nHeight = 600;

        public Form1()
        {
            //WinRT.ComWrappersSupport.InitializeComWrappers();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Seems to work without this line...
            Microsoft.UI.Xaml.Hosting.WindowsXamlManager.InitializeForCurrentThread();
            m_dwxs = new Microsoft.UI.Xaml.Hosting.DesktopWindowXamlSource();
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(this.Handle);
            m_dwxs.Initialize(myWndId);
            // Microsoft.UI.Content.DesktopChildSiteBridge
            var sb = m_dwxs.SiteBridge;
            var csv = sb.SiteView;
            var rs = csv.RasterizationScale;
            Windows.Graphics.RectInt32 rect = new Windows.Graphics.RectInt32((int)(m_nXPos * rs), (int)(m_nYPos * rs), (int)(m_nWidth * rs), (int)(m_nHeight * rs));
            sb.MoveAndResize(rect);
            richTextBox1.Text =
            @"  <Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                    xmlns:controls='using:Microsoft.UI.Xaml.Controls'
                    Background='Black'>
                    <controls:ColorPicker x:Name='cp1' IsAlphaEnabled='False' IsMoreButtonVisible='True' Margin='0,10,0,0'>
                    </controls:ColorPicker>
            </Grid>";
            CenterToScreen();

            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
        }

        private void button1_Click(object sender, EventArgs e)
        {  
            Microsoft.UI.Xaml.Controls.Grid gridRoot = new Microsoft.UI.Xaml.Controls.Grid
            {
                 Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkBlue)
            };

            gridRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            gridRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Pixel) });
            gridRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200, GridUnitType.Pixel) });
            gridRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gridRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            //gridRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            Microsoft.UI.Xaml.Controls.CalendarDatePicker cdp = new Microsoft.UI.Xaml.Controls.CalendarDatePicker()
            {
                PlaceholderText = "Pick a date",
                Header = "Calendar Date Picker",
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
            };
            cdp.SetValue(Grid.RowProperty, 0);
            gridRoot.Children.Add(cdp);

            Microsoft.UI.Xaml.Controls.TextBlock tb = new Microsoft.UI.Xaml.Controls.TextBlock()
            {
                Text = "Test FlipView",
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Yellow),
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                FontSize = 20,
            };
            tb.SetValue(Grid.RowProperty, 1);
            gridRoot.Children.Add(tb);         

            List<string> strings = new List<string> {
                "ms-appx:///Assets/Beach.jpg",
                "ms-appx:///Assets/Beach2.jpg",
                "ms-appx:///Assets/Island.jpg",
                "ms-appx:///Assets/Sunset.jpg",
                "ms-appx:///Assets/Sunset2.jpg",
                "ms-appx:///Assets/Sunset3.jpg",
                "ms-appx:///Assets/Paradise.jpg"};
            var items = new List<FrameworkElement>();
            foreach (string s in strings)
            {
                var img = new Microsoft.UI.Xaml.Controls.Image() { Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(s)) };
                items.Add(img);
            }

            Microsoft.UI.Xaml.Controls.FlipView fv = new Microsoft.UI.Xaml.Controls.FlipView()
            {
                Width = 350,
                Height = 200,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black),
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                ItemsSource = items
            };
            fv.SetValue(Grid.RowProperty, 2);
            gridRoot.Children.Add(fv);

            Microsoft.UI.Xaml.Controls.TreeView tv = new Microsoft.UI.Xaml.Controls.TreeView()
            {               
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black),
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                MinWidth = 350,
                Margin = new Thickness(0, 10, 0, 0),
                CanDragItems = true
            };
            TreeViewNode node1  = new TreeViewNode() { Content = "Node 1" };
            node1.Children.Add(new TreeViewNode() { Content = "Item 1" });
            node1.Children.Add(new TreeViewNode() { Content = "Item 2" });
            TreeViewNode node2 = new TreeViewNode() { Content = "Node 2" };
            TreeViewNode subnode2 = new TreeViewNode() { Content = "Sub Node 2" };
            for (int i = 1; i<=10; i++)
                subnode2.Children.Add(new TreeViewNode() { Content = "Item " + i.ToString() });           
            tv.RootNodes.Add(node1);
            node2.Children.Add(subnode2);
            tv.RootNodes.Add(node2);
            tv.SetValue(Grid.RowProperty, 3);
            gridRoot.Children.Add(tv);

            ToggleSwitch ts;
            ProgressRing pg;
            Microsoft.UI.Xaml.Controls.StackPanel sp = new Microsoft.UI.Xaml.Controls.StackPanel()
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Horizontal,
                Children =
                {
                    (ts = new ToggleSwitch
                    {
                        Header = "Toggle work",
                        OffContent = "Do work",
                        OnContent = "Working",
                        VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
                        //HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                        Margin = new Thickness(80, 15, 0, 0),
                        IsOn = false
                    }),
                    (pg = new ProgressRing
                    {
                        Width = 50,
                        Height = 50,
                        VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
                        //HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                        Margin = new Thickness(20, 15, 0, 0),
                        IsActive = false
                    })
                }
            };
            ts.Toggled += (sender, eargs) =>
            {
                ToggleSwitch? ts = sender as ToggleSwitch;
                pg.IsActive = ts.IsOn ? true : false;
            };
            sp.SetValue(Grid.RowProperty, 4);
            gridRoot.Children.Add(sp);

            if (m_dwxs != null)
                m_dwxs.Content = gridRoot;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UIElement? uiElement = null;
            try
            {
                uiElement = Microsoft.UI.Xaml.Markup.XamlReader.Load(richTextBox1.Text) as UIElement;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (m_dwxs != null)
            {
                try
                {
                    m_dwxs.Content = uiElement;
                    var childElement = FindChildElementByName(uiElement, "cp1");
                    if (childElement != null)
                    {
                        try
                        {
                            ColorPicker cp = (ColorPicker)childElement;
                            cp.ColorChanged += (sender, args) =>
                            {
                                this.BackColor = System.Drawing.Color.FromArgb(255, args.NewColor.R, args.NewColor.G, args.NewColor.B);
                            };
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        // Adapted from MS C++ sample
        private DependencyObject? FindChildElementByName(DependencyObject tree, string sName)
        {
            for (int i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(tree); i++)
            {
                DependencyObject child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(tree, i);
                if (child != null && ((FrameworkElement)child).Name == sName)
                    return child;
                else
                {
                    DependencyObject? childInSubtree = FindChildElementByName(child, sName);
                    if (childInSubtree != null)
                        return childInSubtree;
                }
            }
            return null;
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_DPICHANGED:
                    {
                        var sb = m_dwxs.SiteBridge;
                        var csv = sb.SiteView;
                        var rs = csv.RasterizationScale;
                        Windows.Graphics.RectInt32 rect = new Windows.Graphics.RectInt32((int)(m_nXPos * rs), (int)(m_nYPos * rs), (int)(m_nWidth * rs), (int)(m_nHeight * rs));
                        sb.MoveAndResize(rect);
                    }
                    break;              
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }
}