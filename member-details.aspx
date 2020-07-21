<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="member-details.aspx.cs" Inherits="committee" %>

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
                    <asp:Repeater ID="rpData" runat="server">
                        <ItemTemplate>
                            <div class="col-lg-4 col-xl-4">
                                <div class="card card-profile-1">
                                    <div class="card-body">
                                        <div class="avatar box-shadow-2 mb-3">
                                            <img src='<%# Eval("Member_Avatar") %>' alt="">
                                        </div>
                                        <center>
                                        <h2 class="m-0"><%# Eval("Member_Name") %></h2></center>
                                        <p class="card-text text-18"><span style="color: rebeccapurple; float: right; margin-bottom: 1rem;">كود العضو : </span><span style="float: right;"><%# Eval("Member_Code") %></span><span class='<%# Eval("Member_StatusID").ToString()=="1"?"badge badge-primary float-right":(Eval("Member_StatusID").ToString()=="2"?"badge badge-success float-right":"badge badge-danger float-right") %>'><%# Eval("Status_Name") %></span></p>
                                        <div class="clearfix"></div>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">النوع : </span><%# Eval("MemberType_Name") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">الدور : </span><%# Eval("Role_Name") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">صفة العضوية : </span><%# Eval("MembershipStatus_Name") %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">نسبة الحضور : </span><%#  Math.Round(decimal.Parse(Eval("Attendance").ToString()),2).ToString("G29")+" %" %>
                                        </p>
                                        <p class="card-text text-18">
                                            <span style="color: rebeccapurple;">المستحقات المالية : </span><%# decimal.Parse(Eval("Rewards").ToString()).ToString("G29")+" جنية" %>
                                        </p>
                                        <asp:Repeater ID="rpPhones" runat="server" DataSource='<%# Eval("phones") %>'>
                                            <ItemTemplate>
                                                <p class="card-text text-18"><i class="fa fa-phone" style="color: rebeccapurple;"></i>&nbsp;<%# Eval("MemberPhone_Number") %> <i class="fa fa-whatsapp" <%# Eval("MemberPhone_WhatsApp").ToString()=="False"?"style='display:none;'":"style='color: green;'" %>></i></p>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <p class="card-text text-18"><i class="fa fa-envelope-open" style="color: rebeccapurple;"></i>&nbsp;<%# Eval("Member_Email") %></p>
                                        <p class="card-text text-18"><i class="fa fa-map-marker" style="color: rebeccapurple;"></i>&nbsp;<%# Eval("Member_Address") %></p>
                                        <p class="text-center">
                                            <asp:LinkButton ID="lnkEdit" Visible='<%# Eval("Member_StatusID").ToString()=="1" %>' CommandArgument='<%# Eval("Member_ID") %>' runat="server" ToolTip="تعديل" CssClass="btn btn-outline-warning btn-icon m-1" OnCommand="lnkEdit_Command"><span class="ul-btn__icon"><i class="fa fa-edit"></i></span></asp:LinkButton>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="col-lg-8">
                        <div class="card">
                            <div class="card-body">
                                <ul class="nav nav-pills" id="myPillTab" role="tablist">
                                    <li class="nav-item"><a class="nav-link active show" id="rewards" data-toggle="pill" href="#rewardsPIll" role="tab" aria-controls="homePIll" aria-selected="true"><i class="fa fa-table mr-1"></i>المكافأت</a></li>
                                    <li class="nav-item"><a class="nav-link" id="attendance" data-toggle="pill" href="#attendancePIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-calendar mr-1"></i>الحضور</a></li>
                                    <li class="nav-item"><a class="nav-link" id="attachment" data-toggle="pill" href="#attachmentPIll" role="tab" aria-controls="contactPIll" aria-selected="false"><i class="fa fa-paperclip mr-1"></i>مرفقات</a></li>
                                </ul>
                                <div class="tab-content" id="myPillTabContent">
                                    <div class="tab-pane fade active show" id="rewardsPIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                        <div class="table-responsive">
                                            <div id="user_table_wrapper" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="user_table" role="grid" aria-describedby="user_table_info">
                                                            <thead>
                                                                <tr role="row">
                                                                    <th scope="col" style="width: 11px;">#</th>
                                                                    <th scope="col" style="width: 62px;">التاريخ</th>
                                                                    <th scope="col" style="width: 49px;">قيمة المكافأة</th>
                                                                    <th scope="col" style="width: 49px;">حالة الصرف</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="rpRewards" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr role="row" class="odd">
                                                                            <th><%# Container.ItemIndex + 1 %></th>
                                                                            <td><%# Eval("Reward_PaymentDate") %></td>
                                                                            <td><%# Eval("Reward_Value") %> <%# Eval("Reward_IsException").ToString()=="True"?"<span class='badge badge-success'>مكافأه استثنائية</span>":string.Empty%></td>
                                                                            <td><%# Eval("PaymentStatus_Name") %></td>
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
                                    <div class="tab-pane" id="attendancePIll" role="tabpanel" aria-labelledby="home-icon-pill">
                                        <div class="table-responsive">
                                            <div id="attendance1" class="dataTables_wrapper container-fluid dt-bootstrap4 no-footer">
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <table class="table table-striped dataTable-collapse text-center dataTable no-footer" id="usear_table1" role="grid" aria-describedby="user_table_info">
                                                            <thead>
                                                                <tr role="row">
                                                                    <th scope="col">الاجتماع</th>
                                                                    <th scope="col">التاريخ</th>
                                                                    <th scope="col">حضور</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="rpAttendance" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr role="row" class="odd">
                                                                            <td><%# Eval("Meeting_Code") %></td>
                                                                            <td><%# String.Format("{0:MM/dd/yyyy}", Eval("Meeting_Date")) %></td>
                                                                            <td><%# Eval("Attendance") %></td>
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
                                    <div class="tab-pane" id="attachmentPIll" role="tabpanel" aria-labelledby="home-icon-pill">
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
                                                                            <td><a href='<%# Eval("Attachment_File") %>' target="_blank"><%# Eval("Attachment_Name") %></a> </td>
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
            </section>
            <div class="modal fade" id="memberModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle-2" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalCenterTitle-2">الأعــضــاء</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    اسم العضو
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtName" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    صورة العضو
                                </label>
                                <div class="col-sm-8">
                                    <asp:FileUpload ID="fuAvatar" runat="server" />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    كود العضو
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtCode" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rfvCode" runat="server" Display="Dynamic" ControlToValidate="txtCode" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    النوع
                                </label>
                                <div class="col-sm-3">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radioType" value="1" runat="server" id="rdMale" checked><span>ذكر</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-3">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radioType" value="2" runat="server" id="rdFemale"><span>أنثى</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-2">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radioType" value="2" runat="server" id="rdUnit"><span>جهة</span><span class="checkmark"></span>
                                    </label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    الدور
                                </label>
                                <div class="col-sm-4">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radio" value="1" runat="server" id="rdRole1" checked><span>مقررا</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-4">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="radio" value="2" runat="server" id="rdRole2"><span>عضوا</span><span class="checkmark"></span>
                                    </label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-4 col-form-label">
                                    صفة العضوية
                                </label>
                                <div class="col-sm-4">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="status" value="1" onclick="fnCheck()" runat="server" id="rdStatus1" checked><span>بشخصه</span><span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="col-sm-4">
                                    <label class="radio radio-dark">
                                        <input type="radio" name="status" value="2" onclick="fnCheck()" runat="server" id="rdStatus2"><span>بصفته</span><span class="checkmark"></span>
                                    </label>
                                </div>
                            </div>
                            <div class="form-group row" id="divAs" runat="server" visible="false">
                                <label class="col-sm-4 col-form-label">
                                    بصفته
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAs" TextMode="MultiLine" Rows="2" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAs" Enabled="false" runat="server" Display="Dynamic" ControlToValidate="txtCode" ValidationGroup="vgSave" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="accordion" id="accordionExample">
                                <div class="card ul-card__border-radius">
                                    <div class="card-header">
                                        <h6 class="card-title mb-0"><a class="text-default" data-toggle="collapse" href="#accordion-item-group1" aria-expanded="true">بيانات الاتصال</a></h6>
                                    </div>
                                    <div class="collapse" id="accordion-item-group1" data-parent="#accordionExample" style="">
                                        <div class="card-body">
                                            <div class="form-group row">
                                                <label class="col-sm-4 col-form-label">
                                                    الهاتف المحمول
                                                </label>
                                                <div class="col-sm-8">
                                                    <div class="phone-list">
                                                        <div class="input-group phone-input">
                                                            <input type="text" name="phone[1]" class="form-control" value="<%= firstPhone %>" />
                                                            <span class="input-group-btn">
                                                                <label class="checkbox checkbox-success mt-2">
                                                                    <input type="checkbox" name="chkWhatsapp" value="1" <%= firstWhatsApp %>><span> واتس اب </span><span class="checkmark ml-1"></span>
                                                                </label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <button type="button" onclick="addPhone()" class="btn btn-success btn-sm btn-add-phone"><span class="glyphicon glyphicon-plus"></span>اضافة رقم اخر</button>
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <label class="col-sm-4 col-form-label">
                                                    البريد الألكترونى
                                                </label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <label class="col-sm-4 col-form-label">
                                                    التنبيهات
                                                </label>
                                                <div class="col-sm-3">
                                                    <label class="checkbox checkbox-success">
                                                        <input type="checkbox" name="check" value="1" runat="server" id="chkEmail"><span> الايميل </span><span class="checkmark"></span>
                                                    </label>
                                                </div>
                                                <div class="col-sm-2" style="padding-left: 0px; padding-right: 0px;">
                                                    <label class="checkbox checkbox-success">
                                                        <input type="checkbox" name="check" value="1" runat="server" id="chkSMS"><span> SMS </span><span class="checkmark"></span>
                                                    </label>
                                                </div>
                                                <div class="col-sm-3">
                                                    <label class="checkbox checkbox-success">
                                                        <input type="checkbox" name="check" value="1" runat="server" id="chkWhatsAppNotification"><span> واتس اب </span><span class="checkmark"></span>
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <label class="col-sm-4">
                                                    العنوان
                                                </label>
                                                <div class="col-sm-8">
                                                    <asp:TextBox ID="txtAddress" TextMode="MultiLine" Rows="2" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
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
                            <button class="btn btn-secondary ml-2" type="button" data-dismiss="modal" aria-label="Close" onclick="$('.modal-backdrop').remove();"><span aria-hidden="true">اغلاق</span></button>
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
            <asp:HiddenField ID="hdfIndex" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSaveAttachment" />
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>
    <script>
        function removePhone(para) {
            $(para).closest('.phone-input').remove();
        }
        function addPhone() {
            var index = $('.phone-input').length + 1;
            $('#<%=hdfIndex.ClientID %>').val(index);
            $('.phone-list').append('' +
                '<div class="input-group phone-input">' +
                '<input type="text" name="phone[' + index + ']" class="form-control" />' +
                '<span class="input-group-btn">' +
                '<label class="checkbox checkbox-success mt-2">' +
                '<input type="checkbox" name="chkWhatsapp" value="[' + index + ']"><span> واتس اب </span><span class="checkmark ml-1"></span>' +
                '</label>' +
                '</span>' +
                '<span class="input-group-btn mt-2 ml-2">' +
                '<a class="text-danger" onclick="removePhone(this)"><i class="nav-icon i-Close-Window font-weight-bold"></i></a>' +
                '</span>' +
                '</div>'
            );
        }
        function fnCheck() {
            if ($("input[type='radio'][id*='rdStatus1']").is(":checked")) {
                document.getElementById('ContentPlaceHolder1_divAs').style.display = "none";
            }
            else {
                document.getElementById('ContentPlaceHolder1_divAs').style.display = "flex";
            }
        }
    </script>
</asp:Content>

