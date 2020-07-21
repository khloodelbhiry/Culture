using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Users : System.Web.UI.Page
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
    private DataTable DtBatchTypes
    {
        get { return ((DataTable)ViewState["_dtBatchTypes"]); }
        set
        {
            if (value == null)
            {
                ViewState.Remove("_dtBatchTypes");
            }
            else
            {
                if (ViewState["_dtBatchTypes"] == null)
                {
                    ViewState["_dtBatchTypes"] = new DataTable();
                }
                ViewState["_dtBatchTypes"] = value;
                if (((DataTable)ViewState["_dtBatchTypes"]).Columns.Count == 0)
                {
                    ((DataTable)ViewState["_dtBatchTypes"]).Columns.Add("BatchType_ID", typeof(int))
                        .AutoIncrement = true;
                    ((DataTable)ViewState["_dtBatchTypes"]).Columns.Add("BatchType_Name");
                }
            }
        }
    }
    private DataTable DtBranches
    {
        get { return ((DataTable)ViewState["_dtBranches"]); }
        set
        {
            if (value == null)
            {
                ViewState.Remove("_dtBranches");
            }
            else
            {
                if (ViewState["_dtBranches"] == null)
                {
                    ViewState["_dtBranches"] = new DataTable();
                }
                ViewState["_dtBranches"] = value;
                if (((DataTable)ViewState["_dtBranches"]).Columns.Count == 0)
                {
                    ((DataTable)ViewState["_dtBranches"]).Columns.Add("Branch_ID", typeof(int))
                        .AutoIncrement = true;
                    ((DataTable)ViewState["_dtBranches"]).Columns.Add("Branch_Name");
                }
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
                            p.PageUrl.ToLower().Equals(Common.Users) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                {
                    var per = UserPermissions.FirstOrDefault(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Users) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true)));
                    ((HtmlGenericControl)Page.Master.FindControl("ulBreadcrumb")).InnerHtml = "<li><i class='ace-icon fa fa-home home-icon'></i><a href ='Dashboard.aspx'> الرئيسية </a></li><li>" + per.ModuleName + "</li><li>" + per.PageName + "</li><li></li>";
                    Page.Title = per.PageName;
                }
                else
                {
                    Response.Redirect("NoPermission.aspx");
                }
                BindDDL(false);
                BindUsers();
            }
            else
                Response.Redirect("Login.aspx?ReturnURL=" + Request.Url.AbsolutePath);
        }
    }
    private void BindUsers()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                var query = (from t in db.Users
                             select new
                             {
                                 t.User_MemberID,
                                 t.User_ID,
                                 t.User_FullName,
                                 t.User_Email,
                                 t.User_Mobile1,
                                 t.User_StatusID,
                                 t.Status.Status_Name,
                                 t.Committee.Committee_Name,
                                 t.User_CommitteeID,
                                 Group = t.GroupUsers.Any() ? t.GroupUsers.FirstOrDefault().Group.Group_Name : string.Empty,
                                 GroupID = t.GroupUsers.Any() ? (int?)t.GroupUsers.FirstOrDefault().Group.Group_ID : null
                             });
                if (txtNameSrch.Text.Trim() != string.Empty)
                    query = query.Where(x => x.User_FullName.Contains(txtNameSrch.Text.Trim()));
                if (txtEmailSrch.Text.Trim() != string.Empty)
                    query = query.Where(x => x.User_Email.Contains(txtEmailSrch.Text.Trim()));
                if (txtMobileSrc1.Text.Trim() != string.Empty)
                    query = query.Where(x => x.User_Mobile1.Contains(txtMobileSrc1.Text.Trim()));
                if (ddlGroupSrc.SelectedValue != "-1")
                    query = query.Where(x => x.GroupID.Equals(int.Parse(ddlGroupSrc.SelectedValue)));
                if (ddlCommitteeSrc.SelectedValue != "-1")
                    query = query.Where(x => x.User_CommitteeID.Equals(int.Parse(ddlCommitteeSrc.SelectedValue)));
                if (ddlStatusSrch.SelectedValue != "-1")
                    query = query.Where(x => x.User_StatusID.Equals(int.Parse(ddlStatusSrch.SelectedValue)));
                ltrCount.Text = query.Count().ToString();
                rpData.DataSource = query.OrderByDescending(x => x.User_ID);
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
    private void ClearUserControls()
    {
        txtFullName.Text = txtRegUsername.Text = txtMobile.Text = string.Empty;
        lnkSubmitEditUser.Visible = true;
        ddlGroup.SelectedValue = ddlCommittee.SelectedValue ="-1";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
        ViewState["ID"] = null;
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindUsers();
    }
    protected void btnNewSearch_Click(object sender, EventArgs e)
    {
        txtMobileSrc1.Text = txtNameSrch.Text = txtEmailSrch.Text = string.Empty;
        ddlGroupSrc.SelectedValue = ddlCommitteeSrc.SelectedValue = ddlStatusSrch.SelectedValue = "-1";
        BindUsers();
    }
    protected void lnkSubmitEditUser_Click(object sender, EventArgs e)
    {
        Page.Validate("vgRegister");
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    int? member = ddlMember.SelectedValue != "-1" ? (int?)int.Parse(ddlMember.SelectedValue) : null;
                    if (ViewState["ID"] != null)
                    {
                        if (!db.Users.Any(x => x.User_ID != int.Parse(ViewState["ID"].ToString()) && x.User_MemberID == (member ?? 0)))
                        {
                            User u =
                            db.Users.FirstOrDefault(x => x.User_ID.Equals(int.Parse(ViewState["ID"].ToString())));
                            if (u != null)
                            {
                                u.User_MemberID = member;
                                u.User_CommitteeID = ddlCommittee.SelectedValue != "-1" ? (int?)int.Parse(ddlCommittee.SelectedValue) : null;
                                u.User_FullName = txtFullName.Text;
                                u.User_Email = txtRegUsername.Text;
                                u.User_Mobile1 = txtMobile.Text;
                                if(member!=null)
                                {
                                    Member m = db.Members.FirstOrDefault(x => x.Member_ID == member); ;
                                    m.Member_Name = txtFullName.Text;
                                    m.Member_Email = txtRegUsername.Text;
                                }
                                GroupUser gu = u.GroupUsers.FirstOrDefault();
                                if (gu != null && ddlGroup.SelectedValue != "-1")
                                {
                                    gu.GroupUser_GroupID = int.Parse(ddlGroup.SelectedValue);
                                }
                                else if (gu != null && ddlGroup.SelectedValue == "-1")
                                {
                                    db.GroupUsers.DeleteOnSubmit(gu);
                                }
                                else if (gu == null && ddlGroup.SelectedValue != "-1")
                                {
                                    GroupUser t = new GroupUser();
                                    t.GroupUser_UserID = u.User_ID;
                                    t.GroupUser_GroupID = int.Parse(ddlGroup.SelectedValue);
                                    db.GroupUsers.InsertOnSubmit(t);
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');alert('تم انشاء مستخدم لهذا العضو من قبل');</script>", false);
                            return;
                        }
                    }
                    else
                    {
                        if (!db.Users.Any(x => x.User_MemberID == (member ?? 0)))
                        {
                            User u = new User();
                            u.User_MemberID = member;
                            u.User_CommitteeID = ddlCommittee.SelectedValue != "-1" ? (int?)int.Parse(ddlCommittee.SelectedValue) : null;
                            u.User_FullName = txtFullName.Text.Trim();
                            u.User_Email = txtRegUsername.Text.Trim();
                            u.User_Mobile1 = txtMobile.Text.Trim();
                            u.User_Password = EncryptString.Encrypt("123456");
                            u.User_StatusID = (int)StatusEnum.Approved;
                            db.Users.InsertOnSubmit(u);
                            if (member != null)
                            {
                                Member m = db.Members.FirstOrDefault(x => x.Member_ID == member); ;
                                m.Member_Name = txtFullName.Text;
                                m.Member_Email = txtRegUsername.Text;
                            }
                            if (ddlGroup.SelectedValue != "-1")
                            {
                                var g = db.Groups.FirstOrDefault(x => x.Group_ID.Equals(int.Parse(ddlGroup.SelectedValue)));
                                if (g != null)
                                {
                                    GroupUser t = new GroupUser();
                                    t.GroupUser_GroupID = g.Group_ID;
                                    t.User = u;
                                    db.GroupUsers.InsertOnSubmit(t);
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');$('body').removeClass('modal-open');alert('تم انشاء مستخدم لهذا العضو من قبل');</script>", false);
                            return;
                        }
                    }
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertUserm", "$('#groupAddModal').modal('hide');$('.modal-backdrop').remove();$('body').removeClass('modal-open');alert('تم الحفظ بنجاح .');", true);
                    ClearUserControls();
                    BindDDL(true);
                    BindUsers(); }
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
    protected void lnkNewUser_Click(object sender, EventArgs e)
    {
        ClearUserControls();
    }
    private void BindDDL(bool rebindMembers)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (rebindMembers == false)
            {
                var query = from q in db.Groups
                            select new
                            {
                                q.Group_ID,
                                q.Group_Name
                            };
                ddlGroup.Items.Clear();
                ddlGroup.DataSource = query;
                ddlGroup.DataTextField = "Group_Name";
                ddlGroup.DataValueField = "Group_ID";
                ddlGroup.DataBind();
                ddlGroup.Items.Insert(0, new ListItem("اختر", "-1"));

                ddlGroupSrc.Items.Clear();
                ddlGroupSrc.DataSource = query;
                ddlGroupSrc.DataTextField = "Group_Name";
                ddlGroupSrc.DataValueField = "Group_ID";
                ddlGroupSrc.DataBind();
                ddlGroupSrc.Items.Insert(0, new ListItem("اختر", "-1"));

                var query1 = from q in db.Committees
                             select new
                             {
                                 q.Committee_ID,
                                 q.Committee_Name
                             };
                ddlCommittee.Items.Clear();
                ddlCommittee.DataSource = query1;
                ddlCommittee.DataTextField = "Committee_Name";
                ddlCommittee.DataValueField = "Committee_ID";
                ddlCommittee.DataBind();
                ddlCommittee.Items.Insert(0, new ListItem("اختر", "-1"));

                ddlCommitteeSrc.Items.Clear();
                ddlCommitteeSrc.DataSource = query1;
                ddlCommitteeSrc.DataTextField = "Committee_Name";
                ddlCommitteeSrc.DataValueField = "Committee_ID";
                ddlCommitteeSrc.DataBind();
                ddlCommitteeSrc.Items.Insert(0, new ListItem("اختر", "-1"));
            }
            var query2 = from q in db.Members
                         select new
                         {
                             q.Member_ID,
                             q.Member_Name
                         };
            ddlMember.Items.Clear();
            ddlMember.DataSource = query2;
            ddlMember.DataTextField = "Member_Name";
            ddlMember.DataValueField = "Member_ID";
            ddlMember.DataBind();
            ddlMember.Items.Insert(0, new ListItem("اختر", "-1"));
        }
    }
    protected void lnkEditUser_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                ViewState["ID"] = e.CommandArgument;
                var u = (from t in db.Users
                         where t.User_ID.Equals(int.Parse(e.CommandArgument.ToString()))
                         select new
                         {
                             t.User_ID,
                             t.User_FullName,
                             t.User_Email,
                             t.User_Mobile1,
                             t.User_Password,
                             t.User_StatusID,
                             t.Status.Status_Name,
                             t.User_CommitteeID,
                             t.User_MemberID,
                             GroupUser_GroupID = t.GroupUsers.Any() ? t.GroupUsers.FirstOrDefault().GroupUser_GroupID : -1
                         }).FirstOrDefault();
                if (u != null)
                {
                    ddlMember.SelectedValue = u.User_MemberID != null ? u.User_MemberID.ToString() : "-1";
                    ddlCommittee.SelectedValue = u.User_CommitteeID != null ? u.User_CommitteeID.ToString() : "-1";
                    txtFullName.Text = u.User_FullName;
                    txtRegUsername.Text = u.User_Email;
                    txtMobile.Text = u.User_Mobile1;
                    lnkSubmitEditUser.Visible = u.User_StatusID == (int)StatusEnum.Approved;
                    ddlGroup.SelectedValue = u.GroupUser_GroupID.ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');</script>", false);
                }
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
    protected void lnkDeleteUser_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                User u = db.Users.FirstOrDefault(x => x.User_ID.Equals(int.Parse(e.CommandArgument.ToString())));
                if (u != null)
                    db.Users.DeleteOnSubmit(u);
                db.SubmitChanges();
                BindUsers();
                ScriptManager.RegisterStartupScript(this, GetType(), "alertUser", "alert('تم الحذف بنجاح .');", true);
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
    protected void cvRegUsername_ServerValidate(object source, ServerValidateEventArgs args)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            int id = ViewState["ID"] != null ? int.Parse(ViewState["ID"].ToString()) : 0;
            args.IsValid =
                !db.Users.Any(
                    c => c.User_ID != id &&
                        c.User_Email.Equals(txtRegUsername.Text.Trim()));
        }
    }
    protected void cvMobile_ServerValidate(object source, ServerValidateEventArgs args)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            int id = ViewState["ID"] != null ? int.Parse(ViewState["ID"].ToString()) : 0;
            args.IsValid =
                !db.Users.Any(
                    c => c.User_ID != id &&
                        c.User_Mobile1.Equals(txtMobile.Text.Trim()));
        }
    }
    protected void btnNewUser_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');</script>", false);
    }
    protected void lnkCloseModal_Click(object sender, EventArgs e)
    {
        ClearUserControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
    }

    protected void cvFullName_ServerValidate(object source, ServerValidateEventArgs args)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            int id = ViewState["ID"] != null ? int.Parse(ViewState["ID"].ToString()) : 0;
            args.IsValid =
                !db.Users.Any(
                    c => c.User_ID != id &&
                        c.User_FullName.Equals(txtFullName.Text.Trim()));
        }
    }

    protected void ddlMember_SelectedIndexChanged(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = db.Members.FirstOrDefault(x => x.Member_ID == int.Parse(ddlMember.SelectedValue));
            if (query != null)
            {
                txtFullName.Text = query.Member_Name;
                txtRegUsername.Text = query.Member_Email;
                ddlCommittee.SelectedValue = query.Member_CommitteeID.ToString();
                ddlCommittee.Enabled = false;
            }
            else
            {
                ddlCommittee.Enabled = true;
            }
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#groupAddModal').modal('show');</script>", false);
    }
}