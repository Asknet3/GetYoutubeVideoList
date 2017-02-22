<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetList.aspx.cs" Inherits="GetYoutubeVideoList.GetList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title >-GetVideoList-</title>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" id="ScriptManager1"> </asp:ScriptManager>
        <div>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:Label ID="Status" runat="server"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        
        </div>
    </form>
</body>
</html>
