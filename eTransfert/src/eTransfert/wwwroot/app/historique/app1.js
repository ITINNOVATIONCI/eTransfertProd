(function () {
    'use strict';

    config.$inject = ['$routeProvider', '$locationProvider'];

    angular.module('historiquesApp', [
        'ngRoute', 'kendo.directives'
    ]).config(config);

    function config($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                templateUrl: '/app/historique/Views/listhistorique.html',
                controller: 'historiquesListController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode(true);
    }

})();