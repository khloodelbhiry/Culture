using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Class to save important details for user
/// </summary>
public class ClientDetails
{
    private int _ID;
    private string _Name;
    private string _Mobile;
    private string _UserName;
    private int _GroupId;
    private int _CommitteeID;
    public string Mobile
    {
        get { return _Mobile; }
        set { _Mobile = value; }
    }
    public string UserName
    {
        get { return _UserName; }
        set { _UserName = value; }
    }
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }
    public int ID
    {
        get { return _ID; }
        set { _ID = value; }
    }
    public int GroupId
    {
        get { return _GroupId; }
        set { _GroupId = value; }
    }
    public int CommitteeID
    {
        get { return _CommitteeID; }
        set { _CommitteeID = value; }
    }
    public ClientDetails()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public ClientDetails(int id,
        string name,
        string userName,
        string mobile,
        int groupId,
        int committeeId)
    {
        _ID = id;
        _Name = name;
        _Mobile = mobile;
        _UserName = userName;
        _GroupId = groupId;
        _CommitteeID = committeeId;
    }

    public static string SerializeClientDetails(ClientDetails det)
    {
        StringBuilder value = new StringBuilder();
        value.Append(det.ID);
        value.Append(";" + (det.Name ?? "").Replace(";", ""));
        value.Append(";" + det.UserName);
        value.Append(";" + det.Mobile);
        value.Append(";" + det.GroupId); 
        value.Append(";" + det.CommitteeID);
        return value.ToString();
    }

    public static ClientDetails DeSerializeClientDetails(string det)
    {
        string[] details = det.Split(';');
        if (details.Count() == 6)
        {
            return new ClientDetails(int.Parse(details[0])
                        , details[1]
                        , details[2]
                        , details[3]
                        , int.Parse(details[4])
                        , int.Parse(details[5]));
        }
        else
        {
            return null;
        }
    }
}