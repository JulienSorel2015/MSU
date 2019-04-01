using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;

//using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
//using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI.Selection;

namespace FirstPlugin
{
    class Class3
    {
        
    }
    public class ModelLineSelectionFilter : ISelectionFilter
    {
        
        public bool AllowElement(Element element)
        {
            if ((element.Category.Name == "Lines"))
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
}
