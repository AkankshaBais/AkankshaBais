using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLayer;
using System.Data;
using DAL;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

namespace AstralFFMS
{
    public partial class RptDailyWorkingApprovalL2 : System.Web.UI.Page
    {
        int smID = 0;
        int msg = 0;
        string pageName = string.Empty;
        string userIdQryStr = string.Empty;
        string roleType = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            //12/12/15
            //frmTextBox.Attributes.Add("readonly", "readonly");
            //toTextBox.Attributes.Add("readonly", "readonly");
            string pageName = Path.GetFileName(Request.Path);
            string Pageheader = Settings.Instance.GetPageHeaderName(pageName);
            lblPageHeader.Text = Pageheader;

            if (!Page.IsPostBack)
            {
                //              smID = GetSalesPerId(Convert.ToInt32(Settings.Instance.UserID));
                smID = Convert.ToInt32(Settings.Instance.SMID);
                //Ankita - 18/may/2016- (For Optimization)
                // GetRoleType(Settings.Instance.RoleID);
                roleType = Settings.Instance.RoleType;
                // BindSalesPerson();
                if (Request.QueryString["hasval"] != "Y")
                {
                    fill_TreeArea();
                }
                //12/12/15
                //frmTextBox.Text =Settings.GetUTCTime().AddMonths(-1).ToString("dd/MMM/yyyy");// DateTime.Now.ToShortDateString();
                //toTextBox.Text = Settings.GetUTCTime().ToString("dd/MMM/yyyy");// DateTime.Now.ToShortDateString();
                //             if (Request.QueryString["SMId"] != null && Request.QueryString["VisDocId"] != null)
                if (Request.QueryString["SMId"] != null)
                {
                    //   btnBack.Visible = true;
                    ListBox1.SelectedValue = Request.QueryString["SMId"];
                    //12/12/15
                    //frmdate.Style.Add("display", "none");
                    btnGo.Enabled = false;
                    Button2.Enabled = false;
                    btnGo.CssClass = "btn btn-primary";
                    Button2.CssClass = "btn btn-primary";
                    if (Request.QueryString["Page"] != null)
                    {
                        pageName = Request.QueryString["Page"];
                    }
                    //GetDailyWorkingSummaryL1New(Convert.ToInt32(Request.QueryString["SMId"]), Request.QueryString["VisDocId"]);
                    ViewState["smIDStr"] = "";
                    GetDailyWorkingSummaryL1New(Convert.ToInt32(Request.QueryString["SMId"]));
                }
                else
                {
                    btnBack.Visible = false;
                    //12/12/15
                    //frmdate.Style.Add("display", "block");
                    btnGo.Enabled = true;
                    Button2.Enabled = true;
                }
            }
            if (Request.QueryString["hasval"] == "Y")
            {
                if (hid.Value == "Y")
                {
                    rptmain.Style.Add("display", "block");
                    GetDailyWorkingSummaryL1(Request.QueryString["smIDStr"].ToString(), Request.QueryString["userid"].ToString());
                    ShowAlert("Record Updated Suceesfully");
                    fill_TreeArea();
                    hid.Value = "N";
                }

            }
        }

