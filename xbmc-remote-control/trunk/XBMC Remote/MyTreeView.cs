using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XBMC_Remote
{
    class MyTreeView : TreeView
    {
        //Overrides default behaviour of treeview so nodes don't expand/collapse on double click

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 515)
            {
            }
            else
                base.DefWndProc(ref m);
        }
    }
}
