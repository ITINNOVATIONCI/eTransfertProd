(function () {
    'use strict';

    angular
        .module('contactsServices', ['ngResource'])
        .factory('Contact', Contact);

    Contact.$inject = ['$resource'];

    function Contact($resource) {
        return $resource('/api/contacts/:id');
    }

})();