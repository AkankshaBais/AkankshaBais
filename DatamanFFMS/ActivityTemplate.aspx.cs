using BusinessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AstralFFMS
{
    public partial class ActivityTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string pageName = Path.GetFileName(Request.Path);
            string Pageheader = Settings.Instance.GetPageHeaderName(pageName);
            lblPageHeader.Text = Pageheader;
        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
         
        }
        protected void btnFind_Click1(object sender, EventArgs e)
        {

        }
        protected void btnSave_Click(object sender,EventArgs e)
        {

        }
        protected void btncancel1_Click(object sender, EventArgs e)
        { 
        
        }
    }
}