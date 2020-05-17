<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="查询交通情况.aspx.cs" Inherits="Presentation.查询交通情况" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="DropDownList1" runat="server">
            </asp:DropDownList>
        </div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
        <br />
        <asp:Image ID="Image1" runat="server" Height="800px" style="margin-bottom: 24px" Width="800px" ImageUrl="C:\Users\sharuicheng\source\repos\Presentation\Presentation\data\temp.jpg" />
    </form>
</body>
</html>
