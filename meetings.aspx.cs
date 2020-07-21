using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
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
            using (CultureDataContext db = new CultureDataContext())
            {
                ltrCommitte.Text = db.Committees.FirstOrDefault(x => x.Committee_ID == int.Parse(Request.QueryString["id"])).Committee_Name;
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
                        int code = 1;
                        Meeting c = db.Meetings.Where(x => x.Meeting_CommitteeID == int.Parse(Request.QueryString["id"])).OrderByDescending(x => x.Meeting_ID).FirstOrDefault();
                        if (c != null)
                            code = int.Parse(c.Meeting_Code.Split('-')[2]) + 1;
                        Committee o = db.Committees.FirstOrDefault(x => x.Committee_ID == int.Parse(Request.QueryString["id"]));
                        Meeting q = new Meeting()
                        {
                            Meeting_Date = Convert.ToDateTime(txtDate.Value),
                            Meeting_Code = o.Committee_Code.Split('-')[0]+ code.ToString().PadLeft(3, '0'),
                            Meeting_Place = txtPlace.Text,
                            Meeting_TimeFrom = DateTime.Parse(txtTimeFrom.Value.Trim()).TimeOfDay,
                            Meeting_CommitteeID = int.Parse(Request.QueryString["id"]),
                            Meeting_TimeTo = DateTime.Parse(txtTimeTo.Value.Trim()).TimeOfDay,
                            Meeting_StatusID = (int)StatusEnum.UnderApprove,
                            Meeting_Notes = txtNotes.Text,
                            Meeting_TypeID = rdType1.Checked ? 1 : 2,
                            Meeting_RewardNotMember = decimal.Parse(txtReward.Text),
                            Meeting_RewardMember = decimal.Parse(txtRewardMember.Text)

                        };
                        db.Meetings.InsertOnSubmit(q);
                    }
                    else
                    {
                        Meeting q = db.Meetings.FirstOrDefault(x => x.Meeting_ID.Equals(int.Parse(ViewState["ID"].ToString())));
                        q.Meeting_Date = Convert.ToDateTime(txtDate.Value);
                        q.Meeting_Place = txtPlace.Text;
                        q.Meeting_TimeFrom = DateTime.Parse(txtTimeFrom.Value.Trim()).TimeOfDay;
                        q.Meeting_CommitteeID = int.Parse(Request.QueryString["id"]);
                        q.Meeting_TimeTo = DateTime.Parse(txtTimeTo.Value.Trim()).TimeOfDay;
                        q.Meeting_Notes = txtNotes.Text;
                        q.Meeting_TypeID = rdType1.Checked ? 1 : 2;
                        q.Meeting_RewardNotMember = decimal.Parse(txtReward.Text);
                        q.Meeting_RewardMember = decimal.Parse(txtRewardMember.Text);
                        q.Meeting_PostponedUntil = Convert.ToDateTime(txtDatePostponedUntil.Text);
                    }
                    db.SubmitChanges();
                    ClearControls();
                    BindData();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> $('body').removeClass('modal-open');alert('تم الحفظ بنجاح');$('#meetingModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
                }
                catch (Exception ex)
                {
                    string sPath = HttpContext.Current.Request.Url.AbsolutePath;
                    System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
                    string PageName = oInfo.Name;
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame sf = new StackFrame(0);
                    Common.InsertException(PageName, ex.Message, ex.StackTrace, int.Parse(sf.GetFileLineNumber().ToString()));
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                }
            }
        }
    }
    protected void lnkEditmeeting_Command(object sender, CommandEventArgs e)
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
                var query = db.Meetings.Where(t => t.Meeting_ID.Equals(int.Parse(ViewState["ID"].ToString()))).FirstOrDefault();
                txtDate.Value = Convert.ToDateTime(query.Meeting_Date).ToShortDateString();
                txtCode.Text = query.Meeting_Code;
                txtPlace.Text = query.Meeting_Place;
                txtReward.Text = query.Meeting_RewardNotMember.ToString();
                txtRewardMember.Text = query.Meeting_RewardMember.ToString();

                txtTimeFrom.Value = query.Meeting_TimeFrom.ToString();
                txtTimeTo.Value = query.Meeting_TimeTo.ToString();
                txtDatePostponedUntil.Text = query.Meeting_PostponedUntil != null ? Convert.ToDateTime(query.Meeting_PostponedUntil).ToShortDateString() : string.Empty;
                txtNotes.Text = query.Meeting_Notes;
                divDatePostponedUntil.Visible = query.Meeting_PostponedUntil != null ? true : false;
                btnApprove.Visible = btnSave.Visible = btnFreeze.Visible = btnPostponed.Visible = query.Meeting_StatusID == (int)StatusEnum.UnderApprove;
                rdType1.Checked = query.Meeting_TypeID == 1;
                rdType2.Checked = query.Meeting_TypeID == 2;
                btnSave.Visible = query.Meeting_StatusID == 4 ? true : query.Meeting_StatusID == 1 ? true : false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#meetingModal').modal('show');</script>", false);
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
        txtCode.Text = txtNotes.Text = txtDate.Value = txtPlace.Text = txtDatePostponedUntil.Text = txtReward.Text=txtRewardMember.Text = txtTimeTo.Value = string.Empty;
        rdType1.Checked = true;
        rdType2.Checked = false;
        divDatePostponedUntil.Visible = false;
        txtTimeFrom.Value = string.Empty;
        btnSave.Visible = true;
        btnApprove.Visible = btnFreeze.Visible = btnPostponed.Visible = false;
    }
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Member c = db.Members.Where(x => x.Member_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.Members.DeleteOnSubmit(c);
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
            var query = (from b in db.Meetings
                         where b.Meeting_CommitteeID == int.Parse(Request.QueryString["id"])
                         select new
                         {
                             b.Committee.Committee_Name,
                             b.Meeting_ID,
                             b.Meeting_Date,
                             b.Meeting_Code,
                             b.Meeting_TimeFrom,
                             b.Meeting_TimeTo,
                             b.Meeting_RewardMember,
                             b.Meeting_RewardNotMember,
                             b.Meeting_TypeID,
                             b.Meeting_StatusID,
                             b.Meeting_Place,
                             b.Meeting_Notes,
                             b.MeetingStatus.Status_Name,
                             b.MeetingType.Type_Name
                         });
            dtData = query.OrderByDescending(x => x.Meeting_Date).CopyToDataTable();
            rpData.DataSource = dtData;
            rpData.DataBind();
            ltrCount.Text = dtData.Rows.Count.ToString();
        }
    }
    protected void lnkAddNew_Click(object sender, EventArgs e)
    {
        ClearControls();
        btnFreeze.Visible = btnApprove.Visible = btnPostponed.Visible = false;
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#meetingModal').modal('show');</script>", false);
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Meeting c = db.Meetings.Where(x => x.Meeting_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Meeting_StatusID = 2;
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
            Meeting c = db.Meetings.Where(x => x.Meeting_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Meeting_StatusID = 3;
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
    protected void btnPostponed_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Meeting c = db.Meetings.Where(x => x.Meeting_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Meeting_StatusID = 4;
                db.SubmitChanges();
                FillControls();
                BindData();
                divDatePostponedUntil.Visible = true;
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }
}