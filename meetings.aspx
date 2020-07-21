<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="meetings.aspx.cs" Inherits="meetings" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/css/bootstrap-datepicker3.min.css" rel="stylesheet" />
    <link href="App_Themes/css/bootstrap-timepicker.min.css" rel="stylesheet" />
    <style>
        .datepicker-days {
            display: block !important;
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
            <div class="card-header d-flex align-items-center border-0">
                <h3 class="w-50 float-left card-title m-0">
                    <asp:Literal ID="ltrCommitte" runat="server"></asp:Literal>
                    <i class="fa fa-arrow-circle-o-left"></i>عــدد الاجتماعات (
                    <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                    )</h3>
                <div class="dropdown dropleft text-right w-50 float-right">
                    <asp:LinkButton ID="lnkAddNew" runat="server" OnClick="lnkAddNew_Click" CssClass="btn btn-primary">اجتماع جــديــد</asp:LinkButton>
                </div>
            </div>
            <br />
            <section class="widget-card">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="row">
                            <div class="table-responsive">
                                <div id="user_table_wrapper2" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table22" role="grid" aria-describedby="user_table_info">
                                                <thead>
                                                    <tr role="row">
                                                        <th scope="col" style="width: 11px;">الكود</th>
                                                        <th scope="col" style="width: 62px;">تاريخ الاجتماع</th>
                                                        <th scope="col" style="width: 62px;">وقت الاجتماع من</th>
                                                        <th scope="col" style="width: 62px;">الى</th>
                                                        <th scope="col" style="width: 62px;">المكان</th>
                                                        <th scope="col" style="width: 62px;">النوع</th>
                                                        <th scope="col" style="width: 62px;">مكافأة المقرر</th>
                                                        <th scope="col" style="width: 62px;">مكافأة العضو</th>
                                                        <th scope="col" style="width: 62px;">الحالة</th>
                                                        <th scope="col" style="width: 62px;"><i class="fa fa-cogs"></i></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="rpData" runat="server">
                                                        <ItemTemplate>
                                                            <tr role="row" class="odd">
                                                                <th>
                                                                    <asp:LinkButton ID="lnkMeeting" runat="server" PostBackUrl='<%#"meeting-details.aspx?id="+Eval("Meeting_ID") %>'><%# Eval("Meeting_Code") %></asp:LinkButton>
                                                                </th>
                                                                <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Meeting_Date")) %></td>
                                                                <td><%# Eval("Meeting_TimeFrom") %></td>
                                                                <td><%# Eval("Meeting_TimeTo") %></td>
                                                                <td><%# Eval("Meeting_Place") %></td>
                                                                <td><%# Eval("Type_Name") %></td>
                                                                <td><%# decimal.Parse(Eval("Meeting_RewardNotMember").ToString()).ToString("G29") %></td>
                                                                <td><%# decimal.Parse(Eval("Meeting_RewardMember").ToString()).ToString("G29") %></td>
                                                                <td><%# Eval("Status_Name") %></td>
                                                                <td>
                                                                    <asp:LinkButton ID="lnkEditmeeting" runat="server" CssClass="text-success mr-2" CommandArgument='<%#Eval("Meeting_ID") %>' OnCommand="lnkEditmeeting_Command" ToolTip="تعديل" CausesValidation="false"><i class="nav-icon i-Pen-2 font-weight-bold"></i></asp:LinkButton>
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
            </section>
            <div class="modal fade" id="meetingModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-2">الاجتماعات</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    تاريخ الاجتماع
                                </label>
                                <div class="col-sm-8">
                                    <input id="txtDate" runat="server" class="form-control date-picker" type="text" data-date-format="MM-dd-yyyy" />
                                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" Display="Dynamic" ControlToValidate="txtDate" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    وقت الاجتماع من
                                </label>
                                <div class="col-sm-3">
                                    <input id="txtTimeFrom" runat="server" type="text" class="form-control date-picker" />
                                </div>
                                <label class="col-sm-2 col-form-label">
                                    الي
                                </label>
                                <div class="col-sm-3">
                                    <input id="txtTimeTo" runat="server" type="text" class="form-control date-picker" />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    كود الاجتماع
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtCode" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rfvCode" runat="server" Display="Dynamic" ControlToValidate="txtCode" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    مكان الاجتماع
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtPlace" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPlace" runat="server" Display="Dynamic" ControlToValidate="txtPlace" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    مكافأة المقرر
                                </label>
                                <div class="col-sm-3">
                                    <asp:TextBox ID="txtReward" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvReward" runat="server" Display="Dynamic" ControlToValidate="txtReward" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                                <label class="col-sm-2 col-form-label">
                                    العضو
                                </label>
                                <div class="col-sm-3">
                                    <asp:TextBox ID="txtRewardMember" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRewardMember" runat="server" Display="Dynamic" ControlToValidate="txtRewardMember" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row" id="divDatePostponedUntil" runat="server" visible="false">
                                <label class="col-sm-4 col-form-label">
                                    تاريخ التأجيل
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDatePostponedUntil" CssClass="form-control" runat="server" data-date-format="MM-dd-yyyy"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDatePostponedUntil" runat="server" Display="Dynamic" ControlToValidate="txtDatePostponedUntil" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    نوع الاجتماع
                                </label>
                                <div class="col-sm-4">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radio" value="1" runat="server" id="rdType1" checked><span>دورى</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-4">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radio" value="2" runat="server" id="rdType2"><span>طارئ</span><span class="checkmark"></span>
                                    </label>
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
                            <asp:Button ID="btnApprove" runat="server" CssClass="btn btn-success ml-2" Text="اعــتـمــاد" OnClick="btnApprove_Click" Visible="false" />
                            <asp:Button ID="btnFreeze" runat="server" CssClass="btn btn-danger ml-2" Text="تــجــمــيــد" OnClick="btnFreeze_Click" Visible="false" />
                            <asp:Button ID="btnPostponed" runat="server" CssClass="btn btn-danger ml-2" Text="تــأجــــيــل" OnClick="btnPostponed_Click" Visible="false" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <script src="App_Themes/js/bootstrap-timepicker.min.js"></script>
            <script src="App_Themes/js/bootstrap-datepicker.min.js"></script>
            <script type="text/javascript">
                $(document).ready(function () {
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_initializeRequest(InitializeRequest);
                    prm.add_endRequest(EndRequest);
                    initiateTimePicker();
                });
                function InitializeRequest() {
                }
                function EndRequest() {
                    initiateTimePicker();
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
                }

            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

