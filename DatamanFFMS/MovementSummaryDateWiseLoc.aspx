<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="MovementSummaryDateWiseLoc" Codebehind="MovementSummaryDateWiseLoc.aspx.cs" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%@ Register TagPrefix="asp" Namespace="Saplin.Controls" Assembly="DropDownCheckBoxes" %>
<%@ Register Src="ctlCalendar.ascx" TagName="Calendar" TagPrefix="ctl" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
    <style type="text/css">
        .spinner {
            position: absolute;
            top: 50%;
            left: 50%;
            margin-left: -50px; /* half width of the spinner gif */
            margin-top: -50px; /* half height of the spinner gif */
            text-align: center;
            z-index: 999;
            overflow: auto;
            width: 100px; /* width of the spinner gif */
            height: 102px; /*hight of the spinner gif +2px to fix IE8 issue */
        }

        @media (max-width: 600px) {
            .formlay {
                width: 100% !important;
            }
        }

        .select2-container--default .select2-selection--single, .select2-selection .select2-selection--single {
            padding: 3px 12px;
        }
    </style>
     
    <script type="text/javascript">
        var V1 = "";
        function errormessage(V1) {
            $("#messageNotification").jqxNotification({
                width: 300, position: "top-right", opacity: 2,
                autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 3000, template: "error"
            });
            $('#<%=lblmasg.ClientID %>').html(V1);
            $("#messageNotification").jqxNotification("open");

        }
    </script>
    <script type="text/javascript">
        var V = "";
        function Successmessage(V) {
            $("#messageNotification").jqxNotification({
                width: 300, position: "top-right", opacity: 2,
                autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 3000, template: "success"
            });
            $('#<%=lblmasg.ClientID %>').html(V);
            $("#messageNotification").jqxNotification("open");
        }
    </script>
   
        <script type="text/javascript">
            function validate1() {
                if (document.getElementById("<%=ddlgroup.ClientID%>").value == "--Select--" || document.getElementById("<%=ddlgroup.ClientID%>").value == "0") {
                errormessage("Please Select Group Name");
                document.getElementById("<%=ddlgroup.ClientID%>").focus();
                return false;
            }

            if (document.getElementById("<%=ddlperson.ClientID%>").value == "--Select--") {
                errormessage("Please Select Atleast One Person");
                document.getElementById("<%=ddlgroup.ClientID%>").focus();
                return false;
            }

            if (document.getElementById("<%=txtfromtime.ClientID%>").value == "" || document.getElementById("<%=txtfromtime.ClientID%>").value == "00:00") {
                errormessage("Your Time Format is incorrect. Please try again.");
                document.getElementById("<%=txtfromtime.ClientID%>").focus();
                return false;
            }
            var digits = /^(20|21|22|23|[01]\d|\d)(([:][0-5]\d){1,2})$/;
            var digitsid = document.getElementById("<%=txtfromtime.ClientID %>").value;
            var digitsArray = digitsid.match(digits);
            var temp;
            if (digitsArray == null) {
                errormessage("Your Time Format is incorrect. Please try again.");
                document.getElementById("<%=txtfromtime.ClientID %>").focus();
                return false;
            }

            if (document.getElementById("<%=txttotime.ClientID%>").value == "" || document.getElementById("<%=txttotime.ClientID%>").value == "00:00") {
                errormessage("Your Time Format is incorrect. Please try again.");
                document.getElementById("<%=txttotime.ClientID%>").focus();
                return false;
            }
            var digits = /^(20|21|22|23|[01]\d|\d)(([:][0-5]\d){1,2})$/;
            var digitsid = document.getElementById("<%=txttotime.ClientID %>").value;
            var digitsArray = digitsid.match(digits);
            var temp;
            if (digitsArray == null) {
                errormessage("Your Time seems incorrect. Please try again.");
                document.getElementById("<%=txttotime.ClientID %>").focus();
                return false;
            }
            var start = document.getElementById("<%=txtfromtime.ClientID %>").value;
            var end = document.getElementById("<%=txttotime.ClientID %>").value;
            var dtStart = new Date("1/1/2007 " + start);
            var dtEnd = new Date("1/1/2007 " + end);
            if (Date.parse(dtStart) > Date.parse(dtEnd)) {
                errormessage("To Time Should be greater then From Time");
                document.getElementById("<%=txttotime.ClientID %>").focus();
                return false;
            }
            if (document.getElementById("<%=DropDownList4.ClientID%>").value == "--Select--" || document.getElementById("<%=DropDownList4.ClientID%>").value == "0") {
                errormessage("Please Select Interval Min.");
                document.getElementById("<%=DropDownList4.ClientID%>").focus();
                return false;
            }
        }

    </script>
    <section class="content">
        <div id="spinner" class="spinner" style="display: none;">
            <img id="img-spinner" src="img/loader.gif" alt="Loading" /><br />
            Loading Data....
        </div>
        <div id="messageNotification">
            <div>
                <asp:Label ID="lblmasg" runat="server"></asp:Label>
            </div>
        </div>
        <div class="box-body" id="rptmain" runat="server" >
            <div class="row">
                <div class="col-xs-12">

                    <div class="box">
                        <div class="box-header">
                            <h3 class="box-title">DateWise Movement Summary-Location</h3>
                            </div>
                    
                        <div class="clearfix"></div>
                            <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="container-fluid" style="min-height:600px;">
                <div class="col-md-6"></div>
                <div class="col-md-6"><asp:Button ID="Button5" runat="server" CssClass="btn btn-primary pull-right" Text="Export" 
                            Visible="false" onclick="Button5_Click"
                            /></div>
                <div class="clearfix"></div>
                <div class="form-group col-md-4 col-sm-4 col-xs-12 no-padding" > 
                    <label for="requiredfield" class="back">*</label>                  
                   <label for="Group" >Group :</label>
          
                     <asp:DropDownList ID="ddlgroup" CssClass="textbox form-control" 
                              Width="100%" runat="server"  AutoPostBack="true"
                                onselectedindexchanged="ddlgroup_SelectedIndexChanged">
                            <%-- onchange="GetPerson(this.value);"--%>
                            </asp:DropDownList>
                </div>
                <div class="form-group col-md-4 col-sm-4 col-xs-12" > 
                    <label for="requiredfield" class="back">*</label>                  
                   <label for="Person" >Person :</label>
                     <asp:DropDownCheckBoxes ID="ddlperson" runat="server" AddJQueryReference="true"
                                UseSelectAllNode = "false">
 <Texts SelectBoxCaption="--Select All--" />  
    <Style2 SelectBoxWidth="100%"  DropDownBoxBoxHeight="250" />   
