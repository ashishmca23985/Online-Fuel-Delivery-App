<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DriverStatus.aspx.cs" Inherits="VehicleLocation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
          <script  type="text/javascript"  src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCs-N8XxSQWIfllyifIcpNLKANPTWnd2jk&libraries=places"></script>


<%--     <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAehm7Ca22LdxaMBrqJubmrDr729ge7A0E&libraries=places&language=en"></script>--%>

         <script type="text/javascript">
    var markers = [
    <asp:Repeater ID="rptMarkers" runat="server">
    <ItemTemplate>
             {
                "title": '<%# Eval("DriverName") %>',
                "lat": '<%# Eval("Latitute") %>',
                "lng": '<%# Eval("Longitute") %>',
                "icon": "Images/fueltruck2.png"
    
            }
    </ItemTemplate>
    <SeparatorTemplate>
        ,
    </SeparatorTemplate>
    </asp:Repeater>
    ];
    </script>
        <script type="text/javascript">


            window.onload = function () {
                var custLatLong = document.getElementById("txtDestination").value;
                // alert(custLatLong);
                var mapOptions = {
                    center: new google.maps.LatLng(custLatLong), //16.7773549,96.15851889999999
                    zoom: 11,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
                var infoWindow = new google.maps.InfoWindow();
                var latlngbounds = new google.maps.LatLngBounds();
                var map = new google.maps.Map(document.getElementById("dvMap"), mapOptions);
                var legend = document.getElementById("legend");
                legend.innerHTML = "";
                for (i = 0; i < markers.length; i++) {
                    var data = markers[i]
                    var myLatlng = new google.maps.LatLng(data.lat, data.lng);
                    var marker = new google.maps.Marker({
                        position: myLatlng,
                        map: map,
                        title: data.title,
                        vehicle: data.Vehicle,
                        icon: data.icon
                    });
                    (function (marker, data) {
                        google.maps.event.addListener(marker, "click", function (e) {

                            infoWindow.setContent("<div style = 'width:100px;height:40px'>" + data.title + "</div>");

                            //infoWindow.setContent(data.description);
                            infoWindow.open(map, marker);
                        });
                    })(marker, data);
                    latlngbounds.extend(marker.position);
                    legend.innerHTML += "<div style = 'margin:5px'><img align = 'middle' src = '" + marker.icon + "' />&nbsp;" + marker.title + "</div>";
                }
                var bounds = new google.maps.LatLngBounds();
                map.setCenter(latlngbounds.getCenter());
                map.fitBounds(latlngbounds);
            }

        </script>


        <table border="0" cellpadding="1" cellspacing="3">
            <tr>
                <td>
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </td>    
               
            </tr>
           
        </table>

        <table border="0" cellpadding="1" cellspacing="3">
            <tr>
                 <td>Customer Location:
                     <input type="text" id="txtDestination" style="width: 200px" runat="server" />
                </td>
                 <td valign="top">
                     
                </td>

            </tr>
            <tr>
                <td>
                    <div style="overflow-y: scroll;">
                        <asp:GridView ID="GridView1" runat="server" HeaderStyle-BackColor="#3AC0F2"
                            HeaderStyle-ForeColor="White" AutoGenerateColumns="true"  Width="100%">
                            <Columns>
                            </Columns>
                            <HeaderStyle BackColor="#006600" ForeColor="White" />
                            
                        </asp:GridView>

                    </div>
                    <asp:HiddenField ID="hdnLocation" runat="server" />

                </td>
            </tr>
            <tr>

                <td >
                    <div id="dvMap" style="width: 850px; height: 400px">
                    </div>
                </td>
                <td id="legend" runat="server" valign="top">

                </td>

            </tr>
            <tr>
                <td>

                   
                </td>

            </tr>
        </table>

    </form>
</body>
</html>
