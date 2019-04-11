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

        const double mmToFeet = 0.0032808399;
        const double feetTomm = 304.8;

        const double DOUBLE_EPS = 1.0e-09;
        const double _inch = 1.0 / 12.0;
        const double _sixteenth = _inch / 16.0;
        const double _16 = _inch / 16.0;
        const double _32 = _inch / 32;
        const double _radianToDegree = 57.2957795;

        UIDocument uiDocument;
        ElementId informatikWallId;
        
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
                XYZ point2 = new XYZ(20, 0, 0);
                XYZ point3 = new XYZ(20, 30, 0);
                XYZ point4 = new XYZ(0, 30, 0);
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

        private void button2_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.DuvaraCevir);
        }
        public void analyzeWallGeometry()
        {
            try
            {
                #region
                uiApp.ActiveUIDocument.Selection.GetElementIds().Clear();
                ISelectionFilter selFilter = new WallSelectionFilter();
                this.Hide();
                IList<Reference> walls = uiApp.ActiveUIDocument.Selection.PickObjects(ObjectType.Element, selFilter, "Select walls");
                this.Show();
                //MessageBox.Show(walls.Count.ToString());
                #endregion
                #region set computing options
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.IncludeNonVisibleObjects = true;
                opt.View = doc.ActiveView;
                #endregion
                String faceInfo = "";
                String lineInfo = "";
                List<Face> faceList = new List<Face>();
                foreach (Reference r in walls)
                {
                    Wall wall = doc.GetElement(r) as Wall;
                    Autodesk.Revit.DB.GeometryElement geomElem = wall.get_Geometry(opt);
                    #region loop
                    foreach (GeometryObject geomObj in geomElem)
                    {
                        Solid geomSolid = geomObj as Solid;
                        if (null != geomSolid)
                        {
                            int faces = 0;
                            int edges = 0;
                            double totalArea = 0;
                            double totalLength = 0;
                            foreach (Face geomFace in geomSolid.Faces)
                            {
                                UV myUV = new UV(0, 0);
                                XYZ normal = geomFace.ComputeNormal(myUV);
                                //MessageBox.Show(normal.ToString());
                                if((normal.Z==-1)||(normal.Z==1))
                                {
                                    faceList.Add(geomFace);
                                }
                                //faces++;
                                //faceInfo += "Face " + faces + " area: " + geomFace.Area.ToString() + "\n";
                                //totalArea += geomFace.Area;
                                
                                //List<XYZ> pointList = new List<XYZ>();
                                //foreach(EdgeArray ea in geomFace.EdgeLoops)
                                //{
                                    
                                //    foreach(Edge edge in ea)
                                //    {
                                //        edges++;
                                        
                                //        XYZ start = edge.Evaluate(0);
                                //        XYZ finish = edge.Evaluate(1);
                                //        pointList.Add(start);
                                //        pointList.Add(finish);
                                //        totalLength += edge.ApproximateLength;
                                //        //lineCiz(pointList);
                                //    }
                                //}
                                //faceInfo += "Number of faces: " + faces + "\n";
                                //faceInfo += "Total area: " + totalArea.ToString() + "\n";
                                ////lineInfo += "Number of Edges" + edges + "\n";
                                ////lineInfo += "TotalLength:" + totalLength.ToString() + "\n";
                                //TaskDialog.Show("Faces", faceInfo);
                                ////TaskDialog.Show("Edges", lineInfo);
                            }


                        }
                        

                    }
                    #endregion
                }
                if(faceList.Count>0)
                {
                    for(int i=0;i<faceList.Count;++i)
                    {
                        Face myFace = faceList[i];
                        EdgeArrayArray eaa= myFace.EdgeLoops;
                        foreach(EdgeArray ea in eaa)
                        {
                            foreach(Edge e in ea)
                            {
                                List<XYZ> yeniListe = new List<XYZ>();
                                IList<XYZ> pointList= e.Tessellate();
                                foreach(XYZ point in pointList)
                                {
                                    yeniListe.Add(point);
                                }
                                lineCiz(yeniListe);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        public void createWall()
        {
            try
            {
               
                //Wall.Create(Document document, Curve curve, ElementId wallTypeId, ElementId levelId, double wallHeight, double wallOffset, bool flip, bool structural)
                WallType yeniWallType = null;
                bool bulundu = false;
                FilteredElementCollector wallTypes = new FilteredElementCollector(doc).OfClass(typeof(WallType));
                ElementId defaultElementTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.WallType);
                #region Informatik isimli duvar tipini ara, yoksa yarat
                foreach (WallType wt in wallTypes)
                {
                    if (wt.Name == "Informatik")
                    {
                       
                            informatikWallId = wt.Id;
                            bulundu = true;
                            break;
                    }
                }
                if (bulundu == false)
                {
                    foreach (WallType wt in wallTypes)
                    {
                        if (wt.Id == defaultElementTypeId)
                        {
                            using (Transaction transaction = new Transaction(doc, "DuplicateWallType"))
                            {
                                transaction.Start();
                                yeniWallType = wt.Duplicate("Informatik") as WallType;
                                CompoundStructure cs = yeniWallType.GetCompoundStructure();
                                ElementId materialId = new ElementId(539);
                                IList<CompoundStructureLayer> cslList = new List<CompoundStructureLayer>();
                                CompoundStructureLayer csl = new CompoundStructureLayer(20 * mmToFeet, MaterialFunctionAssignment.Finish1, materialId);
                                cslList.Add(csl);
                                csl = new CompoundStructureLayer(160 * mmToFeet, MaterialFunctionAssignment.Structure, materialId);
                                cslList.Add(csl);
                                csl = new CompoundStructureLayer(20 * mmToFeet, MaterialFunctionAssignment.Finish2, materialId);
                                cslList.Add(csl);
                                CompoundStructure yeniStructure = CompoundStructure.CreateSimpleCompoundStructure(cslList);
                                yeniWallType.SetCompoundStructure(yeniStructure);
                                doc.SetDefaultElementTypeId(ElementTypeGroup.WallType, yeniWallType.Id);
                                transaction.Commit();
                                informatikWallId = yeniWallType.Id;
                            }
                            break;
                        }
                    }
                }
                #endregion
                #region levelId bul
                List<Level> levels = new List<Level>();
                FilteredElementCollector filteredLevelCollector = new FilteredElementCollector(doc);
                filteredLevelCollector.OfClass(typeof(Level));
                levels = filteredLevelCollector.Cast<Level>().ToList();
                ElementId levelId = null;
                foreach (Level l in levels)
                {
                    if (l.Name == "Level 0")
                    {
                        levelId = l.Id;
                        break;
                    }
                }
                #endregion
                #region cizgileri seçtirt
                uiApp.ActiveUIDocument.Selection.GetElementIds().Clear();
                ISelectionFilter selFilter = new ModelLineSelectionFilter();
                this.Hide();
                IList<Reference> curves = uiApp.ActiveUIDocument.Selection.PickObjects(ObjectType.Element, selFilter, "Select path Curve(s) for partition wall");
                this.Show();
                #endregion
                #region set computing options
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.IncludeNonVisibleObjects = true;
                opt.View = doc.ActiveView;

                #endregion
                #region duvar yüksekliği ve offsetini belirle
                double wallHeight = 3000 * mmToFeet;
                double wallOffset = 0;
                #endregion
                List<Wall> yeniDuvarlar = new List<Wall>();
                if (curves.Count > 0)
                {
                    #region döngü
                    for (int i = 0; i < curves.Count; i++)
                    {
                        XYZ globalPoint;
                        Wall yeniDuvar = null;
                        globalPoint = curves[i].GlobalPoint;
                        Element tempCurve = doc.GetElement(curves[i]) as Element;
                        Curve curve = null;
                        if (tempCurve != null)
                        {
                            #region element geometrisinden Curve bulunabiliyor mu
                            foreach (var geoObj in tempCurve.get_Geometry(opt))
                            {
                                Curve cv = geoObj as Curve;
                                if (cv != null)
                                {
                                    curve = cv;
                                    break;
                                }
                            }
                            #endregion
                            #region duvar üret
                            if (curve != null)
                            {
                                using (Transaction wallCreate = new Transaction(doc, "Duvar Yarat"))
                                {
                                    wallCreate.Start();
                                    yeniDuvar = Wall.Create(doc, curve, informatikWallId, levelId, wallHeight, wallOffset, false, false);
                                    if(yeniDuvar!=null)
                                    {
                                        yeniDuvarlar.Add(yeniDuvar);
                                    }
                                    wallCreate.Commit();
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #region parametreleri kontrol et
                    if (yeniDuvarlar.Count > 0)
                    {
                        Wall yeniDuvar = yeniDuvarlar[0];
                        string s = string.Empty;
                        foreach (Parameter p in yeniDuvar.Parameters)
                        {
                            s += "Name="+p.Definition.Name + " StorageType="+p.StorageType.ToString()+"\r\n";

                        }
                        MessageBox.Show(s);
                    }
                    #endregion
                }
                if (curves.Count < 1)
                {
                    TaskDialog.Show("Seçilmedi", "En az bir çizgi seçmelisiniz");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.AnalyzeWallGeometry);
        }
    }
}
