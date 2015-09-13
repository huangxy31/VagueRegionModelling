using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geometry;

namespace VagueRegionModelling.DataOperator
{
    public class SaveShapefile
    {
        private IFeatureLayer m_polygonLayer;
        private IFeatureLayer m_pointLayer;
        private DataInformation m_dataInfo;
        private Clusters m_clusters;

        public SaveShapefile(DataInformation dataInfo, Clusters clusters)
        {
            m_dataInfo = dataInfo;
            m_clusters = clusters;
        }

        public void CreatePolygonShapefile()
        {
            string filePath = m_dataInfo.GetOutputFilePath();
            string fileName = m_dataInfo.GetOutputFileName();
            ISpatialReference spatialReference = m_dataInfo.GetSpatialReference();

            //打开工作空间
            const string strShapeFieldName = "shape";
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pWS = (IFeatureWorkspace)pWSF.OpenFromFile(filePath, 0);

            //设置字段集
            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;

            //设置字段
            IField pField = new FieldClass();
            IFieldEdit pFieldEdit = (IFieldEdit)pField;

            //创建类型为几何类型的字段
            pFieldEdit.Name_2 = strShapeFieldName;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            //为esriFieldTypeGeometry类型的字段创建几何定义，包括类型和空间参照 
            IGeometryDef pGeoDef = new GeometryDefClass();     //The geometry definition for the field if IsGeometry is TRUE.
            IGeometryDefEdit pGeoDefEdit = (IGeometryDefEdit)pGeoDef;
            pGeoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            pGeoDefEdit.SpatialReference_2 = spatialReference;

            pFieldEdit.GeometryDef_2 = pGeoDef;
            pFieldsEdit.AddField(pField);


            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "index";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            pFieldEdit.Precision_2 = 7;//数值精度
            pFieldsEdit.AddField(pField);

            //创建shapefile
            pWS.CreateFeatureClass(fileName, pFields, null, null, esriFeatureType.esriFTSimple, strShapeFieldName, "");

            //在featureclass中创建feature
            IWorkspaceEdit workspaceEdit = pWS as IWorkspaceEdit;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();
            IFeatureClass featureClass = pWS.OpenFeatureClass(fileName);
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            IFeatureCursor featureCursor = featureClass.Search(null, true);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                feature.Delete();
                feature = featureCursor.NextFeature();
            }
            featureCursor = featureClass.Insert(true);
            int typeFieldIndex = featureClass.FindField("index");
            for (int i = 0; i < m_clusters.GetClusterCount(); i++)
            {
                Cluster currentCluster = m_clusters.GetCluster(i);
                IPolygon currentPolygon = currentCluster.GetConvexHull();
                //we know that there are IPoints only in the Geometrycollection. 
                //But this is the safe and recommended way
                if (currentPolygon != null)
                {
                    featureBuffer.set_Value(typeFieldIndex, currentCluster.GetClusterIndex());
                    featureBuffer.Shape = currentPolygon as IGeometry;
                    object featureOID = featureCursor.InsertFeature(featureBuffer);
                }
            }
            featureCursor.Flush();
            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);



            //将新创建的shapfile作为图层添加到map里
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = featureClass.AliasName;

            m_polygonLayer = featureLayer;

            /*
            //提前设置渲染
            DefinePolygonUniqueValueRenderer(featureLayer as IGeoFeatureLayer, "ClusterIndex");
            m_mapControl.AddLayer(featureLayer as ILayer);
            this.m_mapControl.Refresh();
             */
        }

        public void CreatePointsShapefile()
        {
            string filePath = m_dataInfo.GetOutputFilePath();
            string fileName = m_dataInfo.GetOutputFileName();
            ISpatialReference spatialReference = m_dataInfo.GetSpatialReference();
            int index = fileName.LastIndexOf(".");
            fileName = fileName.Substring(0, index);
            fileName = fileName + "_pts.shp";
            //打开工作空间
            const string strShapeFieldName = "shape";
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pWS = (IFeatureWorkspace)pWSF.OpenFromFile(filePath, 0);

            //设置字段集
            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;

            //设置字段
            IField pField = new FieldClass();
            IFieldEdit pFieldEdit = (IFieldEdit)pField;

            //创建类型为几何类型的字段
            pFieldEdit.Name_2 = strShapeFieldName;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            //为esriFieldTypeGeometry类型的字段创建几何定义，包括类型和空间参照 
            IGeometryDef pGeoDef = new GeometryDefClass();     //The geometry definition for the field if IsGeometry is TRUE.
            IGeometryDefEdit pGeoDefEdit = (IGeometryDefEdit)pGeoDef;
            pGeoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            pGeoDefEdit.SpatialReference_2 = spatialReference;

            pFieldEdit.GeometryDef_2 = pGeoDef;
            pFieldsEdit.AddField(pField);

            pField = new FieldClass();
            pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "index";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            pFieldEdit.Precision_2 = 7;//数值精度
            //            pFieldEdit.Scale_2 = 3;//小数点位数
            pFieldsEdit.AddField(pField);


            //创建shapefile
            pWS.CreateFeatureClass(fileName, pFields, null, null, esriFeatureType.esriFTSimple, strShapeFieldName, "");

            //在featureclass中创建feature            
            IWorkspaceEdit workspaceEdit = pWS as IWorkspaceEdit;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();
            IFeatureClass featureClass = pWS.OpenFeatureClass(fileName);
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            IFeatureCursor featureCursor = featureClass.Search(null, true);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                feature.Delete();
                feature = featureCursor.NextFeature();
            }
            featureCursor = featureClass.Insert(true);
            int typeFieldIndex = featureClass.FindField("index");
            //插入点，并写入聚类号
            for (int i = 0; i < m_clusters.GetClusterCount(); i++)
            {
                Cluster currentCluster = m_clusters.GetCluster(i);
                List<IPoint> currentPoints = currentCluster.GetPointsList();
                for (int j = 0; j < currentPoints.Count; j++)
                {
                    featureBuffer.set_Value(typeFieldIndex, currentCluster.GetClusterIndex());
                    featureBuffer.Shape = currentPoints[j] as IGeometry;
                    object featureOID = featureCursor.InsertFeature(featureBuffer);
                }
            }
            featureCursor.Flush();
            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);

            //将新创建的shapfile作为图层添加到map里
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = featureClass.AliasName;

            m_pointLayer = featureLayer;
            ////设置渲染
            //DefinePointUniqueValueRenderer(featureLayer as IGeoFeatureLayer, "Class");
            //m_mapControl.AddLayer(featureLayer as ILayer);
            //this.m_mapControl.Refresh();
        }

        public IFeatureLayer GetPolygonFeatureLayer()
        {
            return m_polygonLayer;
        }

        public IFeatureLayer GetPointFeatureLayer()
        {
            return m_pointLayer;
        }
    }
}
