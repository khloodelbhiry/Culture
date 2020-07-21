<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="committee.aspx.cs" Inherits="committee" %>

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
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card-header d-flex align-items-center border-0">
                <h3 class="w-50 float-left card-title m-0">عــدد الـلــجــان (
                    <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                    )</h3>
                <div class="dropdown dropleft text-right w-50 float-right">
                    <asp:LinkButton ID="lnkAddNew" runat="server" OnClick="lnkAddNew_Click" CssClass="btn btn-primary">انــشــاء لــجــنــة</asp:LinkButton>
                </div>
            </div>
            <section class="widget-card">
                <div class="row">
                    <asp:Repeater ID="rpData" runat="server">
                        <ItemTemplate>
                            <div class="col-lg-4 col-xl-4 mt-3">
                                <div class="card">
                                    <div class="card-body">
                                        <center>
                                         <h3 class="mb-3">
                                            <asp:LinkButton ID="lnkDetails" runat="server" PostBackUrl='<%# "committee-details.aspx?id="+Eval("Committee_ID") %>'><%# Eval("Committee_Name") %></asp:LinkButton>
                                        </h3></center>
                                        <%--<h5 class="card-title mb-2"><%# Eval("Committee_Name") %></h5>--%>
                                        <%--<p class="card-text text-mute">عدد الأعضاء : <%# Eval("Committee_Code") %><span class='<%# Eval("Committee_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Committee_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></p>--%>
                                        <%-- <p class="card-text text-mute text-14 text-center">
                                            <span class="mr-3">عدد الأعضاء : <%# Eval("MembersCount") %></span>
                                            <span class="mr-3">عدد الأهداف : <%# Eval("GoalCount") %></span>
                                            <span class="mr-3">عدد الأجتماعات : <%# Eval("MeetingsCount") %></span>
                                        </p>--%>
                                        <p class="card-text text-18"><span style="color:rebeccapurple; float: right; margin-bottom: 1rem;">كود اللجنة : </span><span style="float: right;"><%# Eval("Committee_Code") %></span><span class='<%# Eval("Committee_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Committee_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></p>
                                        <div class="clearfix"></div>
                                        <p class="card-text text-18">
                                            <span style="color:rebeccapurple;">عدد الأعضاء : </span><%# Eval("MembersCount") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color:rebeccapurple;">عدد الأهداف : </span><%# Eval("GoalCount") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color:rebeccapurple;">عدد الأجتماعات : </span><%# Eval("MeetingsCount") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color:rebeccapurple;">نسبة حضور الأعضاء : </span><%#  Math.Round(decimal.Parse(Eval("Attendance").ToString()),2).ToString("G29")+" %" %>
                                        </p>
                                        <p class="text-center">
                                            <asp:LinkButton ID="lnkGoals" CommandArgument='<%# Eval("Committee_ID") %>' runat="server" ToolTip="الأهداف" CssClass="btn btn-outline-success btn-icon m-1" OnCommand="lnkGoals_Command"><span class="ul-btn__icon"><i class="fa fa-align-justify"></i></span></asp:LinkButton>
                                            <a href='<%# "members.aspx?id="+Eval("Committee_ID") %>' title="الأعضاء" class="btn btn-outline-primary btn-icon m-1"><span class="ul-btn__icon"><i class="fa fa-users"></i></span></a>
                                            <%--<a href='<%# "rewards.aspx?id="+Eval("Committee_ID") %>' title="المكافأت" class="btn btn-outline-danger btn-icon m-1"><span class="ul-btn__icon"><i class="fa fa-money"></i></span></a>--%>
                                            <asp:LinkButton ID="lnkAttachments" CommandArgument='<%# Eval("Committee_ID") %>' runat="server" ToolTip="المرفقات" CssClass="btn btn-outline-secondary btn-icon m-1" OnCommand="lnkAttachments_Command"><span class="ul-btn__icon"><i class="fa fa-paperclip"></i></span></asp:LinkButton>
                                            <asp:LinkButton ID="btnEdit" CommandArgument='<%# Eval("Committee_ID") %>' runat="server" ToolTip="تعديل" CssClass="btn btn-outline-warning btn-icon m-1" OnCommand="btnEdit_Command"><span class="ul-btn__icon"><i class="fa fa-edit"></i></span></asp:LinkButton>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
            <div class="modal fade" id="committeeModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-2">اللــجــان</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم اللجنة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    كود اللجنة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtCode" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    نبذة عن اللجنة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAbout" TextMode="MultiLine" Rows="3" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAbout" runat="server" Display="Dynamic" ControlToValidate="txtAbout" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    الملاحظات
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtNotes" TextMode="MultiLine" Rows="2" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSave_Click" ValidationGroup="vgSave" />
                            <asp:Button ID="btnApprove" runat="server" CssClass="btn btn-sm btn-success ml-2" Text="اعــتـمــاد" OnClick="btnApprove_Click" Visible="false" />
                            <asp:Button ID="btnFreeze" runat="server" CssClass="btn btn-danger ml-2" Text="تــجــمــيــد" OnClick="btnFreeze_Click" Visible="false" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="goalsAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">الأهداف</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    الهدف
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtGoal" TextMode="MultiLine" Rows="2" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvGoal" runat="server" Display="Dynamic" ControlToValidate="txtGoal" ValidationGroup="vgSaveGoal" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveGoal" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveGoal_Click" ValidationGroup="vgSaveGoal" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="goalsModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">الأهداف</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddGoal" CssClass="btn btn-primary" runat="server" OnClick="lnkAddGoal_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="user_table_wrapper" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 11px;">#</th>
                                                                <th scope="col" style="width: 62px;"></th>
                                                                <th scope="col" style="width: 49px;"><i class="fa fa-cogs"></i></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rpGoals" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <th><%# Container.ItemIndex + 1 %></th>
                                                                        <td><%# Eval("Goal_Text") %></td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteGoal" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Goal_ID") %>' OnCommand="lnkDeleteGoal_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="lnkEditGoal" runat="server" CommandArgument='<%# Eval("Goal_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditGoal_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton></td>
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
            <div class="modal fade" id="attachmentsModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">المرفقات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddAttachment" CssClass="btn btn-primary" runat="server" OnClick="lnkAddAttachment_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="attachments" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
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
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSaveAttachment" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

