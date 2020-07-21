using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class meetingsAgenda : System.Web.UI.Page
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
            using (CultureDataContext db = new CultureDataContext())
            {
                ltrCommitte.Text = db.Committees.FirstOrDefault(x => x.Committee_ID == db.Meetings.FirstOrDefault(z=>z.Meeting_ID== int.Parse(Request.QueryString["id"])).Meeting_CommitteeID).Committee_Name;
            }
            FillDropDown();
            BindData();
        }
    }
    private void FillDropDown()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = db.DiscussionPriorities.Select(x => new
            {
                x.DiscussionPriority_ID,
                x.DiscussionPriority_Name
            });
            ddlDiscussionPriority.DataSource = query;
            ddlDiscussionPriority.DataTextField = "DiscussionPriority_Name";
            ddlDiscussionPriority.DataValueField = "DiscussionPriority_ID";
            ddlDiscussionPriority.DataBind();
        }
    }

    private void BindData()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from b in db.MeetingAgendas
                         where b.Agenda_MeetingID== int.Parse(Request.QueryString["id"])
                         select new
                         {
                             b.Agenda_MeetingID,
                             b.Agenda_ID,
                             b.Agenda_Item,
                             b.Status.Status_Name,
                             b.Agenda_StatusID,
                             DiscussionCount = db.MeetingDiscussions.Where(x=>x.Discussion_MeetingID== int.Parse(Request.QueryString["id"])).Count(),
                             ReccommondationCount = db.MeetingRecommendations.Where(x => x.Recommendation_MeetingID == int.Parse(Request.QueryString["id"])).Count()
                         });
            dtData = query.OrderByDescending(x => x.Agenda_ID).CopyToDataTable();
            rpData.DataSource = dtData;
            rpData.DataBind();
            ltrCount.Text = dtData.Rows.Count.ToString();
        }
    }
    protected void lnkAddNew_Click(object sender, EventArgs e)
    {
        ClearControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#AgendaModal').modal('show');</script>", false);
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
                        MeetingAgenda q = new MeetingAgenda()
                        {
                            Agenda_Item = txtName.Text,
                            Agenda_StatusID = (int)StatusEnum.UnderApprove,
                            Agenda_MeetingID= int.Parse(Request.QueryString["id"]),
                            Agenda_DiscussionPriorityID =int.Parse(ddlDiscussionPriority.SelectedValue),
                            Agenda_Notes = txtNotes.Text
                        };
                        db.MeetingAgendas.InsertOnSubmit(q);
                    }
                    else
                    {
                        MeetingAgenda q = db.MeetingAgendas.FirstOrDefault(x => x.Agenda_ID.Equals(int.Parse(ViewState["ID"].ToString())));
                        q.Agenda_Item = txtName.Text;
                        q.Agenda_Notes = txtNotes.Text;
                        q.Agenda_DiscussionPriorityID = int.Parse(ddlDiscussionPriority.SelectedValue);
                    }
                    db.SubmitChanges();
                    ClearControls();
                    BindData();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#AgendaModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    protected void lnkEdit_Command(object sender, CommandEventArgs e)
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
                var query = db.MeetingAgendas.Where(t => t.Agenda_ID.Equals(int.Parse(ViewState["ID"].ToString()))).FirstOrDefault();
                txtName.Text = query.Agenda_Item;
                ddlDiscussionPriority.SelectedValue = query.Agenda_DiscussionPriorityID.ToString();
                txtNotes.Text = query.Agenda_Notes;
                //lblStatus.Text = query.IssueStatus.IssueStatus_Name.ToString();
                btnApprove.Visible = btnSave.Visible = query.Agenda_StatusID == (int)StatusEnum.UnderApprove;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#AgendaModal').modal('show');</script>", false);
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
         txtNotes.Text = txtName.Text = string.Empty;
        ddlDiscussionPriority.SelectedValue = "1";
        btnFreeze.Visible = btnApprove.Visible = false;
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingAgenda c = db.MeetingAgendas.Where(x => x.Agenda_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Agenda_StatusID = (int)StatusEnum.Approved;
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
            MeetingAgenda c = db.MeetingAgendas.Where(x => x.Agenda_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Agenda_StatusID = (int)StatusEnum.Freezed;
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
    protected void lnkDiscussion_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = e.CommandArgument.ToString();
        BindDiscussion();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#DiscussionModal').modal('show');</script>", false);
    }
    protected void lnkRecommendation_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = e.CommandArgument.ToString();
        BindRecommendation();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#RecommondationModal').modal('show');</script>", false);
    }
    private void BindDiscussion()
    {

    }
    private void BindRecommendation()
    {

    }
}