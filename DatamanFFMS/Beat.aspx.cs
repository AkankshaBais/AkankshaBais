using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BAL;
using System.Reflection;
using BusinessLayer;
using System.IO;

namespace AstralFFMS
{
    public partial class Beat : System.Web.UI.Page
    {
        LocationBAL LB = new LocationBAL();
        string parameter = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultButton = btnSave.UniqueID;
            this.Form.DefaultButton = this.btnSave.UniqueID;
            parameter = Request["__EVENTARGUMENT"];
            if (parameter != "")
            {
                ViewState["AreaId"] = parameter;
                FillLocControls(Convert.ToInt32(parameter));
                mainDiv.Style.Add("display", "block");
                rptmain.Style.Add("display", "none");
            }
            //Ankita - 18/may/2016- (For Optimization)
            string pageName = Path.GetFileName(Request.Path);
            string Pageheader = Settings.Instance.GetPageHeaderName(pageName);
            lblPageHeader.Text = Pageheader;

            string PermAll = Settings.Instance.CheckPagePermissions(pageName, Convert.ToString(Session["user_name"]));
            string[] SplitPerm = PermAll.Split(',');
            if (Convert.ToBoolean(SplitPerm[0]) == false)
            {
                Response.Redirect("~/Logout.aspx");
            }
            if (btnSave.Text == "Save")
            {
                btnSave.Enabled = Convert.ToBoolean(SplitPerm[1]);
                //btnSave.Enabled = Settings.Instance.CheckAddPermission(pageName, Convert.ToString(Session["user_name"]));
                btnSave.CssClass = "btn btn-primary";
            }
            else
            {
                btnSave.Enabled = Convert.ToBoolean(SplitPerm[2]);
                //btnSave.Enabled = Settings.Instance.CheckEditPermission(pageName, Convert.ToString(Session["user_name"]));
                btnSave.CssClass = "btn btn-primary";
            }
            btnDelete.Enabled = Convert.ToBoolean(SplitPerm[3]);
            //btnDelete.Enabled = Settings.Instance.CheckDeletePermission(pageName, Convert.ToString(Session["user_name"]));
            btnDelete.CssClass = "btn btn-primary";

