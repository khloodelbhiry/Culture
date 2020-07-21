using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            BindMeetings();
            BindGoals();
            BindAwards();
            BindMembers();
        }
    }
    private void BindMembers()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from b in db.Members
                         where b.Member_CommitteeID==int.Parse(Request.QueryString["id"])
                         select new
                         {
                             b.Member_CommitteeID,
                             b.Member_ID,
                             b.Member_Name,
                             b.Member_Notes,
                             b.Status.Status_Name,
                             b.Member_Address,
                             b.Member_StatusID,
                             b.Member_As,
                             b.Member_Code,
                             b.Member_Email,
                             b.Member_Mobile,
                             b.Role.Role_Name,
                             b.MembershipStatus.MembershipStatus_Name,
                             Member_Avatar = b.Member_Avatar ?? "App_Themes/images/member.png",
                             b.MemberType.MemberType_Name,
                             b.Member_MembershipStatusID,
                             b.Member_RoleID,
                             b.Member_TypeID,
                             Rewards = b.Rewards.Any() ? b.Rewards.Sum(x => x.Reward_Value ?? 0) : 0,
                             Attendance = b.Committee.Meetings.Count() > 0 ? ((Convert.ToDecimal(b.Attendances.Count()) / Convert.ToDecimal(b.Committee.Meetings.Count())) * decimal.Parse("100")) : 0
                         });
            rpMembers.DataSource = query;
            rpMembers.DataBind();
        }
    }
    private void BindData()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from b in db.Committees
                         where b.Committee_ID == int.Parse(Request.QueryString["id"])
                         select new
                         {
                             b.Committee_ID,
                             b.Committee_Name,
                             b.Committee_Code,
                             b.Status.Status_Name,
                             b.Committee_StatusID,
                             b.Committee_About,
                             AttachmentCount = b.CommitteeAttachments.Count(),
                             MembersCount = b.Members.Count(),
                             GoalCount = b.CommitteeGoals.Count(),
                             MeetingsCount = b.Meetings.Count(),
                             Attendance = b.Meetings.Count() > 0 ? ((Convert.ToDecimal(b.Meetings.Sum(x => x.Attendances.Count())) / Convert.ToDecimal(b.Meetings.Count() * b.Members.Count())) * decimal.Parse("100")) : 0
                         });
            dtData = query.CopyToDataTable();
            rpData.DataSource = dtData;
            rpData.DataBind();
        }
    }
    private void BindMeetings()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from b in db.Meetings
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
                        };
            rpMeetings.DataSource = query.OrderByDescending(x=>x.Meeting_Date);
            rpMeetings.DataBind();
        }
    }
    private void BindAwards()
    {
        using(CultureDataContext db=new CultureDataContext())
        {
            var query = from b in db.Rewards
                        where b.Member.Member_CommitteeID == int.Parse(Request.QueryString["id"])
                        select new
                        {
                            b.Reward_Date,
                            b.Reward_ID,
                            Reward_IsException = b.Reward_IsException ?? false,
                            b.Member.Member_Name,
                            b.Reward_PaymentDate,
                            b.Reward_PaymentStatusID,
                            b.Reward_Value,
                            b.PaymentStatus.PaymentStatus_Name,
                            b.Member.Committee.Committee_Name
                        };
            rpFinancial.DataSource = query;
            rpFinancial.DataBind();
        }
    }
    private void BindGoals()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.CommitteeGoals
                        where q.Goal_CommitteeID == int.Parse(Request.QueryString["id"])
                        select new
                        {
                            q.Goal_ID,
                            q.Goal_Text
                        };
            rpGoals.DataSource = query;
            rpGoals.DataBind();
        }
    }
}