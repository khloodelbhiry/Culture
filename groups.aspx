<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="groups.aspx.cs" Inherits="Groups" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTreeView" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdateProgress ID="updateProgress" DisplayAfter="0" runat="server" DynamicLayout="true" AssociatedUpdatePanelID="upnl">
        <ProgressTemplate>
            <div class="loading">
                <div>
                    <img class="" src="App_Themes/img/loading18.gif" alt="loading" width="100px" style="top: 40%; right: 50%; position: absolute;">
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="upnl">
        <ContentTemplate>
            <div class="card-header d-flex align-items-center border-0">
                <h3 class="w-50 float-left card-title m-0">
                    <asp:Literal ID="ltrCommitte" runat="server"></asp:Literal>
                    <i class="fa fa-arrow-circle-o-left"></i>عــدد الـمجـمـوعـات (
                    <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                    )</h3>
                <div class="dropdown dropleft text-right w-50 float-right">
                    <asp:LinkButton ID="lnkAddNew" runat="server" OnClick="lnkAddNew_Click" CssClass="btn btn-primary">مــجــمــوعــة جــديــدة</asp:LinkButton>
                </div>
            </div>
            <br />
            <section class="widget-card">
                <div class="row">
                    <div class="col-sm-4">
                        <h3 class="w-50 float-left card-title m-0">بــحــث بـ :</h3>
                        <div class="clearfix"></div>
                        <div class="form-group row">
                            <label class="col-sm-4 col-form-label">
                                المجموعة
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtNameSrch" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row pull-left">
                            <asp:LinkButton ID="LnkSearch" runat="server" OnClick="LnkSearch_Click" CssClass="btn btn-primary mr-2">بــحــث</asp:LinkButton>
                            <asp:LinkButton ID="LnkNewSearch" runat="server" OnClick="LnkNewSearch_Click" CssClass="btn btn-light mr-3">بــحــث جــديــد</asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-sm-8">
                        <div class="table-responsive">
                            <div id="user_table_wrapper2" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table22" role="grid" aria-describedby="user_table_info">
                                            <thead>
                                                <tr role="row">
                                                    <th scope="col" style="width: 11px;">#</th>
                                                    <th scope="col" style="width: 62px;">المجموعة</th>
                                                    <th scope="col" style="width: 62px;"><i class="fa fa-cogs"></i></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="rpData" runat="server">
                                                    <ItemTemplate>
                                                        <tr role="row" class="odd">
                                                            <th><%# Eval("Group_ID") %></th>
                                                            <td><%# Eval("Group_Name") %></td>
                                                            <td>
                                                                <asp:LinkButton ID="lnkDeleteGroup" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Group_ID") %>' OnCommand="lnkDeleteGroup_Command" ToolTip="حذف" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" CausesValidation="false"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkEditGroup" runat="server" CssClass="text-success mr-2" CommandArgument='<%# Eval("Group_ID") %>' OnCommand="lnkEditGroup_Command" ToolTip="تعديل" CausesValidation="false"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton>
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
            </section>
            <div class="modal fade" id="groupAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">المجموعات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    المجموعة
                                </label>
                                <div class="col-sm-8">
                                   <asp:TextBox ID="txtName" runat="server" CssClass="form-control border-input" placeholder="الأسم"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="cvName" runat="server" Display="Dynamic" ValidateEmptyText="true" SetFocusOnError="true" Enabled="true" ControlToValidate="txtName" ForeColor="Red" OnServerValidate="cvName_ServerValidate" ValidationGroup="vgSave" ErrorMessage="الاسم مكرر"></asp:CustomValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    صلاحيات المجموعة
                                </label>
                                <div class="col-sm-8">
                                     <dx:ASPxTreeView ID="tvPermissions" ShowExpandButtons="true" EnableHotTrack="true" EnableAnimation="true" ShowTreeLines="true" CheckNodesRecursive="true" Width="100%" Theme="Metropolis" AllowSelectNode="true" AllowCheckNodes="true" runat="server"></dx:ASPxTreeView>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                             <asp:LinkButton ID="lnkSubmitEditGroup" OnClick="lnkSubmitEditGroup_Click" runat="server" CssClass="btn btn-primary ml-2" ValidationGroup="vgSave">حفــــــظ</asp:LinkButton>
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script>
        function onClickSubmitGroup() {
            if (!Page_ClientValidate("vgSave")) { $("#GroupModal").modal("show"); return false; }
        }
    </script>
</asp:Content>

