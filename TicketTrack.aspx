<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TicketTrack.aspx.cs" Inherits="VehicleLocation" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
  
          <script  type="text/javascript"  src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCs-N8XxSQWIfllyifIcpNLKANPTWnd2jk&libraries=places"></script>


        
<%--    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAmSZwEGMWpCdpj0_3I-IzAkyF2NOF0AD8&libraries=places"></script>--%>
  
<script type="text/javascript">
var source, destination;
var directionsDisplay;
var directionsService = new google.maps.DirectionsService();
   google.maps.event.addDomListener(window, 'load', function () {
    new google.maps.places.SearchBox(document.getElementById('txtSource'));
    new google.maps.places.SearchBox(document.getElementById('txtDestination'));
    directionsDisplay = new google.maps.DirectionsRenderer({ 'draggable': true });
});
 
function GetRoute() {
    var myanmar = new google.maps.LatLng(16.8730199, 96.1956708);
    var mapOptions = {
        zoom: 7,
        center: myanmar
    };
    
    map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
    directionsDisplay.setMap(map);
    directionsDisplay.setPanel(document.getElementById('dvPanel'));
 
    //*********DIRECTIONS AND ROUTE**********************//
    source = document.getElementById("txtSource").value;
    destination = document.getElementById("txtDestination").value;
 
    var request = {
        origin: source,
        destination: destination,
        travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
        }
    });
 
    //*********DISTANCE AND DURATION**********************//
    var service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix({
        origins: [source],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false
    }, function (response, status) {
        if (status == google.maps.DistanceMatrixStatus.OK && response.rows[0].elements[0].status != "ZERO_RESULTS") {
            var distance = response.rows[0].elements[0].distance.text;
            var duration = response.rows[0].elements[0].duration.text;
            var dvDistance = document.getElementById("dvDistance");
             var dvDuration = document.getElementById("dvDuration");
            dvDistance.innerHTML = "";
            dvDuration.innerHTML = "";
            dvDistance.innerHTML += "" + distance + "";
            dvDuration.innerHTML += "" + duration;
 
        } else {
            alert("Unable to find the distance via road.");
        }
    });
}
</script>
   <table border="0" cellpadding="1" cellspacing="3">
<tr>
    <td>
          Driver Location:
        <input type="text" id="txtSource" style="width: 180px" runat="server" />
    </td>
    <td>
         Customer Location:
        <input type="text" id="txtDestination" style="width: 180px" runat="server" />
    </td>
    <td>
          <input type="button" value="Get Route" onclick="GetRoute()" />
    </td>
     <td>

    </td>
    <td>
        <img src="Images/Distance.png" alt="Distance"/>
          <div id="dvDistance">
        </div>
    </td>
    <td>
         <img src="Images/duration.png"  alt="Duration"/>
          <div id="dvDuration">
        </div>
    </td>
  
</tr>
</table>
       <table border="0" cellpadding="1" cellspacing="3">

<tr>
    <td>
        <div id="dvMap" style="width: 850px; height: 400px">
        </div>
    </td>
    <td>
        <div id="dvPanel" style="width: 400px; height: 400px">
        </div>
    </td>
</tr>
</table>

    </form>
</body>
</html>
