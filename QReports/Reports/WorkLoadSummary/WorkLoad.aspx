<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkLoad.aspx.cs" Inherits="QReports.Reports.WorkLoadSummary.WorkLoad" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="rvWorkLoad" runat="server" ZoomMode="PageWidth" Height="764px" Width="1024px" SizeToReportContent="True">
        </rsweb:ReportViewer>
    </form>
</body>
</html>
