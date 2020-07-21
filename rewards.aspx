<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="rewards.aspx.cs" Inherits="meetings" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/css/bootstrap-datepicker3.min.css" rel="stylesheet" />
    <link href="App_Themes/css/bootstrap-timepicker.min.css" rel="stylesheet" />
    <link href="App_Themes/css/select2.min.css" rel="stylesheet" />
    <style>
        .datepicker-days {
            display: block !important;
        }

        .select2-container .select2-selection--single {
            height: 43px !important;
            width: 100% !important;
            direction: rtl;
            float: right !important;
        }

        .select2 {
            font-size: 15px;
            line-height: 1.428571429;
            color: #333;
            width: 100% !important;
        }

        .select2-results {
            text-align: right !important;
            font-size: 15px;
            line-height: 1.428571429;
            color: #333;
        }

        .select2-container--default .select2-selection--single {
            border: 1px solid #ccc !important;
            border-radius: 0px !important;
            width: 100% !important;
        }

        .select2-selection__arrow {
            left: 1px !important;
        }

        .select2-search__field {
            font-size: 15px;
            line-height: 1.428571429;
            color: #333;
            direction: rtl;
            text-align: right !important;
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
                    <i class="fa fa-arrow-circle-o-left"></i>عــدد المكافأت (
                    <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                    )</h3>
                <div class="dropdown dropleft text-right w-50 float-right">
                    <asp:LinkButton ID="lnkAddNew" runat="server" OnClick="lnkAddNew_Click" CssClass="btn btn-primary">اضــافــة مــكــافــأة اسـتـثـنـائـيـة</asp:LinkButton>
                    <asp:LinkButton ID="lnkPayment" runat="server" OnClick="lnkPayment_Click" CssClass="btn btn-light">صــرف الــمــكــافــأت</asp:LinkButton>
                </div>
            </div>
            <br />
            <section class="widget-card">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="card">
                            <div class="card-body">
                    <div class="row">
                        <div class="clearfix"></div>
                        <div class="form-group col-lg-6">
                            <label class="col-sm-4 col-form-label">
                                التاريخ من
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtDateFromSrc" CssClass="form-control" runat="server" data-date-format="dd-mm-yyyy"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-lg-6">
                            <label class="col-sm-4 col-form-label">
                                الى
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtDateToSrc" CssClass="form-control" runat="server" data-date-format="dd-mm-yyyy"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-lg-6">
                            <label class="col-sm-4 col-form-label">
                                العضو
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtMemberSrc" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-lg-6">
                            <label class="col-sm-4 col-form-label">
                                الحالة
                            </label>
                            <div class="col-sm-8">
                                <asp:DropDownList ID="ddlStatusSrc" runat="server" CssClass="form-control"></asp:DropDownList>
                                <label class="checkbox checkbox-success">
                                    <input type="checkbox" name="check" value="1" runat="server" id="chkException"><span> استثنائية </span><span class="checkmark"></span>
                                </label>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                    <div class="col-lg-12 ">
                        <div class="row pull-left">
                            <asp:LinkButton ID="LnkSearch" runat="server" OnClick="LnkSearch_Click" CssClass="btn btn-primary mr-2">بــحــث</asp:LinkButton>
                            <asp:LinkButton ID="LnkNewSearch" runat="server" OnClick="LnkNewSearch_Click" CssClass="btn btn-light mr-3">بــحــث جــديــد</asp:LinkButton>
                        </div>
                        </div>
                    </div>
                                </div>
                            </div>
                    <div class="clearfix"></div>
                    <br />
                    <div class="clearfix"></div>
                    <div class="col-sm-12">
                        <div class="card mt-3">
                            <div class="card-body">
                    <div class="col-sm-12">
                        <div class="table-responsive">
                            <div id="user_table_wrapper2" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table22" role="grid" aria-describedby="user_table_info">
                                            <thead>
                                                <tr role="row">
                                                    <th scope="col"></th>
                                                    <th scope="col" style="width: 5%;">#</th>
                                                    <th scope="col">قيمة المكافأة</th>
                                                    <th scope="col">العضو</th>
                                                    <th scope="col">اللجنة</th>
                                                    <th scope="col">التاريخ</th>
                                                    <th scope="col">حالة الدفع</th>
                                                    <th scope="col">تاريخ الدفع</th>
                                                    <th scope="col" style="width: 10%;"><i class="fa fa-cogs"></i></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="rpRewards" runat="server">
                                                    <ItemTemplate>
                                                        <tr role="row" class="odd">
                                                            <td>
                                                                <label class="checkbox checkbox-success" <%# Eval("Reward_PaymentStatusID").ToString()=="1"?"":"style='display:none;'"%>>
                                                                    <input type="checkbox" name="check" value='<%# Eval("Reward_ID")%>' runat="server" id="check"><span class="checkmark"></span>
                                                                </label>
                                                            </td>
                                                            <th><%# Eval("Reward_ID") %></th>
                                                            <td><%# Eval("Reward_Value") %> <span class='<%# Eval("Reward_IsException").ToString()=="True"?"badge badge-danger float-right":string.Empty %>'><%# Eval("Reward_IsException").ToString()=="True"?"استثنائية":""%></span></td>
                                                            <td><%# Eval("Member_Name") %></td>
                                                            <td><%# Eval("Committee_Name") %></td>
                                                            <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Reward_Date"))%></td>
                                                            <td><%# Eval("PaymentStatus_Name") %></td>
                                                            <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Reward_PaymentDate"))%></td>
                                                            <td>
                                                                <asp:LinkButton ID="lnkAttachments" CommandArgument='<%# Eval("Reward_ID") %>' runat="server" ToolTip="المرفقات" CssClass="text-danger mr-2" OnCommand="lnkAttachments_Command"><span class="ul-btn__icon"><i class="fa fa-paperclip"></i></span></asp:LinkButton></td>
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
            </section>
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

            <div class="modal fade" id="rewardAddModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">مكافأت استثنائية</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    العضو
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList ID="ddlMember" runat="server" CssClass="form-control select2"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="ddlMember" InitialValue="0" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    قيمة المكافأة
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtValue" TextMode="Number" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbe" runat="server"
                                        TargetControlID="txtValue"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <asp:RequiredFieldValidator ID="rfvValue" runat="server" Display="Dynamic" ControlToValidate="txtValue" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSaveReward" runat="server" CssClass="btn btn-primary ml-2" Text="حفــــــظ" OnClick="btnSaveReward_Click" ValidationGroup="vgSave" />
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
                        </div>
                    </div>
                </div>
            </div>
            <script src="App_Themes/js/select2.min.js"></script>
            <script src="App_Themes/js/bootstrap-timepicker.min.js"></script>
            <script src="App_Themes/js/bootstrap-datepicker.min.js"></script>
            <script type="text/javascript">
                $(document).ready(function () {
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_initializeRequest(InitializeRequest);
                    prm.add_endRequest(EndRequest);
                    initiateTimePicker();
                    $('.select2').select2();
                });
                function InitializeRequest() {
                }
                function EndRequest() {
                    initiateTimePicker();
                    $('.select2').select2();
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

            </script>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSaveAttachment" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

