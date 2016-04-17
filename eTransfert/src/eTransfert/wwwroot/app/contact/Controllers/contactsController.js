(function () {
    'use strict';

    angular
        .module('contactsApp')
        .controller('ContactsListController', ContactsListController)
        .controller('ContactsAddController', ContactsAddController)
        .controller('ContactsEditController', ContactsEditController)
        .controller('ContactsDeleteController', ContactsDeleteController);

    /* Contacts List Controller  */
    ContactsListController.$inject = ['$scope', 'Contact'];

    function ContactsListController($scope, Contact, $document) {
        //$scope.contacts = Contact.query();
        var crudServiceBaseUrl = "/api";
        $scope.mainGridOptions = {
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
                pageSize: 10,
                serverPaging: false,
                serverSorting: true
            },
            height: 550,
            toolbar: ["create"],
            editable: "popup",
            sortable: true,
            pageable: true,
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [{
                field: "Nom",
                title: "Nom",
                width: "120px"
            }, {
                field: "Numero",
                title: "Numero",
                width: "120px"
            },
            { command: ["edit", "destroy"], title: "&nbsp;", width: "250px" }
            ]
        };

        $scope.source = new kendo.data.DataSource({
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
                            nullable: false,
                            type: "string"
                        },
                        Nom: {
                            type: "string",
                            validation: { required: true }
                        },
                        Numero: {
                            type: "string",
                            validation: { required: true }
                        },
                    }
                }
            },
            pageSize: 12,
            toolbar: ["create"],
            editable: "popup",
            serverPaging: false,
            serverSorting: true
        });

        $scope.onClick1 = function () {
            $scope.myGrid.add();
            //var selected = $scope.myGrid.select();
            //if (selected.length == 0) {
            //    alert('No record selected')
            //} else {
            //    $scope.myGrid.editRow(selected);
            //}
        };

    }

    /* Contacts Create Controller */
    ContactsAddController.$inject = ['$scope', '$location', 'Contact'];

    function ContactsAddController($scope, $location, Contact) {
        $scope.contact = new Contact();
        $scope.add = function () {
            $scope.contact.$save(function () {
                $location.path('/');
            });
        };
    }

    /* Contacts Edit Controller */
    ContactsEditController.$inject = ['$scope', '$routeParams', '$location', 'Contact'];

    function ContactsEditController($scope, $routeParams, $location, Contact) {
        $scope.contact = Contact.get({ id: $routeParams.id });
        $scope.edit = function () {
            $scope.contact.$save(function () {
                $location.path('/');
            });
        };
    }

    /* Contacts Delete Controller  */
    ContactsDeleteController.$inject = ['$scope', '$routeParams', '$location', 'Contact'];

    function ContactsDeleteController($scope, $routeParams, $location, Contact) {
        $scope.contact = Contact.get({ id: $routeParams.id });
        $scope.remove = function () {
            $scope.contact.$remove({ id: $scope.contact.Id }, function () {
                $location.path('/');
            });
        };
    }


})();