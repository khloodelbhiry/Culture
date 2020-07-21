using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for UserPermissions
/// </summary>
public class UserPermissions
{
    public UserPermissions()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private int _moduleID;
    private string _moduleName;
    private string _pageName;
    private string _pageUrl;
    private bool _show;
    private bool _add;
    private bool _edit;
    private bool _delete;
    private bool _index;

    #region Public Variables
    public int ModuleID
    {
        get { return _moduleID; }
        set { _moduleID = value; }
    }
    public string ModuleName
    {
        get { return _moduleName; }
        set { _moduleName = value; }
    }
    public string PageName
    {
        get { return _pageName; }
        set { _pageName = value; }
    }
    public string PageUrl
    {
        get { return _pageUrl; }
        set { _pageUrl = value; }
    }
    public bool Show
    {
        get { return _show; }
        set { _show = value; }
    }
    public bool Add
    {
        get { return _add; }
        set { _add = value; }
    }
    public bool Edit
    {
        get { return _edit; }
        set { _edit = value; }
    }
    public bool Delete
    {
        get { return _delete; }
        set { _delete = value; }
    }
    public bool Index
    {
        get { return _index; }
        set { _index = value; }
    }
    #endregion
    public UserPermissions(int moduleID,string moduleName, string pageName, string pageUrl,bool show, bool add, bool edit, bool delete,bool index)
    {
        _moduleID = moduleID;
        _moduleName = moduleName;
        _pageName = pageName;
        _pageUrl = pageUrl;
        _show = show;
        _add = add;
        _edit = edit;
        _delete = delete;
        _index = index;
    }
    private static string SerializePermissions(UserPermissions up)
    {
        StringBuilder value = new StringBuilder();
        value.Append(up.ModuleID);
        value.Append(";" + up.ModuleName);
        value.Append(";" + up.PageName);
        value.Append(";" + up.PageUrl);
        value.Append(";" + up.Show);
        value.Append(";" + up.Add);
        value.Append(";" + up.Edit);
        value.Append(";" + up.Delete);
        value.Append(";" + up.Index);
        return value.ToString();
    }
    private static UserPermissions DeSerializePermissions(string up)
    {
        string[] details = up.Split(';');
        return new UserPermissions(int.Parse(details[0])
            , details[1]
            , details[2]
            , details[3]
            , bool.Parse(details[4])
            , bool.Parse(details[5])
            , bool.Parse(details[6])
            , bool.Parse(details[7])
            , bool.Parse(details[8]));
    }
    public static string SerializePermissionsList(List<UserPermissions> its)
    {
        StringBuilder value = new StringBuilder();
        try
        {
            foreach (UserPermissions item in its)
            {
                if (value.ToString() == string.Empty)
                {
                    value.Append(SerializePermissions(item));
                }
                else
                {
                    value.Append("*" + SerializePermissions(item));
                }
            }
        }

        catch (Exception)
        {
            // ignored
        }

        return value.ToString();
    }

    public static List<UserPermissions> DeSerializePermissionsList(string its)
    {
        string[] details = its.Split('*');
        List<UserPermissions> items = new List<UserPermissions>();
        foreach (string item in details)
        {
            items.Add(DeSerializePermissions(item));
        }
        return items;
    }
}