﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResponseTimeKpiSumm.aspx.cs" Inherits="QReports.Reports.ResponseTimeKPISummary.ResponseTimeKpiSumm" %>

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
        <rsweb:ReportViewer ID="rvRespoTimeKpiSumm" runat="server" Height="600px" InteractivityPostBackMode="SynchronousOnDrillthrough" Width="999px">
        </rsweb:ReportViewer>
    </form>
</body>
</html>
