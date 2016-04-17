(function () {
    'use strict';

    angular
        .module('historiquesApp')
        .controller('historiquesListController', historiquesListController);

    /* Contacts List Controller  */
    historiquesListController.$inject = ['$scope'];

    function historiquesListController($scope) {
        var crudServiceBaseUrl = "/api";
        $scope.mainGridOptions = {
            dataSource: {
                transport:
                {
                    read: {
                        url: crudServiceBaseUrl + "/Transactions",
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
                autoSync:true,
                pageSize: 40,
                serverPaging: true,
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
                field: "Id",
                title: "No",
                width: "180px",
                filterable: {
                    cell: {
                        showOperators: false
                    }
                }
            }, {
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
            }, {
                field: "Total",
                title: "Montant",
                format: "{0:n0}",
                width: "100px"
            }, {
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
        };

    }


})();