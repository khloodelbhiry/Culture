<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width,initial-scale=1" />
    <meta http-equiv="X-UA-Compatible" content="ie=edge" />
    <title>تسجيل الدخول</title>
    <link href="https://fonts.googleapis.com/css?family=Nunito:300,400,400i,600,700,800,900" rel="stylesheet" />
    <link href="App_Themes/css/lite-purple.min.css" rel="stylesheet" />
    <link href="App_Themes/css/perfect-scrollbar.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="auth-layout-wrap">
            <div class="auth-content">
                <div class="card o-hidden">
                    <div class="row">
                        <%--background-image: url(App_Themes/images/download.png)--%>
                        <div class="col-md-12 text-center" style="background-size: cover; ">
                            <div class="p-4">
                                <div class="auth-logo text-center mb-4">
                                    <img src="App_Themes/images/logo.png" alt="">
                                </div>
                                <h1 class="mb-3 text-24">الإدارة الإلكترونية للجان المجلس</h1>
                                <div class="form-group">
                                    <label>البريد الألكترونى</label>
                                    <asp:TextBox CssClass="form-control form-control-rounded" ID="txtEmail" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ForeColor="Red" ValidationGroup="vgSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <label>كلمة السر</label>
                                    <asp:TextBox CssClass="form-control form-control-rounded" ID="txtPassword" TextMode="Password" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ForeColor="Red" ValidationGroup="vgSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                                <asp:LinkButton ID="lnkSignIn" runat="server" OnClick="lnkSignIn_Click" CssClass="btn btn-rounded btn-primary btn-block mt-2" Text="تسجيل الدخول"></asp:LinkButton>
                                <div class="mt-3 text-center">
                                    <a class="text-muted" href="forgot.html">
                                        <u>نسيت كلمة السر?</u></a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
