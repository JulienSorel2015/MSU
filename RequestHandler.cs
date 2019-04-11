using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace FirstPlugin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RequestHandler : IExternalEventHandler
    {
        public Request myRequest = new Request();
        
        public Form1 f1;
        Autodesk.Revit.DB.Document doc;
        public void Execute(UIApplication uiapp)
        {
            try
            {
                switch (this.Request.Take())
                {
                    case RequestId.DikdortgenCiz:
                        this.f1.CreateMyLines();
                        break;
                    case RequestId.DuvaraCevir:
                        this.f1.createWall();
                        break;
                    case RequestId.AnalyzeWallGeometry:
                        this.f1.analyzeWallGeometry();
                        break;
                }
            }
            finally
            {
                Application.thisApp.WakeFormUp();
            }
        }
        public string GetName()
        {
            return "MSUInformatikFirstPlugin";
        }
        public Request Request
        {
            get
            {
                return this.myRequest;
            }
        }

    }
}
