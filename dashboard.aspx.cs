using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class dashboard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            BindData();
            BindCommittees();
        }
    }
    private void BindData()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query1 = db.Rewards;
            ltrAwards.Text = query1.Sum(x => x.Reward_Value).Value.ToString("G29");
            ltrPaied.Text = ((decimal?)query1.Where(x=>x.Reward_PaymentDate!=null).Sum(x =>x.Reward_Value)??0).ToString("G29");
            ltrRemain.Text = query1.Where(x=>x.Reward_PaymentDate==null).Sum(x => x.Reward_Value).Value.ToString("G29");
            ltrMeetings.Text = db.Meetings.Count().ToString();
            var query = db.Members;
            ltrFemale.Text = query.Where(x => x.Member_TypeID == 2).Count().ToString();
            ltrMale.Text = query.Where(x => x.Member_TypeID == 1).Count().ToString();
            ltrUnits.Text = query.Where(x => x.Member_TypeID == 3).Count().ToString();
            ltrUsers.Text = query.Where(x => x.Member_MembershipStatusID == 1).Count().ToString();
            ltrUsersAs.Text = query.Where(x => x.Member_MembershipStatusID == 2).Count().ToString();
        }
    }
            private void BindCommittees()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from b in db.Committees
                         select new
                         {
                             b.Committee_ID,
                             b.Committee_Name,
                             MembersCount = b.Members.Count(),
                             Attendance = b.Meetings.Count() > 0 ? ((Convert.ToDecimal(b.Meetings.Sum(x => x.Attendances.Count())) / Convert.ToDecimal(b.Meetings.Count() * b.Members.Count())) * decimal.Parse("100")) : 0
                         });
            rpCommittees.DataSource = query;
            rpCommittees.DataBind();
            ltrCommittees.Text = query.Count().ToString();
        }
    }
}