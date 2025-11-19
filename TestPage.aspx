<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestPage.aspx.cs" Inherits="VehicleLocation" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
 
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAmSZwEGMWpCdpj0_3I-IzAkyF2NOF0AD8&libraries=places"></script>
  
<script type="text/javascript">
    var markers = [
    {
        "id": "1",
        "lat": "4.66455174",
        "lng": "-74.07867091",
        "name": "Bogot\u00e1"
    }, 
    {
        "id": "2",
        "lat": "6.24478548",
        "lng": "-75.57050110",
        "name": "Medell\u00edn"
    }, 
    {
        "id": "3",
        "lat": "7.06125013",
        "lng": "-73.84928550",
        "name": "Barrancabermeja"
    }, 
    {
        "id": "4",
        "lat": "7.88475514",
        "lng": "-72.49432589",
        "name": "C\u00facuta"
    }, 
    {
        "id": "5",
        "lat": "3.48835279",
        "lng": "-76.51532198",
        "name": "Cali"
    }, 
    {
        "id": "6",
        "lat": "4.13510880",
        "lng": "-73.63690401",
        "name": "Villavicencio"
    }, 
    {
        "id": "7",
        "lat": "6.55526689",
        "lng": "-73.13373892",
        "name": "San Gil"
    }
];

function setMarkers(map) {
    var image = {
        url: 'https://developers.google.com/maps/documentation/javascript/examples/full/images/beachflag.png',
        size: new google.maps.Size(20, 32),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(0, 32)
    };

    for (var i = 0; i < markers.length; i++) {
        var marker = markers[i];
        var marker = new google.maps.Marker({
            position: {
                lat: parseFloat(marker.lat), 
                lng: parseFloat(marker.lng)
            },
            map: map,
            icon: image,
            shape: {
                coords: [1, 1, 1, 20, 18, 20, 18, 1],
                type: 'poly'
            },
            title: marker.name
        });
    }
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
        <asp:HiddenField ID="hdnLocation" runat="server" />
        <asp:GridView ID="GridView1" runat="server" CellPadding="3"></asp:GridView>
        
    </td>
</tr>
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
