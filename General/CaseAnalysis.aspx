<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CaseAnalysis.aspx.cs" Inherits="General_CaseAnalysis" %>

<!DOCTYPE html >

<html>
<head>
    <title></title>
    <link href="../css/examples-offline.css" rel="stylesheet" type="text/css" />
    <link href="../css/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="../css/kendo.default.min.css" rel="stylesheet" type="text/css" />
    <link href="../css/kendo.rtl.min.css" rel="stylesheet" type="text/css" />

    <script src="../js/jquery.min.js" type="text/javascript"></script>
    <script src="../js/kendo.web.min.js" type="text/javascript"></script>
    <script src="../js/console.js" type="text/javascript"></script>
 
</head>
<body>
            <div id="example" class="k-content">
            <div id="grid"></div>

            <script>
                $(document).ready(function () {
                var crudServiceBaseUrl = "../Services/MasterWS.asmx",
                        dataSource = new kendo.data.DataSource({
                            transport: {
                                read:  {
                                url: crudServiceBaseUrl + "/Getrdgtablecolumns",
                                    dataType: "jsonp"
                                },
                                update: {
                                url: crudServiceBaseUrl + "/Getrdgtablecolumns/Update",
                                    dataType: "jsonp"
                                },
                                destroy: {
                                url: crudServiceBaseUrl + "/Getrdgtablecolumns/Destroy",
                                    dataType: "jsonp"
                                },
                                create: {
                                url: crudServiceBaseUrl + "/Getrdgtablecolumns/Create",
                                    dataType: "jsonp"
                                },
                                parameterMap: function(options, operation) {
                                    if (operation !== "read" && options.models) {
                                        return {models: kendo.stringify(options.models)};
                                    }
                                }
                            },
                            batch: true,
                            pageSize: 20,
                            schema: {
                                model: {
                                 //   id: "ProductID",
                                    fields: {
                                       // ProductID: { editable: false, nullable: true },
                                        caseNumber: { validation: { required: true} },
                                        CustomerName: { validation: { required: true} }
//                                        UnitPrice: { type: "number", validation: { required: true, min: 1} },
//                                        Discontinued: { type: "boolean" },
//                                        UnitsInStock: { type: "number", validation: { min: 0, required: true } }
                                    }
                                }
                            }
                        });

                    $("#grid").kendoGrid({
                        dataSource: dataSource,
                        pageable: true,
                        height: 430,
                        toolbar: ["create"],
                        columns: [
                            { field:"caseNumber", title: "Case Number" },
                            { field:"CustomerName", title: "Customer Name" }
//                            { field: "UnitPrice", title:"Unit Price", format: "{0:c}", width: "100px" },
//                            { field: "UnitsInStock", title:"Units In Stock", width: "100px" },
//                            { field: "Discontinued", width: "100px" },
//                            { command: ["edit", "destroy"], title: "&nbsp;", width: "160px" }
                            ],
                        editable: "popup"
                    });
                });
            </script>
        </div>


</body>
</html>