        private void BindSalesPerson()
        {
            try
            {

                if (roleType == "Admin")
                { //Ankita - 18/may/2016- (For Optimization)

                    //string strrole = "select mastrole.RoleName,MastSalesRep.*,mastrole.RoleType from MastSalesRep left join MastRole on MastRole.RoleId=MastSalesRep.RoleId where MastSalesRep.Active=1 and SMName<>'.' order by MastSalesRep.SMName";
                    string strrole = "select mastrole.RoleName,MastSalesRep.SMId,MastSalesRep.SMName,mastrole.RoleType from MastSalesRep left join MastRole on MastRole.RoleId=MastSalesRep.RoleId where MastSalesRep.Active=1 and SMName<>'.' order by MastSalesRep.SMName";
                    DataTable dtcheckrole = new DataTable();
                    dtcheckrole = DbConnectionDAL.GetDataTable(CommandType.Text, strrole);
                    DataView dv1 = new DataView(dtcheckrole);
                    //dv1.RowFilter = "RoleType='CityHead' or RoleType='DistrictHead' and SMName<>'.'";
                    dv1.RowFilter = "SMName<>'.'";
                    dv1.Sort = "SMName asc";

                    ListBox1.DataSource = dv1.ToTable();
                    ListBox1.DataTextField = "SMName";
                    ListBox1.DataValueField = "SMId";
                    ListBox1.DataBind();
                }
                else
                {
                    //DataTable dt = Settings.UnderUsers(Settings.Instance.SMID);
                    DataTable dt = Settings.BindUnderUsersApproval(Settings.Instance.SMID);
                    DataView dv = new DataView(dt);
                    //dv.RowFilter = "RoleType='CityHead' or RoleType='DistrictHead' and SMName<>'.'";
                    dv.RowFilter = "SMName<>'.'";
                    dv.Sort = "SMName asc";

                    ListBox1.DataSource = dv.ToTable();
                    ListBox1.DataTextField = "SMName";
                    ListBox1.DataValueField = "SMId";
                    ListBox1.DataBind();
                }
            }
            catch (Exception ex)
            {

            }
        }
        void fill_TreeArea()
        {
            int lowestlvl = 0;
            DataTable St = new DataTable();
            if (roleType == "Admin")
            {

                //string strrole = "select SMID,SMName from MastSalesRep left join MastRole on MastRole.RoleId=MastSalesRep.RoleId where MastSalesRep.Active=1 and SMName<>'.' order by MastSalesRep.SMName";
                //St = DbConnectionDAL.GetDataTable(CommandType.Text, strrole);
                //    lowestlvl = Settings.UnderUsersforlowest(Settings.Instance.SMID);
                //St = Settings.UnderUsersforlowerlevels(Settings.Instance.SMID,1);
                St = Settings.UnderUsersforlevels(Settings.Instance.SMID, 1);
            }
            else
            {
                //Ankita - 18/may/2016- (For Optimization)
                //lowestlvl = Settings.UnderUsersforlowest(Settings.Instance.SMID);
                //St = Settings.UnderUsersforlowerlevels(Settings.Instance.SMID, lowestlvl);
                string qry = "SELECT mastrole.rolename,mastsalesrep.smid,smname + ' (' + mastsalesrep.syncid + ' - '+ mastrole.rolename + ')' AS smname, mastsalesrep.lvl,mastrole.roletype FROM   mastsalesrep LEFT JOIN mastrole ON mastrole.roleid = mastsalesrep.roleid WHERE  smid =" + Settings.Instance.SMID+"";
                //St = DbConnectionDAL.GetDataTable(CommandType.Text, "SELECT mastrole.rolename,mastsalesrep.smid,smname + ' (' + mastsalesrep.syncid + ' - '+ mastrole.rolename + ')' AS smname, mastsalesrep.lvl,mastrole.roletype FROM   mastsalesrep LEFT JOIN mastrole ON mastrole.roleid = mastsalesrep.roleid WHERE  smid =" + Settings.Instance.SMID + "");
                St = DbConnectionDAL.GetDataTable(CommandType.Text, qry);
            }


            trview.Nodes.Clear();

            if (St.Rows.Count <= 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "errormessage", "errormessage('No Record Found !');", true);
                return;
            }
            foreach (DataRow row in St.Rows)
            {
                TreeNode tnParent = new TreeNode();
                tnParent.Text = row["SMName"].ToString();
                tnParent.Value = row["SMId"].ToString();
                trview.Nodes.Add(tnParent);
                //tnParent.ExpandAll();
                tnParent.CollapseAll();
                //Ankita - 18/may/2016- (For Optimization)s
                // FillChildArea(tnParent, tnParent.Value, (Convert.ToInt32(row["Lvl"])), Convert.ToInt32(row["SMId"].ToString()));
                getchilddata(tnParent, tnParent.Value);
            }
        }
        //Ankita - 18/may/2016- (For Optimization)
        private void getchilddata(TreeNode parent, string ParentId)
        {

            string SmidVar = string.Empty;
            string GetFirstChildData = string.Empty;
            int levelcnt = 0;
            if (Settings.Instance.RoleType == "Admin")
                levelcnt = Convert.ToInt16(Settings.Instance.SalesPersonLevel) + 2;
            else
                levelcnt = Convert.ToInt16(Settings.Instance.SalesPersonLevel) + 1;


            GetFirstChildData = "select msrg.smid,Smname +' ('+ msr.Syncid + ' - ' + mr.RoleName + ')' as smname from mastsalesrepgrp msrg left join mastsalesrep msr on msrg.smid=msr.smid  LEFT JOIN mastrole mr ON mr.RoleId=msr.RoleId where msrg.maingrp =" + ParentId + " and msr.lvl=" + (levelcnt) + " and msrg.smid <> " + ParentId + " and msr.Active=1 order by SMName,lvl desc ";
            DataTable FirstChildDataDt = DbConnectionDAL.GetDataTable(CommandType.Text, GetFirstChildData);

            if (FirstChildDataDt.Rows.Count > 0)
            {

                for (int i = 0; i < FirstChildDataDt.Rows.Count; i++)
                {
                    SmidVar += FirstChildDataDt.Rows[i]["smid"].ToString() + ",";
                    FillChildArea(parent, ParentId, FirstChildDataDt.Rows[i]["smid"].ToString(), FirstChildDataDt.Rows[i]["smname"].ToString());
                }
                SmidVar = SmidVar.Substring(0, SmidVar.Length - 1);

                for (int j = levelcnt + 1; j <= 6; j++)
                {
                    string AreaQueryChild = "select msrg.smid,msr.lvl,maingrp,Smname +' ('+ msr.Syncid + ' - ' + mr.RoleName + ')' as smname from mastsalesrepgrp msrg left join mastsalesrep msr on msrg.smid=msr.smid  LEFT JOIN mastrole mr ON mr.RoleId=msr.RoleId where msrg.maingrp  in (" + SmidVar + ") and msr.lvl=" + j + "  and msr.Active=1 and mr.Roletype in ('CityHead','DistrictHead','StateHead','RegionHead') order by SMName,lvl desc ";
                    DataTable dtChild = DbConnectionDAL.GetDataTable(CommandType.Text, AreaQueryChild);
                    //SmidVar = string.Empty;
                    int mTotRows = dtChild.Rows.Count;
                    if (mTotRows > 0)
                    {
                        SmidVar = string.Empty;
                        var str = "";
                        for (int k = 0; k < mTotRows; k++)
                        {
                            SmidVar += dtChild.Rows[k]["smid"].ToString() + ",";
                        }

                        TreeNode Oparent = parent;
                        switch (j)
                        {
                            case 3:
                                if (Oparent.Parent != null)
                                {
                                    foreach (TreeNode child in Oparent.ChildNodes)
                                    {
                                        str += child.Value + ","; parent = child;
                                        DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                        for (int l = 0; l < dr.Length; l++)
                                        {
                                            FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                        }
                                        dtChild.Select();
                                    }
                                }
                                else
                                {
                                    foreach (TreeNode child in Oparent.ChildNodes)
                                    {
                                        str += child.Value + ","; parent = child;
                                        DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                        for (int l = 0; l < dr.Length; l++)
                                        {
                                            FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                        }
                                        dtChild.Select();
                                    }
                                }
                                break;
                            case 4:
                                if (Oparent.Parent != null)
                                {
                                    foreach (TreeNode child in Oparent.ChildNodes)
                                    {
                                        str += child.Value + ","; parent = child;
                                        DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                        for (int l = 0; l < dr.Length; l++)
                                        {
                                            FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                        }
                                        dtChild.Select();
                                    }
                                }
                                else
                                {
                                    foreach (TreeNode child in Oparent.ChildNodes)
                                    {
                                        str += child.Value + ","; parent = child;
                                        DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                        for (int l = 0; l < dr.Length; l++)
                                        {
                                            FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                        }
                                        dtChild.Select();
                                    }
                                }
                                break;
                            case 5:
                                if (Oparent.Parent != null)
                                {
                                    foreach (TreeNode Pchild in Oparent.Parent.ChildNodes)
                                    {
                                        foreach (TreeNode child in Pchild.ChildNodes)
                                        {
                                            str += child.Value + ","; parent = child;
                                            DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                            for (int l = 0; l < dr.Length; l++)
                                            {
                                                FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                            }
                                            dtChild.Select();
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (TreeNode child in Oparent.ChildNodes)
                                    {
                                        str += child.Value + ","; parent = child;
                                        DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                        for (int l = 0; l < dr.Length; l++)
                                        {
                                            FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                        }
                                        dtChild.Select();
                                    }
                                }


                                break;
                            case 6:
                                if (Oparent.Parent != null)
                                {
                                    if (Settings.Instance.RoleType == "Admin")
                                    {
                                        foreach (TreeNode Pchild in Oparent.Parent.Parent.ChildNodes)
                                        {
                                            foreach (TreeNode Qchild in Pchild.ChildNodes)
                                            {
                                                foreach (TreeNode child in Qchild.ChildNodes)
                                                {
                                                    str += child.Value + ","; parent = child;
                                                    DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                                    for (int l = 0; l < dr.Length; l++)
                                                    {
                                                        FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                                    }
                                                    dtChild.Select();
                                                }
                                            }
                                        }
                                    }

                                    else
                                    {
                                        foreach (TreeNode Pchild in Oparent.Parent.ChildNodes)
                                        {
                                            foreach (TreeNode child in Pchild.ChildNodes)
                                            {
                                                str += child.Value + ","; parent = child;
                                                DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                                for (int l = 0; l < dr.Length; l++)
                                                {
                                                    FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                                }
                                                dtChild.Select();
                                            }
                                        }
                                    }

                                }

                                else
                                {
                                    foreach (TreeNode child in Oparent.ChildNodes)
                                    {
                                        str += child.Value + ","; parent = child;
                                        DataRow[] dr = dtChild.Select("maingrp =" + child.Value);
                                        for (int l = 0; l < dr.Length; l++)
                                        {
                                            FillChildArea(child, child.Value, dr[l]["smid"].ToString(), dr[l]["smname"].ToString());
                                        }
                                        dtChild.Select();
                                    }
                                }


                                break;
                        }

                        SmidVar = SmidVar.Substring(0, SmidVar.Length - 1);
                    }
                }
            }
        }

        //public void FillChildArea(TreeNode parent, string ParentId, string Smid, string SMName)
        //{
        //    TreeNode child = new TreeNode();
        //    child.Text = SMName;
        //    child.Value = Smid;
        //    child.SelectAction = TreeNodeSelectAction.Expand;
        //    parent.ChildNodes.Add(child);
        //    child.CollapseAll();
        //}

        public void FillChildArea(TreeNode parent, string ParentId, string Smid, string SMName)
        {
            TreeNode child = new TreeNode();
            child.Text = SMName;
            child.Value = Smid;
            child.SelectAction = TreeNodeSelectAction.Expand;
            parent.ChildNodes.Add(child);
            child.CollapseAll();
            if (ViewState["smIDStr"] != null)
            {
                string[] SplitSmid = ViewState["smIDStr"].ToString().Split(',');
                if (SplitSmid.Length > 0)
                {
                    for (int i = 0; i < SplitSmid.Length; i++)
                    {
                        if (Smid == SplitSmid[i])
                        {
                            child.Checked = true;
                        }
                    }
                }
            }

        }
        //public void FillChildArea(TreeNode parent, string ParentId, int LVL, int SMId)
        //{
        //    //var AreaQueryChild = "select * from Mastsalesrep where lvl=" + (LVL + 1) + " and UnderId=" + SMId + " order by SMName,lvl";
        //    var AreaQueryChild = "SELECT SMId,Smname +' ('+ ms.Syncid + ' - ' + mr.RoleName + ')' as smname,Lvl from Mastsalesrep ms LEFT JOIN mastrole mr ON mr.RoleId=ms.RoleId where lvl=" + (LVL + 1) + " and UnderId=" + SMId + " and ms.Active=1 AND mr.RoleType IN ('RegionHead','StateHead','CityHead','DistrictHead') order by SMName,lvl";
        //    DataTable dtChild = DbConnectionDAL.GetDataTable(CommandType.Text, AreaQueryChild);
        //    parent.ChildNodes.Clear();
        //    foreach (DataRow dr in dtChild.Rows)
        //    {
        //        TreeNode child = new TreeNode();
        //        child.Text = dr["SMName"].ToString().Trim();
        //        child.Value = dr["SMId"].ToString().Trim();
        //        child.SelectAction = TreeNodeSelectAction.Expand;
        //        parent.ChildNodes.Add(child);
        //        //child.ExpandAll();
        //        child.CollapseAll();
        //        FillChildArea(child, child.Value, (Convert.ToInt32(dr["Lvl"])), Convert.ToInt32(dr["SMId"].ToString()));
        //    }

        //}
        //private void GetRoleType(string p)
        //{
        //    try
        //    {
        //        string roleqry = @"select * from MastRole where RoleId=" + Convert.ToInt32(p) + "";
        //        DataTable roledt = DbConnectionDAL.GetDataTable(CommandType.Text, roleqry);
        //        if (roledt.Rows.Count > 0)
        //        {
        //            roleType = roledt.Rows[0]["RoleType"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }
        //}

        private void GetDailyWorkingSummaryL1(string SPID, string userIDWorkSum)
        {
            try
            {
                string query = "";
                string QryChk = "";
                QryChk = "where a.lock1 =1 and a.Lock2=1";

                query = @"select a.SMID,convert(varchar,a.VDate,106) as [VDate],(a.Level1) as Level1,(a.SyncId) as SyncId,sum(a.TotalOrder) as TotalOrder,
                sum(a.OrderAmountMail) as OrderAmountMail,sum(a.OrderAmountPhone) as OrderAmountPhone,sum(a.DistributorCollection) as DistributorCollection,
                sum(a.PartyCollection) as PartyCollection,iif(max(a.CallsVisited) <>0,isnull(sum(a.TotalOrder)/sum(a.CallsVisited),''),0) as PerCallAvgCell,sum(a.CallsVisited) as [CallsVisited],
               sum(a.RetailerProCalls) as RetailerProCalls,SUM(a.FailedVisit) as [FailedVisit],SUM(a.DistFailVisit) as [DistFailVisit],
               sum(a.DistDiscuss) as DistDiscuss,sum(a.NewParties) as NewParties,SUM(a.LocalExpenses) as [LocalExpenses],sum(a.TourExpenses) as [TourExpenses],SUM(a.Demo) [Demo],
               sum(a.Competitor) [Competitor],0 as [Collections],MAX(a.TotalParty) as TotalParty,max(a.Remarks) as Remarks,
               max(a.AppRemark) as AppRemark11, AType= (case when max(a.AType) is null then 'Pending' else '' end),(case when (a.AppRemark IS NULL OR a.AppRemark='') then a.AppRemark else a.AppRemark end) as AppRemark,
               a.EmpName,  max(a.type) AS type,max(a.RoleType) as RoleType  from ( " +
               "SELECT View_DSR.SMID,View_DSR.VDate,View_DSR.Level1 AS Level1,View_DSR.SyncId,View_DSR.TotalOrder as TotalOrder,View_DSR.OrderAmountMail as OrderAmountMail, View_DSR.OrderAmountPhone as OrderAmountPhone,View_DSR.DistributorCollection as DistributorCollection,View_DSR.PartyCollection AS PartyCollection,View_DSR.PerCallAvgCell as [PerCallAvgCell],View_DSR.CallsVisited as [CallsVisited],View_DSR.RetailerProCalls as [RetailerProCalls],View_DSR.FailedVisit as [FailedVisit],View_DSR.DistFailVisit as DistFailVisit,View_DSR.DistDiscuss as DistDiscuss,View_DSR.NewParties as NewParties,View_DSR.LocalExpenses as [LocalExpenses], View_DSR.TourExpenses as [TourExpenses],View_DSR.Demo as [Demo],View_DSR.Competitor AS Competitor,View_DSR.TotalParty as TotalParty,View_DSR.Remarks as Remarks,View_DSR.AppRemark as AppRemark,View_DSR.AType as AType,View_DSR.EmpName as EmpName,View_DSR.Type AS Type, View_DSR.Lock1,View_DSR.Lock2,View_DSR.RoleType FROM View_DSR WHERE View_DSR.SMID in (SELECT ms.smid from mastsalesrep ms LEFT JOIN MastRole mr ON ms.RoleId= mr.RoleId WHERE ms.SMId IN (SELECT smid FROM mastsalesrepgrp WHERE  maingrp IN (" + SPID + ")) AND mr.RoleType IN ('CityHead','DistrictHead')) and View_DSR.Atype is null) a " + QryChk + " Group by a.VDate,a.SMID,a.Level1,a.SyncId,a.AppRemark,a.lock1,a.smid,a.EmpName Order by a.VDate"; 


                DataTable gdt = DbConnectionDAL.GetDataTable(CommandType.Text, query);
                if (gdt.Rows.Count > 0)
                {
                    ViewState["GridData"] = gdt;
                    ViewState["smIDStr"] = SPID;
                    ViewState["UserId"] = userIDWorkSum;
                    rpt.DataSource = gdt;
                    rpt.DataBind();
                }
                else
                {
                    rpt.DataSource = null;
                    rpt.DataBind();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

        private void GetDailyWorkingSummaryL1New(int SPID)
        {
            try
            {
                string query = "";
                string QryChk = "";
                QryChk = "where a.lock1 =1 and a.Lock2=1";              
        
                query = @"select a.SMID,convert(varchar,a.VDate,106) as [VDate],(a.Level1) as Level1,(a.SyncId) as SyncId,sum(a.TotalOrder) as TotalOrder,
                sum(a.OrderAmountMail) as OrderAmountMail,sum(a.OrderAmountPhone) as OrderAmountPhone,sum(a.DistributorCollection) as DistributorCollection,
                sum(a.PartyCollection) as PartyCollection,iif(max(a.CallsVisited) <>0,isnull(sum(a.TotalOrder)/sum(a.CallsVisited),''),0) as PerCallAvgCell,sum(a.CallsVisited) as [CallsVisited],
               sum(a.RetailerProCalls) as RetailerProCalls,SUM(a.FailedVisit) as [FailedVisit],SUM(a.DistFailVisit) as [DistFailVisit],
               sum(a.DistDiscuss) as DistDiscuss,sum(a.NewParties) as NewParties,SUM(a.LocalExpenses) as [LocalExpenses],sum(a.TourExpenses) as [TourExpenses],SUM(a.Demo) [Demo],
               sum(a.Competitor) [Competitor],0 as [Collections],MAX(a.TotalParty) as TotalParty,max(a.Remarks) as Remarks,
               max(a.AppRemark) as AppRemark11, AType= (case when max(a.AType) is null then 'Pending' else '' end),(case when (a.AppRemark IS NULL OR a.AppRemark='') then a.AppRemark else a.AppRemark end) as AppRemark,
               a.EmpName,  max(a.type) AS type,max(a.RoleType) as RoleType  from ( " +
                "SELECT View_DSR.SMID,View_DSR.VDate,View_DSR.Level1 AS Level1,View_DSR.SyncId,View_DSR.TotalOrder as TotalOrder,View_DSR.OrderAmountMail as OrderAmountMail, View_DSR.OrderAmountPhone as OrderAmountPhone,View_DSR.DistributorCollection as DistributorCollection,View_DSR.PartyCollection AS PartyCollection,View_DSR.PerCallAvgCell as [PerCallAvgCell],View_DSR.CallsVisited as [CallsVisited],View_DSR.RetailerProCalls as [RetailerProCalls],View_DSR.FailedVisit as [FailedVisit],View_DSR.DistFailVisit as DistFailVisit,View_DSR.DistDiscuss as DistDiscuss,View_DSR.NewParties as NewParties,View_DSR.LocalExpenses as [LocalExpenses], View_DSR.TourExpenses as [TourExpenses],View_DSR.Demo as [Demo],View_DSR.Competitor AS Competitor,View_DSR.TotalParty as TotalParty,View_DSR.Remarks as Remarks,View_DSR.AppRemark as AppRemark,View_DSR.AType as AType,View_DSR.EmpName as EmpName,View_DSR.Type AS Type, View_DSR.Lock1,View_DSR.Lock2,View_DSR.RoleType FROM View_DSR WHERE View_DSR.SMID in (SELECT ms.smid from mastsalesrep ms LEFT JOIN MastRole mr ON ms.RoleId= mr.RoleId WHERE ms.SMId IN (SELECT smid FROM mastsalesrepgrp WHERE  maingrp IN (" + SPID + ")) AND mr.RoleType IN ('CityHead','DistrictHead')) and View_DSR.Atype is null) a " + QryChk + " Group by a.VDate,a.SMID,a.Level1,a.SyncId,a.AppRemark,a.lock1,a.smid,a.EmpName Order by a.VDate"; 

                DataTable gdt = DbConnectionDAL.GetDataTable(CommandType.Text, query);
                if (gdt.Rows.Count > 0)
                {
                    rptmain.Style.Add("display", "block");
                    ViewState["smIDStr"] = SPID;
                    ViewState["GridData"] = gdt;
                    rpt.DataSource = gdt;
                    rpt.DataBind();
                }
                else
                {
                    rpt.DataSource = null;
                    rpt.DataBind();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            rptmain.Style.Add("display", "block");
            string smIDStr = "";
            string smIDStr1 = "", userIdStr = "";

            foreach (ListItem item in ListBox1.Items)
            {
                if (item.Selected)
                {
                    smIDStr1 += item.Value + ",";
                }
            }
            smIDStr1 = smIDStr1.TrimStart(',').TrimEnd(',');
            foreach (TreeNode node in trview.CheckedNodes)
            {
                smIDStr1 = node.Value;
                {
                    smIDStr += node.Value + ",";
                }
            }
            smIDStr1 = smIDStr.TrimStart(',').TrimEnd(',');

            if (smIDStr1 == "")
            {
                DataTable dtSMId = Settings.BindUnderUsersApproval(Settings.Instance.SMID);
                DataView dv = new DataView(dtSMId);
                dv.RowFilter = "RoleType='CityHead' or RoleType='DistrictHead' and SMName<>'.'";

                if (dv.ToTable().Rows.Count > 0)
                {
                    foreach (DataRow dr in dv.ToTable().Rows)
                    {
                        smIDStr = smIDStr + "," + Convert.ToString(dr["SMId"]);

                    }
                    smIDStr1 = smIDStr.TrimStart(',').TrimEnd(',');
                }
            }

            if (smIDStr1 != "")
            {
                //string salesRepUserIdQry = @"select UserId from MastSalesRep where SMId in (" + smIDStr1 + ")";
                string salesRepUserIdQry = @"select UserId from MastSalesRep where SMId in (SELECT ms.smid from mastsalesrep ms LEFT JOIN MastRole mr ON ms.RoleId= mr.RoleId WHERE ms.SMId IN (SELECT smid FROM mastsalesrepgrp WHERE  maingrp IN (" + smIDStr1 + ")) AND mr.RoleType IN ('CityHead','DistrictHead'))";

                DataTable userdt = DbConnectionDAL.GetDataTable(CommandType.Text, salesRepUserIdQry);
                if (userdt.Rows.Count > 0)
                {
                    for (int i = 0; i < userdt.Rows.Count; i++)
                    {
                        userIdStr += userdt.Rows[i]["UserId"] + ",";
                    }
                }
                userIdStr = userIdStr.TrimStart(',').TrimEnd(',');
                //

                ViewState["smIDStr"] = smIDStr1;
                GetDailyWorkingSummaryL1(smIDStr1, userIdStr);
            }
            else
            {
                rptmain.Style.Add("display", "none");
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Errormessage", "alert('Please select salesperson');", true);
            }

        }

        //protected void Save_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (TextArea1.Value != "")
        //        {
        //            string transVisitQry = @"update TransVisit set AppBy=" + Settings.Instance.UserID + ",AppBySMId=" + smID + ", AppRemark='" + TextArea1.Value + "',AppStatus='" + approveStatusRadioButtonList.SelectedValue + "' where SMId=" + Convert.ToInt32(visHdf.Value) + " and VDate='" + Settings.dateformat(dateHdf.Value) + "'";
        //            DbConnectionDAL.ExecuteNonQuery(CommandType.Text, transVisitQry);
        //            if (ViewState["smIDStr"].ToString() != "")
        //            {
        //                //12/12/15
        //                // GetDailyWorkingSummaryL1(State["smIDStr"].ToString(), frmTextBox.Text, toTextBox.Text, ViewState["userId"].ToString());
        //                GetDailyWorkingSummaryL1(ViewState["smIDStr"].ToString(), ViewState["UserId"].ToString());
        //            }
        //            else if (Request.QueryString["SMId"] != null)
        //            {
        //                GetDailyWorkingSummaryL1New(Convert.ToInt32(Request.QueryString["SMId"]));

        //            }
        //            ShowAlert("Record Updated Successfully");
        //        }
        //        else
        //        {
        //            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Errormessage", "alert('Please Enter Remark');", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }

        //}

        [System.Web.Services.WebMethod]
        public static void UpdateDSR(string smid, string Vdate, string status, string Remark)
        {
            //return string.Format("Name: {0}{2}Age: {1}", smid, status, Environment.NewLine);
            try
            {
                string transVisitQry = @"update TransVisit set AppBy=" + Settings.Instance.UserID + ",AppBySMId=" + Settings.Instance.SMID + ", AppRemark='" + Remark + "',AppStatus='" + status + "' where SMId=" + Convert.ToInt32(smid) + " and VDate='" + Settings.dateformat(Vdate) + "'";
                DbConnectionDAL.ExecuteNonQuery(CommandType.Text, transVisitQry);           

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public void ShowAlert(string Message)
        {
            string script = "window.alert(\"" + Message.Normalize() + "\");";
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "UniqueKey", script, true);
        }
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            var item = (RepeaterItem)btn.NamingContainer;
            HiddenField hdnVisItCode = (HiddenField)item.FindControl("HiddenField1");
            //    this.ModalPopupExtender4.Show();
        }

        //protected void Close_Click(object sender, EventArgs e)
        //{

        //}

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RptDailyWorkingApprovalL2.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PendingDSRList.aspx?SMId=" + Request.QueryString["SMId"]);
        }

        string visitDate = "";


        protected void rpt_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HiddenField hdnType = (HiddenField)e.Item.FindControl("hdnType");
                Label lblType = (Label)e.Item.FindControl("lblType");
                HiddenField hdnAType = (HiddenField)e.Item.FindControl("hdnAType");
                if (hdnType.Value == "9")
                {
                    lblType.Text = "DSR";
                }
                else if (hdnType.Value == "8")
                {
                    lblType.Text = "LEAVE";
                }
                else if (hdnType.Value == "7")
                {
                    lblType.Text = "EXPENSE";
                }
                else
                {
                    lblType.Text = "HOLIDAY";
                }

                if (hdnAType.Value == "Pending" || hdnAType.Value == "")
                {
                    e.Item.Visible = true;
                }
                else
                {
                    e.Item.Visible = false;
                }

                LinkButton lnkDSRPreview = (LinkButton)e.Item.FindControl("lnkEdit");
                if (hdnAType.Value == "Pending" || hdnAType.Value == "")
                {
                    lnkDSRPreview.Text = "Approve/Reject";

                }
                else
                {
                    lnkDSRPreview.Text = "";

                }

            }
        }
        protected void rpt_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                string[] commandArgs = new string[2];
                commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                visHdf.Value = commandArgs[0];
                dateHdf.Value = commandArgs[1];
                AjaxControlToolkit.ModalPopupExtender mp1 = ((AjaxControlToolkit.ModalPopupExtender)(e.Item.FindControl("ModalPopupExtender4")));
                TextArea1.Value = string.Empty;
                mp1.Show();
            }
            if (e.CommandName == "selectDate")
            {
                RepeaterItem gvRow = (RepeaterItem)(((LinkButton)e.CommandSource).NamingContainer);
                int index = gvRow.ItemIndex;
                RepeaterItem row = rpt.Items[index];
                HiddenField hdnDate = (HiddenField)row.FindControl("hdnDate");
                HiddenField hdnSmiD = (HiddenField)row.FindControl("hdnSMId");
                Response.Redirect("DSRReportL2.aspx?SMID=" + hdnSmiD.Value + "&Date=" + hdnDate.Value + "&PAGE=APPROVAL-L2");
            }

        }

    }
}