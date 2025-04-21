using ElmoMotionControlComponents.Drive.EASComponents.DriveCAN;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Xps.Packaging;

namespace ush4.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для Report.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            FixedDocument fixedDoc = new FixedDocument();

            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            string tempFilename = "temp.xps";
            File.Delete(tempFilename);
            XpsDocument xpsd = new XpsDocument(tempFilename, FileAccess.ReadWrite);
            System.Windows.Xps.XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            xw.Write(report);
            xpsd.Close();
            //PdfSharp.Xps.XpsConverter.Convert(tempFilename, "C:\\Work\\x86\\VNIIM\\USH4\\test_fft\\20240222_142624_F1_X0,001.pdf", 1);
            PdfSharp.Xps.XpsConverter.Convert(tempFilename, RepPathPdf.Text, 1);
        }
    }
}
