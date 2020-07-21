using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class committee : System.Web.UI.Page
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
            return ((DataTable)ViewState["_dtData"]);
        }
        set
        {
            if (value == null)
            {
                ViewState.Remove("_dtData");
            }
            else
            {
                ViewState["_dtData"] = value;
            }
        }
    }
    public List<UserPermissions> UserPermissions
    {
        get
        {
            if (Session["UserPermissions"] != null && Session["UserPermissions"].ToString() != string.Empty)
                return global::UserPermissions.DeSerializePermissionsList(Session["UserPermissions"].ToString());
            else
            {
                return new List<UserPermissions>();
            }
        }
        set { Session["UserPermissions"] = global::UserPermissions.SerializePermissionsList(value); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["User"] != null && Session["User"].ToString() != string.Empty)
            {
                if (UserPermissions.Any(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Committees) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                {
                    var per = UserPermissions.FirstOrDefault(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Committees) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true)));
                    ((HtmlGenericControl)Page.Master.FindControl("ulBreadcrumb")).InnerHtml = "<li><i class='ace-icon fa fa-home home-icon'></i><a href ='Dashboard.aspx'> الرئيسية </a></li><li>" + per.ModuleName + "</li><li>" + per.PageName + "</li><li></li>";
                    Page.Title = per.PageName;
                }
                else
                {
                    Response.Redirect("NoPermission.aspx");
                }
            }
            else
            {
                Session["back"] = Request.Url.AbsoluteUri;
                Response.Redirect("Login.aspx?ReturnURL=" + Request.Url.AbsolutePath);
            }
            BindData();
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["ID"] == null)
                    {
                        //int code = 1;
                        //Committee c = db.Committees.OrderByDescending(x => x.Committee_ID).FirstOrDefault();
                        //if (c != null)
                        //    code = int.Parse(c.Committee_Code.Substring(0, 2)) + 1;
                        int code = 1;
                        Committee c = db.Committees.OrderByDescending(x => x.Committee_ID).FirstOrDefault();
                        if (c != null)
                            code = int.Parse(c.Committee_Code) + 20;
                        Committee q = new Committee()
                        {
                            Committee_Name = txtName.Text,
                            Committee_StatusID = (int)StatusEnum.UnderApprove,
                            Committee_Code = code.ToString().PadLeft(3, '0'),
                            Committee_About = txtAbout.Text,
                            Committee_Notes = txtNotes.Text
                        };
                        db.Committees.InsertOnSubmit(q);
                    }
                    else
                    {
                        Committee q = db.Committees.FirstOrDefault(x => x.Committee_ID.Equals(int.Parse(ViewState["ID"].ToString())));
                        q.Committee_Name = txtName.Text;
                        q.Committee_About = txtAbout.Text;
                        q.Committee_Notes = txtNotes.Text;
                    }
                    db.SubmitChanges();
                    ClearControls();
                    BindData();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#committeeModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    protected void btnEdit_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = int.Parse(e.CommandArgument.ToString());
        FillControls();
    }
    private void FillControls()
    {
        if (ViewState["ID"] != null)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                var query = db.Committees.Where(t => t.Committee_ID.Equals(int.Parse(ViewState["ID"].ToString()))).FirstOrDefault();
                txtName.Text = query.Committee_Name;
                txtCode.Text = query.Committee_Code;
                txtAbout.Text = query.Committee_About;
                txtNotes.Text = query.Committee_Notes;
                //lblStatus.Text = query.IssueStatus.IssueStatus_Name.ToString();
                btnApprove.Visible = btnSave.Visible = query.Committee_StatusID == (int)StatusEnum.UnderApprove;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#committeeModal').modal('show');</script>", false);
            }
        }
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        ViewState["ID"] = null;
    }
    private void ClearControls()
    {
        ViewState["ID"] = null;
        txtCode.Text = txtNotes.Text = txtName.Text=txtAbout.Text = string.Empty;
        btnFreeze.Visible = btnApprove.Visible = false;
        btnSave.Visible = true;
    }
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Committee c = db.Committees.Where(x => x.Committee_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.Committees.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindData();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    private void BindData()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from b in db.Committees
                         select new
                         {
                             b.Committee_ID,
                             b.Committee_Name,
                             b.Committee_Code,
                             b.Status.Status_Name,
                             b.Committee_StatusID,
                             AttachmentCount = b.CommitteeAttachments.Count(),
                             MembersCount = b.Members.Count(),
                            GoalCount = b.CommitteeGoals.Count(),
                             MeetingsCount = b.Meetings.Count(),
                             Attendance = b.Meetings.Count() > 0 ? ((Convert.ToDecimal(b.Meetings.Sum(x => x.Attendances.Count()))/Convert.ToDecimal(b.Meetings.Count()*b.Members.Count())) * decimal.Parse("100")) : 0
                         });
              dtData = query.CopyToDataTable();
            rpData.DataSource = dtData;
            rpData.DataBind();
            ltrCount.Text = dtData.Rows.Count.ToString();
        }
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Committee c = db.Committees.Where(x => x.Committee_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Committee_StatusID = (int)StatusEnum.Approved;
                db.SubmitChanges();
                FillControls();
                BindData();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    protected void btnFreeze_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Committee c = db.Committees.Where(x => x.Committee_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Committee_StatusID = (int)StatusEnum.Freezed;
                db.SubmitChanges();
                FillControls();
                BindData();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    protected void lnkAddNew_Click(object sender, EventArgs e)
    {
        ClearControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#committeeModal').modal('show');</script>", false);
    }
    protected void lnkGoals_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = e.CommandArgument.ToString();
        BindGoals();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#goalsModal').modal('show');</script>", false);
    }

    protected void lnkMembers_Command(object sender, CommandEventArgs e)
    {

    }

    protected void lnkMeetings_Command(object sender, CommandEventArgs e)
    {

    }

    protected void lnkAttachments_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = e.CommandArgument.ToString();
        BindAttachments();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#attachmentsModal').modal('show');</script>", false);
    }
    private void BindAttachments()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.CommitteeAttachments
                        where q.Attachment_CommitteeID == int.Parse(ViewState["ID"].ToString())
                        select new
                        {
                            q.Attachment_ID,
                            q.Attachment_Name,
                            q.Attachment_File
                        };
            rpAttachments.DataSource = query;
            rpAttachments.DataBind();
        }
    }

    protected void btnSaveGoal_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["GoalID"] == null)
                    {
                        CommitteeGoal q = new CommitteeGoal()
                        {
                            Goal_Text = txtGoal.Text,
                            Goal_CommitteeID=int.Parse(ViewState["ID"].ToString())
                        };
                        db.CommitteeGoals.InsertOnSubmit(q);
                    }
                    else
                    {
                        CommitteeGoal q = db.CommitteeGoals.FirstOrDefault(x => x.Goal_ID.Equals(int.Parse(ViewState["GoalID"].ToString())));
                        q.Goal_Text = txtGoal.Text;
                    }
                    db.SubmitChanges();
                    txtGoal.Text = string.Empty;
                    ViewState["GoalID"] = null;
                    BindGoals();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#goalsModal').modal('show');</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }

    private void BindGoals()
    {
        using(CultureDataContext db=new CultureDataContext())
        {
            var query = from q in db.CommitteeGoals
                        where q.Goal_CommitteeID == int.Parse(ViewState["ID"].ToString())
                        select new
                        {
                            q.Goal_ID,
                            q.Goal_Text
                        };
            rpGoals.DataSource = query;
            rpGoals.DataBind();
            BindData();
        }
    }

    protected void lnkAddGoal_Click(object sender, EventArgs e)
    {
        ViewState["GoalID"] = null;
      ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> $('#goalsAddModal').modal('show');</script>", false);
    }

    protected void lnkEditGoal_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            CommitteeGoal g = db.CommitteeGoals.FirstOrDefault(x => x.Goal_ID.Equals(int.Parse(e.CommandArgument.ToString())));
            txtGoal.Text = g.Goal_Text;
            ViewState["GoalID"] = e.CommandArgument.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#goalsAddModal').modal('show');</script>", false);
        }
    }

    protected void lnkDeleteGoal_Command(object sender, CommandEventArgs e)
    {
        using(CultureDataContext db=new CultureDataContext())
        {
            CommitteeGoal g = db.CommitteeGoals.FirstOrDefault(x => x.Goal_ID.Equals(int.Parse(e.CommandArgument.ToString())));
            db.CommitteeGoals.DeleteOnSubmit(g);
            db.SubmitChanges();
            BindGoals();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#goalsModal').modal('show');</script>", false);
        }
    }

    protected void lnkAddAttachment_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> $('#attachmentsAddModal').modal('show');</script>", false);
    }

    protected void btnSaveAttachment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (fuAttach.HasFile)
            {
                string path = Server.MapPath("CommitteeAttachments/");
                while (File.Exists(path + fuAttach.FileName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');</script>", false);
                    return;
                }
                fuAttach.SaveAs(path + fuAttach.FileName);
                CommitteeAttachment q = new CommitteeAttachment();
                q.Attachment_File = "CommitteeAttachments/" + fuAttach.FileName;
                q.Attachment_Name = txtFileName.Text;
                q.Attachment_CommitteeID = int.Parse(ViewState["ID"].ToString());
                db.CommitteeAttachments.InsertOnSubmit(q);
                db.SubmitChanges();
                txtFileName.Text = string.Empty; 
                BindAttachments();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#attachmentsModal').modal('show');</script>", false);
            }
        }
    }

    protected void lnkDeleteAttachment_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            CommitteeAttachment c = db.CommitteeAttachments.Where(x => x.Attachment_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.CommitteeAttachments.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindAttachments();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#attachmentsModal').modal('show');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
}