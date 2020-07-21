<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="committee-details.aspx.cs" Inherits="committee" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="App_Themes/css/dropzone.min.css" rel="stylesheet" />
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
                <div class="row">
                    <div class="col-lg-4 col-xl-4"></div>
                    <asp:Repeater ID="rpData" runat="server">
                        <ItemTemplate>
                            <div class="col-lg-4 col-xl-4">
                                <div class="card">
                                    <div class="card-body">
                                        <center>
                                        <h2 class="m-0">
                                            <%# Eval("Committee_Name") %>
                                        </h2></center>
                                        <p class="card-text text-18">
                                            <%# Eval("Committee_About") %>
                                        </p>
                                        <p class="card-text text-18"><span style="color: rebeccapurple; float: right; margin-bottom: 1rem;">كود اللجنة : </span><span style="float: right;"><%# Eval("Committee_Code") %></span><span class='<%# Eval("Committee_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Committee_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></p>
                                        <div class="clearfix"></div>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">عدد الأعضاء : </span><%# Eval("MembersCount") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">عدد الأهداف : </span><%# Eval("GoalCount") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">عدد الأجتماعات : </span><%# Eval("MeetingsCount") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">نسبة حضور الأعضاء : </span><%#  Math.Round(decimal.Parse(Eval("Attendance").ToString()),2).ToString("G29")+" %" %>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="col-lg-12 mt-2">
                        <div class="card">
                            <div class="card-body">
                                <ul class="nav nav-pills" id="myPillTab" role="tablist">
                                    <li class="nav-item"><a class="nav-link active show" id="goals" data-toggle="pill" href="#goalsPIll" role="tab" aria-controls="homePIll" aria-selected="true"><i class="fa fa-align-justify mr-1"></i>الأهداف</a></li>
                                    <li class="nav-item"><a class="nav-link" id="members" data-toggle="pill" href="#membersPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-users mr-1"></i>الأعضاء</a></li>
                                    <li class="nav-item"><a class="nav-link" id="meetings" data-toggle="pill" href="#meetingsPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-calendar mr-1"></i>الأجتماعات</a></li>
                                    <li class="nav-item"><a class="nav-link" id="financial" data-toggle="pill" href="#financialPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-money mr-1"></i>المستحقات المالية</a></li>
                                </ul>
                                <div class="tab-content" id="myPillTabContent">
                                    <div class="tab-pane fade active show" id="goalsPIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <ul>
                                                    <asp:Repeater ID="rpGoals" runat="server">
                                                        <ItemTemplate>
                                                            <tr role="row" class="odd">
                                                                <li class="text-20"><%# Eval("Goal_Text") %></li>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane" id="membersPIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                        <div class="table-responsive">
                                            <div id="attendance1" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="usear_table21" role="grid" aria-describedby="user_table_info">
                                                            <thead>
                                                                <tr role="row">
                                                                    <th scope="col">الكود</th>
                                                                    <th scope="col">العضو</th>
                                                                    <th scope="col">النوع</th>
                                                                    <th scope="col">الدور</th>
                                                                    <th scope="col">صفة العضوية</th>
                                                                    <th scope="col">الايميل</th>
                                                                    <th scope="col">نسبة الحضور</th>
                                                                    <th scope="col">المستحقات المالية</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="rpMembers" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr role="row" class="odd">
                                                                            <td><%# Eval("Member_Code") %></td>
                                                                            <td><%# Eval("Member_Name") %></td>
                                                                            <td><%# Eval("MemberType_Name") %></td>
                                                                            <td><%# Eval("Role_Name") %></td>
                                                                            <td><%# Eval("MembershipStatus_Name") %></td>
                                                                            <td><%# Eval("Member_Email") %></td>
                                                                            <td><%# Math.Round(decimal.Parse(Eval("Attendance").ToString()),2).ToString("G29")+" %" %></td>
                                                                            <td><%# decimal.Parse(Eval("Rewards").ToString()).ToString("G29")+" جنية" %></td>
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
                                    <div class="tab-pane" id="meetingsPIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                        <div class="table-responsive">
                                            <div id="meetingsS" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="usear_table1" role="grid" aria-describedby="user_table_info">
                                                            <thead>
                                                                <tr role="row">
                                                                    <th scope="col" style="width: 11px;">الكود</th>
                                                                    <th scope="col" style="width: 62px;">التاريخ</th>
                                                                    <th scope="col" style="width: 62px;">من</th>
                                                                    <th scope="col" style="width: 62px;">الى</th>
                                                                    <th scope="col" style="width: 62px;">المكان</th>
                                                                    <th scope="col" style="width: 62px;">النوع</th>
                                                                    <th scope="col" style="width: 62px;">م المقرر</th>
                                                                    <th scope="col" style="width: 62px;">م العضو</th>
                                                                    <th scope="col" style="width: 62px;">الحالة</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="rpMeetings" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr role="row" class="odd">
                                                                            <td><%# Eval("Meeting_Code") %></td>
                                                                            <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Meeting_Date")) %></td>
                                                                            <td><%# Eval("Meeting_TimeFrom") %></td>
                                                                            <td><%# Eval("Meeting_TimeTo") %></td>
                                                                            <td><%# Eval("Meeting_Place") %></td>
                                                                            <td><%# Eval("Type_Name") %></td>
                                                                            <td><%# decimal.Parse(Eval("Meeting_RewardNotMember").ToString()).ToString("G29") %></td>
                                                                            <td><%# decimal.Parse(Eval("Meeting_RewardMember").ToString()).ToString("G29") %></td>
                                                                            <td><%# Eval("Status_Name") %></td>
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
                                    <div class="tab-pane" id="financialPIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                        <div class="table-responsive">
                                            <div id="financialS" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="usear_table1" role="grid" aria-describedby="user_table_info">
                                                            <thead>
                                                                <tr role="row">
                                                                    <th scope="col">قيمة المكافأة</th>
                                                                    <th scope="col">العضو</th>
                                                                    <th scope="col">اللجنة</th>
                                                                    <th scope="col">التاريخ</th>
                                                                    <th scope="col">حالة الدفع</th>
                                                                    <th scope="col">تاريخ الدفع</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="rpFinancial" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr role="row" class="odd">
                                                                            <td><%# decimal.Parse(Eval("Reward_Value").ToString()).ToString("G29")%> <span class='<%# Eval("Reward_IsException").ToString()=="True"?"badge badge-danger float-right":string.Empty %>'><%# Eval("Reward_IsException").ToString()=="True"?"استثنائية":""%></span></td>
                                                                            <td><%# Eval("Member_Name") %></td>
                                                                            <td><%# Eval("Committee_Name") %></td>
                                                                            <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Reward_Date"))%></td>
                                                                            <td><%# Eval("PaymentStatus_Name") %></td>
                                                                            <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Reward_PaymentDate"))%></td>
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
            </section>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

