<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VehicleLocation.aspx.cs" Inherits="VehicleLocation" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>
    <form id="form1" runat="server">

          <script  type="text/javascript"  src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCs-N8XxSQWIfllyifIcpNLKANPTWnd2jk&libraries=places"></script>

   <%-- <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAehm7Ca22LdxaMBrqJubmrDr729ge7A0E&libraries=places&language=en"></script>--%>

        
    <script type="text/javascript">
    var markers = [
    <asp:Repeater ID="rptMarkers" runat="server">
    <ItemTemplate>
             {
                "title": '<%# Eval("Name") %>',
                "lat": '<%# Eval("Latitude") %>',
                "lng": '<%# Eval("Longitude") %>',
                "description": '<%# Eval("Description") %>',
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
            var mapOptions = {
                center: new google.maps.LatLng(markers[0].lat, markers[0].lng),
                zoom: 7,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            var infoWindow = new google.maps.InfoWindow();
            var map = new google.maps.Map(document.getElementById("dvMap"), mapOptions);
            for (i = 0; i < markers.length; i++) {
                var data = markers[i]
                var myLatlng = new google.maps.LatLng(data.lat, data.lng);
                var marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    title: data.title
                });
                (function (marker, data) {
                    google.maps.event.addListener(marker, "click", function (e) {
                        infoWindow.setContent(data.description);
                        infoWindow.open(map, marker);
                    });
                })(marker, data);
            }
        }
    </script>

           <div id="dvMap" align="center" style="width: 99.9%; height: 470px">
            </div>
    </form>
</body>
</html>
