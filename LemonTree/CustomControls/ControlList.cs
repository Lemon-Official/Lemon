using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LemonTree.CustomControls
{
    public partial class ControlList : UserControl
    {
        List<Control> controls = new List<Control>();
        List<Point> offsets = new List<Point>();
        public int YOffset = 0;

        public ControlList()
        {
            InitializeComponent();
        }

        public void Add(Control ctrl, Point offset)
        {
            controls.Add(ctrl);
            offsets.Add(offset);
            RefreshList();
        }

        public void Remove(Control ctrl)
        {
            controls.Remove(ctrl);
            RefreshList();
        }

        public void Clear()
        {
            controls.Clear();
            RefreshList();
        }

        public void RefreshList()
        {
            Controls.Clear();
            int y = 0;
            foreach (Control ctrl in controls)
            {
                ctrl.Location = addPoints(new Point(0, y), offsets[controls.IndexOf(ctrl)]);
                Controls.Add(ctrl);
                y += ctrl.Height + YOffset;
            }
        }

        Point addPoints(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        private void ControlList_Load(object sender, EventArgs e)
        {

        }
    }
}
