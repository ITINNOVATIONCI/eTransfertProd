(function () {
    'use strict';

    angular
        .module('transactionServices', ['ngResource'])
        .factory('Transaction', Transaction);

    Transaction.$inject = ['$resource'];

    function Transaction($resource) {
        return $resource('/api/transactions/:id');
    }

})();