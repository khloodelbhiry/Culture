using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class meetings : System.Web.UI.Page
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
                            p.PageUrl.ToLower().Equals(Common.Meetings) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                {
                    var per = UserPermissions.FirstOrDefault(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Meetings) &&
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
            if (Request.QueryString["id"] != null)
            {
                BindAgenda();
                GetMember();
                BindDiscussion();
                BindRecommendations();
                BindAttachments();
            }
        }
    }

    #region Agenda
    private void BindAgenda()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from b in db.MeetingAgendas
                         where b.Agenda_MeetingID == int.Parse(Request.QueryString["id"])
                         select new
                         {
                             b.Agenda_MeetingID,
                             b.Agenda_ID,
                             b.Agenda_Item,
                             b.Status.Status_Name,
                             b.Agenda_StatusID,
                             b.DiscussionPriority.DiscussionPriority_Name
                         });
            rpAgenda.DataSource = query;
            rpAgenda.DataBind();
        }
    }
    protected void lnkAddNewAgenda_Click(object sender, EventArgs e)
    {
        ClearControlsAgenda();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#AddAgendaModal').modal('show');</script>", false);
    }
    protected void btnSaveAgenda_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["AgendaID"] == null)
                    {
                        MeetingAgenda q = new MeetingAgenda()
                        {
                            Agenda_Item = txtAgendaName.Text,
                            Agenda_StatusID = (int)StatusEnum.UnderApprove,
                            Agenda_MeetingID = int.Parse(Request.QueryString["id"]),
                            Agenda_DiscussionPriorityID = rdPriority1.Checked ? 1 : (rdPriority2.Checked ? 2 : 3),
                            Agenda_Notes = txtAgendaNotes.Text
                        };
                        db.MeetingAgendas.InsertOnSubmit(q);
                    }
                    else
                    {
                        MeetingAgenda q = db.MeetingAgendas.FirstOrDefault(x => x.Agenda_ID.Equals(int.Parse(ViewState["AgendaID"].ToString())));
                        q.Agenda_Item = txtAgendaName.Text;
                        q.Agenda_Notes = txtAgendaNotes.Text;
                        q.Agenda_DiscussionPriorityID = rdPriority1.Checked ? 1 : (rdPriority2.Checked ? 2 : 3);
                    }
                    db.SubmitChanges();
                    ClearControlsAgenda();
                    BindAgenda();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#AddAgendaModal').modal('hide');$('.modal-backdrop').remove();$('#AgendaModalrpAgenda').modal('show');</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    protected void lnkEditAgenda_Command(object sender, CommandEventArgs e)
    {
        ViewState["AgendaID"] = int.Parse(e.CommandArgument.ToString());
        FillControlsAgenda();
    }
    private void FillControlsAgenda()
    {
        if (ViewState["AgendaID"] != null)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                var query = db.MeetingAgendas.Where(t => t.Agenda_ID.Equals(int.Parse(ViewState["AgendaID"].ToString()))).FirstOrDefault();
                txtAgendaName.Text = query.Agenda_Item;
                rdPriority1.Checked = query.Agenda_DiscussionPriorityID == 1;
                rdPriority2.Checked = query.Agenda_DiscussionPriorityID == 2;
                rdPriority3.Checked = query.Agenda_DiscussionPriorityID == 3;
                txtAgendaNotes.Text = query.Agenda_Notes;
                btnApproveAgenda.Visible = btnFreezeAgenda.Visible = btnSaveAgenda.Visible = query.Agenda_StatusID == (int)StatusEnum.UnderApprove;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#AddAgendaModal').modal('show');</script>", false);
            }
        }
    }
    private void ClearControlsAgenda()
    {
        ViewState["AgendaID"] = null;
        txtAgendaNotes.Text = txtAgendaName.Text = string.Empty;
        rdPriority1.Checked = true;
        btnFreezeAgenda.Visible = btnApproveAgenda.Visible = false;
        rdPriority2.Checked = rdPriority3.Checked = false;
        btnSaveAgenda.Visible = true;
    }
    protected void btnApproveAgenda_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingAgenda c = db.MeetingAgendas.Where(x => x.Agenda_ID == int.Parse(ViewState["AgendaID"].ToString())).FirstOrDefault();
            try
            {
                c.Agenda_StatusID = (int)StatusEnum.Approved;
                db.SubmitChanges();
                FillControlsAgenda();
                BindAgenda();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    protected void btnFreezeAgenda_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingAgenda c = db.MeetingAgendas.Where(x => x.Agenda_ID == int.Parse(ViewState["AgendaID"].ToString())).FirstOrDefault();
            try
            {
                c.Agenda_StatusID = (int)StatusEnum.Freezed;
                db.SubmitChanges();
                FillControlsAgenda();
                BindAgenda();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region Members
    private void GetMember()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query1 = from x in db.Attendances
                         where x.Attendance_MeetingID == int.Parse(Request.QueryString["id"])
                         select new
                         {
                             x.Attendance_MemberID,
                             x.Member.Member_Name
                         };
            rpAttendance.DataSource = query1;
            rpAttendance.DataBind();
            var lst = query1.Select(x => x.Attendance_MemberID).ToList();
            int c = db.Meetings.FirstOrDefault(x => x.Meeting_ID == int.Parse(Request.QueryString["id"])).Meeting_CommitteeID ?? 0;
            var all = db.Members.Where(x => x.Member_CommitteeID == c);
            var query = all.Where(x => !lst.Contains(x.Member_ID) && x.Member_CommitteeID == c).Select(x => new
            {
                x.Member_ID,
                x.Member_Name
            });
            rpMember.DataSource = query;
            rpMember.DataBind();
            divCheckAll.Visible = btnSaveAttendance.Visible = query.Count() != 0;
            ddlImplementation.DataSource = all;
            ddlImplementation.DataValueField = "Member_ID";
            ddlImplementation.DataTextField = "Member_Name";
            ddlImplementation.DataBind();

            ddlDiscussionRecImplementation.DataSource = all;
            ddlDiscussionRecImplementation.DataValueField = "Member_ID";
            ddlDiscussionRecImplementation.DataTextField = "Member_Name";
            ddlDiscussionRecImplementation.DataBind();
        }
    }
    protected void btnSaveAttendance_Click(object sender, EventArgs e)
    {

        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                int count = 0;
                foreach (RepeaterItem item in rpMember.Items)
                {
                    HtmlInputCheckBox chkDisplayTitle = (HtmlInputCheckBox)item.FindControl("check");
                    if (chkDisplayTitle.Checked)
                    {
                        Attendance a = new Attendance()
                        {
                            Attendance_MeetingID = int.Parse(Request.QueryString["id"]),
                            Attendance_MemberID = int.Parse(chkDisplayTitle.Value)
                        };
                        db.Attendances.InsertOnSubmit(a);
                        Reward r = new Reward()
                        {
                            Reward_MemberID = int.Parse(chkDisplayTitle.Value),
                            Reward_Date = DateTime.Now,
                            Reward_PaymentStatusID = 1,
                            Reward_Value = db.Members.FirstOrDefault(x => x.Member_ID == int.Parse(chkDisplayTitle.Value)).Member_RoleID == 2 ? db.Meetings.FirstOrDefault(x => x.Meeting_ID == int.Parse(Request.QueryString["id"])).Meeting_RewardMember : db.Meetings.FirstOrDefault(x => x.Meeting_ID == int.Parse(Request.QueryString["id"])).Meeting_RewardNotMember
                        };
                        db.Rewards.InsertOnSubmit(r);
                        count++;
                    }

                }
                if (count == 0)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show'); alert('اختر اعضاء الاجتماع');</script>", false);
                db.SubmitChanges();
                checkAll.Checked = false;
                GetMember();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show'); alert('تم الحفظ بنجاح');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                //Logger.ErrorLog(exception);
            }
        }

    }
    protected void Check_Clicked(object sender, EventArgs e)
    {
        if (checkAll.Checked)
        {
            foreach (RepeaterItem item in rpMember.Items)
            {
                HtmlInputCheckBox chkDisplayTitle = (HtmlInputCheckBox)item.FindControl("check");
                chkDisplayTitle.Checked = true;

            }
        }
        else
        {
            foreach (RepeaterItem item in rpMember.Items)
            {
                HtmlInputCheckBox chkDisplayTitle = (HtmlInputCheckBox)item.FindControl("check");
                chkDisplayTitle.Checked = false;
            }
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attendance').addClass('active show');$('#attendancePIll').addClass('active show');</script>", false);
    }
    #endregion

    #region Discussions
    private void BindDiscussion()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.MeetingDiscussions
                        where q.Discussion_MeetingID == int.Parse(Request.QueryString["id"])
                        select new
                        {
                            q.Discussion_Text,
                            q.Discussion_ID
                        };
            rpDiscussion.DataSource = query;
            rpDiscussion.DataBind();
        }
    }
    protected void lnkAddDiscussion_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show');$('#DiscussionAddModal').modal('show');</script>", false);
    }

    protected void btnSaveDiscussion_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["DiscussionID"] == null)
                    {
                        MeetingDiscussion q = new MeetingDiscussion()
                        {
                            Discussion_Text = txtDiscussion.Text,
                            Discussion_MeetingID = int.Parse(Request.QueryString["id"])

                        };
                        db.MeetingDiscussions.InsertOnSubmit(q);
                    }
                    else
                    {
                        MeetingDiscussion q = db.MeetingDiscussions.FirstOrDefault(x => x.Discussion_ID.Equals(int.Parse(ViewState["DiscussionID"].ToString())));
                        q.Discussion_Text = txtDiscussion.Text;
                    }
                    db.SubmitChanges();
                    BindDiscussion();
                    txtDiscussion.Text = string.Empty;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show'); alert('تم الحفظ بنجاح');$('#DiscussionAddModal').modal('hide');$('.modal-backdrop').remove();$('#DiscussionModal').modal('show');</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    protected void lnkEditDiscussion_Command(object sender, CommandEventArgs e)
    {
        ViewState["DiscussionID"] = int.Parse(e.CommandArgument.ToString());
        FillDiscussionControl();
    }
    private void FillDiscussionControl()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingDiscussion m = db.MeetingDiscussions.FirstOrDefault(x => x.Discussion_ID == int.Parse(ViewState["DiscussionID"].ToString()));
            txtDiscussion.Text = m.Discussion_Text;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show');$('#DiscussionAddModal').modal('show');</script>", false);
        }
    }
    protected void lnkDeleteDiscussion_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingDiscussion c = db.MeetingDiscussions.Where(x => x.Discussion_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.MeetingDiscussions.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindDiscussion();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>alert('تم الحذف بنجاح');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show'); </script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region Recommendations
    private void BindRecommendations()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.MeetingRecommendations
                        where q.Recommendation_MeetingID == int.Parse(Request.QueryString["id"])
                        select new
                        {
                            q.Recommendation_Code,
                            q.Recommendation_Text,
                            q.Recommendation_StatusID,
                            Recommendation_ResponsibleForImplementation = string.Empty,// q.Recommendation_ResponsibleForImplementation,
                            q.Recommendation_Progress,
                            q.Recommendation_ID,
                            q.Status.Status_Name,
                            Implementers = from x in q.ResponsibleForImplementations
                                           select new
                                           {
                                               x.Member.Member_Name
                                           }
                        };
            RpRecommondation.DataSource = query;
            RpRecommondation.DataBind();
        }
    }
    protected void lnkAddRecommondation_Click(object sender, EventArgs e)
    {
        ClearRecommondationControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#RecommondationAddModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');</script>", false);
    }

    protected void btnSaveRec_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["RecommondationID"] == null)
                    {
                        MeetingRecommendation q = new MeetingRecommendation()
                        {
                            Recommendation_Text = txtRecomName.Text,
                            Recommendation_MeetingID = int.Parse(Request.QueryString["id"]),
                            Recommendation_Code = txtRecCode.Text,
                            Recommendation_Progress = int.Parse(txtProgress.Text),
                            Recommendation_StatusID = (int)StatusEnum.UnderApprove
                        };
                        db.MeetingRecommendations.InsertOnSubmit(q);
                        for (int i = 0; i < ddlImplementation.Items.Count; i++)
                        {
                            if (ddlImplementation.Items[i].Selected)
                            {
                                ResponsibleForImplementation r = new ResponsibleForImplementation();
                                r.MeetingRecommendation = q;
                                r.ResponsibleForImplementation_MemberID = int.Parse(ddlImplementation.Items[i].Value);
                                db.ResponsibleForImplementations.InsertOnSubmit(r);
                            }
                        }
                    }
                    else
                    {
                        MeetingRecommendation q = db.MeetingRecommendations.FirstOrDefault(x => x.Recommendation_ID.Equals(int.Parse(ViewState["RecommondationID"].ToString())));
                        q.Recommendation_Text = txtRecomName.Text;
                        q.Recommendation_Code = txtRecCode.Text;
                        q.Recommendation_Progress = int.Parse(txtProgress.Text);
                        var imp = q.ResponsibleForImplementations;
                        db.ResponsibleForImplementations.DeleteAllOnSubmit(imp);
                        for (int i = 0; i < ddlImplementation.Items.Count; i++)
                        {
                            if (ddlImplementation.Items[i].Selected)
                            {
                                ResponsibleForImplementation r = new ResponsibleForImplementation();
                                r.MeetingRecommendation = q;
                                r.ResponsibleForImplementation_MemberID = int.Parse(ddlImplementation.Items[i].Value);
                                db.ResponsibleForImplementations.InsertOnSubmit(r);
                            }
                        }
                    }
                    db.SubmitChanges();
                    BindRecommendations();
                    ClearRecommondationControls();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#RecommondationAddModal').modal('hide');$('.modal-backdrop').remove();$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    private void ClearRecommondationControls()
    {
        ViewState["RecommondationID"] = null;
        btnApproveRec.Visible = btnFreezeRec.Visible = false;
        btnSaveRec.Visible = true;
        txtRecomName.Text = txtRecCode.Text = txtRecCode.Text = txtProgress.Text = string.Empty;
    }

    protected void lnkEditRecommondation_Command(object sender, CommandEventArgs e)
    {
        ViewState["RecommondationID"] = int.Parse(e.CommandArgument.ToString());
        FillRecommondationControl();

    }
    private void FillRecommondationControl()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingRecommendation m = db.MeetingRecommendations.FirstOrDefault(x => x.Recommendation_ID == int.Parse(ViewState["RecommondationID"].ToString()));
            txtRecomName.Text = m.Recommendation_Text;
            txtRecCode.Text = m.Recommendation_Code;
            txtProgress.Text = m.Recommendation_Progress.ToString();
            var imp = m.ResponsibleForImplementations;
            foreach (var i in imp)
            {
                ListItem li = ddlImplementation.Items.FindByValue(i.ResponsibleForImplementation_MemberID.ToString());
                li.Selected = true;
            }
            btnFreezeRec.Visible = btnApproveRec.Visible = btnSaveRec.Visible = m.Recommendation_StatusID == (int)StatusEnum.UnderApprove;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#RecommondationAddModal').modal('show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');</script>", false);
        }
    }

    protected void btnApproveRec_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingRecommendation c = db.MeetingRecommendations.Where(x => x.Recommendation_ID == int.Parse(ViewState["RecommondationID"].ToString())).FirstOrDefault();
            try
            {
                c.Recommendation_StatusID = 2;
                db.SubmitChanges();
                FillRecommondationControl();
                BindRecommendations();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }

    protected void btnFreezeRec_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingRecommendation c = db.MeetingRecommendations.Where(x => x.Recommendation_ID == int.Parse(ViewState["RecommondationID"].ToString())).FirstOrDefault();
            try
            {
                c.Recommendation_StatusID = 3;
                db.SubmitChanges();
                FillRecommondationControl();
                BindRecommendations();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region Recommendation Discussions
    protected void lnkRecDiscussions_Command(object sender, CommandEventArgs e)
    {
        ViewState["RecID"] = e.CommandArgument.ToString();
        BindRecDiscussion();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#RecDiscussionModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');</script>", false);
    }
    private void BindRecDiscussion()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.RecommendationDiscussions
                        where q.RecommendationDiscussion_RecommendationID == int.Parse(ViewState["RecID"].ToString())
                        select new
                        {
                            q.RecommendationDiscussion_Text,
                            q.RecommendationDiscussion_ID
                        };
            rpRecDiscussion.DataSource = query;
            rpRecDiscussion.DataBind();
        }
    }
    protected void lnkAddRecDiscussion_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#RecDiscussionAddModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');</script>", false);
    }

    protected void btnSaveRecDiscussion_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["RecDiscussionID"] == null)
                    {
                        var xx = ViewState["RecID"].ToString();
                        RecommendationDiscussion q = new RecommendationDiscussion()
                        {
                            RecommendationDiscussion_Text = txtRecDiscussion.Text,
                            RecommendationDiscussion_RecommendationID = int.Parse(ViewState["RecID"].ToString())

                        };
                        db.RecommendationDiscussions.InsertOnSubmit(q);
                    }
                    else
                    {
                        RecommendationDiscussion q = db.RecommendationDiscussions.FirstOrDefault(x => x.RecommendationDiscussion_ID.Equals(int.Parse(ViewState["RecDiscussionID"].ToString())));
                        q.RecommendationDiscussion_Text = txtRecDiscussion.Text;
                    }
                    db.SubmitChanges();
                    BindRecDiscussion();
                    txtRecDiscussion.Text = string.Empty;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#DiscussionAddModal').modal('hide');$('.modal-backdrop').remove();$('#RecDiscussionModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    protected void lnkEditRecDiscussion_Command(object sender, CommandEventArgs e)
    {
        ViewState["RecDiscussionID"] = int.Parse(e.CommandArgument.ToString());
        FillRecDiscussionControl();

    }
    private void FillRecDiscussionControl()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            RecommendationDiscussion m = db.RecommendationDiscussions.FirstOrDefault(x => x.RecommendationDiscussion_ID == int.Parse(ViewState["RecDiscussionID"].ToString()));
            txtRecDiscussion.Text = m.RecommendationDiscussion_Text;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#RecDiscussionAddModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');</script>", false);

        }
    }
    protected void lnkDeleteRecDiscussion_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            RecommendationDiscussion c = db.RecommendationDiscussions.Where(x => x.RecommendationDiscussion_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.RecommendationDiscussions.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindRecDiscussion();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#recommendation').addClass('active show');$('#recommendationPIll').addClass('active show');$('#RecDiscussionModal').modal('show'); alert('تم الحذف بنجاح');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region Attachments
    private void BindAttachments()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.MeetingAttachments
                        where q.Attachment_MeetingID == int.Parse(Request.QueryString["id"])
                        select new
                        {
                            q.Attachment_Name,
                            q.Attachment_File,
                            q.Attachment_ID
                        };
            rpAttachments.DataSource = query;
            rpAttachments.DataBind();
        }
    }
    protected void lnkAddAttachment_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#attachmentsAddModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attachment').addClass('active show');$('#attachmentPIll').addClass('active show');</script>", false);
    }

    protected void btnSaveAttachment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (fuAttach.HasFile)
            {
                string path = Server.MapPath("MeetingsAttachments/");
                while (File.Exists(path + fuAttach.FileName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attachment').addClass('active show');$('#attachmentPIll').addClass('active show');</script>", false);
                    return;
                }
                fuAttach.SaveAs(path + fuAttach.FileName);
                MeetingAttachment q = new MeetingAttachment();
                q.Attachment_File = "MeetingsAttachments/" + fuAttach.FileName;
                q.Attachment_Name = txtFileName.Text;
                q.Attachment_MeetingID = int.Parse(Request.QueryString["id"]);
                db.MeetingAttachments.InsertOnSubmit(q);
                db.SubmitChanges();
                txtFileName.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#attachmentsModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attachment').addClass('active show');$('#attachmentPIll').addClass('active show');</script>", false);
                BindAttachments();
            }
        }
    }

    protected void lnkDeleteAttachment_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingAttachment c = db.MeetingAttachments.Where(x => x.Attachment_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.MeetingAttachments.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindAttachments();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#attachmentsModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#attachment').addClass('active show');$('#attachmentPIll').addClass('active show');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region Agenda Attachments

    protected void lnkAgendaAttachments_Command(object sender, CommandEventArgs e)
    {
        ViewState["AgendaAttachID"] = int.Parse(e.CommandArgument.ToString());
        BindAgendaAttachments();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agendaAttachmentsModal').modal('show');</script>", false);
    }

    private void BindAgendaAttachments()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.AgendaAttachments
                        where q.Attachment_AgendaID == int.Parse(ViewState["AgendaAttachID"].ToString())
                        select new
                        {
                            q.Attachment_Name,
                            q.Attachment_File,
                            q.Attachment_ID
                        };
            rpAgendaAttachments.DataSource = query;
            rpAgendaAttachments.DataBind();
        }
    }

    protected void lnkAddAgendaAttachment_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#agendaAttachmentsAddModal').modal('show');</script>", false);
    }

    protected void btnSaveAgendaAttachment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (fuAgendaAttachment.HasFile)
            {
                string path = Server.MapPath("AgendaAttachments/");
                while (File.Exists(path + fuAgendaAttachment.FileName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');</script>", false);
                    return;
                }
                fuAgendaAttachment.SaveAs(path + fuAgendaAttachment.FileName);
                AgendaAttachment q = new AgendaAttachment();
                q.Attachment_File = "AgendaAttachments/" + fuAgendaAttachment.FileName;
                q.Attachment_Name = txtAgendaFileName.Text;
                q.Attachment_AgendaID = int.Parse(ViewState["AgendaAttachID"].ToString());
                db.AgendaAttachments.InsertOnSubmit(q);
                db.SubmitChanges();
                txtAgendaFileName.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#agendaAttachmentsModal').modal('show');</script>", false);
                BindAgendaAttachments();
            }
        }
    }

    protected void lnkDeleteAgendaAttachment_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            AgendaAttachment c = db.AgendaAttachments.Where(x => x.Attachment_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.AgendaAttachments.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindAgendaAttachments();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#agendaAttachmentsModal').modal('show');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region All Agenda Attachments

    protected void lnkAllAgendaAttachments_Click(object sender, EventArgs e)
    {
        BindAllAgendaAttachments();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#allAgendaAttachmentsModal').modal('show');</script>", false);
    }

    private void BindAllAgendaAttachments()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.MeetingAgendaAttachments
                        where q.Attachment_MeetingID == int.Parse(Request.QueryString["id"])
                        select new
                        {
                            q.Attachment_Name,
                            q.Attachment_File,
                            q.Attachment_ID
                        };
            rpAllAgendaAttachment.DataSource = query;
            rpAllAgendaAttachment.DataBind();
        }
    }

    protected void lnkAddAllAgendaAttachment_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#allAgendaAttachmentsAddModal').modal('show');</script>", false);
    }

    protected void btnSaveAllAgendaAttachment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (fuAllAgenda.HasFile)
            {
                string path = Server.MapPath("AllAgendaAttachments/");
                while (File.Exists(path + fuAllAgenda.FileName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');</script>", false);
                    return;
                }
                fuAllAgenda.SaveAs(path + fuAllAgenda.FileName);
                MeetingAgendaAttachment q = new MeetingAgendaAttachment();
                q.Attachment_File = "AllAgendaAttachments/" + fuAllAgenda.FileName;
                q.Attachment_Name = txtAllAgendaFileName.Text;
                q.Attachment_MeetingID = int.Parse(Request.QueryString["id"]);
                db.MeetingAgendaAttachments.InsertOnSubmit(q);
                db.SubmitChanges();
                txtAllAgendaFileName.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#allAgendaAttachmentsModal').modal('show');</script>", false);
                BindAllAgendaAttachments();
            }
        }
    }

    protected void lnkDeleteAllAgendaAttachment_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MeetingAgendaAttachment c = db.MeetingAgendaAttachments.Where(x => x.Attachment_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.MeetingAgendaAttachments.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindAllAgendaAttachments();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#allAgendaAttachmentsModal').modal('show');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    #region Discussion Recommendations
    private void BindDiscussionRecommendations()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.DiscussionRecommendations
                        where q.Recommendation_DiscussionID == int.Parse(ViewState["DiscID"].ToString())
                        select new
                        {
                            q.Recommendation_Code,
                            q.Recommendation_Text,
                            q.Recommendation_StatusID,
                            q.Recommendation_Progress,
                            q.Recommendation_ID,
                            q.Status.Status_Name,
                            Implementers = from x in q.RespForImpDiscussionRecommendations
                                           select new
                                           {
                                               x.Member.Member_Name
                                           }
                        };
            rpDiscussionRec.DataSource = query;
            rpDiscussionRec.DataBind();
        }
    }
    private void ClearDiscRecommondationControls()
    {
        ViewState["DiscRecommondationID"] = null;
        btnApproveDiscussionRec.Visible = btnFreezeDiscussionRec.Visible = false;
        btnSaveDiscussionRec.Visible = true;
        txtDiscussionRecName.Text = txtDiscussionRecCode.Text = txtDiscussionRecProgress.Text = string.Empty;
    }
    private void FillDiscRecommondationControl()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            DiscussionRecommendation m = db.DiscussionRecommendations.FirstOrDefault(x => x.Recommendation_ID == int.Parse(ViewState["DiscRecommondationID"].ToString()));
            txtDiscussionRecName.Text = m.Recommendation_Text;
            txtDiscussionRecCode.Text = m.Recommendation_Code;
            txtDiscussionRecProgress.Text = m.Recommendation_Progress.ToString();
            var imp = m.RespForImpDiscussionRecommendations;
            foreach (var i in imp)
            {
                ListItem li = ddlDiscussionRecImplementation.Items.FindByValue(i.ResponsibleForImplementation_MemberID.ToString());
                li.Selected = true;
            }
            btnFreezeDiscussionRec.Visible = btnApproveDiscussionRec.Visible = btnSaveDiscussionRec.Visible = m.Recommendation_StatusID == (int)StatusEnum.UnderApprove;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#DiscussionRecAddModal').modal('show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');</script>", false);
        }
    }
    protected void lnkDiscussionRecommndations_Command(object sender, CommandEventArgs e)
    {
        ViewState["DiscID"] = e.CommandArgument.ToString();
        BindDiscussionRecommendations();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#DiscussionRecModal').modal('show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');</script>", false);
    }

    protected void btnSaveDiscussionRec_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                    if (ViewState["DiscRecommondationID"] == null)
                    {
                        DiscussionRecommendation q = new DiscussionRecommendation()
                        {
                            Recommendation_Text = txtDiscussionRecName.Text,
                            Recommendation_DiscussionID = int.Parse(ViewState["DiscID"].ToString()),
                            Recommendation_Code = txtDiscussionRecCode.Text,
                            Recommendation_Progress = int.Parse(txtDiscussionRecProgress.Text),
                            Recommendation_StatusID = (int)StatusEnum.UnderApprove
                        };
                        db.DiscussionRecommendations.InsertOnSubmit(q);
                        for (int i = 0; i < ddlDiscussionRecImplementation.Items.Count; i++)
                        {
                            if (ddlDiscussionRecImplementation.Items[i].Selected)
                            {
                                RespForImpDiscussionRecommendation r = new RespForImpDiscussionRecommendation();
                                r.DiscussionRecommendation = q;
                                r.ResponsibleForImplementation_MemberID = int.Parse(ddlDiscussionRecImplementation.Items[i].Value);
                                db.RespForImpDiscussionRecommendations.InsertOnSubmit(r);
                            }
                        }
                    }
                    else
                    {
                        DiscussionRecommendation q = db.DiscussionRecommendations.FirstOrDefault(x => x.Recommendation_ID.Equals(int.Parse(ViewState["DiscRecommondationID"].ToString())));
                        q.Recommendation_Text = txtDiscussionRecName.Text;
                        q.Recommendation_Code = txtDiscussionRecCode.Text;
                        q.Recommendation_Progress = int.Parse(txtDiscussionRecProgress.Text);
                        var imp = q.RespForImpDiscussionRecommendations;
                        db.RespForImpDiscussionRecommendations.DeleteAllOnSubmit(imp);
                        for (int i = 0; i < ddlDiscussionRecImplementation.Items.Count; i++)
                        {
                            if (ddlDiscussionRecImplementation.Items[i].Selected)
                            {
                                RespForImpDiscussionRecommendation r = new RespForImpDiscussionRecommendation();
                                r.DiscussionRecommendation = q;
                                r.ResponsibleForImplementation_MemberID = int.Parse(ddlDiscussionRecImplementation.Items[i].Value);
                                db.RespForImpDiscussionRecommendations.InsertOnSubmit(r);
                            }
                        }
                    }
                    db.SubmitChanges();
                    BindDiscussionRecommendations();
                    ClearDiscRecommondationControls();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#RecommondationAddModal').modal('hide');$('.modal-backdrop').remove();$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show');$('#DiscussionRecModal').modal('show');</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }

    protected void lnkAddDiscussionRec_Click(object sender, EventArgs e)
    {
        ClearDiscRecommondationControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#DiscussionRecAddModal').modal('show');$('#agenda').removeClass('active show');$('#agendaPIll').removeClass('active show');$('#discussions').addClass('active show');$('#discussionsPIll').addClass('active show');</script>", false);
    }

    protected void lnkEditDiscussionRec_Command(object sender, CommandEventArgs e)
    {
        ViewState["DiscRecommondationID"] = int.Parse(e.CommandArgument.ToString());
        FillDiscRecommondationControl();
    }

    protected void btnApproveDiscussionRec_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            DiscussionRecommendation c = db.DiscussionRecommendations.Where(x => x.Recommendation_ID == int.Parse(ViewState["DiscRecommondationID"].ToString())).FirstOrDefault();
            try
            {
                c.Recommendation_StatusID = 2;
                db.SubmitChanges();
                FillDiscRecommondationControl();
                BindDiscussionRecommendations();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }

    protected void btnFreezeDiscussionRec_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            DiscussionRecommendation c = db.DiscussionRecommendations.Where(x => x.Recommendation_ID == int.Parse(ViewState["DiscRecommondationID"].ToString())).FirstOrDefault();
            try
            {
                c.Recommendation_StatusID = 3;
                db.SubmitChanges();
                FillDiscRecommondationControl();
                BindDiscussionRecommendations();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
    #endregion

    protected void lnkAgendaDiscussions_Command(object sender, CommandEventArgs e)
    {
        div.Visible = true;
        Session["agenda"] = e.CommandArgument.ToString();
        using (CultureDataContext db = new CultureDataContext())
        {
            hAgenda.InnerText = "المناقشات على بند : " + db.MeetingAgendas.FirstOrDefault(x => x.Agenda_ID == int.Parse(e.CommandArgument.ToString())).Agenda_Item;
        }
        //   GetAgendaDiscussions();
        timer.Enabled = true;
        BindAgendaDiscussions();
    }
    private void BindAgendaDiscussions()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.AgendaDiscussions
                        where q.Discussion_AgendaID == int.Parse(Session["agenda"].ToString())
                        select new
                        {
                            q.Discussion_Date,
                            q.Discussion_Text,
                            q.User.User_FullName,
                            Avatar = q.User.Member.Member_Avatar ?? "App_Themes/images/member.png",
                            q.Discussion_UserID
                        };
            rpAgendaDiscussions.DataSource = query.OrderByDescending(x => x.Discussion_Date);
            rpAgendaDiscussions.DataBind();
        }
    }

    protected void lnkSendMessage_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                AgendaDiscussion q = new AgendaDiscussion()
                {
                    Discussion_Text = txtMessage.Value,
                    Discussion_Date = DateTime.Now,
                    Discussion_AgendaID = int.Parse(Session["agenda"].ToString()),
                    Discussion_UserID = ClientDetails.DeSerializeClientDetails(Session["User"].ToString()).ID
                };
                db.AgendaDiscussions.InsertOnSubmit(q);
                db.SubmitChanges();
                txtMessage.Value = string.Empty;
                BindAgendaDiscussions();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#AgendaDiscussionsModal').modal('show');</script>", false);
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                //Logger.ErrorLog(exception);
            }
        }
    }
    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
    public static dynamic GetAgendaDiscussions()
    {
        string message = string.Empty;
        if (HttpContext.Current.Session["agenda"] != null)
        {
            string conStr = ConfigurationManager.ConnectionStrings["Culture_OneTrackConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                StringBuilder sb = new StringBuilder();
                string query = "SELECT Discussion_Date,Discussion_Text,User_FullName,ISNULL(Member_Avatar,'App_Themes/images/member.png') AS Avatar,Discussion_UserID FROM [dbo].[AgendaDiscussions] join [dbo].[Users] on Discussion_UserID=User_ID left JOIN [dbo].[Members] on Member_ID=User_MemberID where Discussion_AgendaID=" + int.Parse(HttpContext.Current.Session["agenda"].ToString()) + " order by Discussion_ID desc";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Notification = null;
                        SqlDependency dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sb.Append("<div class='" + (ClientDetails.DeSerializeClientDetails(HttpContext.Current.Session["User"].ToString()).ID == int.Parse(dt.Rows[i]["Discussion_UserID"].ToString()) ? "d-flex mb-4 user" : "d-flex mb-4") + "'>");
                            sb.Append("<img class='" + (ClientDetails.DeSerializeClientDetails(HttpContext.Current.Session["User"].ToString()).ID == int.Parse(dt.Rows[i]["Discussion_UserID"].ToString()) ? "avatar-sm rounded-circle ml-3 " : "avatar-sm rounded-circle ml-3 d-none") + "' src='" + dt.Rows[i]["Avatar"] + "' alt='alt'>");
                            sb.Append("<div class='message flex-grow-1'>");
                            sb.Append("<div class='d-flex'>");
                            sb.Append("<p class='mb-1 text-title text-16 flex-grow-1'>" + dt.Rows[i]["User_FullName"] + "</p>");
                            sb.Append("<span class='text-small text-muted'>" + dt.Rows[i]["Discussion_Date"] + "</span>");
                            sb.Append("</div>");
                            sb.Append("<p class='m-0'>" + dt.Rows[i]["Discussion_Text"] + "</p>");
                            sb.Append("</div>");
                            sb.Append("<img class='" + (ClientDetails.DeSerializeClientDetails(HttpContext.Current.Session["User"].ToString()).ID != int.Parse(dt.Rows[i]["Discussion_UserID"].ToString()) ? "avatar-sm rounded-circle ml-3 " : "avatar-sm rounded-circle ml-3 d-none") + "' src='" + dt.Rows[i]["Avatar"] + "' alt='alt'>");
                            sb.Append("</div>");
                        }
                        message = sb.ToString();
                    }
                }
            }
        }
        return message;
    }

    private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
    {
        if (e.Type == SqlNotificationType.Change)
        {
            DiscussionsHub.Show();
        }
    }

    protected void timer_Tick(object sender, EventArgs e)
    {
        BindAgendaDiscussions();
        txtMessage.Focus();
    }
}
