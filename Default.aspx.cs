using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
  //                  var dataTable = new DataTable();
  //          string con =
  //@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\Culture\data.xls;" +
  //@"Extended Properties='Excel 8.0;HDR=Yes;'";
  //          using (OleDbConnection connection = new OleDbConnection(con))
  //          {
  //              connection.Open();
  //              OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);
  //              using (OleDbDataReader dr = command.ExecuteReader())
  //              {
  //                  dataTable.Load(dr);
  //              }
  //          }
            //var distinctNames = (from row in dataTable.AsEnumerable()
            //                     select row.Field<double>("كود اللجنة")).Distinct();
            //using (CultureDataContext db = new CultureDataContext())
            //{
            //    foreach (var name in distinctNames)
            //    {
            //        var result = dataTable.AsEnumerable().Where(dr => dr.Field<double>("كود اللجنة") == name).FirstOrDefault();

            //        Committee c = new Committee();
            //        c.Committee_Code = result["كود اللجنة"].ToString();
            //        c.Committee_Name = result["اسم اللجنة"].ToString();
            //        c.Committee_StatusID = 1;
            //        db.Committees.InsertOnSubmit(c);
            //    }
            //    db.SubmitChanges();
            //}
            //using (CultureDataContext db = new CultureDataContext())
            //{
            //    for (int i = 0; i < dataTable.Rows.Count; i++)
            //    {
            //        if (dataTable.Rows[i]["اسم العضو"].ToString().Length > 0)
            //        {
            //            Member c = new Member();
            //            c.Member_CommitteeID = db.Committees.FirstOrDefault(x => x.Committee_Code == dataTable.Rows[i]["كود اللجنة"].ToString()).Committee_ID;
            //            c.Member_RoleID = dataTable.Rows[i]["الصفة"].ToString() == "عضوا بمنصبه" || dataTable.Rows[i]["الصفة"].ToString() == "عضوا" ? 2 : 1;
            //            c.Member_TypeID = db.MemberTypes.FirstOrDefault(x => x.MemberType_Name == dataTable.Rows[i]["النوع"].ToString()).MemberType_ID;
            //            c.Member_Name = dataTable.Rows[i]["اسم العضو"].ToString();
            //            c.Member_Code = dataTable.Rows[i]["كود العضو"].ToString();
            //            c.Member_MembershipStatusID = dataTable.Rows[i]["الصفة"].ToString() == "عضوا بمنصبه" ? 2 : 1;
            //            c.Member_Email = dataTable.Rows[i]["Email"].ToString();
            //            c.Member_NationalID = dataTable.Rows[i]["الرقم القومي"].ToString();
            //            c.Member_Mobile = dataTable.Rows[i][9].ToString();
            //            //c.Member_Mobile = dataTable.Rows[i]["Mobile"].ToString();
            //            c.Member_StatusID = 1;
            //            db.Members.InsertOnSubmit(c);
            //        }
            //    }
            //    db.SubmitChanges();
            //}
        }
    }
}