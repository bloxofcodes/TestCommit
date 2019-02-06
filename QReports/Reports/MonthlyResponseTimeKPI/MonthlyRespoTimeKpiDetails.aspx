<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyRespoTimeKpiDetails.aspx.cs" Inherits="QReports.Reports.MonthlyResponseTimeKPI.MonthlyRespoTimeKpiDetails" %>

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
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="546px" Width="995px">
        </rsweb:ReportViewer>
    </form>
</body>
</html>
