<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="meetingsAgenda.aspx.cs" Inherits="meetingsAgenda" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
                <h3 class="w-50 float-left card-title m-0">  
                    <asp:Literal ID="ltrCommitte" runat="server"></asp:Literal>
                    <i class="fa fa-arrow-circle-o-left"></i>عــدد اجندة الاجتماع (
                    <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                    )</h3>
                <div class="dropdown dropleft text-right w-50 float-right">
                    <asp:LinkButton ID="lnkAddNew" runat="server" OnClick="lnkAddNew_Click" CssClass="btn btn-primary">اجندة جــديــدة</asp:LinkButton>
                </div>
            </div>
            <section class="widget-card">
                <div class="row">
                    <asp:Repeater ID="rpData" runat="server">
                        <ItemTemplate>
                            <div class="col-lg-4 col-xl-4 mt-3">
                                <div class="card">
                                    <div class="card-body">
                                        <h5 class="card-title mb-2"><%# Eval("Agenda_Item") %></h5>
                                        <p class="card-text text-mute"><span class='<%# Eval("Agenda_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Agenda_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></p>
                                        <p class="card-text text-mute text-14 text-center">
                                            <span class="mr-3">عدد المناقشات : <%# Eval("DiscussionCount") %></span>
                                            <span class="mr-3">عدد التوصيات : <%# Eval("ReccommondationCount") %></span>
                                        </p>
                                        <p class="text-center">
                                            <asp:LinkButton ID="lnkDiscussion" CommandArgument='<%# Eval("Agenda_MeetingID") %>' runat="server" ToolTip="المناقشات" CssClass="btn btn-outline-success btn-icon m-1" OnCommand="lnkDiscussion_Command"><span class="ul-btn__icon"><i class="fa fa-align-justify"></i></span></asp:LinkButton>
                                            <asp:LinkButton ID="lnkRecommendation" CommandArgument='<%# Eval("Agenda_MeetingID") %>' runat="server" ToolTip="التوصيات" CssClass="btn btn-outline-danger btn-icon m-1" OnCommand="lnkRecommendation_Command"><span class="ul-btn__icon"><i class="fa fa-table"></i></span></asp:LinkButton>                                       
                                            <asp:LinkButton ID="lnkEdit" CommandArgument='<%# Eval("Agenda_ID") %>' runat="server" ToolTip="تعديل" CssClass="btn btn-outline-warning btn-icon m-1" OnCommand="lnkEdit_Command"><span class="ul-btn__icon"><i class="fa fa-edit"></i></span></asp:LinkButton>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
            <div class="modal fade" id="AgendaModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-2">اجندة</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم الاجندة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    أولوية المناقشة
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList ID="ddlDiscussionPriority" CssClass="form-control" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvDiscussionPriority" runat="server" Display="Dynamic" ControlToValidate="ddlDiscussionPriority" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    الملاحظات
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtNotes" TextMode="MultiLine" Rows="3" CssClass="form-control" runat="server"></asp:TextBox>
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
         <%--   <div class="modal fade" id="DiscussionAddModal" tabindex="-1" role="dialog" aria-hidden="true">
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
            <div class="modal fade" id="DiscussionModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">المناقشات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddDiscussion" CssClass="btn btn-primary" runat="server" OnClick="lnkAddDiscussion_Click"><i class="fa fa-plus"></i></asp:LinkButton>
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
                                                                            <asp:LinkButton ID="lnkDeleteDiscussion" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Goal_ID") %>' OnCommand="lnkDeleteDiscussion_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="lnkEditDiscussion" runat="server" CommandArgument='<%# Eval("Goal_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditDiscussion_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton></td>
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
            <div class="modal fade" id="RecommondationAddModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-3">التوصيات </h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم التوصية
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
                                    <asp:TextBox ID="txtCode" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" Display="Dynamic" ControlToValidate="txtCode" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4">
                                    المسئول عن التنفيذ
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtResponsibleForImplementation"  CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvResponsibleForImplementation" runat="server" Display="Dynamic" ControlToValidate="txtResponsibleForImplementation" ValidationGroup="vgSaveRec" ForeColor="Red">*</asp:RequiredFieldValidator>

                                </div>
                            </div>
                              <div class="form-group row">
                                <label class="col-sm-4">
                                    نسبة التنفيذ
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtProgress"  CssClass="form-control" runat="server"></asp:TextBox>
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
            <div class="modal fade" id="RecommondationModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="card o-hidden mb-4">
                                <div class="card-header d-flex align-items-center">
                                    <h3 class="w-50 float-left card-title m-0">التوصيات</h3>
                                    <div class="dropdown dropleft text-right w-50 float-right">
                                        <asp:LinkButton ID="lnkAddRecommondation" CssClass="btn btn-primary" runat="server" OnClick="lnkAddRecommondation_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                        <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <div id="user_table_wrapper1" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table1" role="grid" aria-describedby="user_table_info">
                                                        <thead>
                                                            <tr role="row">
                                                                <th scope="col" style="width: 11px;">#</th>
                                                                <th scope="col" style="width: 62px;">التوصيه</th>
                                                                <th scope="col" style="width: 62px;">كود التوصية</th>
                                                                <th scope="col" style="width: 62px;">المسؤل عن التنفيذ</th>
                                                                <th scope="col" style="width: 62px;">نسبه التنفيذ</th>
                                                                <th scope="col" style="width: 62px;">الحالة</th>
                                                                
                                                                <th scope="col" style="width: 49px;"><i class="fa fa-cogs"></i></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="Repeater1" runat="server">
                                                                <ItemTemplate>
                                                                    <tr role="row" class="odd">
                                                                        <th><%# Container.ItemIndex + 1 %></th>
                                                                        <td><%# Eval("Goal_Text") %></td>
                                                                        <td><%# Eval("Goal_Text") %></td>
                                                                        <td><%# Eval("Goal_Text") %></td>
                                                                        <td><%# Eval("Goal_Text") %></td>
                                                                        <td><%# Eval("Goal_Text") %></td>

                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteRecommondation" OnClientClick="return confirm('هل أنت متأكد من اتمام الحذف؟');" runat="server" CssClass="text-danger mr-2" CommandArgument='<%# Eval("Goal_ID") %>' OnCommand="lnkDeleteRecommondation_Command"><i class="nav-icon i-Close-Window font-weight-bold"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="lnkEditRecommondation" runat="server" CommandArgument='<%# Eval("Goal_ID") %>' CssClass="text-success mr-2" OnCommand="lnkEditRecommondation_Command"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton></td>
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
            </div>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

