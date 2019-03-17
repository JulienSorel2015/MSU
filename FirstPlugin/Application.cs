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

                PushButtonData pushButtonData1 = new PushButtonData("Test1", "MSU", Assembly.GetExecutingAssembly().Location, "FirstPlugin.Class1");
                PushButton pushButton1 = ribbonPanel.AddItem((RibbonItemData)pushButtonData1) as PushButton;
                pushButton1.ToolTip = "Tooltip";
                pushButton1.LargeImage = this.BmpImageSource("FirstPlugin.Resources.Report32x32.bmp");
                pushButton1.Image = this.BmpImageSource("FirstPlugin.Resources.Report32x32.bmp");
               
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
    }
}
