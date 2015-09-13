using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using VagueRegionModelling.DataOperator;
using System.IO;

namespace VagueRegionModelling.ClusterForms
{
    public partial class DBSCANForm : Form
    {
        private IMapControl3 m_mapControl = null;
        private DataInformation m_dataInfo;
        private double m_dEps = 0;
        private int m_nMinPts = 0;
        public DBSCAN m_DBSCANCluster = null;
        private Clusters m_Clusters = new Clusters();

        public DBSCANForm()
        {
            InitializeComponent();
        }

        public DBSCANForm(IMapControl3 mapControl)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            string fileName = "DBSCAN_Cluster.shp";
            m_dataInfo = new DataInformation(mapControl, fileName);
            inputAndOutput.SetValues(mapControl, m_dataInfo);
          //  inputAndOutput.AddLayerNames();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //有效性验证
            
            m_dataInfo = inputAndOutput.GetDataInfo();
            string filePath = m_dataInfo.GetOutputFilePath();
            string fileName = m_dataInfo.GetOutputFileName();

            #region 参数有效性验证
            if (m_dataInfo.IsValid() == false)
                return;

            if (textBoxEps.Text == "")
            {
                MessageBox.Show("区域半径输入错误！");
                return;
            }
            if (textBoxMinPts.Text == "")
            {
                MessageBox.Show("最小点数输入错误！");
                return;
            }
            if (File.Exists(filePath + "\\" + fileName))
            {
                MessageBox.Show(fileName + "文件已存在，请重新输入！");
                return;
            }
            #endregion

            //参数获取
            m_dEps = m_dataInfo.UnitsConvert(float.Parse(textBoxEps.Text), m_mapControl.MapUnits);
            m_nMinPts = int.Parse(textBoxMinPts.Text);

            //计算得到聚类结果
            DBSCAN();
            //保存到shp
            SaveShapefile shapefile = new SaveShapefile(m_dataInfo, m_Clusters);
            shapefile.CreatePolygonShapefile();
            shapefile.CreatePointsShapefile();
            IFeatureLayer layer1 = shapefile.GetPolygonFeatureLayer();
            IFeatureLayer layer2 = shapefile.GetPointFeatureLayer();
            //渲染显示

            m_mapControl.AddLayer(layer1 as ILayer);
            m_mapControl.AddLayer(layer2 as ILayer);
            this.m_mapControl.Refresh();
        }

        private void DBSCAN()
        {
            m_DBSCANCluster = new DBSCAN(m_dataInfo, m_dEps, m_nMinPts);
            m_Clusters = m_DBSCANCluster.GetClusters();
        }

        private void test(IPoint point)
        {

           IGraphicsContainer gc = m_mapControl.Map as IGraphicsContainer;
           ITopologicalOperator topologicalOperator = point as ITopologicalOperator; ;
           IElement element;
           element = new PolygonElementClass();
           element.Geometry = topologicalOperator.Buffer(m_dEps);
           gc.AddElement(element, 0);           
           m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }


    }
}
