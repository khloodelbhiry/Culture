using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Common
/// </summary>
/// 
public enum FunctionsEnum
{
    Show = 1,
    Add = 2,
    Edit = 3,
    Delete = 4,
    Index=5
}
public enum StatusEnum
{
    UnderApprove = 1,
    Approved = 2,
    Freezed = 3
}
public enum ModulesEnum
{
    Committees = 1,
    CommitteeWorks = 2,
    Financial = 3,
    Security=4
}
public class Common
{
    public const string Committees = "committee.aspx";
    public const string Meetings = "meetings.aspx";
    public const string Groups = "groups.aspx";
    public const string Settings = "settings.aspx";
    public const string Users = "users.aspx";
    public const string Members = "members.aspx";
    public const string Rewards = "rewards.aspx";
    public static void InsertException(string exceptionPageName, string exceptionMessage, string exceptionFuncation,
    int exceptionLineNumber)
    {
        var db = new CultureDataContext();
        var tblLogException = new LogException();
        tblLogException.Excep_Date = DateTime.Now;
        tblLogException.Excep_Message = exceptionMessage;
        tblLogException.Excep_Page_Name = exceptionPageName;
        tblLogException.Excep_Fun_Name = exceptionFuncation;
        tblLogException.Excep_Line_Number = exceptionLineNumber;
        db.LogExceptions.InsertOnSubmit(tblLogException);
        db.SubmitChanges();
        db.Dispose();
    }
    public Common()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}