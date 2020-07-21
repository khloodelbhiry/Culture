using System;
using System.Collections.Generic;
using System.Data;
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
                            p.PageUrl.ToLower().Equals(Common.Rewards) &&
                            (p.Show.Equals(true) || p.Add.Equals(true) || p.Edit.Equals(true) || p.Delete.Equals(true))))
                {
                    var per = UserPermissions.FirstOrDefault(
                        p =>
                            p.PageUrl.ToLower().Equals(Common.Rewards) &&
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
            BindDDl();
            BindData();
        }
    }
    private void BindDDl()
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            var query = db.PaymentStatus.Select(x => new
            {
                x.PaymentStatus_ID,
                x.PaymentStatus_Name
            });
            ddlStatusSrc.DataSource = query;
            ddlStatusSrc.DataTextField = "PaymentStatus_Name";
            ddlStatusSrc.DataValueField = "PaymentStatus_ID";
            ddlStatusSrc.DataBind();
            ddlStatusSrc.Items.Insert(0, new ListItem("اختر", "0"));

            var query2 = from m in db.Members
                         select new
                         { m.Member_ID, m.Member_Name };
            ddlMember.DataSource = query2;
            ddlMember.DataTextField = "Member_Name";
            ddlMember.DataValueField = "Member_ID";
            ddlMember.DataBind();
            ddlMember.Items.Insert(0, new ListItem("اختر", "0"));
        }
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        ViewState["ID"] = null;
    }
    private void ClearControls()
    {
        ViewState["ID"] = null;
       txtValue.Text = string.Empty;
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
            var query = (from b in db.Rewards
                         select new
                         {
                             b.Reward_Date,
                             b.Reward_ID,
                             Reward_IsException=  b.Reward_IsException??false,
                             b.Member.Member_Name,
                             b.Reward_PaymentDate,
                             b.Reward_PaymentStatusID,
                             b.Reward_Value,
                             b.PaymentStatus.PaymentStatus_Name,
                             b.Member.Committee.Committee_Name
                         });
            if (txtDateFromSrc.Text.Trim() != string.Empty)
            {
                query = query.Where(x => x.Reward_Date >= DateTime.Parse(txtDateFromSrc.Text));
            }
            if (txtDateToSrc.Text.Trim() != string.Empty)
            {
                query = query.Where(x => x.Reward_Date <= DateTime.Parse(txtDateToSrc.Text));
            }
            if (txtMemberSrc.Text.Trim() != string.Empty)
            {
                query = query.Where(x => x.Member_Name.StartsWith(txtMemberSrc.Text.Trim()));
            }
            if (ddlStatusSrc.SelectedValue != "0")
            {
                query = query.Where(x => x.Reward_PaymentStatusID == int.Parse(ddlStatusSrc.SelectedValue));
            }
            if (chkException.Checked)
            {
                query = query.Where(x => x.Reward_IsException == true);
            }
            dtData = query.OrderByDescending(x => x.Reward_Date).CopyToDataTable();
            rpRewards.DataSource = dtData;
            rpRewards.DataBind();
            ltrCount.Text = dtData.Rows.Count.ToString();
        }
    }
    protected void LnkSearch_Click(object sender, EventArgs e)
    {
        BindData();
    }
    protected void LnkNewSearch_Click(object sender, EventArgs e)
    {
         txtDateFromSrc.Text = txtDateToSrc.Text=txtMemberSrc.Text = string.Empty;
        ddlStatusSrc.SelectedValue = "0";
        chkException.Checked =false;
        BindData();

    }
    protected void lnkAddNew_Click(object sender, EventArgs e)
    {
        ClearControls();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#rewardAddModal').modal('show');</script>", false);
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
            var query = from q in db.RewardAttachments
                        where q.Attachment_RewardID == int.Parse(ViewState["ID"].ToString())
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
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('#attachmentsAddModal').modal('show');</script>", false);
    }

    protected void btnSaveAttachment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            if (fuAttach.HasFile)
            {
                string path = Server.MapPath("RewardsAttachments/");
                while (File.Exists(path + fuAttach.FileName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('يوجد مرفق بنفس الاسم');</script>", false);
                    return;
                }
                fuAttach.SaveAs(path + fuAttach.FileName);
               RewardAttachment q = new RewardAttachment();
                q.Attachment_File = "RewardsAttachments/" + fuAttach.FileName;
                q.Attachment_Name = txtFileName.Text;
                q.Attachment_RewardID = int.Parse(ViewState["ID"].ToString());
                db.RewardAttachments.InsertOnSubmit(q);
                db.SubmitChanges();
                txtFileName.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> $('body').removeClass('modal-open');alert('تم الحفظ بنجاح');$('#attachmentsModal').modal('show');</script>", false);
                BindAttachments();
            }
        }
    }

    protected void lnkDeleteAttachment_Command(object sender, CommandEventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            RewardAttachment c = db.RewardAttachments.Where(x => x.Attachment_ID == int.Parse(e.CommandArgument.ToString())).FirstOrDefault();
            try
            {
                db.RewardAttachments.DeleteOnSubmit(c);
                db.SubmitChanges();
                BindData();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطأ اثناء الحذف'); </script>", false);
            }
        }
    }

    protected void btnSaveReward_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            using (CultureDataContext db = new CultureDataContext())
            {
                try
                {
                        Reward q = new Reward()
                        {
                            Reward_Value =decimal.Parse(txtValue.Text),
                            Reward_Date = DateTime.Now,
                            Reward_IsException=true,
                            Reward_PaymentStatusID=1,
                            Reward_MemberID=int.Parse(ddlMember.SelectedValue)
                        };
                        db.Rewards.InsertOnSubmit(q);
                    db.SubmitChanges();
                    BindData();
                    txtValue.Text = string.Empty;
                    ddlMember.SelectedValue = "0";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'>$('body').removeClass('modal-open'); alert('تم الحفظ بنجاح');$('#rewardAddModal').modal('hide');$('.modal-backdrop').remove();</script>", false);
                }
                catch (Exception exception)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                    //Logger.ErrorLog(exception);
                }
            }
        }
    }
    protected void lnkPayment_Click(object sender, EventArgs e)
    {
        using (CultureDataContext db = new CultureDataContext())
        {
            try
            {
                int count = 0;
                foreach (RepeaterItem item in rpRewards.Items)
                {
                    HtmlInputCheckBox chkDisplayTitle = (HtmlInputCheckBox)item.FindControl("check");
                    if (chkDisplayTitle.Checked)
                    {
                        Reward a = db.Rewards.FirstOrDefault(x => x.Reward_ID == int.Parse(chkDisplayTitle.Value));
                        a.Reward_PaymentStatusID = 2;
                        a.Reward_PaymentDate = DateTime.Now;
                        db.SubmitChanges();
                        count++;
                    }
                }
                if (count == 0)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> $('body').removeClass('modal-open');alert('يرجى تحديد المكافأت');</script>", false);
                db.SubmitChanges();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> $('body').removeClass('modal-open');alert('تم الحفظ بنجاح');</script>", false);
                BindData();
            }
            catch (Exception exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "<script language='javascript'> alert('حدث خطا اثناء الحفظ');</script>", false);
                //Logger.ErrorLog(exception);
            }
        }
    }
}