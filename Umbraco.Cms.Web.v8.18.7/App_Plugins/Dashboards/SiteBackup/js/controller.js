angular.module("umbraco").controller("splatDev.SiteBackup.Controller", function ($scope, $http) {
    'use strict';
    const baseUrl = '/umbraco/backoffice/api/Backup/';
    const vm = this;
    vm.model = {
        connection: null
    };

    function init() {
        $http.get(`${baseUrl}GetBackupDetails`).then(response => {
            vm.model.connection = response.data;
            console.log(vm.model.connection);
        })
    }
    init();
});