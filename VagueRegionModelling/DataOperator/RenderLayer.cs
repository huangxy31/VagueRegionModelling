using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace VagueRegionModelling.DataOperator
{
    /// <summary>
    /// 渲染图层
    /// </summary>
    public class RenderLayer
    {
        public RenderLayer()
        { }

        /// <summary>
        /// 按聚类号对凸包进行渲染
        /// </summary>
        /// <param name="pGeoFeatureLayer"></param>
        /// <param name="fieldName"></param>
        public void DefinePolygonUniqueValueRenderer(IGeoFeatureLayer pGeoFeatureLayer, string fieldName)
        {
            IRandomColorRamp pRandomColorRamp = new RandomColorRampClass();
            //Create the color ramp for the symbols in the renderer.
            pRandomColorRamp.MinSaturation = 20;
            pRandomColorRamp.MaxSaturation = 40;
            pRandomColorRamp.MinValue = 85;
            pRandomColorRamp.MaxValue = 100;
            pRandomColorRamp.StartHue = 76;
            pRandomColorRamp.EndHue = 188;
            pRandomColorRamp.UseSeed = true;
            pRandomColorRamp.Seed = 43;

            //Create the renderer.
            IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();

            ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
            pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
            pSimpleFillSymbol.Outline.Width = 0.4;

            /*  ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
             pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
             pSimpleMarkerSymbol.Size = 5;
             pSimpleMarkerSymbol.Outline = true;
             IRgbColor pLineRgbColor = new RgbColorClass();
             pLineRgbColor.Red = 0;
             pLineRgbColor.Green = 0;
             pLineRgbColor.Blue = 0;
             pSimpleMarkerSymbol.OutlineColor = pLineRgbColor as IColor;
 */
            //These properties should be set prior to adding values.
            pUniqueValueRenderer.FieldCount = 1;
            pUniqueValueRenderer.set_Field(0, fieldName);
            pUniqueValueRenderer.DefaultSymbol = pSimpleFillSymbol as ISymbol;
            pUniqueValueRenderer.UseDefaultSymbol = true;

            IDisplayTable pDisplayTable = pGeoFeatureLayer as IDisplayTable;
            IFeatureCursor pFeatureCursor = pDisplayTable.SearchDisplayTable(null, false) as IFeatureCursor;
            IFeature pFeature = pFeatureCursor.NextFeature();


            bool ValFound;
            int fieldIndex;

            IFields pFields = pFeatureCursor.Fields;
            fieldIndex = pFields.FindField(fieldName);
            while (pFeature != null)
            {
                ISimpleFillSymbol pClassSymbol = new SimpleFillSymbolClass();
                pClassSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                pClassSymbol.Outline.Width = 0.4;

                /*    ISimpleMarkerSymbol pClassSymbol = new SimpleMarkerSymbolClass();
                  pClassSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                  pClassSymbol.Size = 5;
                  pClassSymbol.Outline = true;
                  pClassSymbol.OutlineColor = pLineRgbColor as IColor;
  */
                string classValue;
                classValue = pFeature.get_Value(fieldIndex).ToString();

                //Test to see if this value was added to the renderer. If not, add it.
                ValFound = false;
                for (int i = 0; i <= pUniqueValueRenderer.ValueCount - 1; i++)
                {
                    if (pUniqueValueRenderer.get_Value(i) == classValue)
                    {
                        ValFound = true;
                        break; //Exit the loop if the value was found.
                    }
                }
                //If the value was not found, it's new and will be added.
                if (ValFound == false)
                {
                    pUniqueValueRenderer.AddValue(classValue, fieldName, pClassSymbol as ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, pClassSymbol as ISymbol);
                }
                pFeature = pFeatureCursor.NextFeature();
            }
            //Since the number of unique values is known, the color ramp can be sized and the colors assigned.
            pRandomColorRamp.Size = pUniqueValueRenderer.ValueCount;
            bool bOK;
            pRandomColorRamp.CreateRamp(out bOK);

            IEnumColors pEnumColors = pRandomColorRamp.Colors;
            pEnumColors.Reset();
            for (int j = 0; j <= pUniqueValueRenderer.ValueCount - 1; j++)
            {
                string xv;
                xv = pUniqueValueRenderer.get_Value(j);
                if (xv != "")
                {
                    ISimpleFillSymbol pSimpleFillColor = pUniqueValueRenderer.get_Symbol(xv) as ISimpleFillSymbol;
                    pSimpleFillColor.Color = pEnumColors.Next();
                    pUniqueValueRenderer.set_Symbol(xv, pSimpleFillColor as ISymbol);

                }
            }
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pUniqueValueRenderer;
            pGeoFeatureLayer.DisplayField = fieldName;
        }

        /// <summary>
        /// 按聚类号对凸包进行渲染
        /// </summary>
        /// <param name="pGeoFeatureLayer"></param>
        /// <param name="fieldName"></param>
        public void DefinePointUniqueValueRenderer(IGeoFeatureLayer pGeoFeatureLayer, string fieldName)
        {
            IRandomColorRamp pRandomColorRamp = new RandomColorRampClass();
            //Create the color ramp for the symbols in the renderer.
            pRandomColorRamp.MinSaturation = 20;
            pRandomColorRamp.MaxSaturation = 40;
            pRandomColorRamp.MinValue = 85;
            pRandomColorRamp.MaxValue = 100;
            pRandomColorRamp.StartHue = 76;
            pRandomColorRamp.EndHue = 188;
            pRandomColorRamp.UseSeed = true;
            pRandomColorRamp.Seed = 43;

            //Create the renderer.
            IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();

            /*             ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                        pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                        pSimpleFillSymbol.Outline.Width = 0.4;
            */
            ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
            pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            pSimpleMarkerSymbol.Size = 5;
            pSimpleMarkerSymbol.Outline = true;
            IRgbColor pLineRgbColor = new RgbColorClass();
            pLineRgbColor.Red = 0;
            pLineRgbColor.Green = 0;
            pLineRgbColor.Blue = 0;
            pSimpleMarkerSymbol.OutlineColor = pLineRgbColor as IColor;

            //These properties should be set prior to adding values.
            pUniqueValueRenderer.FieldCount = 1;
            pUniqueValueRenderer.set_Field(0, fieldName);
            pUniqueValueRenderer.DefaultSymbol = pSimpleMarkerSymbol as ISymbol;
            pUniqueValueRenderer.UseDefaultSymbol = true;

            IDisplayTable pDisplayTable = pGeoFeatureLayer as IDisplayTable;
            IFeatureCursor pFeatureCursor = pDisplayTable.SearchDisplayTable(null, false) as IFeatureCursor;
            IFeature pFeature = pFeatureCursor.NextFeature();


            bool ValFound;
            int fieldIndex;

            IFields pFields = pFeatureCursor.Fields;
            fieldIndex = pFields.FindField(fieldName);
            while (pFeature != null)
            {
                /*               ISimpleFillSymbol pClassSymbol = new SimpleFillSymbolClass();
                               pClassSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                               pClassSymbol.Outline.Width = 0.4;
               */
                ISimpleMarkerSymbol pClassSymbol = new SimpleMarkerSymbolClass();
                pClassSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                pClassSymbol.Size = 5;
                pClassSymbol.Outline = true;
                pClassSymbol.OutlineColor = pLineRgbColor as IColor;

                string classValue;
                classValue = pFeature.get_Value(fieldIndex).ToString();

                //Test to see if this value was added to the renderer. If not, add it.
                ValFound = false;
                for (int i = 0; i <= pUniqueValueRenderer.ValueCount - 1; i++)
                {
                    if (pUniqueValueRenderer.get_Value(i) == classValue)
                    {
                        ValFound = true;
                        break; //Exit the loop if the value was found.
                    }
                }
                //If the value was not found, it's new and will be added.
                if (ValFound == false)
                {
                    pUniqueValueRenderer.AddValue(classValue, fieldName, pClassSymbol as ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, pClassSymbol as ISymbol);
                }
                pFeature = pFeatureCursor.NextFeature();
            }
            //Since the number of unique values is known, the color ramp can be sized and the colors assigned.
            pRandomColorRamp.Size = pUniqueValueRenderer.ValueCount;
            bool bOK;
            pRandomColorRamp.CreateRamp(out bOK);

            IEnumColors pEnumColors = pRandomColorRamp.Colors;
            pEnumColors.Reset();
            for (int j = 0; j <= pUniqueValueRenderer.ValueCount - 1; j++)
            {
                string xv;
                xv = pUniqueValueRenderer.get_Value(j);
                if (xv != "")
                {
                    ISimpleMarkerSymbol pSimpleMarkerColor = pUniqueValueRenderer.get_Symbol(xv) as ISimpleMarkerSymbol;
                    pSimpleMarkerColor.Color = pEnumColors.Next();
                    pUniqueValueRenderer.set_Symbol(xv, pSimpleMarkerColor as ISymbol);

                }
            }
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pUniqueValueRenderer;
            pGeoFeatureLayer.DisplayField = fieldName;
        }

        /// <summary>
        /// 点图层的简单渲染
        /// </summary>
        /// <param name="pGeoFeatureLayer"></param>
        public void DefinePointSimpleValueRenderer(IGeoFeatureLayer pGeoFeatureLayer)
        {
            ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
            pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            pSimpleMarkerSymbol.Size = 5;
            pSimpleMarkerSymbol.Outline = true;
            IRgbColor pLineRgbColor = new RgbColorClass();
            pLineRgbColor.RGB = 0;
            //            pLineRgbColor.Red = 0;
            //           pLineRgbColor.Green = 0;
            //           pLineRgbColor.Blue = 0;
            pSimpleMarkerSymbol.OutlineColor = pLineRgbColor as IColor;
            pLineRgbColor.Red = 34;
            pLineRgbColor.Green = 139;
            pLineRgbColor.Blue = 34;
            pSimpleMarkerSymbol.Color = pLineRgbColor as IColor;

            ISimpleRenderer simpleRender = new SimpleRendererClass();

            simpleRender.Symbol = pSimpleMarkerSymbol as ISymbol;
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)simpleRender;
        }
    }
}
