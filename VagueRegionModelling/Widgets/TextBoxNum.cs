using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VagueRegionModelling.Widgets
{
    public partial class TextBoxNum : UserControl
    {
        public TextBoxNum()
        {
            InitializeComponent();
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 重写空间的Text属性
        /// </summary>
        public override string Text
        {
            get
            {
                return TextBox.Text;
            }
            set
            {
                TextBox.Text = value;
                base.Text = value;
            }
        }
    }
}
