using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxTreeView;
using System.Web;
using System.Diagnostics;
using System.Web.UI.HtmlControls;

public partial class Groups : System.Web.UI.Page
{
    public SortDirection dir
    {
        get
        {
            if (ViewState["dirState"] == null)
            {
                ViewState["dirState"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["dirState"];
        }
        set
        {
            ViewState["dirState"] = value;
        }
    }
    private DataTable dtData
    {
        get
        {
            return ((DataTable)Session["_dtSelectedData"]);
        }
        set
        {
            if (value == null)
            {
                Session.Remove("_dtSelectedData");
            }
            else
            {
                Session["_dtSelectedData"] = value;
            }
        }
    }
    public List<UserPermissions> UserPermissions
    {
        get
        {
            if (Session["UserPermissions"] != null && Session["UserPermissions"].ToString() != string.Empty)
                return global::UserPermissions.DeSerializePermissionsList(Session["UserPermissions"].ToString());
            return new List<UserPermissions>();
        }
        set { Session["UserPermissions"] = global::UserPermissions.SerializePermissionsList(value); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["User"] != null)
            {
                if (UserPermissions.Any(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Groups) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                {
                    var per = UserPermissions.FirstOrDefault(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Groups) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true)));
                    ((HtmlGenericControl)Page.Master.FindControl("ulBreadcrumb")).InnerHtml = "<li><i class='ace-icon fa fa-home home-icon'></i><a href ='Dashboard.aspx'> الرئيسية </a></li><li>" + per.ModuleName + "</li><li>" + per.PageName + "</li><li></li>";
                    Page.Title = per.PageName;
                }
                else
                {
                    Response.Redirect("NoPermission.aspx");
                }
                tvPermissions.Nodes.Clear();
                    DataTable table = GetDataTable();
                    CreateTreeViewNodesRecursive(table, tvPermissions.Nodes, "0");
                    BindGroups();
            }
            else
                Response.Redirect("Login.aspx?ReturnURL=" + Request.Url.AbsolutePath);
        }
    }
    private DataTable GetDataTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(String));
        dt.Columns.Add("Name", typeof(String));
        dt.Columns.Add("ParentID", typeof(String));
        dt.Columns.Add("Checked", typeof(Boolean));

        dt.PrimaryKey = new[] { dt.Columns["ID"] };
        int groupId = ViewState["ID"] != null ? int.Parse(ViewState["ID"].ToString()) : 0;
        using (CultureDataContext db = new CultureDataContext())
        {
            var modules = (from m in db.SystemModules
                           select new
                           {
                               ID = "m" + m.SystemModule_ID.ToString(),
                               Name = m.SystemModule_Name,
                               ParentID = "0",
                               Checked = db.GroupPrivileges.Count(x => x.SystemPageFunction.SystemModulePage.SystemModulePage_ModuleID.Equals(m.SystemModule_ID) && x.GroupPrivilege_GroupID.Equals(groupId)) == db.SystemPageFunctions.Where(x => x.SystemModulePage.SystemModulePage_ModuleID.Equals(m.SystemModule_ID)).Count() && db.SystemPageFunctions.Where(x => x.SystemModulePage.SystemModulePage_ModuleID.Equals(m.SystemModule_ID)).Count() > 0
                           });
            var pages = (from m in db.SystemModulePages
                         select new
                         {
                             ID = "p" + m.SystemModulePage_ID.ToString(),
                             Name = m.SystemModulePage_PageName,
                             ParentID = "m" + (m.SystemModulePage_ModuleID ?? 0).ToString(),
                             Checked = db.GroupPrivileges.Count(x => x.SystemPageFunction.SystemPageFunction_PageID.Equals(m.SystemModulePage_ID) && x.GroupPrivilege_GroupID.Equals(groupId)) == m.SystemPageFunctions.Count
                         });
            var functions = (from m in db.SystemPageFunctions
                             select new
                             {
                                 ID = m.SystemPageFunction_ID.ToString(),
                                 Name = m.SystemFunction.SystemFunction_Name,
                                 ParentID = "p" + (m.SystemPageFunction_PageID ?? 0).ToString(),
                                 Checked = db.GroupPrivileges.Any(x => x.GroupPrivilege_PageFunctionID.Equals(m.SystemPageFunction_ID) && x.GroupPrivilege_GroupID.Equals(groupId))
                             });
            var data = modules.Union(pages).Union(functions);
            dt = data.CopyToDataTable();
        }
        return dt;
    }
    private void CreateTreeViewNodesRecursive(DataTable table, TreeViewNodeCollection nodesCollection, string parentId)
    {
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (table.Rows[i]["ParentID"].ToString() == parentId)
            {
                TreeViewNode node = new TreeViewNode(table.Rows[i]["Name"].ToString(), table.Rows[i]["ID"].ToString())
                {
                    Checked = bool.Parse(table.Rows[i]["Checked"].ToString())
                };
                nodesCollection.Add(node);
                CreateTreeViewNodesRecursive(table, node.Nodes, node.Name);
            }
        }
    }
    private void BindGroups()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try {
                var query = from t in db.Groups
                            select new
                            {
                                t.Group_ID,
                                t.Group_Name
                            };
                if (txtNameSrch.Text.Trim() != string.Empty)
                    query = query.Where(x => x.Group_Name.Contains(txtNameSrch.Text.Trim()));
                ltrCount.Text =query.Count().ToString();
                rpData.DataSource = query;
                rpData.DataBind();
            }
            catch (Exception ex)
            {
                string sPath = HttpContext.Current.Request.Url.AbsolutePath;
                System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
                string PageName = oInfo.Name;
                StackTrace st = new StackTrace(ex, true);
                StackFrame sf = new StackFrame(0);
                Common.InsertException(PageName, ex.Message, ex.StackTrace, int.Parse(sf.GetFileLineNumber().ToString()));
                ScriptManager.RegisterStartupScript(this, GetType(), "alertUser",
                     "alert('حدث خطأ اثناء التنفيذ');", true);
            }
        }
    }
    private void ClearGroupControls()
    {
        txtName.Text = string.Empty;
        lnkSubmitEditGroup.Visible = true;
        tvPermissions.Nodes.Clear();
        ViewState["ID"] = null;
    }
    protected void LnkSearch_Click(object sender, EventArgs e)
    {
        BindGroups();
    }

    protected void LnkNewSearch_Click(object sender, EventArgs e)
    {
        txtNameSrch.Text = string.Empty;
        BindGroups();
    }
    protected void lnkSubmitEditGroup_Click(object sender, EventArgs e)
    {
        Page.Validate("vgSave");
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    int groupId = 0;
                    if (ViewState["ID"] != null)
                    {
                        Group u =
                            db.Groups.FirstOrDefault(x => x.Group_ID.Equals(int.Parse(ViewState["ID"].ToString())));
                        if (u != null)
                        {
                            u.Group_Name = txtName.Text;
                            groupId = u.Group_ID;
                        }
                    }
                    else
                    {
                        Group u = new Group();
                        u.Group_Name = txtName.Text.Trim();
                        db.Groups.InsertOnSubmit(u);
                        db.SubmitChanges();
                        groupId = u.Group_ID;
                    }
                    db.SubmitChanges();
                    SavePermissions(groupId);
                    ClearGroupControls();
                    DataTable table = GetDataTable();
                    CreateTreeViewNodesRecursive(table, tvPermissions.Nodes, "0");
                    BindGroups();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertGroup",
                           "alert('تم الحفظ بنجاح .');$('#groupAddModal').modal('hide');$('.modal-backdrop').remove();", true);
                }
                catch (Exception ex)
                {
                    string sPath = HttpContext.Current.Request.Url.AbsolutePath;
                    System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
                    string PageName = oInfo.Name;
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame sf = new StackFrame(0);
                    Common.InsertException(PageName, ex.Message, ex.StackTrace, int.Parse(sf.GetFileLineNumber().ToString()));
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertUser",
                         "alert('حدث خطأ اثناء التنفيذ');", true);
                }
            }
        }
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');</script>", false);
    }
    private void SavePermissions(int group)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var pr = db.GroupPrivileges.Where(x => x.GroupPrivilege_GroupID.Equals(group));
            db.GroupPrivileges.DeleteAllOnSubmit(pr);
            db.SubmitChanges();
            TreeViewNode MainNode = tvPermissions.Nodes[0];
            PrintNodesRecursive(MainNode, group);
        }

    }
    int z = 0;
    public void PrintNodesRecursive(TreeViewNode oParentNode, int group)
    {
        var x = oParentNode.Text;
        foreach (TreeViewNode SubNode in oParentNode.Nodes)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                foreach (TreeViewNode item in SubNode.Nodes)
                {
                    if (item.Checked)
                    {
                        if (item.Text == "عرض")
                        {
                            var optionsByPage = db.SystemPageFunctions.FirstOrDefault(p => p.SystemPageFunction_PageID.Equals(Convert.ToInt32(SubNode.Name.Replace('p', ' ').Trim())) && p.SystemPageFunction_FunctionID == (int)FunctionsEnum.Show);
                            GroupPrivilege a = new GroupPrivilege
                            {
                                GroupPrivilege_PageFunctionID = optionsByPage.SystemPageFunction_ID,
                                GroupPrivilege_GroupID = group
                            };
                            db.GroupPrivileges.InsertOnSubmit(a);
                        }
                        else if (item.Text == "اضافة")
                        {
                            var optionsByPage = db.SystemPageFunctions.FirstOrDefault(p => p.SystemPageFunction_PageID.Equals(Convert.ToInt32(SubNode.Name.Replace('p', ' ').Trim())) && p.SystemPageFunction_FunctionID == (int)FunctionsEnum.Add);
                            GroupPrivilege a = new GroupPrivilege
                            {
                                GroupPrivilege_PageFunctionID = optionsByPage.SystemPageFunction_ID,
                                GroupPrivilege_GroupID = group
                            };
                            db.GroupPrivileges.InsertOnSubmit(a);
                        }
                        else if (item.Text == "تعديل")
                        {
                            var optionsByPage = db.SystemPageFunctions.FirstOrDefault(p => p.SystemPageFunction_PageID.Equals(Convert.ToInt32(SubNode.Name.Replace('p', ' ').Trim())) && p.SystemPageFunction_FunctionID == (int)FunctionsEnum.Edit);
                            GroupPrivilege a = new GroupPrivilege
                            {
                                GroupPrivilege_PageFunctionID = optionsByPage.SystemPageFunction_ID,
                                GroupPrivilege_GroupID = group
                            };
                            db.GroupPrivileges.InsertOnSubmit(a);
                        }
                        else if (item.Text == "حذف")
                        {
                            var optionsByPage = db.SystemPageFunctions.FirstOrDefault(p => p.SystemPageFunction_PageID.Equals(Convert.ToInt32(SubNode.Name.Replace('p', ' ').Trim())) && p.SystemPageFunction_FunctionID == (int)FunctionsEnum.Delete);
                            GroupPrivilege a = new GroupPrivilege
                            {
                                GroupPrivilege_PageFunctionID = optionsByPage.SystemPageFunction_ID,
                                GroupPrivilege_GroupID = group
                            };
                            db.GroupPrivileges.InsertOnSubmit(a);
                        }
                        else if (item.Text == "فهرسة")
                        {
                            var optionsByPage = db.SystemPageFunctions.FirstOrDefault(p => p.SystemPageFunction_PageID.Equals(Convert.ToInt32(SubNode.Name.Replace('p', ' ').Trim())) && p.SystemPageFunction_FunctionID == (int)FunctionsEnum.Index);
                            GroupPrivilege a = new GroupPrivilege
                            {
                                GroupPrivilege_PageFunctionID = optionsByPage.SystemPageFunction_ID,
                                GroupPrivilege_GroupID = group
                            };
                            db.GroupPrivileges.InsertOnSubmit(a);
                        }
                    }
                }

                db.SubmitChanges();
            }
        }
        ++z; 
        if (z < tvPermissions.Nodes.Count)
        {
            TreeViewNode MainNode = tvPermissions.Nodes[z];
            PrintNodesRecursive(MainNode, group);
        }
    }
    protected void lnkEditGroup_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                ViewState["ID"] = e.CommandArgument;
                var u = (from t in db.Groups
                         where t.Group_ID.Equals(int.Parse(e.CommandArgument.ToString()))
                         select new
                         {
                             t.Group_ID,
                             t.Group_Name
                         }).FirstOrDefault();
                if (u != null)
                {
                    txtName.Text = u.Group_Name;
                }
                tvPermissions.Nodes.Clear();
                DataTable table = GetDataTable();
                CreateTreeViewNodesRecursive(table, tvPermissions.Nodes, "0");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');</script>", false);
            }
            catch (Exception ex)
            {
                string sPath = HttpContext.Current.Request.Url.AbsolutePath;
                System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
                string PageName = oInfo.Name;
                StackTrace st = new StackTrace(ex, true);
                StackFrame sf = new StackFrame(0);
                Common.InsertException(PageName, ex.Message, ex.StackTrace, int.Parse(sf.GetFileLineNumber().ToString()));
                ScriptManager.RegisterStartupScript(this, GetType(), "alertUser",
                     "alert('حدث خطأ اثناء التنفيذ');", true);
            }
        }
    }
    protected void lnkDeleteGroup_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                var pr = db.GroupPrivileges.Where(x => x.GroupPrivilege_GroupID.Equals(int.Parse(e.CommandArgument.ToString())));
                db.GroupPrivileges.DeleteAllOnSubmit(pr);
                Group u = db.Groups.FirstOrDefault(x => x.Group_ID.Equals(int.Parse(e.CommandArgument.ToString())));
                if (u != null)
                    db.Groups.DeleteOnSubmit(u);
                db.SubmitChanges();
                BindGroups();
                ScriptManager.RegisterStartupScript(this, GetType(), "alertGroup", "alert('تم الحذف بنجاح .');", true);
            }
            catch (Exception ex)
            {
                string sPath = HttpContext.Current.Request.Url.AbsolutePath;
                System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
                string PageName = oInfo.Name;
                StackTrace st = new StackTrace(ex, true);
                StackFrame sf = new StackFrame(0);
                Common.InsertException(PageName, ex.Message, ex.StackTrace, int.Parse(sf.GetFileLineNumber().ToString()));
                ScriptManager.RegisterStartupScript(this, GetType(), "alertUser",
                     "alert('حدث خطأ اثناء التنفيذ');", true);
            }
        }
    }
    protected void lnkAddNew_Click(object sender, EventArgs e)
    {
        ClearGroupControls();
        DataTable table = GetDataTable();
        CreateTreeViewNodesRecursive(table, tvPermissions.Nodes, "0");
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');</script>", false);
    }
    protected void lnkCloseModal_Click(object sender, EventArgs e)
    {
        ClearGroupControls();
        DataTable table = GetDataTable();
        CreateTreeViewNodesRecursive(table, tvPermissions.Nodes, "0");
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
    }

    protected void cvName_ServerValidate(object source, ServerValidateEventArgs args)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            int id = ViewState["ID"] != null ? int.Parse(ViewState["ID"].ToString()) : 0;
            args.IsValid =
                !db.Groups.Any(
                    c => c.Group_ID != id &&
                        c.Group_Name.ToLower().Equals(txtName.Text.Trim().ToLower()));
        }
    }
}