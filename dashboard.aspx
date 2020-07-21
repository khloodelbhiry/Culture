<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="dashboard.aspx.cs" Inherits="dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="col-lg-6 col-md-12">
            <div class="row">
                <div class="col-lg-4 col-md-6 col-sm-6"></div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-table fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد اللجان</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrCommittees" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6"></div>
                <div class="clearfix"></div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-calendar fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد الاجتماعات</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrMeetings" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-user fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد الأعضاء بشخصهم</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrUsers" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-user-circle fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد الأعضاء بصفتهم</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrUsersAs" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon-big mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-male fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد الأعضاء (ذكر)</p>
                            <p class="line-height-1 text-title text-24 mt-2 mb-0"><asp:Literal ID="ltrMale" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon-big mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-female fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد الأعضاء (أنثى)</p>
                            <p class="line-height-1 text-title text-24 mt-2 mb-0"><asp:Literal ID="ltrFemale" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon-big mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-university fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">عدد الأعضاء (جهة)</p>
                            <p class="line-height-1 text-title text-24 mt-2 mb-0"><asp:Literal ID="ltrUnits" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-money fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">مجموع المستحقات المالية</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrAwards" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-user fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">المدفوعة</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrPaied" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-6">
                    <div class="card card-icon mb-4">
                        <div class="card-body text-center">
                            <i class="fa fa-user-circle fa-2x"></i>
                            <p class="text-muted mt-2 mb-2">المستحقة</p>
                            <p class="text-primary text-24 line-height-1 m-0"><asp:Literal ID="ltrRemain" runat="server"></asp:Literal></p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-6 col-md-12">
            <div class="row">
                <div class="table-responsive">
                    <div id="user_table_wrapper2" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                        <div class="row">
                            <div class="col-sm-12">
                                <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table22" role="grid" aria-describedby="user_table_info">
                                    <thead>
                                        <tr role="row">
                                            <th scope="col">اللجنة</th>
                                            <th scope="col">عدد الاجتماعات</th>
                                            <th scope="col">نسبة الحضور</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rpCommittees" runat="server">
                                            <ItemTemplate>
                                                <tr role="row" class="odd">
                                                    <td><%# Eval("Committee_Name") %></td>
                                                    <td><%# Eval("MembersCount") %></td>
                                                    <td><%#  Math.Round(decimal.Parse(Eval("Attendance").ToString()),2).ToString("G29")+" %" %></td>
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
</asp:Content>

