using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.ExtensibleStorage;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;//Add PresentationCore
using System.Windows.Media.Imaging;//AddPresentationCore

namespace FirstPlugin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication, IDisposable
    {
        internal static Application thisApp;
        public static UIControlledApplication controlledApplication;
        Form1 f1;
        public Result OnStartup(UIControlledApplication application)
        {
            Application.controlledApplication = application;
            Application.thisApp = this;
            CreateRibbonPanel();
            
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public void ShowForm1(UIApplication uiapp, ExternalCommandData cData)
        {
            if (this.f1 != null && !this.f1.IsDisposed)
                return;
            RequestHandler handler = new RequestHandler();
            this.f1 = new Form1(ExternalEvent.Create((IExternalEventHandler)handler), handler, cData);
            this.f1.Show();
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            
        }
        private RibbonPanel CreateRibbonPanel()
        {
            RibbonPanel ribbonPanel = null;
            try
            {
                controlledApplication.CreateRibbonTab("MSU");
                ribbonPanel = Application.controlledApplication.CreateRibbonPanel("MSU", Guid.NewGuid().ToString());
                ribbonPanel.Name = "MSU Informatik";
                ribbonPanel.Title = "Informatik";

                PushButtonData pushButtonData1 = 
                new PushButtonData("Test1", "MSU", Assembly.GetExecutingAssembly().Location, "FirstPlugin.Class1");
                PushButton pushButton1 = ribbonPanel.AddItem((RibbonItemData)pushButtonData1) as PushButton;
                pushButton1.ToolTip = "Tooltip";
                pushButton1.LargeImage = this.BmpImageSource("FirstPlugin.Resources.Report32x32.bmp");
                pushButton1.Image = this.BmpImageSource("FirstPlugin.Resources.Report32x32.bmp");
                PushButtonData pushButtonData2 =
                new PushButtonData("Test2", "Informatik", Assembly.GetExecutingAssembly().Location, "FirstPlugin.Class2");
                PushButton pushButton2 = ribbonPanel.AddItem((RibbonItemData)pushButtonData2) as PushButton;
                pushButton2.ToolTip = "Tooltip2";
                pushButton2.LargeImage = this.BmpImageSource("FirstPlugin.Resources.Report32x32.bmp");
                pushButton2.Image = this.BmpImageSource("FirstPlugin.Resources.Report32x32.bmp");

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            return ribbonPanel;
        }
        private ImageSource BmpImageSource(string embeddedPath)
        {
            return (ImageSource)new BmpBitmapDecoder(this.GetType().Assembly.GetManifestResourceStream(embeddedPath), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
        }
        public void WakeFormUp()
        {
            if (this.f1 != null)
            {
                this.f1.WakeUp();
            }
        }
        

    }
}