            if (!IsPostBack)
            {
                chkIsActive.Checked = true;
                btnDelete.Visible = false;
             
                mainDiv.Style.Add("display", "block");
                if (Request.QueryString["AreaId"] != null)
                {
                    FillLocControls(Convert.ToInt32(Request.QueryString["AreaId"]));
                }
                else { 
                   /// BindParent(0); 
                }
            }
        }
        private void BindParent(int AreaId)
        {
            string strQ = "";
            if (AreaId > 0)
                strQ = "select AreaID,AreaName from mastarea where AreaType='Area' and (Active='1' or Areaid in (" + AreaId + "))  order by AreaName";

            else
                strQ = "select AreaID,AreaName from mastarea where AreaType='Area' and Active='1' order by AreaName";
            fillDDLDirect(ddlParentLoc, strQ, "AreaID", "AreaName", 1);
        }
        public static void fillDDLDirect(DropDownList xddl, string xmQry, string xvalue, string xtext, int sele)
        {
            DataTable xdt = new DataTable();
            xdt = DbConnectionDAL.GetDataTable(CommandType.Text, xmQry);
            xddl.DataSource = xdt;
            xddl.DataTextField = xtext.Trim();
            xddl.DataValueField = xvalue.Trim();
            xddl.DataBind();
            if (sele == 1)
            {
                if (xdt.Rows.Count >= 1)
                    xddl.Items.Insert(0, new ListItem("--Select--", "0"));
                else if (xdt.Rows.Count == 0)
                    xddl.Items.Insert(0, new ListItem("---", "0"));
            }
            else if (sele == 2)
            {
                xddl.Items.Insert(0, new ListItem("--Others--", "0"));
            }
            xdt.Dispose();
        }
        private void fillRepeter()
        {
            //Query Optimized - Abhishek - 28-05-2016
            //string str = @"select ma.*,ma1.DisplayName as DisplayName1,case ma.Active when 1 then 'Yes' else 'No' end as Active1,ma1.AreaName as Parent from MastArea ma inner join MastArea ma1 on ma.UnderId=ma1.AreaID  where ma.AreaType='BEAT'";
            string str = @"select ma.AreaId,ma.AreaName,ma.SyncId,ma.AreaDesc,ma1.DisplayName as DisplayName1,case ma.Active when 1 then 'Yes' else 'No' end as Active1,ma1.AreaName as Parent from MastArea ma inner join MastArea ma1 on ma.UnderId=ma1.AreaID  where ma.AreaType='BEAT'";
            DataTable Locdt = DbConnectionDAL.GetDataTable(CommandType.Text, str);
            rpt.DataSource = Locdt;
            rpt.DataBind();

            Locdt.Dispose();
        }
        private void FillLocControls(int AreaId)
        {
            try
            { //Ankita - 18/may/2016- (For Optimization)
                string MatGrpQry = @"select AreaName,SyncId,UnderId,Active,AreaDesc from MastArea where AreaId=" + AreaId;
                //string MatGrpQry = @"select * from MastArea where AreaId=" + AreaId;
                DataTable LocValueDt = DbConnectionDAL.GetDataTable(CommandType.Text, MatGrpQry);
                if (LocValueDt.Rows.Count > 0)
                {
                    Location.Value = LocValueDt.Rows[0]["AreaName"].ToString();
                    SyncId.Value = LocValueDt.Rows[0]["SyncId"].ToString();
                    Desc.Value = LocValueDt.Rows[0]["AreaDesc"].ToString();
                    //BindParent(Convert.ToInt32(LocValueDt.Rows[0]["UnderId"]));

                    HiddenAreaUnderID.Value = LocValueDt.Rows[0]["UnderId"].ToString();
                    ddlParentLoc.SelectedValue = LocValueDt.Rows[0]["UnderId"].ToString();
                    if (Convert.ToBoolean(LocValueDt.Rows[0]["Active"]) == true)
                    {
                        chkIsActive.Checked = true;
                    }
                    else
                    {
                        chkIsActive.Checked = false;
                    }
                    btnSave.Text = "Update";
                    btnDelete.Visible = true;
                }

                LocValueDt.Dispose();
            }
            catch (Exception ex)
            {
                ex.ToString();
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Error while inserting the records');", true);
            }
        }
        private void InsertLoc()
        {
            string active = "0", AreaID = Request.Form[HiddenAreaUnderID.UniqueID];
            if (chkIsActive.Checked)
                active = "1";

          //  int retval = LB.InsertLocation(Convert.ToInt32(ddlParentLoc.SelectedValue), Location.Value, SyncId.Value, active, Desc.Value, "BEAT", 0, 0, "", "");

            int retval = LB.InsertLocation(Convert.ToInt32(AreaID), Location.Value, SyncId.Value, active, Desc.Value, "BEAT", 0, 0, "", "",0,0,0);
            if (retval == -1)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Duplicate Beat Exists');", true);
                Location.Focus();
            }
            else if (retval == -2)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Duplicate SyncId Exists');", true);
                SyncId.Focus();
            }
            else
            {
                if (SyncId.Value == "")
                {
                    string syncid = "update MastArea set SyncId='" + retval + "' where areaid=" + retval + "";
                    DbConnectionDAL.ExecuteNonQuery(CommandType.Text, syncid);
                }
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Successmessage", "Successmessage('Record Inserted Successfully');", true);
                ClearControls();
                Location.Focus();
            }
        }
        private void UpdateLoc()
        {
            string active = "0", AreaID = Request.Form[HiddenAreaUnderID.UniqueID];
            if (chkIsActive.Checked)
                active = "1";

          ///  int retval = LB.UpdateLocation(Convert.ToInt32(ViewState["AreaId"]), Convert.ToInt32(ddlParentLoc.SelectedValue), Location.Value, SyncId.Value, active, Desc.Value, "BEAT", 0, 0, "", "");

            int retval = LB.UpdateLocation(Convert.ToInt32(ViewState["AreaId"]), Convert.ToInt32(AreaID), Location.Value, SyncId.Value, active, Desc.Value, "BEAT", 0, 0, "", "",0,0,0);

            if (retval == -1)
            {               
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Duplicate Beat Exists');", true);
                Location.Focus();
            }
            else if (retval == -2)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Duplicate SyncId Exists');", true);
                SyncId.Focus();
            }
            else
            {
                if (active == "0")
                {
                    string query = "insert into DeleteLog (EntityName,EntityKey,DeletedDate) values('MastBeat','" + ViewState["AreaId"] + "',GETDATE())";
                    DbConnectionDAL.ExecuteQuery(query);
                }
                if (SyncId.Value == "")
                {
                    string syncid = "update MastArea set SyncId='" + retval + "' where areaid=" + Convert.ToInt32(ViewState["AreaId"]) + "";
                    DbConnectionDAL.ExecuteNonQuery(CommandType.Text, syncid);
                }
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Successmessage", "Successmessage('Record Updated Successfully');", true);
                ClearControls();
                Location.Focus();
                btnDelete.Visible = false;
                btnSave.Text = "Save";
            }
        }
        private void ClearControls()
        {
            Location.Value = "";
            Desc.Value = "";
            SyncId.Value = "";
            chkIsActive.Checked = true;
            ddlParentLoc.SelectedIndex = 0;
            HiddenAreaUnderID.Value = string.Empty;
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
           // BindParent(0);
            mainDiv.Style.Add("display", "block");
            rptmain.Style.Add("display", "none");
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                int retdel = LB.delete(Convert.ToString(ViewState["AreaId"]));
                if (retdel == 1)
                {
                    string query = "insert into DeleteLog (EntityName,EntityKey,DeletedDate) values('MastBeat','" + ViewState["AreaId"] + "',GETDATE())";
                    DbConnectionDAL.ExecuteQuery(query);
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Successmessage", "Successmessage('Record Deleted Successfully');", true);
                    ClearControls();
                    btnDelete.Visible = false;
                    btnSave.Text = "Save";
                    Location.Focus();
                }
                else
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Record cannot be deleted as it is in use');", true);
                    ClearControls();
                    Location.Focus();
                }
              
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearControls();
            Response.Redirect("~/Beat.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Text == "Update")
                {
                    UpdateLoc();
                }
                else
                {
                    InsertLoc();
                }
               
            }
            catch (Exception ex)
            {
                ex.ToString();
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('Error while inserting the records');", true);
            }
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            fillRepeter();
            ClearControls();
            btnDelete.Visible = false;
            Desc.Value = "";
            btnSave.Text = "Save";
            mainDiv.Style.Add("display", "none");
            rptmain.Style.Add("display", "block");
        }
    }
}