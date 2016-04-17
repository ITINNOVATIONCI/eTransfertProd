(function () {
    'use strict';

    config.$inject = ['$routeProvider', '$locationProvider'];

    angular.module('contactsApp', [
        'ngRoute', 'contactsServices', 'kendo.directives'
    ]).config(config);

    function config($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                templateUrl: '/app/contact/Views/list.html',
                controller: 'ContactsListController'
            })
            .when('/contacts/add', {
                templateUrl: '/app/contact/Views/add.html',
                controller: 'ContactsAddController'
            })
            .when('/contacts/edit/:id', {
                templateUrl: '/app/contact/Views/edit.html',
                controller: 'ContactsEditController'
            })
            .when('/contacts/delete/:id', {
                templateUrl: '/app/contact/Views/delete.html',
                controller: 'ContactsDeleteController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode(true);
    }

})();