</asp:DropDownCheckBoxes>     
 
                </div>

                <div class="clearfix"></div>
                <asp:ExtendedRequiredFieldValidator ID = "ExtendedRequiredFieldValidator1" runat = "server" ControlToValidate = "ddlperson" ErrorMessage = "*" ForeColor = "Red"></asp:ExtendedRequiredFieldValidator>                                                 
                <div class="form-group col-md-8 col-sm-8 col-xs-12 no-padding" >
                   
                   <label for="Group Name" >Date :</label>
                      <div class="clearfix"></div>
                     <div class="col-md-6 col-sm-6 col-xs-9 no-padding">

                    <asp:TextBox ID="txtfromdate" Enabled="false" runat="server" CssClass="textbox form-control"></asp:TextBox>
                         
                         </div>
<%--                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/img/Calendar.png" />--%>
                     <div class="col-md-1 col-xs-1 col-sm-1 no-padding">
                        <a href="javascript:;" id="ImageButton2" class="cal-icon"><i class="fa fa-calendar" aria-hidden="true"></i></a>
                         </div>
                      <div class="col-md-2 col-sm-2 col-xs-4">
                            <asp:TextBox ID="txtfromtime" runat="server" MaxLength="6" CssClass="textbox form-control"></asp:TextBox></div>
                                
                                       <div class="col-md-2 col-sm-2 col-xs-4">   <asp:TextBox ID="txttotime" runat="server" MaxLength="6" CssClass="textbox form-control"></asp:TextBox></div>
                            <ajax:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtfromdate"
                                PopupButtonID="ImageButton2" Format="dd-MMM-yyyy">
                            </ajax:CalendarExtender>
                </div>
                <div class="clearfix"></div>
                <div class="form-group col-md-2 col-sm-2 col-xs-5" > 
                    <label for="requiredfield" class="back">*</label>                  
                   <label for="Interval" >Interval :</label><br/>
                    <asp:DropDownList ID="DropDownList4" CssClass="textbox form-control"  runat="server">
                                <asp:ListItem Text="30 Min" Value="30"></asp:ListItem>
                                <asp:ListItem Text="60 Min" Selected="True" Value="60"></asp:ListItem>
                               <%-- <asp:ListItem Text="90 Min" Value="90"></asp:ListItem>--%>
                            </asp:DropDownList>
                            <asp:HiddenField ID="txtaccu" runat="server" />
                </div>
                  <div class="form-group col-md-2 col-sm-2 col-xs-5 no-padding">
                    <label for="Location">Location :</label>
                    <asp:DropDownList ID="ddlloc" runat="server" CssClass="textbox form-control" Width="100%"
                         AutoPostBack="true">

                        <asp:ListItem Text="GPS" Value="G" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Tower" Value="C"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="clearfix"></div>
                <div class="form-group col-md-6 col-sm-6 col-xs-12" > 
                    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Submit"
                                OnClientClick="return validate1()" onclick="btnSave_Click" />
                </div>
            

                <div class="clearfix"></div>
                <div class="table-responsive" >
                
                    
                <table class="table">
                    <tr>
                        <td colspan="4">

                            <div style=" display:none;">
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" >
                                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sno.">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSerialNo" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        </asp:TemplateField>
                                    </Columns>
                                 
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    </table>
                
                    <table>
                    <tr>
                    <td colspan="4">
                       <div style=" width: 100%;">
                    <asp:GridView ID="gdw" runat="server" AutoGenerateColumns="true" BackColor="White" 
                                    BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" 
                               CellPadding="3" GridLines="Vertical"
                                    Width="100%" onrowdatabound="gdw_RowDataBound" >
                                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <PagerStyle BackColor="#3c8dbc" ForeColor="White"  /> 
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#3C8DBC" Font-Bold="True" ForeColor="White" />
                              <AlternatingRowStyle BackColor="#FFFFFF" />
                                    </asp:GridView>
                                    </div>
                    </td></tr>
                </table>
                    </div>
           
                </div>
        </ContentTemplate>
    </asp:UpdatePanel>
                        <!-- /.box-body -->
                    </div>
                    <!-- /.box -->

                </div>
                <!-- /.col -->
            </div>

        </div>
    </section>
   
    
  
</asp:Content>