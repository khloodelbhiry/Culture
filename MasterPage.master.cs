using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
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
            if (Session["User"] != null)
                ltUsername.Text = ClientDetails.DeSerializeClientDetails(Session["User"].ToString()).UserName;
            else
                Response.Redirect("login.aspx");
        }
    }

    protected void lnkLogout_ServerClick(object sender, EventArgs e)
    {
        Session.Remove("User");
        Session.Remove("UserPermissions");
        Response.Redirect("login.aspx");
    }

    protected void lnkChangePassword_ServerClick(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#changePasswordModal').modal('show');</script>", false);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            User u = db.Users.FirstOrDefault(x => x.User_ID == ClientDetails.DeSerializeClientDetails(Session["User"].ToString()).ID);
            if (u != null)
            {
                if (u.User_Password == EncryptString.Encrypt(txtOldPassword.Text))
                {
                    u.User_Password = EncryptString.Encrypt(txtNewPassword.Text);
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertUser", "$('body').removeClass('modal-open');alert('تم الحفظ بنجاح');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertUser", "alert('عفوا، كلمة السر القديمة غير صحيحة');$('#changePasswordModal').modal('show');", true);
                }
            }
        }
    }
}
