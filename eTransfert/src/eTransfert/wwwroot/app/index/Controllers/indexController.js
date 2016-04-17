(function () {
    'use strict';

    angular
        .module('indexApp')
        .controller('indexController', indexController);

    /* Contacts List Controller  */
    indexController.$inject = ['$scope', '$location', 'Transaction'];

    function indexController($scope, $location, Transaction) {
    
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
                field: "Id",
                title: "No",
                width: "200px",
                filterable: {
                    cell: {
                        showOperators: false
                    }
                }
            }, {
                field: "DateTransaction",
                title: "Date",
                format: "{0:dd-MM-yyyy}",
                width: "70px"
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
                field: "Montant",
                title: "Montant",
                format: "{0:n0}",
                width: "100px"
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
            },
            { command: ["edit"], title: "Action", width: "85px" }
            ]
        };

        $scope.onClick1 = function () {
            alert('No record selected')
            //$scope.myGrid.add();
            //var selected = $scope.myGrid.select();
            //if (selected.length == 0) {
            //    alert('No record selected')
            //} else {
            //    $scope.myGrid.editRow(selected);
            //}
        };

        $scope.contactGridOptions = {
            dataSource: {
                transport:
                {
                    read: {
                        url: crudServiceBaseUrl + "/contacts",
                        dataType: "json"
                    },
                    update: {
                        url: crudServiceBaseUrl + "/contacts",
                        dataType: "json",
                        type: "POST"
                    },
                    destroy: {
                        url: crudServiceBaseUrl + "/contacts",
                        dataType: "json",
                        type: "DELETE"
                    },
                    create: {
                        url: crudServiceBaseUrl + "/contacts",
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
                                type: "string"
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
                pageSize: 5,
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
            { command: { text: "Ajouter", click: function (e) { e.preventDefault(); var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); $scope.transaction.Numero = data.Numero; $scope.win2.center().open(); } }, title: "Action", width: "85px" }
            ]
        };

        $scope.transaction = new Transaction();
        $scope.transaction.Montant = 1;
        $scope.transaction.Numero = "09917435";

        $scope.onValidateTransfert = function () {
            //alert($scope.transaction.Numero);
            $scope.transaction.$save(
				// success
				function (e) {
				    //alert(e.ToString());
                    $location.path('/test/');
				},
				// error
				function (error) {
				    _showValidationErrors($scope, error);
				}
			);
        };

    }

    /* Utility Functions */

    function _showValidationErrors($scope, error) {
        $scope.validationErrors = [];
        if (error.data && angular.isObject(error.data)) {
            for (var key in error.data) {
                $scope.validationErrors.push(error.data[key][0]);
            }
        } else {
            $scope.validationErrors.push('Could not add movie.');
        };
    }

})();