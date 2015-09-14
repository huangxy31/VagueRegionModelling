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
using VagueRegionModelling.ClusteringAlgorithm;

namespace VagueRegionModelling.ClusterForms
{
    public partial class ASCDTForm : Form
    {
        private IMapControl3 m_mapControl = null;
        private DataInformation m_dataInfo;
        private Clusters m_Clusters = new Clusters();   //聚类结果
        private bool m_bLocal = false;
        private ASCDT m_ASCDTCluster = null;    //聚类操作

        public ASCDTForm()
        {
            InitializeComponent();
        }

        public ASCDTForm(IMapControl3 mapControl)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            string fileName = "ASCDT_Cluster.shp";
            m_dataInfo = new DataInformation(mapControl, fileName);
            inputAndOutput.SetValues(mapControl, m_dataInfo);            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //有效性验证
            m_dataInfo = inputAndOutput.GetDataInfo();
            m_bLocal = checkBoxLocal.Checked;
            string filePath = m_dataInfo.GetOutputFilePath();
            string fileName = m_dataInfo.GetOutputFileName();

            #region 参数有效性验证
            if (m_dataInfo.IsValid() == false)
                return;
            if (File.Exists(filePath + "\\" + fileName))
            {
                MessageBox.Show(fileName + "文件已存在，请重新输入！");
                return;
            }
            #endregion

            //计算得到聚类结果
            m_ASCDTCluster = new ASCDT(m_dataInfo, m_bLocal);
            m_Clusters = m_ASCDTCluster.GetClusters();
            //保存到shp
            SaveShapefile shapefile = new SaveShapefile(m_dataInfo, m_Clusters);
            shapefile.CreatePolygonShapefile();
            shapefile.CreatePointsShapefile();

            IFeatureLayer pointLayer = shapefile.GetPointFeatureLayer();
            IFeatureLayer polygonLayer = shapefile.GetPolygonFeatureLayer();
            //渲染显示
            RenderLayer layerRenderer = new RenderLayer();
            layerRenderer.DefinePointUniqueValueRenderer(pointLayer as IGeoFeatureLayer, "index");
            layerRenderer.DefinePolygonUniqueValueRenderer(polygonLayer as IGeoFeatureLayer, "index");
            m_mapControl.AddLayer(polygonLayer as ILayer);
            m_mapControl.AddLayer(pointLayer as ILayer);
            this.m_mapControl.Refresh();

            this.Close();
            //需要添加如果一类都没有获得的情况
        }

    }
}
