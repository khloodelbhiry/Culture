<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="Users" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
    <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card-header d-flex align-items-center border-0">
                <h3 class="w-50 float-left card-title m-0">عــدد المستخدمين (
                    <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                    )</h3>
                <div class="dropdown dropleft text-right w-50 float-right">
                    <asp:LinkButton ID="btnNewUser" runat="server" OnClick="btnNewUser_Click" CssClass="btn btn-primary">مــســتــخــدم جــديــد</asp:LinkButton>
                </div>
            </div>
            <br />
            <section class="widget-card">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="form-group col-lg-6">
                                        <label class="col-sm-4 col-form-label">
                                            الاسم
                                        </label>
                                        <div class="col-sm-8">
                                            <asp:TextBox ID="txtNameSrch" CssClass="form-control border-input" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-6">
                                        <label class="col-sm-4 col-form-label">
                                            البريد الألكتروني
                                        </label>
                                        <div class="col-sm-8">
                                            <asp:TextBox ID="txtEmailSrch" CssClass="form-control border-input" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-6">
                                        <label class="col-sm-4 col-form-label">
                                            رقم الهاتف
                                        </label>
                                        <div class="col-sm-8">
                                            <asp:TextBox ID="txtMobileSrc1" CssClass="form-control border-input" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-6">
                                        <label class="col-sm-4 col-form-label">
                                            اللجنة
                                        </label>
                                        <div class="col-sm-8">
                                            <asp:DropDownList ID="ddlCommitteeSrc" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-6">
                                        <label class="col-sm-4 col-form-label">
                                            المجموعة
                                        </label>
                                        <div class="col-sm-8">
                                            <asp:DropDownList ID="ddlGroupSrc" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-6">
                                        <label class="col-sm-4 col-form-label">
                                            الحالة
                                        </label>
                                        <div class="col-sm-8">
                                            <asp:DropDownList ID="ddlStatusSrch" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="-1" Text="اختر"></asp:ListItem>
                                                <asp:ListItem Value="1" Text="قيد الاعتماد"></asp:ListItem>
                                                <asp:ListItem Value="2" Text="معتمد"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="مجمد"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="col-lg-12 ">
                                        <div class="row pull-left">
                                            <asp:LinkButton ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="btn btn-primary mr-2">بــحــث</asp:LinkButton>
                                            <asp:LinkButton ID="btnNewSearch" runat="server" OnClick="btnNewSearch_Click" CssClass="btn btn-light mr-3">بــحــث جــديــد</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-12 mt-3">
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="table-responsive">
                                        <div id="user_table_wrapper2" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table22" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 11px;">#</th>
                                                                <th scope="col">عضو</th>
                                                                <th scope="col">الاسم بالكامل</th>
                                                                <th scope="col">البريد الألكترونى</th>
                                                                <th scope="col">رقم الهاتف</th>
                                                                <th scope="col">اللجنة</th>
                                                                <th scope="col">المجموعة</th>
                                                                <th scope="col">الحالة</th>
                                                                <th scope="col"><i class="fa fa-cogs"></i></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rpData" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <th><%# Eval("User_ID") %></th>
                                                                        <td>
                                                                            <label class="checkbox checkbox-success">
                                                                                <input type="checkbox" name="check" <%# Eval("User_MemberID")!=null?"checked":""%>><span class="checkmark"></span>
                                                                            </label>
                                                                        </td>
                                                                        <td><%# Eval("User_FullName") %></td>
                                                                        <td><%# Eval("User_Email") %></td>
                                                                        <td><%# Eval("User_Mobile1") %></td>
                                                                        <td><%# Eval("Committee_Name") %></td>
                                                                        <td><%# Eval("Group") %></td>
                                                                        <td><%# Eval("Status_Name") %></td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteUser" CssClass="text-danger mr-2" runat="server" CommandArgument='<%#Eval("User_ID") %>' OnCommand="lnkDeleteUser_Command" ToolTip="حذف" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" CausesValidation="false"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="lnkEditUser" runat="server" CssClass="text-success mr-2" CommandArgument='<%#Eval("User_ID") %>' OnCommand="lnkEditUser_Command" ToolTip="تعديل" CausesValidation="false"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton>
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
                <div class="modal fade" id="groupAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">المستخدمين</h5>
                            </div>
                            <div class="modal-body">
                                <div class="form-group row">
                                    <label class="col-sm-4 col-form-label">
                                        العضو
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="ddlMember" AutoPostBack="true" OnSelectedIndexChanged="ddlMember_SelectedIndexChanged" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-4 col-form-label">
                                        الأسم بالكامل
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control border-input" placeholder="الأسم بالكامل"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvFullName" runat="server" Display="Dynamic" ControlToValidate="txtFullName" ValidationGroup="vgRegister" ForeColor="Red">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="cvFullName" runat="server" OnServerValidate="cvFullName_ServerValidate" ErrorMessage="الأسم مكرر" SetFocusOnError="true" Display="Dynamic" ControlToValidate="txtFullName" ValidationGroup="vgRegister" ForeColor="Red"></asp:CustomValidator>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-4 col-form-label">
                                        البريد الألكترونى
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtRegUsername" runat="server" CssClass="form-control border-input" placeholder="البريد الألكترونى"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvRegUsername" runat="server" Display="Dynamic" ControlToValidate="txtRegUsername" ValidationGroup="vgRegister" ForeColor="Red">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="cvRegUsername" runat="server" OnServerValidate="cvRegUsername_ServerValidate" ErrorMessage="البريد لألكترونى مكرر" SetFocusOnError="true" Display="Dynamic" ControlToValidate="txtRegUsername" ValidationGroup="vgRegister" ForeColor="Red"></asp:CustomValidator>
                                        <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="txtRegUsername" ErrorMessage="البريد الألكترونى غير صحيح" ValidationGroup="vgRegister" ForeColor="Red"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-4 col-form-label">
                                        رقم الهاتف
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtMobile" TextMode="Number" runat="server" CssClass="form-control border-input" placeholder="رقم الهاتف"></asp:TextBox>
                                        <asp:CustomValidator ID="cvMobile" runat="server" OnServerValidate="cvMobile_ServerValidate" ErrorMessage="رقم الهاتف مكرر" SetFocusOnError="true" Display="Dynamic" ControlToValidate="txtMobile" ValidationGroup="vgRegister" ForeColor="Red"></asp:CustomValidator>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-4 col-form-label">
                                        اللجنة
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList runat="server" ID="ddlCommittee" CssClass="form-control border-input" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-4 col-form-label">
                                        المجموعة
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList runat="server" ID="ddlGroup" CssClass="form-control border-input" />
                                        <asp:RequiredFieldValidator ID="rfvGroup" runat="server" InitialValue="-1" ControlToValidate="ddlGroup" ValidationGroup="vgRegister" Display="Dynamic" ForeColor="Red">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:LinkButton ID="lnkSubmitEditUser" OnClick="lnkSubmitEditUser_Click" runat="server" CssClass="btn btn-primary ml-2" ValidationGroup="vgRegister">حفــــــظ</asp:LinkButton>
                                <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

