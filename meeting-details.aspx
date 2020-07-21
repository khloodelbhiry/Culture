<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="meeting-details.aspx.cs" Inherits="meetings" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/css/bootstrap-datepicker3.min.css" rel="stylesheet" />
    <link href="App_Themes/css/bootstrap-timepicker.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.13.1/css/bootstrap-select.css" />
    <style>
        /*.container-fluid{
            padding-left:0px !important;
            padding-right:0px !important;
        }*/
        .datepicker-days {
            display: block !important;
        }

        .bootstrap-select .dropdown-toggle .filter-option-inner {
            padding-right: inherit !important;
        }

        .btn-light {
            background-color: #f8f9fa !important;
        }

        .panel {
            margin-bottom: 0px;
        }

        .chat-window {
            bottom: 0;
            position: fixed;
            float: right;
            margin-left: 10px;
        }

            .chat-window > div > .panel {
                border-radius: 5px 5px 0 0;
            }

        .icon_minim {
            padding: 2px 10px;
        }

        .msg_container_base {
            background: #e5e5e5;
            margin: 0;
            padding: 0 10px 10px;
            max-height: 300px;
            overflow-x: hidden;
        }

        .top-bar {
            background: #666;
            color: white;
            padding: 10px;
            position: relative;
            overflow: hidden;
        }

        .msg_receive {
            padding-left: 0;
            margin-left: 0;
        }

        .msg_sent {
            padding-bottom: 20px !important;
            margin-right: 0;
        }

        .messages {
            background: white;
            padding: 10px;
            border-radius: 2px;
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
            max-width: 100%;
        }

            .messages > p {
                font-size: 13px;
                margin: 0 0 0.2rem 0;
            }

            .messages > time {
                font-size: 11px;
                color: #ccc;
            }

        .msg_container {
            padding: 10px;
            overflow: hidden;
            display: flex;
        }

        img {
            display: block;
            width: 100%;
        }

        .avatar {
            position: relative;
        }

        .base_receive > .avatar:after {
            content: "";
            position: absolute;
            top: 0;
            right: 0;
            width: 0;
            height: 0;
            border: 5px solid #FFF;
            border-left-color: rgba(0, 0, 0, 0);
            border-bottom-color: rgba(0, 0, 0, 0);
        }

        .base_sent {
            justify-content: flex-end;
            align-items: flex-end;
        }

            .base_sent > .avatar:after {
                content: "";
                position: absolute;
                bottom: 0;
                left: 0;
                width: 0;
                height: 0;
                border: 5px solid white;
                border-right-color: transparent;
                border-top-color: transparent;
                box-shadow: 1px 1px 2px rgba(black, 0.2);
                // not quite perfect but close
            }

        .msg_sent > time {
            float: right;
        }



        .msg_container_base::-webkit-scrollbar-track {
            -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3);
            background-color: #F5F5F5;
        }

        .msg_container_base::-webkit-scrollbar {
            width: 12px;
            background-color: #F5F5F5;
        }

        .msg_container_base::-webkit-scrollbar-thumb {
            -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3);
            background-color: #555;
        }

        .btn-group.dropup {
            position: fixed;
            left: 0px;
            bottom: 0;
        }

        .outer {
            position: absolute;
            width: 97%;
        }

        .inner {
            z-index: 9 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdateProgress ID="UpdateProgress" runat="server" AssociatedUpdatePanelID="UpdatePanel">
        <ProgressTemplate>
            <div class="overlay">
                <div class="center-overlay">
                    <i class="ace-icon fa fa-spinner fa-spin orange bigger-300"></i>انتظر من فضلك ...
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <section class="widget-card">
                <div class="row ">
                    <div class="outer">
                        <div class="col-sm-12">
                            <div class="card text-left">
                                <div class="card-body">
                                    <ul class="nav nav-pills" id="myPillTab" role="tablist">
                                        <li class="nav-item"><a class="nav-link active show" id="agenda" data-toggle="pill" href="#agendaPIll" role="tab" aria-controls="homePIll" aria-selected="true"><i class="fa fa-table mr-1"></i>اجندة</a></li>
                                        <li class="nav-item"><a class="nav-link" id="attendance" data-toggle="pill" href="#attendancePIll" role="tab" aria-controls="profilePIll" aria-selected="false"><i class="fa fa-users mr-1"></i>حضور</a></li>
                                        <li class="nav-item"><a class="nav-link" id="discussions" data-toggle="pill" href="#discussionsPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-microphone mr-1"></i>مناقشات عامة</a></li>
                                        <li class="nav-item"><a class="nav-link" id="recommendation" data-toggle="pill" href="#recommendationPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-list mr-1"></i>توصيات عامة</a></li>
                                        <li class="nav-item"><a class="nav-link" id="attachment" data-toggle="pill" href="#attachmentPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-paperclip mr-1"></i>مرفقات</a></li>
                                    </ul>
                                    <div class="tab-content" id="myPillTabContent">
                                        <div class="tab-pane fade active show" id="agendaPIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                            <asp:LinkButton ID="lnkAddNewAgenda" Style="margin-left: 15px !important;" CssClass="btn btn-primary pull-left" runat="server" OnClick="lnkAddNewAgenda_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                            <asp:LinkButton ID="lnkAllAgendaAttachments" Style="margin-left: 15px !important;" CssClass="btn btn-dark pull-left" runat="server" OnClick="lnkAllAgendaAttachments_Click"><i class="fa fa-paperclip"></i></asp:LinkButton>
                                            <div class="clearfix"></div>
                                            <br />
                                            <div class="clearfix"></div>
                                            <div class="table-responsive">
                                                <div id="user_table_wrapper4" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table4" role="grid" aria-describedby="user_table_info">
                                                                <thead>
                                                                    <tr role="row">
                                                                        <th scope="col" style="width: 5%;">#</th>
                                                                        <th scope="col">البند</th>
                                                                        <th scope="col">أولوية المناقشة </th>
                                                                        <th scope="col" style="width: 10%;">الحالة</th>
                                                                        <th scope="col" style="width: 10%;"><i class="fa fa-cogs"></i></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <asp:Repeater ID="rpAgenda" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr role="row" class="even">
                                                                                <td><%# Container.ItemIndex + 1 %> </td>
                                                                                <td><%# Eval("Agenda_Item") %></td>
                                                                                <td><%# Eval("DiscussionPriority_Name") %></td>
                                                                                <td><span class='<%# Eval("Agenda_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Agenda_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></td>
                                                                                <td>
                                                                                    <asp:LinkButton ID="lnkEditAgenda" runat="server" CommandArgument='<%# Eval("Agenda_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditAgenda_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton>
                                                                                    <asp:LinkButton ID="lnkAgendaAttachments" CommandArgument='<%# Eval("Agenda_ID") %>' runat="server" ToolTip="المرفقات" CssClass="text-danger mr-2" OnCommand="lnkAgendaAttachments_Command"><span class="ul-btn__icon"><i class="fa fa-paperclip"></i></span></asp:LinkButton>
                                                                                    <asp:LinkButton ID="lnkAgendaDiscussions" CommandArgument='<%# Eval("Agenda_ID") %>' runat="server" ToolTip="المناقشات" CssClass="text-purple mr-2" OnCommand="lnkAgendaDiscussions_Command"><span class="ul-btn__icon"><i class="fa fa-microphone"></i></span></asp:LinkButton>
                                                                                </td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="attendancePIll" role="tabpanel" aria-labelledby="profile-icon-pill">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <asp:Repeater ID="rpAttendance" runat="server">
                                                        <ItemTemplate>
                                                            <div class="form-group">
                                                                <label class="checkbox checkbox-success">
                                                                    <input type="checkbox" name="check" value='<%# Eval("Attendance_MemberID")%>' checked disabled><span style="color: black !important;"> <%# Eval("Member_Name")%> </span><span class="checkmark"></span>
                                                                </label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group" id="divCheckAll" runat="server">
                                                        <label class="checkbox checkbox-success">
                                                            <asp:CheckBox ID="checkAll" runat="server" OnCheckedChanged="Check_Clicked" AutoPostBack="true" /><span style="color: darkred;">تحديد الكل</span><span class="checkmark"></span>
                                                        </label>
                                                    </div>
                                                    <asp:Repeater ID="rpMember" runat="server">
                                                        <ItemTemplate>
                                                            <div class="form-group">
                                                                <label class="checkbox checkbox-success">
                                                                    <input type="checkbox" name="check" value='<%# Eval("Member_ID")%>' runat="server" id="check"><span> <%# Eval("Member_Name")%> </span><span class="checkmark"></span>
                                                                </label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 text-center">
                                                <asp:Button ID="btnSaveAttendance" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveAttendance_Click" />
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="discussionsPIll" role="tabpanel" aria-labelledby="profile-icon-pill">
                                            <asp:LinkButton ID="lnkAddDiscussion" Style="margin-left: 15px !important;" CssClass="btn btn-primary pull-left" runat="server" OnClick="lnkAddDiscussion_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                            <div class="clearfix"></div>
                                            <br />
                                            <div class="clearfix"></div>
                                            <div class="table-responsive">
                                                <div id="user_table_wrapper" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table" role="grid" aria-describedby="user_table_info">
                                                                <thead>
                                                                    <tr role="row">
                                                                        <th scope="col" style="width: 5%;">#</th>
                                                                        <th scope="col" style="width: 62px;">المناقشة</th>
                                                                        <th scope="col" style="width: 15%;"><i class="fa fa-cogs"></i></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <asp:Repeater ID="rpDiscussion" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr role="row" class="odd">
                                                                                <th class="text-center"><%# Container.ItemIndex + 1 %></th>
                                                                                <td><%# Eval("Discussion_Text") %></td>
                                                                                <td>
                                                                                    <asp:LinkButton ID="lnkDeleteDiscussion" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Discussion_ID") %>' OnCommand="lnkDeleteDiscussion_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                                    <asp:LinkButton ID="lnkEditDiscussion" runat="server" CommandArgument='<%# Eval("Discussion_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditDiscussion_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton>
                                                                                    <asp:LinkButton ID="lnkDiscussionRecommndations" runat="server" CommandArgument='<%# Eval("Discussion_ID") %>' CssClass="text-primary mr-2 btn sm" ToolTip="التوصيات" OnCommand="lnkDiscussionRecommndations_Command"><i class="nav-icon fa fa-list font-weight-bold"></i></asp:LinkButton>
                                                                                </td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="recommendationPIll" role="tabpanel" aria-labelledby="profile-icon-pill">
                                            <asp:LinkButton ID="lnkAddRecommondation" Style="margin-left: 15px !important;" CssClass="btn btn-primary pull-left" runat="server" OnClick="lnkAddRecommondation_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                            <div class="clearfix"></div>
                                            <br />
                                            <div class="clearfix"></div>
                                            <div class="table-responsive">
                                                <div id="user_table_wrapper1" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table2" role="grid" aria-describedby="user_table_info">
                                                                <thead>
                                                                    <tr role="row">
                                                                        <th scope="col" style="width: 5%;">#</th>
                                                                        <th scope="col">التوصيه</th>
                                                                        <th scope="col">كود التوصية</th>
                                                                        <th scope="col">المسئول عن متابعة التنفيذ</th>
                                                                        <th scope="col">نسبه التنفيذ</th>
                                                                        <th scope="col" style="width: 10%;">الحالة</th>
                                                                        <th scope="col" style="width: 10%;"><i class="fa fa-cogs"></i></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <asp:Repeater ID="RpRecommondation" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr role="row" class="odd">
                                                                                <td><%# Container.ItemIndex + 1 %></td>
                                                                                <td><%# Eval("Recommendation_Text") %></td>
                                                                                <td><%# Eval("Recommendation_Code") %></td>
                                                                                <td>
                                                                                    <ul>
                                                                                        <asp:Repeater ID="rpImplementers" runat="server" DataSource='<%# Eval("Implementers") %>'>
                                                                                            <ItemTemplate>
                                                                                                <li><%# Eval("Member_Name") %></li>
                                                                                            </ItemTemplate>
                                                                                        </asp:Repeater>
                                                                                    </ul>
                                                                                </td>
                                                                                <td><%# Eval("Recommendation_Progress") %></td>
                                                                                <td><span class='<%# Eval("Recommendation_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Recommendation_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></td>
                                                                                <td>
                                                                                    <%--<asp:LinkButton ID="lnkDeleteRecommondation" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Recommendation_ID") %>' OnCommand="lnkDeleteRecommondation_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>--%>
                                                                                    <asp:LinkButton ID="lnkEditRecommondation" runat="server" CommandArgument='<%# Eval("Recommendation_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditRecommondation_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton>
                                                                                    <asp:LinkButton ID="lnkRecDiscussions" CommandArgument='<%# Eval("Recommendation_ID") %>' runat="server" ToolTip="المناقشات" CssClass="text-primary mr-2 btn sm" OnCommand="lnkRecDiscussions_Command"><span class="ul-btn__icon"><i class="fa fa-microphone"></i></span></asp:LinkButton></td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="attachmentPIll" role="tabpanel" aria-labelledby="contact-icon-pill">
                                            <asp:LinkButton ID="lnkAddAttachment" Style="margin-left: 15px !important;" CssClass="btn btn-primary pull-left" runat="server" OnClick="lnkAddAttachment_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                            <div class="clearfix"></div>
                                            <br />
                                            <div class="clearfix"></div>
                                            <div class="table-responsive">
                                                <div id="attachments" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table1" role="grid" aria-describedby="user_table_info">
                                                                <thead>
                                                                    <tr role="row">
                                                                        <th scope="col" style="width: 5%;">#</th>
                                                                        <th scope="col" style="width: 62px;">المرفق</th>
                                                                        <th scope="col" style="width: 10%;"><i class="fa fa-cogs"></i></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <asp:Repeater ID="rpAttachments" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr role="row" class="odd">
                                                                                <th><%# Container.ItemIndex + 1 %></th>
                                                                                <td><a href='<%# Eval("Attachment_File") %>'><%# Eval("Attachment_Name") %></a> </td>
                                                                                <td>
                                                                                    <asp:LinkButton ID="lnkDeleteAttachment" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Attachment_ID") %>' OnCommand="lnkDeleteAttachment_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                                </td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="div" runat="server" visible="false" class="fixed-bottom">
                        <div class="card o-hidden float-right" style="width: 50% !important;">
                            <div class="card-header d-flex align-items-center">
                                <h3 class="w-50 float-left card-title m-0" id="hAgenda" runat="server">المناقشات</h3>
                                <div class="dropdown dropleft text-right w-50 float-right">
                                    <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="fnCloseDiscussion()"><span aria-hidden="true">اغلاق</span></button>
                                </div>
                            </div>
                            <div class="card-body" style="padding: 0px !important;">
                                <div class="card chat-sidebar-container sidebar-container" data-sidebar-container="chat">
                                    <div class="chat-content-wrap sidebar-content" data-sidebar-content="chat">
                                        <div style="overflow-y: auto !important; max-height: 350px !important;">
                                            <div class="chat-content perfect-scrollbar ps ps--active-y" data-suppress-scroll-x="true">
                                                <%--<div id="newData"></div>--%>
                                                <asp:UpdatePanel ID="up" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Timer ID="timer" Enabled="false" runat="server" OnTick="timer_Tick" Interval="2000"></asp:Timer>
                                                        <asp:Repeater ID="rpAgendaDiscussions" runat="server">
                                                            <ItemTemplate>
                                                                <div class='<%# ClientDetails.DeSerializeClientDetails(Session["User"].ToString()).ID == int.Parse(Eval("Discussion_UserID").ToString()) ? "d-flex mb-4 user":"d-flex mb-4" %>'>
                                                                    <img class='<%# ClientDetails.DeSerializeClientDetails(Session["User"].ToString()).ID == int.Parse(Eval("Discussion_UserID").ToString()) ? "avatar-sm rounded-circle ml-3 ":"avatar-sm rounded-circle ml-3 d-none" %>' src='<%# Eval("Avatar") %>' alt="alt">
                                                                    <div class="message flex-grow-1">
                                                                        <div class="d-flex">
                                                                            <p class="mb-1 text-title text-16 flex-grow-1"><%# Eval("User_FullName") %></p>
                                                                            <span class="text-small text-muted"><%# Eval("Discussion_Date") %></span>
                                                                        </div>
                                                                        <p class="m-0"><%# Eval("Discussion_Text") %></p>
                                                                    </div>
                                                                    <img class='<%# ClientDetails.DeSerializeClientDetails(Session["User"].ToString()).ID != int.Parse(Eval("Discussion_UserID").ToString()) ? "avatar-sm rounded-circle ml-3 ":"avatar-sm rounded-circle ml-3 d-none" %>' src='<%# Eval("Avatar") %>' alt="alt">
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <div class="ps__rail-x" style="left: 0px; bottom: 0px;">
                                                    <div class="ps__thumb-x" tabindex="0" style="left: 0px; width: 0px;"></div>
                                                </div>
                                                <div class="ps__rail-y" style="top: 0px; height: 322px; right: 1159px;">
                                                    <div class="ps__thumb-y" tabindex="0" style="top: 0px; height: 233px;"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="pl-3 pr-3 pt-3 pb-3 chat-input-area">
                                            <form class="inputForm">
                                                <asp:Panel ID="pnl" runat="server" DefaultButton="lnkSendMessage">
                                                    <div class="form-group">
                                                        <textarea class="form-control form-control-rounded" id="txtMessage" runat="server" placeholder="اكتب هنا" name="message" cols="30" rows="3"></textarea>
                                                    </div>
                                                    <div class="d-flex">
                                                        <div class="flex-grow-1"></div>
                                                        <asp:LinkButton ID="lnkSendMessage" CssClass="btn btn-icon btn-rounded btn-primary mr-2" runat="server" OnClick="lnkSendMessage_Click"><i class="i-Paper-Plane"></i></asp:LinkButton>
                                                    </div>
                                                </asp:Panel>
                                            </form>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <div class="modal fade" id="attachmentsAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">المرفقات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم الملف
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtFileName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAttachment" runat="server" Display="Dynamic" ControlToValidate="txtFileName" ValidationGroup="vgSaveAttachment" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    الملف
                                </label>
                                <div class="col-sm-8 ">
                                    <div class="dropzone">
                                        <div class="fallback">
                                            <asp:FileUpload ID="fuAttach" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveAttachment" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveAttachment_Click" ValidationGroup="vgSaveAttachment" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>

            <!--Discussion -->
            <div class="modal fade" id="DiscussionAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">المناقشات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    المناقشة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDiscussion" TextMode="MultiLine" Rows="2" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDiscussion" runat="server" Display="Dynamic" ControlToValidate="txtDiscussion" ValidationGroup="vgSaveDiscussion" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveDiscussion" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveDiscussion_Click" ValidationGroup="vgSaveDiscussion" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <!-- EndDiscussion -->

            <!-- recommondation -->
            <div class="modal fade" id="RecommondationAddModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-3">التوصيات </h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    التوصية
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtRecomName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRecomName" runat="server" Display="Dynamic" ControlToValidate="txtRecomName" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    كود التوصية 
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtRecCode" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRecCode" runat="server" Display="Dynamic" ControlToValidate="txtRecCode" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    المسئول عن متابعة التنفيذ
                                </label>
                                <div class="col-sm-8">
                                    <select id="ddlImplementation" runat="server" class="form-control selectpicker" multiple="true" data-live-search="true"></select>
                                    <%--<asp:RequiredFieldValidator ID="rfvResponsibleForImplementation" runat="server" Display="Dynamic" ControlToValidate="txtResponsibleForImplementation" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    نسبة التنفيذ
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtProgress" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvProgress" runat="server" Display="Dynamic" ControlToValidate="txtProgress" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>

                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveRec" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveRec_Click" ValidationGroup="vgSaveRec" />
                            <asp:Button ID="btnApproveRec" runat="server" CssClass="btn btn-sm btn-success ml-2" Text="اعــتـمــاد" OnClick="btnApproveRec_Click" Visible="false" />
                            <asp:Button ID="btnFreezeRec" runat="server" CssClass="btn btn-danger ml-2" Text="تــجــمــيــد" OnClick="btnFreezeRec_Click" Visible="false" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <!-- end recommondation -->
            <!-- RecDiscussion -->
            <div class="modal fade" id="RecDiscussionAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">المناقشات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    المناقشة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtRecDiscussion" TextMode="MultiLine" Rows="2" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRecDiscussion" runat="server" Display="Dynamic" ControlToValidate="txtRecDiscussion" ValidationGroup="vgSaveRecDiscussion" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveRecDiscussion" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveRecDiscussion_Click" ValidationGroup="vgSaveRecDiscussion" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="RecDiscussionModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">المناقشات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddRecDiscussion" CssClass="btn btn-primary" runat="server" OnClick="lnkAddRecDiscussion_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="user_table_wrapper3" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table3" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 5%;">#</th>
                                                                <th scope="col" style="width: 62px;"></th>
                                                                <th scope="col" style="width: 5%;">#</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rpRecDiscussion" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <th><%# Container.ItemIndex + 1 %></th>
                                                                        <td><%# Eval("RecommendationDiscussion_Text") %></td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteRecDiscussion" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("RecommendationDiscussion_ID") %>' OnCommand="lnkDeleteRecDiscussion_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="lnkEditRecDiscussion" runat="server" CommandArgument='<%# Eval("RecommendationDiscussion_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditRecDiscussion_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- End RecDiscussion -->
            <!-- Agenda -->
            <div class="modal fade" id="AddAgendaModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-4">اجندة</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    البند
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAgendaName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAgendaName" runat="server" Display="Dynamic" ControlToValidate="txtAgendaName" ValidationGroup="vgSaveAgenda" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    أولوية المناقشة
                                </label>
                                <div class="col-sm-2">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="rdPriority" value="1" runat="server" id="rdPriority1" checked><span>قصوى</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-2">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="rdPriority" value="2" runat="server" id="rdPriority2"><span>مهم</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-2">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="rdPriority" value="3" runat="server" id="rdPriority3"><span>عادى</span><span class="checkmark"></span>
                                    </label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    الملاحظات
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAgendaNotes" TextMode="MultiLine" Rows="3" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveAgenda" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveAgenda_Click" ValidationGroup="vgSaveAgenda" />
                            <asp:Button ID="btnApproveAgenda" runat="server" CssClass="btn btn-success ml-2" Text="اعــتـمــاد" OnClick="btnApproveAgenda_Click" Visible="false" />
                            <asp:Button ID="btnFreezeAgenda" runat="server" CssClass="btn btn-danger ml-2" Text="تــجــمــيــد" OnClick="btnFreezeAgenda_Click" Visible="false" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <!-- end Agenda -->

            <!-- Agenda attachments -->
            <div class="modal fade" id="agendaAttachmentsModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">المرفقات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddAgendaAttachment" CssClass="btn btn-primary" runat="server" OnClick="lnkAddAgendaAttachment_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="agendaAttachments" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table1" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 11px;">#</th>
                                                                <th scope="col" style="width: 62px;">المرفق</th>
                                                                <th scope="col" style="width: 49px;"><i class="fa fa-cogs"></i></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rpAgendaAttachments" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <th><%# Container.ItemIndex + 1 %></th>
                                                                        <td><a href='<%# Eval("Attachment_File") %>'><%# Eval("Attachment_Name") %></a> </td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteAgendaAttachment" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Attachment_ID") %>' OnCommand="lnkDeleteAgendaAttachment_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="agendaAttachmentsAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">المرفقات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم الملف
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAgendaFileName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAgendaFileName" runat="server" Display="Dynamic" ControlToValidate="txtAgendaFileName" ValidationGroup="vgSaveAgendaAttachment" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    الملف
                                </label>
                                <div class="col-sm-8 ">
                                    <div class="dropzone">
                                        <div class="fallback">
                                            <asp:FileUpload ID="fuAgendaAttachment" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveAgendaAttachment" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveAgendaAttachment_Click" ValidationGroup="vgSaveAgendaAttachment" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <!-- end Agenda attachments -->

            <!-- all Agenda attachments -->
            <div class="modal fade" id="allAgendaAttachmentsModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">المرفقات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddAllAgendaAttachment" CssClass="btn btn-primary" runat="server" OnClick="lnkAddAllAgendaAttachment_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="allAgendaAttachments" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="usera_table1" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 11px;">#</th>
                                                                <th scope="col" style="width: 62px;">المرفق</th>
                                                                <th scope="col" style="width: 49px;"><i class="fa fa-cogs"></i></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rpAllAgendaAttachment" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <th><%# Container.ItemIndex + 1 %></th>
                                                                        <td><a href='<%# Eval("Attachment_File") %>'><%# Eval("Attachment_Name") %></a> </td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteAllAgendaAttachment" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Attachment_ID") %>' OnCommand="lnkDeleteAllAgendaAttachment_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="allAgendaAttachmentsAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">المرفقات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم الملف
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAllAgendaFileName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="txtAllAgendaFileName" ValidationGroup="vgSaveAllAgendaAttachment" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    الملف
                                </label>
                                <div class="col-sm-8 ">
                                    <div class="dropzone">
                                        <div class="fallback">
                                            <asp:FileUpload ID="fuAllAgenda" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveAllAgendaAttachment" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveAllAgendaAttachment_Click" ValidationGroup="vgSaveAllAgendaAttachment" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <!-- end Agenda attachments -->

            <!-- DiscussionRec -->
            <div class="modal fade" id="DiscussionRecAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">التوصيات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    التوصية
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDiscussionRecName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ControlToValidate="txtDiscussionRecName" ValidationGroup="vgSaveDiscussionRec" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    كود التوصية 
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDiscussionRecCode" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ControlToValidate="txtDiscussionRecCode" ValidationGroup="vgSaveDiscussionRec" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    المسئول عن متابعة التنفيذ
                                </label>
                                <div class="col-sm-8">
                                    <select id="ddlDiscussionRecImplementation" runat="server" class="form-control selectpicker" multiple="true" data-live-search="true"></select>
                                    <%--<asp:RequiredFieldValidator ID="rfvResponsibleForImplementation" runat="server" Display="Dynamic" ControlToValidate="txtResponsibleForImplementation" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    نسبة التنفيذ
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDiscussionRecProgress" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ControlToValidate="txtDiscussionRecProgress" ValidationGroup="vgSaveDiscussionRec" ForeColor="Red">*</asp:RequiredFieldValidator>

                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveDiscussionRec" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveDiscussionRec_Click" ValidationGroup="vgSaveDiscussionRec" />
                            <asp:Button ID="btnApproveDiscussionRec" runat="server" CssClass="btn btn-sm btn-success ml-2" Text="اعــتـمــاد" OnClick="btnApproveDiscussionRec_Click" Visible="false" />
                            <asp:Button ID="btnFreezeDiscussionRec" runat="server" CssClass="btn btn-danger ml-2" Text="تــجــمــيــد" OnClick="btnFreezeDiscussionRec_Click" Visible="false" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="DiscussionRecModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">التوصيات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddDiscussionRec" CssClass="btn btn-primary" runat="server" OnClick="lnkAddDiscussionRec_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="user_table_wrapper32" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table2" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 5%;">#</th>
                                                                <th scope="col">التوصيه</th>
                                                                <th scope="col">كود التوصية</th>
                                                                <th scope="col">المسئول عن متابعة التنفيذ</th>
                                                                <th scope="col">نسبه التنفيذ</th>
                                                                <th scope="col" style="width: 10%;">الحالة</th>
                                                                <th scope="col" style="width: 10%;"><i class="fa fa-cogs"></i></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rpDiscussionRec" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <td><%# Container.ItemIndex + 1 %></td>
                                                                        <td><%# Eval("Recommendation_Text") %></td>
                                                                        <td><%# Eval("Recommendation_Code") %></td>
                                                                        <td>
                                                                            <ul>
                                                                                <asp:Repeater ID="rpImplementers" runat="server" DataSource='<%# Eval("Implementers") %>'>
                                                                                    <ItemTemplate>
                                                                                        <li><%# Eval("Member_Name") %></li>
                                                                                    </ItemTemplate>
                                                                                </asp:Repeater>
                                                                            </ul>
                                                                        </td>
                                                                        <td><%# Eval("Recommendation_Progress") %></td>
                                                                        <td><span class='<%# Eval("Recommendation_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Recommendation_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkEditDiscussionRec" runat="server" CommandArgument='<%# Eval("Recommendation_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditDiscussionRec_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- End RecDiscussion -->
            <script src="App_Themes/js/bootstrap-timepicker.min.js"></script>
            <script src="App_Themes/js/bootstrap-datepicker.min.js"></script>
            <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.13.1/js/bootstrap-select.min.js"></script>
            <%--<script src="Scripts/jquery.signalR-1.0.0-rc1.min.js"></script>--%>
            <%--   <script src="Scripts/jquery.signalR-2.4.1.min.js"></script>
            <script src="signalr/hubs" type="text/javascript"></script>--%>
            <%-- <script>
                $(function () {
                    var discussion = $.connection.discussionsHub;
                    discussion.client.updateMessages = function () {
                        getData();
                    };
                    $.connection.hub.start().done(function () {
                        getData();
                    }).fail(function (e) {
                        alert(e);
                    });
                });

                function getData() {
                    $.ajax({
                        url: 'meeting-details.aspx/GetAgendaDiscussions',
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            $("#newData").html(data.d);
                        }
                    });
                }
            </script>--%>
            <script type="text/javascript">
                function fnCloseDiscussion() {
                    document.getElementById('ContentPlaceHolder1_div').style.display = "none";
                    $('.modal-backdrop').remove();
                }
                $(document).ready(function () {
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_initializeRequest(InitializeRequest);
                    prm.add_endRequest(EndRequest);
                    initiateTimePicker();
                    initiateMultiSelect();
                });
                function InitializeRequest() {
                }
                function EndRequest() {
                    initiateTimePicker();
                    initiateMultiSelect();
                }
                function initiateMultiSelect() {
                    $('select').selectpicker();
                }
                function initiateTimePicker() {
                    $("[id$=txtTimeFrom]").timepicker({
                        minuteStep: 1,
                        disableFocus: true,
                        icons: {
                            up: 'fa fa-chevron-up',
                            down: 'fa fa-chevron-down'
                        }
                    }).on('focus', function () {
                        $("[id$=txtTimeFrom]").timepicker('showWidget');
                    });
                    $("[id$=txtTimeTo]").timepicker({
                        minuteStep: 1,
                        disableFocus: true,
                        icons: {
                            up: 'fa fa-chevron-up',
                            down: 'fa fa-chevron-down'
                        }
                    }).on('focus', function () {
                        $("[id$=txtTimeTo]").timepicker('showWidget');
                    });
                    $("[id$=txtDate]").datepicker({
                        autoclose: true,
                        todayHighlight: true
                    });
                    $("[id$=txtDatePostponedUntil]").datepicker({
                        autoclose: true,
                        todayHighlight: true
                    });
                    $("[id$=txtDateFromSrc]").datepicker({
                        autoclose: true,
                        todayHighlight: true
                    });
                    $("[id$=txtDateToSrc]").datepicker({
                        autoclose: true,
                        todayHighlight: true
                    });
                    $("[id$=txtTimeFromSrc]").timepicker({
                        minuteStep: 1,
                        disableFocus: true,
                        icons: {
                            up: 'fa fa-chevron-up',
                            down: 'fa fa-chevron-down'
                        }
                    }).on('focus', function () {
                        $("[id$=txtTimeFromSrc]").timepicker('showWidget');
                    });
                    $("[id$=txtTimeToSrc]").timepicker({
                        minuteStep: 1,
                        disableFocus: true,
                        icons: {
                            up: 'fa fa-chevron-up',
                            down: 'fa fa-chevron-down'
                        }
                    }).on('focus', function () {
                        $("[id$=txtTimeToSrc]").timepicker('showWidget');
                    });
                }
                $(document).on('click', '.panel-heading span.icon_minim', function (e) {
                    var $this = $(this);
                    if (!$this.hasClass('panel-collapsed')) {
                        $this.parents('.panel').find('.panel-body').slideUp();
                        $this.addClass('panel-collapsed');
                        $this.removeClass('glyphicon-minus').addClass('glyphicon-plus');
                    } else {
                        $this.parents('.panel').find('.panel-body').slideDown();
                        $this.removeClass('panel-collapsed');
                        $this.removeClass('glyphicon-plus').addClass('glyphicon-minus');
                    }
                });
                $(document).on('focus', '.panel-footer input.chat_input', function (e) {
                    var $this = $(this);
                    if ($('#minim_chat_window').hasClass('panel-collapsed')) {
                        $this.parents('.panel').find('.panel-body').slideDown();
                        $('#minim_chat_window').removeClass('panel-collapsed');
                        $('#minim_chat_window').removeClass('glyphicon-plus').addClass('glyphicon-minus');
                    }
                });
                $(document).on('click', '#new_chat', function (e) {
                    var size = $(".chat-window:last-child").css("margin-left");
                    size_total = parseInt(size) + 400;
                    alert(size_total);
                    var clone = $("#chat_window_1").clone().appendTo(".container");
                    clone.css("margin-left", size_total);
                });
                $(document).on('click', '.icon_close', function (e) {
                    //$(this).parent().parent().parent().parent().remove();
                    $("#chat_window_1").remove();
                });

            </script>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSaveAttachment" />
            <asp:PostBackTrigger ControlID="btnSaveAgendaAttachment" />
            <asp:PostBackTrigger ControlID="btnSaveAllAgendaAttachment" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

