using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace FirstPlugin
{
    public partial class Form1 :System.Windows.Forms.Form
    {
        private RequestHandler myHandler;
        private ExternalEvent myExEvent;
        ExternalCommandData commandData;
        UIApplication uiApp;
        Autodesk.Revit.DB.Document doc;
        UIDocument uiDocument;
        
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(ExternalEvent exEvent, RequestHandler handler, ExternalCommandData cData)
        {
            InitializeComponent();
            myExEvent = exEvent;
            myHandler = handler;
            commandData = cData;
            uiApp = commandData.Application;
            uiDocument = commandData.Application.ActiveUIDocument;
            doc = commandData.Application.ActiveUIDocument.Document;
            myHandler.f1 = this;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.DikdortgenCiz);
        }
        public void CreateMyLines()
        {
            try
            {

                XYZ point1 = new XYZ(0, 0, 0);
                XYZ point2 = new XYZ(2, 0, 0);
                XYZ point3 = new XYZ(2, 2, 0);
                XYZ point4 = new XYZ(0, 2, 0);
                List<XYZ> vertices = new List<XYZ>();
                vertices.Add(point1);
                vertices.Add(point2);
                vertices.Add(point3);
                vertices.Add(point4);
                vertices.Add(point1);
                lineCiz(vertices);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            

        }
        private void lineCiz(List<XYZ> vertexList)
        {
            try
            {
                if (vertexList.Count > 1)
                {
                    for (int i = 0; i < vertexList.Count - 1; i++)
                    {
                        XYZ v1 = vertexList[i];
                        XYZ v2 = vertexList[i + 1];
                        XYZ normal = new XYZ(0, 0, 1);
                        using (Transaction tr = new Transaction(doc, "Yüzey Çiz"))
                        {
                            tr.Start();
                            Plane plane = Plane.CreateByNormalAndOrigin(normal, v1);
                            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                            Line line = Line.CreateBound(vertexList[i], vertexList[i + 1]);
                            ModelCurve ml = doc.Create.NewModelCurve(line, sketchPlane);
                            GraphicsStyle gs = ml.LineStyle as GraphicsStyle;
                            gs.GraphicsStyleCategory.LineColor = new Autodesk.Revit.DB.Color(255, 10, 10);
                            tr.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MakeRequest(RequestId request)
        {
            myHandler.Request.Make(request);
            myExEvent.Raise();
            DozeOff();
        }
        private void EnableCommands(bool status)
        {
            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                ctrl.Enabled = status;
            }
        }
        public void WakeUp()
        {
            EnableCommands(true);
        }
        private void DozeOff()
        {
            EnableCommands(false);
        }
    }
}
