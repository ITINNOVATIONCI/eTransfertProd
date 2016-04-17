$(document).ready(function () {
    $("#grid").kendoGrid({
        dataSource: {
            transport:
            {
                read: {
                    url: "/api/Transactions",
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: {
                            editable: false,
                            nullable: false,
                            type: "string"
                        },
                        numerotransaction: {
                            type: "string"
                        },
                        DateTransaction: {
                            type: "date"
                        },
                        Numero: {
                            type: "string"
                        },
                        Montant: {
                            type: "number"
                        },
                        Pourcentage: {
                            type: "number"
                        },
                        Total: {
                            type: "number"
                        },
                        TypeTransfert: {
                            type: "string"
                        },
                        status: {
                            type: "string"
                        },
                    }
                }
            },
            autoSync: true,
            pageSize: 10,
            serverPaging: false,
            sort: { field: 'DateTransaction', dir: 'desc' },
            serverSorting: false
        },
        height: 550,
        sortable: true,
        pageable: true,
        filterable: {
            mode: "row"
        },
        reorderable: true,
        resizable: true,
        columns: [{
            field: "DateTransaction",
            title: "Date",
            format: "{0:dd-MM-yyyy hh:mm:ss}",
            width: "120px"
        },
        {
            field: "Numero",
            title: "Numero",
            width: "70px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        },
                {
                    field: "Montant",
                    title: "Montant",
                    format: "{0:n0}",
                    width: "100px",
                    filterable: {
                        cell: {
                            showOperators: false
                        }
                    }
                },
                        {
                            field: "Pourcentage",
                            title: "%",
                            format: "{0:n0}",
                            width: "50px",
                            filterable: {
                                cell: {
                                    showOperators: false
                                }
                            }
                        },
        {
            field: "Total",
            title: "Total",
            format: "{0:n0}",
            width: "100px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        },
        {
            field: "TypeTransfert",
            title: "Type",
            width: "100px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        },
        {
            field: "status",
            title: "Etat",
            width: "120px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        }
        ]
    });

    $("#contactGrid").kendoGrid({
        dataSource: {
            transport:
            {
                read: {
                    url: "/api/contacts",
                    dataType: "json"
                },
                update: {
                    url: "/api/contacts",
                    dataType: "json",
                    type: "POST"
                },
                destroy: {
                    url: "/api/contacts",
                    dataType: "json",
                    type: "DELETE"
                },
                create: {
                    url: "/api/contacts",
                    dataType: "json",
                    type: "POST"
                }
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: {
                            editable: false,
                            nullable: false,
                            type: "number"
                        },
                        Nom: {
                            type: "string",
                            validation: { required: true }
                        },
                        Numero: {
                            type: "string"
                        },
                    }
                }
            },
            pageSize: 100,
            toolbar: ["create"],
            editable: "popup",
            serverPaging: false,
            serverSorting: false
        },
        height: 320,
        sortable: true,
        selectable: true,
        pageable: true,
        filterable: {
            mode: "row"
        },
        reorderable: true,
        resizable: true,
        columns: [{
            field: "Nom",
            title: "Nom",
            width: "120px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        },
        {
            field: "Numero",
            title: "Numero",
            width: "120px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        },
        { command: { text: "Choisir", click: showDetails }, title: "Action", width: "85px" }
        ]
    });

    $("#phone_number").kendoMaskedTextBox({
        mask: "00000000"
    });

    $("#montant").kendoNumericTextBox({
        format: "{0:n0}",
        decimals: 0,
        step: 100,
        change: onChange,
        spin: onSpin
    });

    $("#pourcentage").kendoNumericTextBox({
        format: "{0:n0}",
        decimals: 0
    });

    //$("#total").kendoNumericTextBox({
    //    format: "{0:n0}",
    //    decimals: 0,
    //    step: 100
    //});

    //var numerictextbox = $("#total").data("kendoNumericTextBox");
    //numerictextbox.readonly();

    function showDetails(e) {
        e.preventDefault();

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $("#phone_number").val(dataItem.Numero);
    }

    var mode;
    function onChange() {
        var d1 = this.value();

        if (d1 < 0) d1 = -d1;

        if (mode == "RAPIDE") {
            $("#total").text(Math.ceil( d1 + (d1 * 0.07)));
            $("#total").val(Math.ceil( d1 + (d1 * 0.07)));
        }
        else if (mode == "VIP") {
            $("#total").text(Math.ceil(d1 + (d1 * 0.05)));
            $("#total").val(Math.ceil(d1 + (d1 * 0.05)));
        }
        else {
            $("#total").text(d1);
            $("#total").val(d1);
        }

    }

    function onSpin() {
        var d1 = this.value();
        if (d1 < 0) d1 = -d1;

        if (mode == "RAPIDE") {
            $("#total").text(Math.ceil(d1 + (d1 * 0.07)));
            $("#total").val(Math.ceil(d1 + (d1 * 0.07)));
        }
        else if (mode == "VIP") {

            $("#total").text(Math.ceil(d1 + (d1 * 0.05)));
            $("#total").val(Math.ceil(d1 + (d1 * 0.05)));
        }
        else {
            $("#total").text(d1);
            $("#total").val(d1);
        }
    }

    $("input").on("click", function () {
        mode = $("input:checked").val();

        var d1 = $("#montant").val();

        if (mode == "RAPIDE") {
            $("#total").text(Math.ceil(parseInt(d1) + (parseInt(d1) * 0.07)));
            $("#total").val(Math.ceil(parseInt(d1) + (parseInt(d1) * 0.07)));
        }
        else if (mode == "VIP") {
            $("#total").text(Math.ceil(parseInt(d1) + (parseInt(d1) * 0.05)));
            $("#total").val(Math.ceil(parseInt(d1) + (parseInt(d1) * 0.05)));
        }
        else {
            $("#total").text(parseInt(d1));
            $("#total").val(parseInt(d1));
        }

    });

});