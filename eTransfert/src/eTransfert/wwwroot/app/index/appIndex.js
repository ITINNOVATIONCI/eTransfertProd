(function () {
    'use strict';

    config.$inject = ['$routeProvider', '$locationProvider'];

    angular.module('indexApp', [
        'ngRoute', 'transactionServices', 'kendo.directives'
    ]).config(config);

    function config($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                templateUrl: '/app/Index/Views/main.html',
                controller: 'indexController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode(true);
    }

})();