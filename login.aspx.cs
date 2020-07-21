using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class login : Page
{
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
           Session.Remove("Back");
            if (Session["User"] != null)
            {
                if (Session["Back"] != null)
                    Response.Redirect(Session["Back"].ToString());
                else
                    Response.Redirect("dashboard.aspx");
            }
        }
    }
    protected void lnkSignIn_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = (from q in db.Users
                         where q.User_Email.Equals(txtEmail.Text.Trim())
                         && q.User_Password.Equals(EncryptString.Encrypt(txtPassword.Text))
                         select q).FirstOrDefault();
            if (query != null)
            {
                if (query.User_StatusID == (int)StatusEnum.Approved)
                {
                    Session["User"] = ClientDetails.SerializeClientDetails(new ClientDetails(query.User_ID, query.User_FullName, query.User_Email, query.User_Mobile1,db.GroupUsers.FirstOrDefault(x => x.GroupUser_UserID.Equals(query.User_ID)).GroupUser_GroupID ?? 0 ,query.User_CommitteeID??0));
                    var per = db.SP_SelectUserPermission(query.User_ID).ToList();
                    List<UserPermissions> tempList = UserPermissions;
                    tempList.AddRange(
                        per.Select(
                            i =>
                    new UserPermissions(i.ModuleID, i.ModuleName, i.PageName, i.PageURL,
                        bool.Parse(i.Show), bool.Parse(i.Add), bool.Parse(i.Edit), bool.Parse(i.Delete), bool.Parse(i.Index))));
                    UserPermissions = tempList;
                    Response.Redirect("default.aspx");
                }
            }
            else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alertUser",
                    "alert('عفوا، المستخدم غير موجود');", true);
        }
    }
}