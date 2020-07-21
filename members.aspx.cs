using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
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
    protected string firstPhone { get; set; }
    protected string firstWhatsApp { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["User"] != null && Session["User"].ToString() != string.Empty)
            {
                if (Request.QueryString["id"] == null)
                {
                    if (UserPermissions.Any(
                            p =>
                                p.PageUrl.ToLower().Equals(Common.Members) &&
                                 p.ModuleID == (int)ModulesEnum.Committees &&
                                (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                    {
                        var per = UserPermissions.FirstOrDefault(
                            p =>
                                p.PageUrl.ToLower().Equals(Common.Members) &&
                                 p.ModuleID == (int)ModulesEnum.Committees &&
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
                    if (UserPermissions.Any(
                            p =>
                                p.PageUrl.ToLower().Equals(Common.Members) &&
                                p.ModuleID == (int)ModulesEnum.CommitteeWorks &&
                                (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                    {
                        var per = UserPermissions.FirstOrDefault(
                            p =>
                                p.PageUrl.ToLower().Equals(Common.Members) &&
                                p.ModuleID == (int)ModulesEnum.CommitteeWorks &&
                                (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true)));
                        ((HtmlGenericControl)Page.Master.FindControl("ulBreadcrumb")).InnerHtml = "<li><i class='ace-icon fa fa-home home-icon'></i><a href ='Dashboard.aspx'> الرئيسية </a></li><li>" + per.ModuleName + "</li><li>" + per.PageName + "</li><li></li>";
                        Page.Title = per.PageName;
                    }
                    else
                    {
                        Response.Redirect("NoPermission.aspx");
                    }
                }
            }
            else
            {
                Session["back"] = Request.Url.AbsoluteUri;
                Response.Redirect("Login.aspx?ReturnURL=" + Request.Url.AbsolutePath);
            }
            if (Request.QueryString["id"] != null)
            {
                using (CultureDataContext db = new CultureDataContext())
                {
                    ltrCommitte.Text = db.Committees.FirstOrDefault(x => x.Committee_ID == int.Parse(Request.QueryString["id"])).Committee_Name;
                }
                divSearch.Visible = false;
            }
            lnkAddNew.Visible = Request.QueryString["id"] != null;
            BindDDL();
            BindData();
        }
    }
    private void BindDDL()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.Committees
                        select new
                        {
                            q.Committee_ID,
                            q.Committee_Name
                        };
            ddlCommittee.DataSource = query;
            ddlCommittee.DataTextField = "Committee_Name";
            ddlCommittee.DataValueField = "Committee_ID";
            ddlCommittee.DataBind();
            ddlCommittee.Items.Insert(0, new ListItem("-- اختر --", "0"));

            ddlMembershipSrc.DataSource = db.MembershipStatus;
            ddlMembershipSrc.DataTextField = "MembershipStatus_Name";
            ddlMembershipSrc.DataValueField = "MembershipStatus_ID";
            ddlMembershipSrc.DataBind();
            ddlMembershipSrc.Items.Insert(0, new ListItem("-- اختر --", "0"));

            ddlRoleSrc.DataSource = db.Roles;
            ddlRoleSrc.DataTextField = "Role_Name";
            ddlRoleSrc.DataValueField = "Role_ID";
            ddlRoleSrc.DataBind();
            ddlRoleSrc.Items.Insert(0, new ListItem("-- اختر --", "0"));

            ddlTypeSrc.DataSource = db.MemberTypes;
            ddlTypeSrc.DataTextField = "MemberType_Name";
            ddlTypeSrc.DataValueField = "MemberType_ID";
            ddlTypeSrc.DataBind();
            ddlTypeSrc.Items.Insert(0, new ListItem("-- اختر --", "0"));
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
                    string avatar = string.Empty;
                    if (fuAvatar.HasFile)
                    {
                        string path = Server.MapPath("MembersAttachments/");
                        while (File.Exists(path + fuAvatar.FileName))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');</script>", false);
                            return;
                        }
                        fuAvatar.SaveAs(path + fuAvatar.FileName);
                        avatar = "MembersAttachments/" + fuAvatar.FileName;
                    }
                    if (ViewState["ID"] == null)
                    {
                        //int code = 1;
                        //Member c = db.Members.Where(x=>x.Member_CommitteeID== int.Parse(Request.QueryString["id"])).OrderByDescending(x => x.Member_ID).FirstOrDefault();
                        //if (c != null)
                        //    code = int.Parse(c.Member_Code.Split('-')[2]) + 1;
                        //Committee o = db.Committees.FirstOrDefault(x => x.Committee_ID == int.Parse(Request.QueryString["id"]));
                        int code = 100;
                        Member c = db.Members.Where(x => x.Member_CommitteeID == int.Parse(Request.QueryString["id"])).OrderByDescending(x => x.Member_ID).FirstOrDefault();
                        if (c != null)
                            code = int.Parse(c.Member_Code.Split('-')[1]) + 10;
                        Committee o = db.Committees.FirstOrDefault(x => x.Committee_ID == int.Parse(Request.QueryString["id"]));
                        Member q = new Member()
                        {
                            Member_Name = txtName.Text,
                            Member_Code = o.Committee_Code +"-"+ code.ToString().PadLeft(3, '0'),
                            Member_Address = txtAddress.Text,
                            Member_As = txtAs.Text,
                            Member_CommitteeID = int.Parse(Request.QueryString["id"]),
                            Member_Email = txtEmail.Text,
                            Member_MembershipStatusID = rdStatus1.Checked ? 1 : 2,
                            Member_Notes = txtNotes.Text,
                            Member_RoleID = rdRole1.Checked ? 1 : 2,
                            Member_StatusID = (int)StatusEnum.UnderApprove,
                            Member_Avatar = fuAvatar.HasFile ? avatar : null,
                            Member_TypeID = rdMale.Checked ? int.Parse(rdMale.Value) : (rdFemale.Checked ? int.Parse(rdFemale.Value) : int.Parse(rdUnit.Value))
                        };
                        db.Members.InsertOnSubmit(q);
                        SavePhone(db, q);
                        if (chkEmail.Checked)
                            AddNotification(1, q, db);
                        if (chkSMS.Checked)
                            AddNotification(2, q, db);
                        if (chkWhatsAppNotification.Checked)
                            AddNotification(3, q, db);
                    }
                    else
                    {
                        Member q = db.Members.FirstOrDefault(x => x.Member_ID.Equals(int.Parse(ViewState["ID"].ToString())));
                        q.Member_Name = txtName.Text;
                        q.Member_Address = txtAddress.Text;
                        q.Member_As = txtAs.Text;
                        q.Member_CommitteeID = int.Parse(Request.QueryString["id"]);
                        q.Member_Email = txtEmail.Text;
                        q.Member_MembershipStatusID = rdStatus1.Checked ? 1 : 2;
                        q.Member_Notes = txtNotes.Text;
                        q.Member_RoleID = rdRole1.Checked ? 1 : 2;
                        q.Member_StatusID = (int)StatusEnum.UnderApprove;
                        q.Member_TypeID = rdMale.Checked ? int.Parse(rdMale.Value) : (rdFemale.Checked ? int.Parse(rdFemale.Value) : int.Parse(rdUnit.Value));
                        if (fuAvatar.HasFile)
                            q.Member_Avatar = avatar;
                        var query = db.MemberNotifications.Where(x => x.MemberNotification_MemberID == int.Parse(ViewState["ID"].ToString()));
                        db.MemberNotifications.DeleteAllOnSubmit(query);
                        if (chkEmail.Checked)
                            AddNotification(1, q, db);
                        if (chkSMS.Checked)
                            AddNotification(2, q, db);
                        if (chkWhatsAppNotification.Checked)
                            AddNotification(3, q, db);
                        if (q.Users.Any())
                        {
                            User u = q.Users.FirstOrDefault();
                            u.User_FullName = txtName.Text;
                            u.User_Email = txtEmail.Text;
                        }
                        var phone = q.MemberPhones;
                        db.MemberPhones.DeleteAllOnSubmit(phone);
                        SavePhone(db, q);
                    }
                    db.SubmitChanges();
                    ClearControls();
                    BindData();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#memberModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    private void SavePhone(CultureDataContext db,Member q)
    {
        string[] checkedCheckBoxes = (Request.Form["chkWhatsapp"] ?? string.Empty).ToString().Split(',');
        for (int i = 1; i <= int.Parse(hdfIndex.Value); i++)
        {
            var val = Request.Form["phone[" + i.ToString() + "]"];
            if (val != null && val != string.Empty)
            {
                MemberPhone p = new MemberPhone();
                p.Member = q;
                p.MemberPhone_Number = val;
                p.MemberPhone_WhatsApp = checkedCheckBoxes.Any(s => s.Contains(i.ToString()));
                db.MemberPhones.InsertOnSubmit(p);
            }
        }
    }
    private void AddNotification(int type, Member member, CultureDataContext db)
    {
        MemberNotification n = new MemberNotification();
        n.Member = member;
        n.MemberNotification_NotificationTypeID = type;
        db.MemberNotifications.InsertOnSubmit(n);
        db.SubmitChanges();
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
                var query = db.Members.Where(t => t.Member_ID.Equals(int.Parse(ViewState["ID"].ToString()))).FirstOrDefault();
                txtName.Text = query.Member_Name;
                txtCode.Text = query.Member_Code;
                txtAddress.Text = query.Member_Address;
                txtAs.Text = query.Member_As;
                txtEmail.Text = query.Member_Email;
                rdStatus1.Checked = query.Member_MembershipStatusID == 1;
                rdStatus2.Checked = query.Member_MembershipStatusID == 2;
                txtNotes.Text = query.Member_Notes;
                rdRole1.Checked = query.Member_RoleID == 1;
                rdRole2.Checked = query.Member_RoleID == 2;
                rdMale.Checked = query.Member_TypeID == 1;
                rdFemale.Checked = query.Member_TypeID == 2;
                rdUnit.Checked = query.Member_TypeID == 3;
                divAs.Visible = query.Member_MembershipStatusID == 2;
                btnApprove.Visible = btnSave.Visible = query.Member_StatusID == (int)StatusEnum.UnderApprove;
                var not = query.MemberNotifications;
                foreach (var i in not)
                {
                    if (i.MemberNotification_NotificationTypeID == 1)
                        chkEmail.Checked = true;
                    else if (i.MemberNotification_NotificationTypeID == 2)
                        chkSMS.Checked = true;
                    else if (i.MemberNotification_NotificationTypeID == 3)
                        chkWhatsAppNotification.Checked = true;
                }
                var phones = query.MemberPhones;
                hdfIndex.Value = phones.Count().ToString();
                StringBuilder sb = new StringBuilder();
                int index = 0;
                foreach (var p in phones)
                {
                    if (index == 0)
                    {
                        firstPhone = p.MemberPhone_Number;
                        firstWhatsApp = (p.MemberPhone_WhatsApp ?? false) ? "checked" : string.Empty;
                    }
                    else
                    {
                        sb.Append("$('.phone-list').append('");
                        sb.Append(@"<div class=""input-group phone-input"">");
                        sb.Append(@"<input type=""text"" name=""phone[" + (index + 1) + @"]"" class=""form-control"" value="""+p.MemberPhone_Number+@""" />");
                        sb.Append(@"<span class=""input-group-btn"">");
                        sb.Append(@"<label class=""checkbox checkbox-success mt-2"">");
                        sb.Append(@"<input type=""checkbox"" name=""chkWhatsapp"" value=""[" + (index + 1) + @"]"" "+ ((p.MemberPhone_WhatsApp ?? false) ? "checked" : string.Empty)+ @"><span> واتس اب </span><span class=""checkmark ml-1""></span>");
                        sb.Append(@"</label>");
                        sb.Append(@"</span>");
                        sb.Append(@"<span class=""input -group-btn mt-2 ml-2"">");
                        sb.Append(@"<a class=""text-danger"" onclick=""removePhone(this)""><i class=""nav-icon i-Close-Window font-weight-bold""></i></a>");
                        sb.Append(@"</span>");
                        sb.Append(@"</div>");
                        sb.Append(@"');");
                    }
                    index++;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>" + sb.ToString() + "$('#memberModal').modal('show');</script>", false);
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
        txtCode.Text = txtNotes.Text = txtName.Text = txtAddress.Text = txtAs.Text = txtEmail.Text =string.Empty;
        btnSave.Visible = true;
        btnFreeze.Visible = btnApprove.Visible = false;
         rdStatus2.Checked = chkEmail.Checked = chkSMS.Checked = chkWhatsAppNotification.Checked = false;
        rdStatus1.Checked = rdMale.Checked = true;
        divAs.Visible = false;
        rdFemale.Checked = rdUnit.Checked = false;
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
            var query = (from b in db.Members
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
                             Member_Phone=string.Empty,//b.Member_Phone,
                             Member_WhatsApp =false,// b.Member_WhatsApp ?? false,
                             b.Role.Role_Name,
                             b.MembershipStatus.MembershipStatus_Name,
                             Member_Avatar = b.Member_Avatar ?? "App_Themes/images/member.png",
                             b.MemberType.MemberType_Name,
                             b.Member_MembershipStatusID,
                             b.Member_RoleID,
                             b.Member_TypeID,
                             Rewards = b.Rewards.Any() ? b.Rewards.Sum(x => x.Reward_Value ?? 0) : 0,
                             Attendance = b.Committee.Meetings.Count() > 0 ? ((Convert.ToDecimal(b.Attendances.Count()) / Convert.ToDecimal(b.Committee.Meetings.Count())) * decimal.Parse("100")) : 0,
                             phones = from x in b.MemberPhones
                                      select new
                                      {
                                          x.MemberPhone_Number,
                                          MemberPhone_WhatsApp = x.MemberPhone_WhatsApp ?? false
                                      }
                         });
            if (Request.QueryString["id"] != null)
            {
                ddlCommittee.SelectedValue = Request.QueryString["id"];
                ddlCommittee.Enabled = false;
            }
            if (ddlCommittee.SelectedValue != "0")
                query = query.Where(x => x.Member_CommitteeID == int.Parse(ddlCommittee.SelectedValue));
            if (ddlMembershipSrc.SelectedValue != "0")
                query = query.Where(x => x.Member_MembershipStatusID == int.Parse(ddlMembershipSrc.SelectedValue));
            if (ddlRoleSrc.SelectedValue != "0")
                query = query.Where(x => x.Member_RoleID == int.Parse(ddlRoleSrc.SelectedValue));
            if (ddlTypeSrc.SelectedValue != "0")
                query = query.Where(x => x.Member_TypeID == int.Parse(ddlTypeSrc.SelectedValue));
            if (txtMobileSrc.Text.Trim() != string.Empty)
                query = query.Where(x => x.phones.Any(u=>u.MemberPhone_Number == txtMobileSrc.Text.Trim()));
            if (txtNameSrc.Text.Trim() != string.Empty)
                query = query.Where(x => x.Member_Name.Contains(txtNameSrc.Text.Trim()));
            rpData.DataSource =query.OrderBy(x => x.Member_Code);
            rpData.DataBind();
            ltrCount.Text = query.Count().ToString();
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindData();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#memberModal').modal('show');</script>", false);
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            Member c = db.Members.Where(x => x.Member_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Member_StatusID = (int)StatusEnum.Approved;
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
            Member c = db.Members.Where(x => x.Member_ID == int.Parse(ViewState["ID"].ToString())).FirstOrDefault();
            try
            {
                c.Member_StatusID = (int)StatusEnum.Freezed;
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
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#memberModal').modal('show');</script>", false);
    }
    protected void lnkAwards_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = e.CommandArgument.ToString();
        BindAwards();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#awardsModal').modal('show');</script>", false);
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
            var query = from q in db.MemberAttachments
                        where q.Attachment_MemberID == int.Parse(ViewState["ID"].ToString())
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

    protected void lnkEdit_Command(object sender, CommandEventArgs e)
    {
        ViewState["ID"] = e.CommandArgument.ToString();
        FillControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#memberModal').modal('show');</script>", false);
    }

    private void BindAwards()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = from q in db.Rewards
                        where q.Reward_MemberID == int.Parse(ViewState["ID"].ToString())
                        select new
                        {
                            q.Reward_ID,
                            Reward_IsException = q.Reward_IsException ?? false,
                            q.Reward_PaymentDate,
                            q.Reward_Value,
                            q.PaymentStatus.PaymentStatus_Name
                        };
            rpRewards.DataSource = query;
            rpRewards.DataBind();
        }
    }

    protected void lnkDeleteAttachment_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            MemberAttachment c = db.MemberAttachments.Where(x => x.Attachment_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            db.MemberAttachments.DeleteOnSubmit(c);
            db.SubmitChanges();
            try
            {
                if (File.Exists(Server.MapPath(c.Attachment_File)))
                    File.Delete(Server.MapPath(c.Attachment_File));
            }
            catch (Exception exception)
            {
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحذف بنجاح');$('#attachmentsModal').modal('show');</script>", false);
            BindAttachments();
        }
    }

    protected void lnkAddAttachment_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#attachmentsAddModal').modal('show');</script>", false);
    }

    protected void btnSaveAttachment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (fuAttach.HasFile)
            {
                string path = Server.MapPath("MembersAttachments/");
                while (File.Exists(path + fuAttach.FileName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');</script>", false);
                    return;
                }
                fuAttach.SaveAs(path + fuAttach.FileName);
                MemberAttachment q = new MemberAttachment();
                q.Attachment_File = "MembersAttachments/" + fuAttach.FileName;
                q.Attachment_Name = txtFileName.Text; ;
                q.Attachment_MemberID = int.Parse(ViewState["ID"].ToString());
                db.MemberAttachments.InsertOnSubmit(q);
                db.SubmitChanges();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('تم الحفظ بنجاح');$('#attachmentsModal').modal('show');</script>", false);
                BindAttachments();
            }
        }
    }

    protected void btnNewSearch_Click(object sender, EventArgs e)
    {
        txtNameSrc.Text = txtMobileSrc.Text = string.Empty;
        ddlCommittee.SelectedValue = ddlMembershipSrc.SelectedValue = ddlRoleSrc.SelectedValue = ddlTypeSrc.SelectedValue = "0";
        BindData();
    }
}